using WorkerServiceTimesheet;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    q.UseInMemoryStore();
});
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "WorkerTimesheet";
});
builder.Services.AddHostedService<Worker>();


var host = builder.Build();
host.Run();
