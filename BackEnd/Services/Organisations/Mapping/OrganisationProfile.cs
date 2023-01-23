using ALP.Data.Models.Organisations;
using ALP.Data.Models.Relationships;
using ALP.Services.Organisations.Dtos;
using AutoMapper;

namespace ALP.Services.Organisations.Mapping
{
    public class OrganisationProfile : Profile
    {
        public OrganisationProfile()
        {
            CreateMap<Organisation, OrganisationListDto>();
            CreateMap<Organisation, OrganisationDto>();
            CreateMap<Organisation, OrganisationInput>();
            CreateMap<OrganisationInput, Organisation>();
            CreateMap<OrganisationInput, OrganisationDto>().ReverseMap();
            CreateMap<OrganisationRelationship, OrganisationRelationshipDto>();
            CreateMap<OrganisationRelationshipDto, OrganisationRelationship>().ReverseMap();
        }
    }
}