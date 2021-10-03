using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToDoAPI.Data;
using ToDoAPI.Data.Repositories;
using ToDoAPI.Services;
using ToDoAPI.Settings;

namespace ToDoAPI
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
			string mysqlConnectionString = Configuration.GetConnectionString("DefaultConnection");
			services.AddDbContext<DataContext>(options =>
				options.UseMySql(mysqlConnectionString,
				ServerVersion.AutoDetect(mysqlConnectionString)));

			services.AddIdentity<IdentityUser, IdentityRole>(options =>
			{
				options.User = new UserOptions
				{
					RequireUniqueEmail = true,
				};
				options.Password = new PasswordOptions
				{
					RequiredLength = 12
				};
			}).AddEntityFrameworkStores<DataContext>();

			JwtSettings jwtSettings = new JwtSettings();
			Configuration.Bind(nameof(jwtSettings), jwtSettings);
			services.AddSingleton(jwtSettings);

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
					ValidateIssuer = false,
					ValidateAudience = false,
					RequireExpirationTime = false,
					ValidateLifetime = false
				};
			});

			services.AddControllers();

			services.AddSwaggerGen(options =>
			{
				options.AddSecurityRequirement(new OpenApiSecurityRequirement
					{
						{
							new OpenApiSecurityScheme
							{
								Reference = new OpenApiReference
								{
									Type = ReferenceType.SecurityScheme,
									Id = "Bearer"
								}
							},
							Array.Empty<string>()
						}
					});
				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Description = "Bearer JWT in Authorization header",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					Name = "Authorization"
				});
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				options.IncludeXmlComments(xmlPath);
			});

			services.AddScoped(typeof(IAccountService), typeof(AccountService));
			services.AddScoped(typeof(ITaskRepository), typeof(TaskRepository));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();

				app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "TODO API");
					c.RoutePrefix = string.Empty;
				});

				using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
				{
					var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();

					context.Database.EnsureDeleted();

					context.Database.Migrate();

					SeedData(context, serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>());
				}
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
		private void SeedData(DataContext context, UserManager<IdentityUser> userManager)
		{
			context.Roles.AddRange(new IdentityRole
			{
				Name = "Administrator",
				NormalizedName = "ADMINISTRATOR"
			},
			new IdentityRole
			{
				Name = "User",
				NormalizedName = "USER"
			});
			context.SaveChanges();

			IdentityUser admin = new IdentityUser
			{
				Email = "admin@email.com",
				UserName = "admin@email.com"
			};
			IdentityUser user = new IdentityUser
			{
				Email = "user@email.com",
				UserName = "user@email.com"
			};
			IdentityUser user2 = new IdentityUser
			{
				Email = "user2@email.com",
				UserName = "user2@email.com"
			};
			userManager.CreateAsync(admin, "Password@123").Wait();
			userManager.CreateAsync(user, "Password@123").Wait();
			userManager.CreateAsync(user2, "Password@123").Wait();
			userManager.AddToRoleAsync(admin, "Administrator").Wait();
			userManager.AddToRoleAsync(user, "User").Wait();
			userManager.AddToRoleAsync(user2, "User").Wait();

			context.Database.ExecuteSqlRaw($"INSERT INTO todotasks (UserId, Name, IsCompleted) VALUES ('{user.Id}', 'Create a task API', 1), ('{user.Id}', 'Get some sleep', 0)," +
				$"('{user2.Id}', 'Buy a laptop', 0),('{user2.Id}', 'Harness the wind', 0)");
		}
	}
}
