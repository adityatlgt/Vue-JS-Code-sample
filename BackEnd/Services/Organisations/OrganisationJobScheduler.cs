using System.Threading.Tasks;
using ALP.Data;
using ALP.Services.Notifications;
using ALP.Services.Quartz;
using Quartz;

namespace ALP.Services.Organisations
{
    public class OrganisationJobScheduler : IOrganisationJobScheduler
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly INotifier _notifier;
        private readonly AuthenticatedUser _authenticatedUser;

        public OrganisationJobScheduler(ISchedulerFactory schedulerFactory, INotifier notifier,
            AuthenticatedUser authenticatedUser)
        {
            _schedulerFactory = schedulerFactory;
            _notifier = notifier;
            _authenticatedUser = authenticatedUser;
        }

        public async Task ScheduleOrganisationEmailAssignment(int organisationId)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobKey = new JobKey(nameof(OrganisationEmailAssignmentJob), organisationId.ToString());
            var triggerKey = new TriggerKey(nameof(OrganisationEmailAssignmentJob), organisationId.ToString());

            var job = await scheduler.GetJobDetail(jobKey);

            if (job != null)
            {
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .ForJob(job)
                    .StartNow()
                    .Build();

                await scheduler.RescheduleJob(triggerKey, trigger);
            }
            else
            {
                job = JobBuilder.Create<OrganisationEmailAssignmentJob>()
                    .WithIdentity(jobKey)
                    .UsingJobData("OrganisationId", organisationId)
                    .RequestRecovery()
                    .Build();
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .StartNow()
                    .Build();

                await scheduler.ScheduleJob(job, trigger);
            }

            if (_authenticatedUser.UserId != null)
            {
                await _notifier.Notify(_authenticatedUser.UserId.Value, "Email assignment for this organisation has been scheduled.");
            }
        }
    }
}