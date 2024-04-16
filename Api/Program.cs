using Api.Filters;
using Api.Providers;
using Api.Services;
using Application;
using Application.Common.Interfaces;
using Infrastructure.Persistence;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
{
    var conf = builder.Configuration;

    builder.Services
        .AddApplicationServices()
        .AddInfrastructureServices(conf);
    //Enable Cors
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
    });
    builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
    builder.Services.AddScoped<ValidationFilterAttribute>();

    builder.Services.AddTransient<IChatHub, ChatHub>();
    builder.Services.AddSignalR();

    //Need to Explore
    builder.Services.AddHttpContextAccessor();

    //For Api Endpoint and handle application exceptions
    builder.Services.AddControllersWithViews(options =>
        options.Filters.Add<ApiExceptionFilterAttribute>())
        .AddFluentValidation(x => x.AutomaticValidationEnabled = false);

    //Remove default behavior of ModelState Validations
    builder.Services.Configure<ApiBehaviorOptions>(options =>
        options.SuppressModelStateInvalidFilter = false
    );

    //Registered Microsoft.AspNetCore.Mvc.Versioning
    builder.Services.AddApiVersioning(opt => {
        //Set default version other than else application thorw an error if end user didn't specified any version
        opt.DefaultApiVersion = new ApiVersion(1, 0);
        opt.AssumeDefaultVersionWhenUnspecified = true;
        //Show end user which version currently supported and which version is depricted
        opt.ReportApiVersions = true;
        //api version global error handling
        opt.ErrorResponses = new ApiVersioningErrorResponseProvider();
    });

    //Registered Jwt Token Services
    //Jwt Bearer Initializer            
    var tokenProvider = new TokenProviderService(conf["JWT:ValidIssuer"], conf["JWT:ValidAudience"], conf["JWT:Secret"]);

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = tokenProvider.GetValidationParameters();
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    // Put a breakpoint here
                    Console.WriteLine(context.Exception);
                    return Task.CompletedTask;
                },
            };
        });

    // This is for the [Authorize] attributes.
    builder.Services.AddAuthorization(auth => {
        auth.DefaultPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();
    });
    var securityScheme = new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT" // Optional
    };
    var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "bearerAuth"
                        }
                    },
                    new string[] {}
                }
            };
    //Registered Swagger Api Documentation
    builder.Services.AddSwaggerGen(o =>
    {
        o.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Court Connect Version 1",
            Description = "These documentation contain all list of REST Services. Provided by Court Connect Community.",
            Version = "v1",
        });

        o.SwaggerDoc("v2", new OpenApiInfo
        {
            Title = "Court Connect Version 2",
            Description = "These documentation contain all list of REST Services. Provided by Court Connect Community.",
            Version = "v2",
        });
        o.AddSecurityDefinition("bearerAuth", securityScheme);
        o.AddSecurityRequirement(securityRequirement);
        o.ResolveConflictingActions(a => a.First());
        o.OperationFilter<RemoveVersionFromParameter>();
        o.DocumentFilter<ReplaceVersionWithExactValueInPath>();
    });


}

var app = builder.Build();
{
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        //await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
    //Allow all origin to use my resource
    app.UseCors("AllowAll");
    // Configure swagger documentation 
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/v1/swagger.json", "API V1");
        c.SwaggerEndpoint($"/swagger/v2/swagger.json", "API V2");
    });
    // Configure the HTTP request pipeline.

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    //Request Pipeline
    app.UseAuthentication();
    app.UseAuthorization();

    //Attend Request
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
       endpoints.MapHub<ChatHub>("/hub");

    });

    app.Run();

}

