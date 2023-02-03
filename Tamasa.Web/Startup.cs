  using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
  using System.Reflection;
  using System.Text;
  using System.Threading.Tasks;
  using Tamasa.Core.interfere.IReposetory;
  using Tamasa.Core.Logic;
  using Tamasa.Inferastracter;
  using Microsoft.EntityFrameworkCore;
  using MediatR;
  using Microsoft.AspNetCore.Authentication.JwtBearer;
  using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

  namespace Tamasa.Web
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
            services.AddDbContext<AppDbContext>(opetions =>
                opetions.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddUnitOfWork<AppDbContext>();

            services.AddControllers();
            
            
            
            services.AddMediatR(typeof(Program).GetTypeInfo().Assembly);
            services.AddSingleton<IAES, AesOperator>();
            //services.AddMediatR(typeof(Startup));

            services.AddCors(options =>
            {
                options.AddPolicy(name: "_myAllowSpecificOrigins",
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:19006",
                                                          "http://www.contoso.com").AllowAnyHeader()
                                                  .AllowAnyMethod(); // add the allowed origins  
                                  });
            });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "My App API",
                    Description = "My App API - Version01",
                    TermsOfService = new Uri("https://devtube.ir/"),
                    License = new OpenApiLicense
                    {
                        Name = "DevTube",
                        Url = new Uri("https://devtube.ir/"),
                    }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "Bearer",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
                });

            });

            var sp = services.BuildServiceProvider();

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
                    ClockSkew = TimeSpan.FromMinutes(30),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TamasaTamasaTamasaTamasaTamasaTamasa1234")),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });


            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(opetion =>
            //    {
            //        opetion.TokenValidationParameters = new TokenValidationParameters()
            //        {
            //            ValidateIssuerSigningKey =  true,
            //            IssuerSigningKey =new SymmetricSecurityKey(Encoding.UTF8.GetBytes("123*/")),
            //            ValidateIssuer = false,
            //            ValidateLifetime = true
            //        };
            //    });


            services.AddHttpContextAccessor();
            //services.addMediator

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IUnitOfWork<AppDbContext> unitOfWork)
        {


            unitOfWork.DbContext.Database.GetAppliedMigrations();
            unitOfWork.DbContext.Database.Migrate();



            MapsterDtosConfigurations
                .Instance
                .Initialize();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();    
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tamasa.Web v1"));
                ///app.UseCors("CorsPolicy");
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("_myAllowSpecificOrigins");
            app.UseAuthentication();
            app.UseAuthorization();




            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
