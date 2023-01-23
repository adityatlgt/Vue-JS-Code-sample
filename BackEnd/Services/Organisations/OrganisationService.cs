using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ALP.Data;
using ALP.Data.Models.Organisations;
using ALP.Data.Models.Relationships;
using ALP.Services.Accounts;
using ALP.Services.Generic;
using ALP.Services.Organisations.Dtos;
using ALP.Services.XeroServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ALP.Services.Organisations
{
    public class OrganisationService : IOrganisationService
    {
        private readonly ALPDbContext _context;
        private readonly IMapper _mapper;
        private readonly IOrganisationJobScheduler _organisationJobScheduler;
        private readonly IPermissionService _permissionService;
        private readonly IXeroService _xeroservice;

        public OrganisationService(IMapper mapper, ALPDbContext context,
            IOrganisationJobScheduler organisationJobScheduler, IPermissionService permissionService, IXeroService xeroService)
        {
            _mapper = mapper;
            _context = context;
            _organisationJobScheduler = organisationJobScheduler;
            _permissionService = permissionService;
            _xeroservice = xeroService;            
        }

        public async Task<OrganisationDto> GetById(int id)
        {
            return _mapper.Map<OrganisationDto>(await _context.Organisations
                .Include(c => c.OrganisationType)
                .Include(c => c.IndustryCategory)
                .Include(c => c.IndustrySubCategory)
                .Include(c => c.Address).DefaultIfEmpty()
                .Include(c => c.PostalAddress).DefaultIfEmpty()
                .FirstOrDefaultAsync(c => c.Id == id));
        }

        public async Task<PaginatedDto<OrganisationListDto>> GetOrganisations(PaginatedInput filters)
        {
            IQueryable<Organisation> query = _context.Organisations;

            if (!string.IsNullOrEmpty(filters.Search))
            {
                query = query.WhereOr(
                    c => EF.Functions.ILike(c.Name, $"%{filters.Search}%"),
                    c => EF.Functions.ILike(c.Website, $"%{filters.Search}%"),
                    c => EF.Functions.ILike(c.Mainline, $"%{filters.Search}%")
                );
            }

            query = query.OrderBy(d => d.Name);

            return await query.ToPaginatedDtoAsync<Organisation, OrganisationListDto>(_mapper, filters);
        }



        public async Task<OrganisationDto> CreateOrganisation(OrganisationInput input)
        {
            return await Helpers.CheckDuplicates(async () =>
            {
                if (!input.PostalSameAsAddress && input.PostalAddress == null)
                {
                    throw new Exception("Postal address is required");
                }

                if (input.PostalSameAsAddress)
                {
                    input.PostalAddress = input.Address;
                }

                // Check Industry Category/Sub Category combination
                if (input.IndustrySubCategoryId.HasValue)
                {
                    var industrySubCategory =
                        await _context.IndustrySubCategories.FirstOrDefaultAsync(c =>
                            c.Id == input.IndustrySubCategoryId);

                    if (industrySubCategory == null ||
                        industrySubCategory.IndustryCategoryId != input.IndustryCategoryId)
                    {
                        throw new Exception("Invalid Industry Sub Category Selected");
                    }
                }

                var organisation = _mapper.Map<Organisation>(input);

                if (!string.IsNullOrEmpty(organisation.Website))
                {
                    try
                    {
                        var domain = new Uri(organisation.Website).Host;
                        //var regex = new Regex(@"/^www\./");
                        domain = Regex.Replace(domain, @"^www\.", ""); // regex.Replace(domain, "");
                        organisation.Domain = domain;
                    }
                    catch (Exception e)
                    {
                    }
                }

                await _context.Organisations.AddAsync(organisation);
                await _context.SaveChangesAsync();
                return _mapper.Map<OrganisationDto>(organisation);
            }, "Failed to create organisation. Conflict with another organisation");
        }

        public async Task<PaginatedDto<OrganisationRelationshipDto>> GetOrganisationRelationships(int id, PaginatedInput filters)
        {
            IQueryable<OrganisationRelationship> query = _context.Organisation_Relationship.Where(i => i.self_id == id);

            if (!string.IsNullOrEmpty(filters.Search))
            {
                query = query.WhereOr(c => EF.Functions.ILike(c.primaryOrganisationFullName, $"%{filters.Search}%"));
            }

            query = query.OrderBy(d => d.primaryOrganisationFullName);

            return await query.ToPaginatedDtoAsync<OrganisationRelationship, OrganisationRelationshipDto>(_mapper, filters);
        }

        public async Task<OrganisationRelationshipDto> CreateOrganisationRelationships(int id, OrganisationRelationship input)
        {
            var self_organisation = await _context.Organisations.FirstOrDefaultAsync(i => i.Id == id);
            OrganisationRelationship childOrganisation = new OrganisationRelationship
            {
                self_id = input.primaryOrganisationId,
                primaryOrganisationId = id,
                primaryOrganisationFullName = self_organisation.Name,
                organisation_relationshipType = OrganisationRelationship.organisationRelationshipType.Child,
                organisation_relationship = input.organisation_relationship
            };

            input.self_id = id;
            await _context.Organisation_Relationship.AddAsync(childOrganisation);
            await _context.Organisation_Relationship.AddAsync(input);
            await _context.SaveChangesAsync();
            return _mapper.Map<OrganisationRelationshipDto>(input);
        }

        public async Task<OrganisationDto> UpdateOrganisation(int organisationId, OrganisationInput input)
        {
            var result =  await Helpers.CheckDuplicates(     
            async() =>
            {
                // Check Industry Category/Sub Category combination
                if (input.IndustrySubCategoryId.HasValue)
                {
                    var industrySubCategory =
                        await _context.IndustrySubCategories.FirstOrDefaultAsync(c =>
                            c.Id == input.IndustrySubCategoryId);

                    if (industrySubCategory.IndustryCategoryId != input.IndustryCategoryId)
                    {
                        throw new Exception("Invalid Industry Sub Category Selected");
                    }
                }

                var organisation = await _context.Organisations
                    .Include(c => c.Address).DefaultIfEmpty()
                    .Include(c => c.PostalAddress).DefaultIfEmpty()
                    .FirstOrDefaultAsync(c => c.Id == organisationId);

                var domain = organisation.Domain;
                var needToReassignEmails = false;
                var xeroTrigger = false;

                if (organisation.Website != input.Website || organisation.Website != null && organisation.Domain == null)
                {
                    try
                    {
                        domain = new Uri(input.Website).Host;
                        // var regex = new Regex(@"/^www\./");
                        domain = Regex.Replace(domain, @"^www\.", ""); // regex.Replace(domain, "");
                        needToReassignEmails = true;
                    }
                    catch (Exception e)
                    {
                        domain = null;
                    }
                }

                if (organisation.Name != input.Name)
                {
                    xeroTrigger = true;
                }

                var originalShowEmails = organisation.ShowEmails;

                _mapper.Map(input, organisation);

                if (!await _permissionService.CanAccess(Permissions.EmailManagePermissions))
                {
                    organisation.ShowEmails = originalShowEmails;
                }

                organisation.Domain = domain;

                await _context.SaveChangesAsync();

                if (needToReassignEmails)
                {
                    await _organisationJobScheduler.ScheduleOrganisationEmailAssignment(organisationId);
                }


                if (xeroTrigger) 
                {
                    var clients_list = _context.Clients
                            .Include(c => c.Organisation)
                            .Where(c => c.Organisation.Id == organisation.Id).ToList();

                    foreach (var client in clients_list)
                    {
                       await _xeroservice.UpdateXeroContactbyClientId(client.Id);
                    }
                   
                }

                return _mapper.Map<OrganisationDto>(organisation);
            }, "Failed to update organisation. Conflict with another organisation");
            return result;
        }


        public async Task DeleteOrganisation(int organisationId)
        {
            var organisation = await _context.Organisations.FindAsync(organisationId);
            _context.Organisations.Remove(organisation);
            await _context.SaveChangesAsync();
        }
    }
}