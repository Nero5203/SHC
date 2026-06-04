using adapters.Driven.Persistence.Data;
using application.UseCases.Users;
using application.UseCases.Purchases;
using application.UseCases.LinkSharing;
using application.UseCases.StorageNodes;
using application.Ports.Driven;
using application.Ports.Driving;
using adapters.Driven.Persistence.Repositories;
using adapters.Driven.Persistence.Repositories.Purchases;
using adapters.Driven.Persistence.Repositories.LinkSharing;
using adapters.Driven.Persistence.Repositories.StorageNodes;
using application.Ports.Driven.Purchases;
using application.Ports.Driving.Purchases;
using application.Ports.Driven.LinkSharing;
using application.Ports.Driving.LinkSharing;
using application.Ports.Driven.StorageNodes;
using application.Ports.Driving.StorageNodes;
using Microsoft.EntityFrameworkCore;
using adapters.Driven.ExternalServices.Auth;
using application.Ports.Driven.Auth;
using application.Ports.Driving.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using adapters.Driven.Persistence.Repositories.Auth;
using Application.UseCases.Auth;
using adapters.Driving.Api.Mapping;
using Microsoft.Extensions.DependencyInjection;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ShcDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));



builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGetUserByEmailUseCase, GetUserByEmailUseCase>();
builder.Services.AddScoped<IGetUserByIdUseCase, GetUserByIdUseCase>();
builder.Services.AddScoped<IGetUserSettingsUseCase, GetUserSettingsUseCase>();
builder.Services.AddScoped<IUpdateUserProfileUseCase, UpdateUserProfileUseCase>();
builder.Services.AddScoped<IUpdateUserSettingsUseCase, UpdateUserSettingsUseCase>();
builder.Services.AddScoped<IDeleteUserUseCase, DeleteUserUseCase>();
builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
builder.Services.AddScoped<ICreatePurchaseUseCase, CreatePurchaseUseCase>();
builder.Services.AddScoped<IGetPurchaseByIdUseCase, GetPurchaseByIdUseCase>();
builder.Services.AddScoped<IGetPurchasesByUserIdUseCase, GetPurchasesByUserIdUseCase>();
builder.Services.AddScoped<IUpdatePurchaseStatusUseCase, UpdatePurchaseStatusUseCase>();
builder.Services.AddScoped<IGetInvoiceByPurchaseIdUseCase, GetInvoiceByPurchaseIdUseCase>();
builder.Services.AddScoped<ISharedLinkRepository, SharedLinkRepository>();
builder.Services.AddScoped<ICreateSharedLinkUseCase, CreateSharedLinkUseCase>();
builder.Services.AddScoped<IGetSharedLinkByIdUseCase, GetSharedLinkByIdUseCase>();
builder.Services.AddScoped<IGetSharedLinkByTokenUseCase, GetSharedLinkByTokenUseCase>();
builder.Services.AddScoped<IGetSharedLinksByUserIdUseCase, GetSharedLinksByUserIdUseCase>();
builder.Services.AddScoped<IUpdateSharedLinkUseCase, UpdateSharedLinkUseCase>();
builder.Services.AddScoped<IDeactivateSharedLinkUseCase, DeactivateSharedLinkUseCase>();
builder.Services.AddScoped<IStorageNodeRepository, StorageNodeRepository>();
builder.Services.AddScoped<ICreateStorageNodeUseCase, CreateStorageNodeUseCase>();
builder.Services.AddScoped<IGetStorageNodeByIdUseCase, GetStorageNodeByIdUseCase>();
builder.Services.AddScoped<IGetStorageNodesUseCase, GetStorageNodesUseCase>();
builder.Services.AddScoped<IGetBestAvailableStorageNodeUseCase, GetBestAvailableStorageNodeUseCase>();
builder.Services.AddScoped<IUpdateStorageNodeUseCase, UpdateStorageNodeUseCase>();
builder.Services.AddScoped<IUpdateStorageNodeHeartbeatUseCase, UpdateStorageNodeHeartbeatUseCase>();
builder.Services.AddScoped<IUpdateStorageNodeStatusUseCase, UpdateStorageNodeStatusUseCase>();
builder.Services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUserCredentialRepository, EfUserCredentialRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasherAdapter>();
builder.Services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
