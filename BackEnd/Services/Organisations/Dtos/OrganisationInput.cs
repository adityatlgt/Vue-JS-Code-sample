using System;
using System.ComponentModel.DataAnnotations;
using ALP.Data.Models.Organisations;
using ALP.Services.Common.Dtos;

namespace ALP.Services.Organisations.Dtos
{
    public class OrganisationInput
    {
        [Required] public string Name { get; set; }
        public string Website { get; set; }
        public string Mainline { get; set; }

        [Required] public Organisation.EstimatedRevenueType EstimatedRevenue { get; set; }
        [Required] public Organisation.NumberOfEmployeesType NumberOfEmployees { get; set; }
        public Organisation.BusinessLifecycleType? BusinessLifecycle { get; set; }
        public Organisation.NumberOfLocationsType? NumberOfLocations { get; set; }
        public DateTime? EstablishmentDate { get; set; }
        public Organisation.OrganisationsStatusType? Status { get; set; }
        public string Abn { get; set; }
        public string Acn { get; set; }
        public int? OrganisationTypeId { get; set; }
        public int? IndustryCategoryId { get; set; }
        public int? IndustrySubCategoryId { get; set; }
        public bool EstablishedByUs { get; set; }
        public bool ShowEmails { get; set; }
        [Required] public AddressInput Address { get; set; }
        [Required] public bool PostalSameAsAddress { get; set; }
        public AddressInput PostalAddress { get; set; }
    }
}