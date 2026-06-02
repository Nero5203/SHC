using adapters.DrivenAdapters.Data;
using application.UseCases.Users;
using ports.DrivenPorts;
using ports.DrivingPorts;
using adapters.DrivenAdapters.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ShcDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGetUserByIdUseCase, GetUserByIdUseCase>();
builder.Services.AddScoped<IGetUserSettingsUseCase, GetUserSettingsUseCase>();
builder.Services.AddScoped<IUpdateUserProfileUseCase, UpdateUserProfileUseCase>();
builder.Services.AddScoped<IUpdateUserSettingsUseCase, UpdateUserSettingsUseCase>();
builder.Services.AddScoped<IDeleteUserUseCase, DeleteUserUseCase>();


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
