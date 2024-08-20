using Microsoft.EntityFrameworkCore;
using SimarAlertNotifier.Data;
using SimarAlertNotifier.Services;
using SimarAlertNotifier.Services.Mail;
using Quartz;
using SimarAlertNotifier.DependencyInjection;
using SimarAlertNotifier.Schedulers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<SimarAlertService>();
builder.Services.AddTransient<IMailService, SendgridMailService>();

// Add database context
builder.Services.AddDbContext<SimarDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SimarDb"),
        op => op.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: System.TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null));
});

// Add Scheduler
builder.Services.AddQuartz(op =>
{
    op.AddQuartzScheduler<SimarEmailNotificationJob>();
});
builder.Services.AddQuartzHostedService(op =>
{
    op.WaitForJobsToComplete = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
