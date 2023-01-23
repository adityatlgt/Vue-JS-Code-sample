using System.Threading.Tasks;
using ALP.Data.Models.Relationships;
using ALP.Services.Generic;
using ALP.Services.Organisations.Dtos;

namespace ALP.Services.Organisations
{
    public interface IOrganisationService
    {
        Task<OrganisationDto> GetById(int id);
        Task<PaginatedDto<OrganisationListDto>> GetOrganisations(PaginatedInput filters);
        Task<OrganisationDto> CreateOrganisation(OrganisationInput input);
        Task<OrganisationDto> UpdateOrganisation(int organisationId, OrganisationInput input);
        Task DeleteOrganisation(int organisationId);
        Task<PaginatedDto<OrganisationRelationshipDto>> GetOrganisationRelationships(int id, PaginatedInput filters);
        Task<OrganisationRelationshipDto> CreateOrganisationRelationships(int id, OrganisationRelationship input);
    }
}