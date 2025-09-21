using AdPlatforms.Services;
using AdPlatforms.Repository;
using AdPlatforms.Repository.Impl;
using AdPlatforms.Services.Impl;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IDataRepository, DataRepository>();
builder.Services.AddScoped<IDataService, DataService>();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Advertisement platforms",
        Description = "Implementation of Web API for uploading and searching advertisement platforms.",
        Contact = new OpenApiContact
        {
            Name = "Andrew Genergard",
            Email = "av.genergard@gmail.com",
            Url = new Uri("https://t.me/klasster13")
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    swagger.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AdPlatformsAPI");
    c.RoutePrefix = "api/swagger";
});

app.UseRouting();
app.MapControllers();

app.Run();
