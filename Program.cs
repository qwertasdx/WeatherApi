using Microsoft.OpenApi.Models;
using WeatherApi.Services;

var builder = WebApplication.CreateBuilder(args);

// �K�[ CORS �A��
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:5253") // Weather MVC �M�ת� URL
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

builder.Services.AddControllers();

// �K�[ Swagger �A��
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo

    {
        Version = "v1",
        Title = "WeatherApi",
        Description = "�Ѯ�API",
    });
});
builder.Services.AddHttpClient<WeatherService>();

var app = builder.Build();

// �ϥ� CORS
app.UseCors("AllowSpecificOrigin");

// �ϥ� Swagger �M Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
