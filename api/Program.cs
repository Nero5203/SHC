using adapters.DrivenAdapters.Data;
using application.UseCases.Users;
using ports.DrivenPorts;
using adapters.DrivenAdapters.Repositories;
using Microsoft.EntityFrameworkCore;
using adapters.DrivenAdapters.Auth;
using ports.DrivenPorts.Auth;


var builder = WebApplication.CreateBuilder(args);

var apiKey = builder.Configuration["SendGrid:ApiKey"];

builder.Services.AddScoped<IEmailSender>(_ =>
    new SendGridEmailSender(apiKey)
);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ShcDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserUseCase, UserUseCase>();


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
