using AdsPlatform.Data;
using AdsPlatform.Data.Impl;
using AdsPlatform.Domain.ParsingService;
using AdsPlatform.Domain.ParsingService.Impl;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IDataRepository, DataRepository>();
builder.Services.AddScoped<IDataService, DataService>();


builder.Services.AddOpenApi();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
