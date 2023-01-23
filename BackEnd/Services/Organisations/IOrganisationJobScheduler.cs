using System.Threading.Tasks;

namespace ALP.Services.Organisations
{
    public interface IOrganisationJobScheduler
    {
        Task ScheduleOrganisationEmailAssignment(int organisationId);
    }
}