﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GHDWebAPI.Data;
using Microsoft.AspNetCore.Mvc;
using GHDWebAPI.Model;
using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using System.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Immutable;
using System.Reflection;
using GHDWebAPI.Services;
using GHDWebAPI.Controllers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<GHDWebAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GHDWebAPIContext") ?? throw new InvalidOperationException("Connection string 'GHDWebAPIContext' not found.")));

// Add services to the container.

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
    
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ILogger, Logger<ProductsController>>(); ;

builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "GHD Product API",
        Description = "API for retrieving products",
        Contact = new OpenApiContact
        {
            Name = "Subha Venkatasubramanian",
            Email = "subhadeepak3011@gmail.com"
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    s.IncludeXmlComments(xmlPath);
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
