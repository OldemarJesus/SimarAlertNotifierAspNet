using Microsoft.EntityFrameworkCore;
using SimarAlertNotifier.Data;
using SimarAlertNotifier.Services;
using SimarAlertNotifier.Services.Mail;
using Quartz;
using SimarAlertNotifier.DependencyInjection;
using SimarAlertNotifier.Schedulers;
using Azure.Identity;
using Azure.Core;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<SimarAlertService>();
builder.Services.AddTransient<IMailService, SendgridMailService>();

// Add database context
builder.Services.AddDbContext<SimarDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SimarDb"));
    }
    else
    {
        var sqlConnection = new SqlConnection(builder.Configuration.GetConnectionString("LocalSqlServer"));
        var tokenCredential = new DefaultAzureCredential();
        sqlConnection.AccessToken = tokenCredential.GetToken(new TokenRequestContext(new[] { "https://simardb.database.windows.net/.default" })).Token;
        options.UseSqlServer(sqlConnection);
    }
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
