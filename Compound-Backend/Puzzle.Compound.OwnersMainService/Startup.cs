using AutoMapper;
using AutoWrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Puzzle.Compound.Amazon;
using Puzzle.Compound.Common.Hubs;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Mapper.Profiles;
using Puzzle.Compound.Models.PushNotifications;
using Puzzle.Compound.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Compound.OwnersMainService
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
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddDbContext<CompoundDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("CompoundDbConnection")));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                        builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IOwnerUnitRepository, OwnerUnitRepository>();
            services.AddScoped<IOwnerRegistrationRepository, OwnerRegistrationRepository>();
            services.AddScoped<IOwnerUnitRequestRepository, OwnerUnitRequestRepository>();
            services.AddScoped<ICompoundOwnerRepository, CompoundOwnerRepository>();
            services.AddScoped<ICompoundUnitRepository, CompoundUnitRepository>();
            services.AddScoped<IOwnerAssignedUnitRepository, OwnerAssignedUnitRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ICompanyUserRepository, CompanyUserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ICompoundRepository, CompoundRepository>();
            services.AddScoped<IServiceTypeRepository, ServiceTypeRepository>();
            services.AddScoped<ICompoundServiceRepository, CompoundServiceRepository>();
            services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
            services.AddScoped<IIssueTypeRepository, IssueTypeRepository>();
            services.AddScoped<ICompoundIssueRepository, CompoundIssueRepository>();
            services.AddScoped<IIssueRequestRepository, IssueRequestRepository>();
            services.AddScoped<IServiceSubTypeRepository, ServiceSubTypeRepository>();
            services.AddScoped<IServiceRequestSubTypeRepository, ServiceRequestSubTypeRepository>();
            services.AddScoped<IRegistrationForUserRepository, RegistrationForUserRepository>();
            services.AddScoped<INotificationScheduleRepository, NotificationScheduleRepository>();
            services.AddScoped<IVisitorRequestRepository, VisitorRequestRepository>();
            services.AddScoped<ICompoundNotificationRepository, CompoundNotificationRepository>();
            services.AddScoped<ICompoundAdRepository, CompoundAdRepository>();
            services.AddScoped<ICompoundNewsRepository, CompoundNewsRepository>();

            services.AddScoped<IS3Service, S3Service>();
            services.AddScoped<IOwnerUnitService, OwnerUnitService>();
            services.AddScoped<IOwnerRegistrationService, OwnerRegistrationService>();
            services.AddScoped<IOwnerUnitRequestService, OwnerUnitRequestService>();
            services.AddScoped<ICompoundOwnerService, CompoundOwnerService>();
            services.AddScoped<ICompoundUnitService, CompoundUnitService>();
            services.AddScoped<IOwnerAssignedUnitService, OwnerAssignedUnitService>();
            services.AddScoped<IReportTypeRepository, ReportTypeRepository>();
            services.AddScoped<ICompanyRoleRepository, CompanyRoleRepository>();
            services.AddScoped<ICompanyRoleActionsRepository, CompanyRoleActionsRepository>();
            services.AddScoped<ICompanyRoleService, CompanyRoleService>();
            services.AddScoped<ICompanyUserRoleRepository, CompanyUserRoleRepository>();
            services.AddScoped<ICompanyUserRoleService, CompanyUserRoleService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<ICompanyUserService, CompanyUserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICompoundService, Services.CompoundService>();
            services.AddScoped<IServiceRequestService, ServiceRequestService>();
            services.AddScoped<IIssueRequestService, IssueRequestService>();
            //Push Notification
            services.AddScoped<IRegistrationForUsersService, RegistrationForUsersService>();
            services.AddScoped<IPushNotificationService, PushNotificationService>();
            services.Configure<NotificationMessages>(Configuration.GetSection("NotificationMessages"));
            services.Configure<RouteAndroid>(Configuration.GetSection("RouteAndroid"));
            //
            services.AddHttpContextAccessor();
            services.AddScoped<UserIdentity>(provider =>
            {
                var user = provider.GetService<IHttpContextAccessor>().HttpContext.User;
                var name = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                Guid? id = null;
                if (!string.IsNullOrEmpty(name))
                    id = Guid.Parse(name);

                return new UserIdentity(id);
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
            app.UseApiResponseAndExceptionWrapper(new AutoWrapperOptions { ShowStatusCode = true, UseCustomSchema = true, IgnoreNullValue = false, UseApiProblemDetailsException = true });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Owners Service");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
