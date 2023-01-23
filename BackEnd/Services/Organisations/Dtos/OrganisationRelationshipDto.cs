using ALP.Data.Models.Relationships;


namespace ALP.Services.Organisations.Dtos
{
    public class OrganisationRelationshipDto
    {
        public int self_id { get; set; }
        public int primaryOrganisationId { get; set; }
        public string primaryOrganisationFullName { get; set; }
        public OrganisationRelationship.organisationRelationshipType organisation_relationshipType { get; set; }
        public OrganisationRelationship.organisationRelationship organisation_relationship { get; set; }

    }
}