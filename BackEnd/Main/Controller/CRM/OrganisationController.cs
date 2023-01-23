using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALP.Data;
using ALP.Data.Models.Relationships;
using ALP.Services.Generic;
using ALP.Services.Metadata;
using ALP.Services.Metadata.Dtos;
using ALP.Services.Organisations;
using ALP.Services.Organisations.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace ALP.Controllers.CRM
{
    [Route("api/organisations")]
    [Authorize]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        private readonly IEntityDynamicParameterService _entityDynamicParameterService;
        private readonly IMapper _mapper;
        private readonly IOrganisationService _organisationService;

        public OrganisationController(IMapper mapper, IOrganisationService organisationService, IEntityDynamicParameterService entityDynamicParameterService)
        {
            _mapper = mapper;
            _organisationService = organisationService;
            _entityDynamicParameterService = entityDynamicParameterService;
        }

        // GET
        [HttpGet]
        [Can(Permissions.OrganisationView)]
        public async Task<PaginatedDto<OrganisationListDto>> GetList(
            [FromQuery] PaginatedInput filterInput)
        {
            return await _organisationService.GetOrganisations(filterInput);
        }

        [HttpGet("{id}")]
        [Can(Permissions.OrganisationView)]
        public async Task<OrganisationDto> GetById(int id)
        {
            return await _organisationService.GetById(id);
        }

        [HttpPost]
        [Can(Permissions.OrganisationCreate)]
        public async Task<ActionResult<OrganisationDto>> Create(OrganisationInput input)
        {
            try
            {
                return await _organisationService.CreateOrganisation(input);
            }
            catch (DataConflictException e)
            {
                return Conflict("Failed to create organisation. Conflicts with an existing organisation");
            }
        }

        [HttpPut("{id}")]
        [Can(Permissions.OrganisationEdit)]
        public async Task<ActionResult<OrganisationDto>> Update(int id, OrganisationInput input)
        {
            try
            {
                return Ok(await _organisationService.UpdateOrganisation(id, input));
            }
            catch (DataConflictException e)
            {
                return Conflict("This change conflicts with an existing organisation");
            }
        }

        [HttpPatch("{id}")]
        [Can(Permissions.OrganisationEdit)]
        public async Task<ActionResult<OrganisationDto>> Patch(int id,
            [FromBody] JsonPatchDocument<OrganisationInput> patch)
        {
            try
            {
                var organisation = _mapper.Map<OrganisationInput>(await _organisationService.GetById(id));
                patch.ApplyTo(organisation);
                if (TryValidateModel(organisation))
                {
                    return Ok(await _organisationService.UpdateOrganisation(id, organisation));
                }

                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new {x.Key, x.Value.Errors})
                    .ToArray();

                return BadRequest(errors);
            }
            catch (DataConflictException e)
            {
                return Conflict("This change conflicts with an existing organisation");
            }
        }

        [HttpDelete("{id}")]
        [Can(Permissions.OrganisationDelete)]
        public async Task Delete(int id)
        {
            await _organisationService.DeleteOrganisation(id);
        }

        [HttpGet("metadata")]
        public async Task<IEnumerable<EntityDynamicParameterValueOptionsDto>> GetMetadataParameters()
        {
            return await _entityDynamicParameterService.GetEntityDynamicParametersAndValues(MetadataConsts.EntityType
                .Organisation);
        }

        [HttpGet("{id}/metadata")]
        public async Task<IEnumerable<EntityDynamicParameterValuesDto>> GetMetadataValues(int id)
        {
            return await _entityDynamicParameterService.GetEntityDynamicParameterValues(
                MetadataConsts.EntityType.Organisation, id);
        }

        [HttpPost("{id}/metadata")]
        public async Task SetMetadataValue(int id, EntityDynamicParameterValueInput input)
        {
            await _entityDynamicParameterService.SetEntityDynamicParameterValue(MetadataConsts.EntityType.Organisation,
                id,
                input);
        }

        [HttpPost("{id}/metadata/values")]
        public async Task AddMetadataValue(int id, EntityDynamicParameterValueInput input)
        {
            await _entityDynamicParameterService.AddEntityDynamicParameterValue(MetadataConsts.EntityType.Organisation,
                id,
                input);
        }

        [HttpDelete("{id}/metadata/values/valueId")]
        public async Task RemoveMetadataValue(int id, int valueId)
        {
            await _entityDynamicParameterService.RemoveEntityDynamicParameterValue(
                MetadataConsts.EntityType.Organisation,
                id,
                valueId);
        }

        // GET
        [HttpGet("{id}/relationship/")]
        [Can(Permissions.OrganisationView)]
        public async Task<PaginatedDto<OrganisationRelationshipDto>> GetContactRelationship(int id, [FromQuery] PaginatedInput filterInput)
        {
            return await _organisationService.GetOrganisationRelationships(id, filterInput);
        }

        [HttpPost("{id}/relationship/")]
        [Can(Permissions.OrganisationView)]
        public async Task<OrganisationRelationshipDto> createContactRelationship(int id, OrganisationRelationship filterInput)
        {
            return await _organisationService.CreateOrganisationRelationships(id, filterInput);
        }
    }
}