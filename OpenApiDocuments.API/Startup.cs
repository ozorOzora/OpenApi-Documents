﻿using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using OpenApiDocuments.API;
using OpenApiDocuments.Core.BLL;
using OpenApiDocuments.Core.BO;
using OpenApiDocuments.Core.DAL;
using OpenApiDocuments.Core.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace OpenApiDocuments
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddAutoMapper(typeof(MappingProfile)); // since 9.0, use injection: https://docs.automapper.org/en/stable/9.0-Upgrade-Guide.html

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Version = "v1", Title = "OpenApiDocuments" });
            });

            services.AddTransient<GitHubService>();
            services.AddTransient<DocumentManager>();
            services.AddTransient<IDocumentRepository, DocumentRepository>();
            services.AddTransient<IDocumentMetadataRepository, DocumentMetadataRepository>();
            services.AddTransient<MongoDbContext>();

            // see https://stackoverflow.com/questions/56584655/c-sharp-mongodb-serialize-enum-dictionary-keys-to-string
            BsonSerializer.RegisterSerializer(new EnumSerializer<OperationType>(BsonType.String));
            BsonClassMap.RegisterClassMap<DocumentMetadata>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenApiDocuments");
            });
        }
    }
}
