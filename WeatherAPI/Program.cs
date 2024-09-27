
using Microsoft.EntityFrameworkCore;
using WeatherAPI.Data;
using WeatherAPI.Repositories;
using WeatherAPI.Services;
using WeatherAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Allowlocalhost",
        policy =>
        {
            policy
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .SetIsOriginAllowed((host) => true)
                  .AllowCredentials();
        }
    );
});

builder.Services.AddDbContext<SubcsribersDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("SubcribersConnectionString")));

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddHostedService<WeatherEmailService>();

var app = builder.Build();



// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseCors("Allowlocalhost");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
