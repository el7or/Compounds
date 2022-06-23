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
using Puzzle.Compound.Amazon;
using Puzzle.Compound.Common;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Mapper.Profiles;
using Puzzle.Compound.Services;
using Puzzle.Compound.VisitsService.Middleware;
using Serilog;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Hosting;
using Puzzle.Compound.Common.Hubs;
using Puzzle.Compound.Models.PushNotifications;

namespace Puzzle.Compound.VisitsService {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			services.AddAutoMapper(typeof(CompanyProfile));
			services.AddControllers();
			services.AddDbContext<CompoundDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("CompoundDbConnection"), x => x.UseNetTopologySuite()));
			services.AddCors(options => {
				options.AddPolicy("AllowAllOrigins",
						builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
			});
			var key = Encoding.ASCII.GetBytes("1d6fdd5c-bab6-4baa-859f-a1f83f923c68");
			services.AddAuthentication(x => {
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(x => {
				x.RequireHttpsMetadata = false;
				x.SaveToken = true;
				x.TokenValidationParameters = new TokenValidationParameters {
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false,
								// set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
								ClockSkew = TimeSpan.Zero
				};
			});

			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddScoped<IVisitorRequestRepository, VisitorRequestRepository>();
			services.AddScoped<IOwnerUnitRepository, OwnerUnitRepository>();
			services.AddScoped<ICompoundGateRepository, CompoundGateRepository>();
			services.AddScoped<IPrintCardRequestRepository, PrintCardRequestRepository>();
			services.AddScoped<IVisitTransactionHistoryRepository, VisitTransactionHistoryRepository>();
			services.AddScoped<IOwnerAssignedUnitRepository, OwnerAssignedUnitRepository>();
			services.AddScoped<IVisitRequestService, VisitRequestService>();
			services.AddScoped<IPrintCardRequestService, PrintCardRequestService>();
			services.AddScoped<IVisitTranscationHistoryService, VisitTranscationHistoryService>();
			services.AddScoped<IRegistrationForUserRepository, RegistrationForUserRepository>();
			services.AddScoped<INotificationScheduleRepository, NotificationScheduleRepository>();
			services.AddScoped<IVisitRequestService, VisitRequestService>();
			services.AddScoped<IOwnerRegistrationRepository, OwnerRegistrationRepository>(); 
			services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>(); 
			services.AddScoped<ICompoundNotificationRepository, CompoundNotificationRepository>(); 
			services.AddScoped<IIssueRequestRepository, IssueRequestRepository>(); 
			services.AddScoped<ICompoundAdRepository, CompoundAdRepository>(); 
			services.AddScoped<ICompoundNewsRepository, CompoundNewsRepository>();
			//Push Notification
			services.AddScoped<IRegistrationForUsersService, RegistrationForUsersService>();
			services.AddScoped<IPushNotificationService, PushNotificationService>();
			services.Configure<NotificationMessages>(Configuration.GetSection("NotificationMessages"));
			services.Configure<RouteAndroid>(Configuration.GetSection("RouteAndroid"));
			//
			services.AddScoped<IS3Service, S3Service>();

			services.AddHttpContextAccessor();
			services.AddScoped<UserIdentity>(provider => {
				var user = provider.GetService<IHttpContextAccessor>().HttpContext.User;
				var name = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
				Guid? id = null;
				if (!string.IsNullOrEmpty(name))
					id = Guid.Parse(name);

				return new UserIdentity(id);
			});

			services.AddSignalR(hubOptions =>
			{
				hubOptions.EnableDetailedErrors = true;
				//hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(15);
			});

			services.AddSwaggerGen(c => {
				var securitySchema = new OpenApiSecurityScheme {
					Description = "JWT Auth Bearer Scheme",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.Http,
					Scheme = "Bearer",
					Reference = new OpenApiReference {
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
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			app.UseSerilogRequestLogging();

			app.UseApiResponseAndExceptionWrapper<PuzzleApiResponse>(new AutoWrapperOptions { ShowStatusCode = true, UseCustomSchema = true, IgnoreNullValue = false });

			app.UseMiddleware<ExceptionMiddleware>();
			app.UseSwagger();

			app.UseSwaggerUI(c => {
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Visits Service");
			});
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();
			app.UseCors("AllowAllOrigins");
			app.UseRouting();

			app.UseAuthentication();

			app.UseAuthorization();

			app.UseEndpoints(endpoints => {
				endpoints.MapControllers();
				endpoints.MapHub<CounterHub>("/counterhub");
			});
		}
	}
}
