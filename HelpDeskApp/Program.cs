using HelpDeskApp.Data;
using HelpDeskApp.Hubs;
using HelpDeskApp.Repositories;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

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

/*    builder.Services.AddIdentityCore<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>(TokenOptions.DefaultProvider);*/

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
            logger.Error(ex, "An error occurred while seeding the database.");
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

    //logger.Info("Application started");
    
    app.Run();

}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}