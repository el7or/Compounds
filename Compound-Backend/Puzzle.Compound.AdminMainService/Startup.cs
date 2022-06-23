using AutoMapper;
using AutoWrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Puzzle.Compound.AdminMainService.Middlewares;
using Puzzle.Compound.Amazon;
using Puzzle.Compound.Common;
using Puzzle.Compound.Common.Hubs;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Mapper.Profiles;
using Puzzle.Compound.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Puzzle.Compound.Models.PushNotifications;
using Newtonsoft.Json.Converters;
using Puzzle.Compound.AdminMainService.SwaggerHelper;

namespace Puzzle.Compound.AdminMainService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(CompanyProfile));
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            services.AddDbContext<CompoundDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("CompoundDbConnection"), x => x.UseNetTopologySuite()));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                                builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ICompoundRepository, CompoundRepository>();
            services.AddScoped<ICompoundGroupRepository, CompoundGroupRepository>();
            services.AddScoped<ICompoundOwnerRepository, CompoundOwnerRepository>();
            services.AddScoped<IPlanRepository, PlanRepository>();
            services.AddScoped<ICompoundUnitRepository, CompoundUnitRepository>();
            services.AddScoped<ICompoundUnitTypeRepository, CompoundUnitTypeRepository>();
            services.AddScoped<ICompanyUserRepository, CompanyUserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IOwnerUnitRepository, OwnerUnitRepository>();
            services.AddScoped<IGateRepository, GateRepository>();
            services.AddScoped<IOwnerUnitRequestRepository, OwnerUnitRequestRepository>();
            services.AddScoped<IOwnerRegistrationRepository, OwnerRegistrationRepository>();
            services.AddScoped<IOwnerAssignedUnitRepository, OwnerAssignedUnitRepository>();
            services.AddScoped<ICompoundNewsRepository, CompoundNewsRepository>();
            services.AddScoped<IServiceTypeRepository, ServiceTypeRepository>();
            services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
            services.AddScoped<ICompoundServiceRepository, CompoundServiceRepository>();
            services.AddScoped<IIssueTypeRepository, IssueTypeRepository>();
            services.AddScoped<IIssueRequestRepository, IssueRequestRepository>();
            services.AddScoped<ICompoundIssueRepository, CompoundIssueRepository>();
            services.AddScoped<ICompoundNotificationRepository, CompoundNotificationRepository>();
            services.AddScoped<IReportTypeRepository, ReportTypeRepository>();
            services.AddScoped<ISystemPageRepository, SystemPageRepository>();
            services.AddScoped<ISystemPageActionRepository, SystemPageActionRepository>();
            services.AddScoped<ICompanyRoleRepository, CompanyRoleRepository>();
            services.AddScoped<ICompanyUserRoleRepository, CompanyUserRoleRepository>();
            services.AddScoped<ICompanyRoleActionsRepository, CompanyRoleActionsRepository>();
            services.AddScoped<IReportTypeRepository, ReportTypeRepository>();
            services.AddScoped<IServiceSubTypeRepository, ServiceSubTypeRepository>();
            services.AddScoped<IServiceRequestSubTypeRepository, ServiceRequestSubTypeRepository>();
            services.AddScoped<IVisitorRequestRepository, VisitorRequestRepository>();
            services.AddScoped<ICompoundAdRepository, CompoundAdRepository>();
            services.AddScoped<IRegistrationForUserRepository, RegistrationForUserRepository>();
            services.AddScoped<INotificationScheduleRepository, NotificationScheduleRepository>();

            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<ICompoundService, Services.CompoundService>();
            services.AddScoped<ICompoundGroupService, CompoundGroupService>();
            services.AddScoped<ICompoundOwnerService, CompoundOwnerService>();
            services.AddScoped<IPlanService, PlanService>();
            services.AddScoped<ICompoundUnitService, CompoundUnitService>();
            services.AddScoped<ICompoundUnitTypeService, CompoundUnitTypeService>();
            services.AddScoped<ICompanyUserService, CompanyUserService>();
            services.AddScoped<IOwnerUnitService, OwnerUnitService>();
            services.AddScoped<IOwnerUnitRequestService, OwnerUnitRequestService>();
            services.AddScoped<IOwnerRegistrationService, OwnerRegistrationService>();
            services.AddScoped<IOwnerAssignedUnitService, OwnerAssignedUnitService>();
            services.AddScoped<ICompoundNewsService, CompoundNewsService>();
            services.AddScoped<IServiceRequestService, ServiceRequestService>();
            services.AddScoped<IIssueRequestService, IssueRequestService>();
            services.AddScoped<ICompoundNotificationService, CompoundNotificationService>();
            services.AddScoped<ICompoundReportService, CompoundReportService>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddScoped<ISystemPageService, SystemPageService>();
            services.AddScoped<ISystemPageActionService, SystemPageActionService>();
            services.AddScoped<ICompanyRoleService, CompanyRoleService>();
            services.AddScoped<ICompanyUserRoleService, CompanyUserRoleService>();
            services.AddScoped<ICompanyRoleActionsService, CompanyRoleActionsService>();
            services.AddScoped<IServiceSubTypeService, ServiceSubTypeService>();

            //Push Notification
            services.AddScoped<IRegistrationForUsersService, RegistrationForUsersService>();
            services.AddScoped<IPushNotificationService, PushNotificationService>();
            services.Configure<NotificationMessages>(Configuration.GetSection("NotificationMessages"));
            services.Configure<RouteAndroid>(Configuration.GetSection("RouteAndroid"));
            //
            services.AddScoped<IS3Service, S3Service>();
            services.AddScoped<IGateService, GateService>();

            services.AddHttpContextAccessor();
            services.AddScoped<UserIdentity>(provider =>
            {
                var user = provider.GetService<IHttpContextAccessor>().HttpContext.User;
                var name = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                Guid? id = null;
                if (!string.IsNullOrEmpty(name))
                    id = Guid.Parse(name);

                var companyClaim = user?.Claims?.FirstOrDefault(x => x.Type == "CompanyId")?.Value;
                Guid? companyId = null;
                if (!string.IsNullOrEmpty(companyClaim))
                    companyId = Guid.Parse(companyClaim);


                return new UserIdentity(id, companyId);
            });

            // configure jwt authentication
            var jwtKey = Configuration.GetSection("Security:JWTKey").Value;
            var key = Encoding.ASCII.GetBytes(jwtKey);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/chathub")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                //hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(15);
            });

            services.AddSwaggerGen(c =>
            {
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Auth Bearer Scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.SchemaFilter<EnumSchemaFilter>();
                c.AddSecurityDefinition("Bearer", securitySchema);
                var securityRequirments = new OpenApiSecurityRequirement
                {
                    {securitySchema, new []{"Bearer"}}
                };
                c.AddSecurityRequirement(securityRequirments);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseApiResponseAndExceptionWrapper<PuzzleApiResponse>(new AutoWrapperOptions { ShowStatusCode = true, UseCustomSchema = true, IgnoreNullValue = false });

            app.UseMiddleware<ExceptionMiddleware>();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Admin Main Service");
            });

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            app.UseHttpsRedirection();

            app.UseCors("AllowAllOrigins");
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<CounterHub>("/counterhub");
            });
        }
    }
}
