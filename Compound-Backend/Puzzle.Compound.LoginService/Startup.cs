using AutoWrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
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

namespace Puzzle.Compound.LoginService {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			services.AddAutoMapper(typeof(CompanyProfile));
			services.AddControllers();
			services.AddDbContext<CompoundDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("CompoundDbConnection")));

			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddDbContext<CompoundDbContext>(item => item.UseSqlServer(Configuration.GetConnectionString("CompoundDbConnection")));

			services.AddCors(options =>
			{
				options.AddPolicy("AllowAllOrigins",
								builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
			});

			//// configure strongly typed settings objects
			//var appSettingsSection = Configuration.GetSection("AppSettings");
			//services.Configure<AppSettings>(appSettingsSection);

			// configure jwt authentication
			var jwtKey = Configuration.GetSection("Security:JWTKey").Value;
			var key = Encoding.ASCII.GetBytes(jwtKey);

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

			services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
			services.AddScoped<ICompanyUserRepository, CompanyUserRepository>();
			services.AddScoped<IOwnerRegistrationRepository, OwnerRegistrationRepository>();
			services.AddScoped<IGateRepository, GateRepository>();
			services.AddScoped<ICompoundRepository, CompoundRepository>();
			services.AddScoped<ICompanyRoleRepository, CompanyRoleRepository>();
			services.AddScoped<ICompanyRoleActionsRepository, CompanyRoleActionsRepository>();
			services.AddScoped<ICompanyUserRoleRepository, CompanyUserRoleRepository>();
			services.AddScoped<ICompoundRepository, CompoundRepository>();

			services.AddScoped<ICompanyUserService, CompanyUserService>();
			services.AddScoped<ITokenService, TokenService>();
			services.AddScoped<IS3Service, S3Service>();
			services.AddScoped<IOwnerRegistrationService, OwnerRegistrationService>();
			services.AddScoped<IGateService, GateService>();
			services.AddScoped<ICompanyRoleService, CompanyRoleService>();
			services.AddScoped<ICompanyUserRoleService, CompanyUserRoleService>();
			services.AddScoped<ICompoundService, Services.CompoundService>();

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

			services.AddSwaggerGen();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			app.UseApiResponseAndExceptionWrapper<PuzzleApiResponse>(new AutoWrapperOptions { ShowStatusCode = true, UseCustomSchema = true, IgnoreNullValue = false });

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c => {
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Login Service");
			});

			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			// global cors policy
			app.UseCors("AllowAllOrigins");

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
