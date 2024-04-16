using Application;
using Application.Common.Interfaces;
using Infrastructure;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
{
    var conf = builder.Configuration;

    builder.Services
        .AddApplicationServices()
        .AddInfrastructureServices(conf);
    builder.Services.AddMvc().AddRazorRuntimeCompilation();
    builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
    builder.Services.AddControllers();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddTransient<IChatHub, ChatHub>();
    builder.Services.AddSignalR();
    // Add services to the container.
    builder.Services.AddRazorPages();
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/account/login";
    });
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
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
        endpoints.MapControllers();
        endpoints.MapRazorPages();
    });
    app.Run();

}