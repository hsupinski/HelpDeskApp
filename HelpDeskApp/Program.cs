using HelpDeskApp.Data;
using HelpDeskApp.Hubs;
using HelpDeskApp.Repositories;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});
builder.Services.AddControllersWithViews();

var helpDeskConnectionString = builder.Configuration.GetConnectionString("HelpDeskDbContextConnection");
var authConnectionString = builder.Configuration.GetConnectionString("AuthDbContextConnection");

builder.Services.AddDbContext<HelpDeskDbContext>(options =>
    options.UseSqlServer(helpDeskConnectionString));

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(authConnectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<ITopicService, TopicService>();

builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();

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

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chatHub");
    Console.WriteLine("ChatHub mapped to /chatHub");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
