//BEGIN LICENSE BLOCK 
//Interneuron Autonomic

//Copyright(C) 2025  Interneuron Limited

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

//See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.If not, see<http://www.gnu.org/licenses/>.
//END LICENSE BLOCK 
﻿//Interneuron Synapse

//Copyright(C) 2023  Interneuron Holdings Ltd

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

//See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.If not, see<http://www.gnu.org/licenses/>.

using System;
using InterneuronAutonomic.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Generic;
using Interneuron.Common.Extensions;
using Elastic.Apm.NetCoreAll;
using Elastic.Apm.Api;
using Elastic.Apm;
using Interneuron.Infrastructure.Web.Exceptions.Handlers;
using Serilog;

namespace Interneuron.Autonomic
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
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllHeaders",
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
            });

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });

            services.AddHttpContextAccessor();

            string SynapseDynamicAPIURI = Configuration.GetSection("DynamicAPISettings").GetSection("uri").Value;
            services.AddHttpClient();
            services.AddHttpClient<DynamicAPIClient>(clientConfig =>
            {
                clientConfig.BaseAddress = new Uri(SynapseDynamicAPIURI);
            });

            //Authorization

            services.AddAuthentication("Bearer")
              .AddIdentityServerAuthentication(options =>
              {
                  options.Authority = Configuration["Settings:AuthorizationAuthority"];
                  options.RequireHttpsMetadata = false;
                  options.ApiName = Configuration["Settings:AuthorizationAudience"];
                  options.EnableCaching = false;

              });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DynamicAPIWriters", builder =>
                {
                    builder.RequireClaim(Configuration["Settings:SynapseRolesClaimType"], Configuration["Settings:DynamicAPIWriteAccessRole"]);
                    builder.RequireScope(Configuration["Settings:WriteAccessAPIScope"]);
                });
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DynamicAPIReaders", builder =>
                {
                    builder.RequireClaim(Configuration["Settings:SynapseRolesClaimType"], Configuration["Settings:DynamicAPIReadAccessRole"]);
                    builder.RequireScope(Configuration["Settings:ReadAccessAPIScope"]);
                });
            });

            var swaggerAccessScopes = Configuration.GetSection("Swagger").GetValue<string>("AccessScopes");

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Autonomic API", Version = "v1" });

                // Define the OAuth2 scheme
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(Configuration["Settings:AuthorizationAuthority"] + "/connect/authorize"), 
                            TokenUrl = new Uri(Configuration["Settings:AuthorizationAuthority"] + "/connect/token"), 
                            Scopes = GetScopes(swaggerAccessScopes)
                        }
                    }
                });

                // Add security requirement to operations
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            }
                        },
                        GetScopeKeys(swaggerAccessScopes)
                    }
                });

            });

           services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());

           services.AddAllElasticApm();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseInterneuronExceptionHandler(options =>
            {
                options.OnExceptionHandlingComplete = (ex, errorId) =>
                {
                    LogException(ex, errorId);
                };
            });

            app.UseAuthentication();

            app.UseCors("AllowAllHeaders");
            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Autonomic API v1");
                options.OAuthClientId(Configuration["Swagger:OAuthClientId"]);
            });

            app.UseHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });

            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        }

        private static Dictionary<string, string> GetScopes(string swaggerAccessScopes)
        {
            var scopes = new Dictionary<string, string>();

            swaggerAccessScopes.Split(';')
                            .Each(scopeUnit =>
                            {
                                if (scopeUnit.IsNotEmpty())
                                {
                                    var scopeKV = scopeUnit.Split(':');
                                    if (scopeKV.IsCollectionValid())
                                    {
                                        scopes[scopeKV[0]] = scopeKV[1];
                                    }
                                }
                            });
            return scopes;
        }

        private static List<string> GetScopeKeys(string swaggerAccessScopes)
        {
            var scopes = new List<string>();

            swaggerAccessScopes.Split(';')
                            .Each(scopeUnit =>
                            {
                                if (scopeUnit.IsNotEmpty())
                                {
                                    var scopeKV = scopeUnit.Split(':');
                                    if (scopeKV.IsCollectionValid())
                                    {
                                        scopes.Add(scopeKV[0]);
                                    }
                                }
                            });
            return scopes;
        }

        private static void LogException(Exception ex, string errorId)
        {
            if (Agent.Tracer != null && Agent.Tracer.CurrentTransaction != null)
            {
                ITransaction transaction = Agent.Tracer.CurrentTransaction;
                transaction.SetLabel("ErrorId", errorId);
            }

            if (ex.Message.StartsWith("cannot open database", StringComparison.InvariantCultureIgnoreCase) || ex.Message.StartsWith("a network", StringComparison.InvariantCultureIgnoreCase))
                Log.Logger.ForContext("ErrorId", errorId).Fatal(ex, ex.Message);
            else
                Log.Logger.ForContext("ErrorId", errorId).Error(ex, ex.Message);
        }
    }
}
