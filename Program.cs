using Microsoft.OpenApi.Models;
using WeatherApi.Services;

var builder = WebApplication.CreateBuilder(args);

// 添加 CORS 服務
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:5253") // Weather MVC 專案的 URL
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

builder.Services.AddControllers();

// 添加 Swagger 服務
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo

    {
        Version = "v1",
        Title = "WeatherApi",
        Description = "天氣API",
    });
});
builder.Services.AddHttpClient<WeatherService>();

var app = builder.Build();

// 使用 CORS
app.UseCors("AllowSpecificOrigin");

// 使用 Swagger 和 Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
