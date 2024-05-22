using Quartz;
using WorkerServiceTimesheet.Jobs;
namespace WorkerServiceTimesheet
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISchedulerFactory _schedulerFactory;
        public Worker(ILogger<Worker> logger, ISchedulerFactory schedulerFactory)
        {
            _logger = logger;
            _schedulerFactory = schedulerFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // get scheduler instance
            var scheduler = await _schedulerFactory.GetScheduler(stoppingToken);
            // Start the scheduller
            await scheduler.Start();

            // define job
            IJobDetail job = JobBuilder.Create<CreateTimesheet>()
                .WithIdentity("Create Timesheet", "CreateTimesheetGroup")
                .Build();
            // trigger
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("Trigger Create Timesheet", "TriggerCreateTimesheetGroup")
                //.WithCronSchedule("0 0/2 * * * ?")
                .WithCronSchedule("0 0 12 30 * ?")
                .Build();

            // schedule the job
            await scheduler.ScheduleJob(job, trigger, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            await scheduler.Shutdown();
        }
    }
}
