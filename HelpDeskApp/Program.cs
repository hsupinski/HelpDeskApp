using HelpDeskApp.Data;
using HelpDeskApp.Hubs;
using HelpDeskApp.Repositories;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "HelpDeskApp")
    .MinimumLevel.Information()
    .WriteTo.GrafanaLoki(
        "http://loki:3100",
        new[] { new LokiLabel { Key = "app", Value = "helpdeskapp" } },
        credentials: null,
        batchPostingLimit: 1000,
        queueLimit: 100000,
        period: TimeSpan.FromSeconds(2),
        textFormatter: new Serilog.Formatting.Json.JsonFormatter(renderMessage: true)
    )
    .CreateLogger();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.GrafanaLoki(
        "http://loki:3100",
        new[] { new LokiLabel { Key = "app", Value = "helpdeskapp" } },
        credentials: null,
        batchPostingLimit: 1000,
        queueLimit: 100000,
        period: TimeSpan.FromSeconds(2),
        textFormatter: new Serilog.Formatting.Json.JsonFormatter(renderMessage: true)
));

// Add services to the container.
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});
builder.Services.AddControllersWithViews();

var helpDeskConnectionString = builder.Configuration.GetConnectionString("HelpDeskDbContextConnection");
var authConnectionString = builder.Configuration.GetConnectionString("AuthDbContextConnection");
var loggerConnectionString = builder.Configuration.GetConnectionString("NLogConnection");

builder.Services.AddDbContext<HelpDeskDbContext>(options =>
    options.UseSqlServer(helpDeskConnectionString));

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(authConnectionString));

builder.Services.AddDbContext<LoggerDbContext>(options =>
    options.UseSqlServer(loggerConnectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders()
.AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("HelpDeskApp");


builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<IHelpDeskService, HelpDeskService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();

var app = builder.Build();

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
        diagnosticContext.Set("RequestProtocol", httpContext.Request.Protocol);
    };
});

using (var Scope = app.Services.CreateScope())
{
    var services = Scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<HelpDeskDbContext>();
        var authContext = services.GetRequiredService<AuthDbContext>();
        var loggerContext = services.GetRequiredService<LoggerDbContext>();

        context.Database.Migrate();
        authContext.Database.Migrate();
        loggerContext.Database.Migrate();

        await AuthDbContext.SeedData(services);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while seeding the database.");
    }

}

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

app.UseAuthentication();
app.UseAuthorization();


app.MapHub<ChatHub>("/chatHub");
Console.WriteLine("ChatHub mapped to /chatHub");
app.MapHub<NotificationHub>("/notificationHub");
Console.WriteLine("NotificationHub mapped to /notificationHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

try
{
    Log.Information("Application starting up");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
} 
