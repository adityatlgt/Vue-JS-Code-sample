using System;
using ALP.Data.Models.Organisations;
using ALP.Services.Common.Dtos;
using ALP.Services.Generic;

namespace ALP.Services.Organisations.Dtos
{
    public class OrganisationDto : EntityDto
    {
        public string Name { get; set; }
        public string Website { get; set; }
        public string Mainline { get; set; }
        
        public Organisation.EstimatedRevenueType EstimatedRevenue { get; set; }
        public Organisation.NumberOfEmployeesType NumberOfEmployees { get; set; }
        public Organisation.BusinessLifecycleType? BusinessLifecycle { get; set; }
        public Organisation.NumberOfLocationsType? NumberOfLocations { get; set; }
        public Organisation.OrganisationsStatusType? Status { get; set; }
        public DateTime? EstablishmentDate { get; set; }
        public string Abn { get; set; }
        public string Acn { get; set; }
        public OrganisationTypeDto OrganisationType { get; set; }
        public IndustryCategoryDto IndustryCategory { get; set; }
        public IndustrySubCategoryDto IndustrySubCategory { get; set; }
        public bool EstablishedByUs { get; set; }
        public bool ShowEmails { get; set; }
        public AddressDto Address { get; set; }
        public bool PostalSameAsAddress { get; set; }
        public AddressDto PostalAddress { get; set; }
    }
}