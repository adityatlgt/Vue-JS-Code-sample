using ALP.Services.Common.Dtos;
using ALP.Data.Models.Organisations;
using ALP.Services.Generic;

namespace ALP.Services.Organisations.Dtos
{
    public class OrganisationListDto : EntityDto
    {
        public string Name { get; set; }
        public string Website { get; set; }
        public string Mainline { get; set; }
        public Organisation.OrganisationsStatusType? Status { get; set; }
        public OrganisationTypeDto OrganisationType { get; set; }
    }
}