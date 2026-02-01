using Iprogs.Matesup.CoreAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Iprogs.Matesup.Models;
using Iprogs.Matesup.CoreAPI.Models;

using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using NLog;
using NLog.Web;
using System;
using System.Reflection;
using Iprogs.Matesup.CoreAPI.Services;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using Microsoft.AspNetCore.Server.HttpSys;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

//NLog.GlobalDiagnosticsContext.Set("AppDirectory", @"C:\logs\");
//NLog.Common.InternalLogger.LogFile = @"C:\logs\nlog-internal.log";

try
{
    var _localhostCorsPolicy = "Localhost_Development";

    string[] CorsAppsURLList = ["https://localhost:4200",
            "http://localhost:4200",
            "http://app.matesup.com",
            "https://app.matesup.com",
            "http://www.app.matesup.com",
            "https://www.app.matesup.com",
            "http://matesupdevapp.azurewebsites.net",
            "https://matesupdevapp.azurewebsites.net",
            "http://www.matesupdevapp.azurewebsites.net",
            "https://www.matesupdevapp.azurewebsites.net",
        ];

    var builder = WebApplication.CreateBuilder(args);
    var config = builder.Configuration;

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: _localhostCorsPolicy,
            policy =>
            {
                policy.WithOrigins(CorsAppsURLList).AllowAnyHeader().AllowAnyMethod();
                //policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
    });


    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    // Add services to the container.
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    builder.Services.AddDbContext<DevContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.
        AddIdentityApiEndpoints<IdentityUser<long>>().AddDefaultUI()
        //AddDefaultIdentity<IdentityUser<long>>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>();

    builder.Services.AddRazorPages();

    builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        // using System.Reflection;
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });

    builder.Services.AddAuthentication().AddBearerToken();

    //builder.Services.AddAuthentication(options =>
    //    {
    //        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //    })
    //    .AddCookie();
    // .AddGoogle(options =>
    // {
    //     IConfigurationSection googleAuthNSection = config.GetSection("Authentication:Google");
    //     options.ClientId = googleAuthNSection["ClientID"] ?? string.Empty;
    //     options.ClientSecret = googleAuthNSection["ClientSecret"] ?? string.Empty;
    // })
    // .AddFacebook(options =>
    // {
    //     IConfigurationSection FBAuthNSection =
    //     config.GetSection("Authentication:FB");
    //     options.ClientId = FBAuthNSection["ClientId"] ?? string.Empty;
    //     options.ClientSecret = FBAuthNSection["ClientSecret"] ?? string.Empty;
    // })
    //.AddMicrosoftAccount(microsoftOptions =>
    //{
    //    microsoftOptions.ClientId = config["Authentication:Microsoft:ClientId"] ?? string.Empty;
    //    microsoftOptions.ClientSecret = config["Authentication:Microsoft:ClientSecret"] ?? string.Empty;
    //})
    //.AddTwitter(twitterOptions =>
    //{
    //    twitterOptions.ConsumerKey = config["Authentication:Twitter:ConsumerAPIKey"] ?? string.Empty;
    //    twitterOptions.ConsumerSecret = config["Authentication:Twitter:ConsumerSecret"] ?? string.Empty;
    //    twitterOptions.RetrieveUserDetails = true;
    //});

    builder.Services.AddAuthorization();

    builder.Services.AddAutoMapper(typeof(MappingProfile));

    builder.Services.AddHttpContextAccessor();

    //builder.Services.AddCors(options =>
    //{
    //    options.AddPolicy(name: _localhostCorsPolicy,
    //        policy =>
    //        {
    //            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    //        });
    //});

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseRouting();

    app.MapIdentityApi<IdentityUser<long>>();

    app.UseCors(_localhostCorsPolicy);

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorPages();

    app.UseEndpoints(endpoints =>
    {
        ControllerActionEndpointConventionBuilder controllerActionEndpointConventionBuilder = endpoints.MapControllers().RequireCors(_localhostCorsPolicy);
    });

    app.UseStaticFiles();

    app.MapControllerRoute(
        name: "Default",
        pattern: "{controller}/{action}/{id}"
        );

    app.MapControllerRoute(
        name: "RazorPages",
        pattern: "{controller}/{action}/{id}"
        );

    app.MapControllerRoute(
        name: "API",
        pattern: "api/{controller}/{action}/{id}"
        );

    app.Use(async (context, next) =>
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Remove("Access-Control-Allow-Origin");
            context.Response.Headers.Append("Access-Control-Allow-Origin",
                "http://app.matesup.com, " +
                "https://app.matesup.com, ");

            context.Response.Headers.Remove("Access-Control-Allow-Headers");
            context.Response.Headers.Append("Access-Control-Allow-Headers", "*");

            context.Response.Headers.Remove("Access-Control-Allow-Methods");
            context.Response.Headers.Append("Access-Control-Allow-Methods", "*");

            return Task.FromResult(0);
        });

        await next.Invoke();
    });

    //app.MapSwagger().RequireAuthorization("Admin");

    app.Run();

}
catch (Exception ex)
{
    // NLog: catch setup errors
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}


public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<LookupCountry, LookupCountryDTO>().ReverseMap();
        CreateMap<LookupState, LookupStateDTO>().ReverseMap();
        CreateMap<LookupCity, LookupCityDTO>().ReverseMap();
        CreateMap<LookupGender, LookupGenderDTO>().ReverseMap();
        CreateMap<LookupChatRoomPrivacy, LookupChatRoomPrivacyDTO>().ReverseMap();
        CreateMap<LookupChatRoomType, LookupChatRoomTypeDTO>().ReverseMap();

        CreateMap<UserProfileMaster, UserProfileMasterDTO>().ReverseMap();
        CreateMap<ChatMaster, ChatMasterDTO>().ReverseMap();
        CreateMap<ChatRoomMaster, ChatRoomMasterDTO>().ReverseMap();
        CreateMap<ChatRoomUserMapping, ChatRoomUserMappingDTO>().ReverseMap();
        CreateMap<MegaPhoneMaster, MegaPhoneMasterDTO>().ReverseMap();
        CreateMap<UserPics, UserPicsDTO>().ReverseMap();

        CreateMap<UserProfileMaster, UserModalModel>().ReverseMap();
        CreateMap<ChatRoomMaster, RoomModalModel>().ReverseMap();
        CreateMap<ChatMaster, ChatMessagesModel>().ReverseMap();
        CreateMap<MegaPhoneMaster, AnnouncementsModel>().ReverseMap();
    }
}