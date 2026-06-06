using adapters.Driven.Persistence.Data;
using application.UseCases.Users;
using application.UseCases.Purchases;
using application.UseCases.LinkSharing;
using application.UseCases.StorageNodes;
using application.UseCases.Trash;
using application.UseCases.Settings;
using application.UseCases.Permissions;
using application.UseCases.Notifications;
using application.UseCases.Roles;
using application.UseCases.FileStorage.File;
using application.UseCases.FileStorage.Folder;
using application.Ports.Driven;
using application.Ports.Driving;
using adapters.Driven.Persistence.Repositories;
using adapters.Driven.Persistence.Repositories.Purchases;
using adapters.Driven.Persistence.Repositories.LinkSharing;
using adapters.Driven.Persistence.Repositories.StorageNodes;
using adapters.Driven.Persistence.Repositories.Trash;
using adapters.Driven.Persistence.Repositories.Settings;
using adapters.Driven.Persistence.Repositories.Permissions;
using adapters.Driven.Persistence.Repositories.Notifications;
using adapters.Driven.Persistence.Repositories.Roles;
using adapters.Driven.Persistence.Repositories.FileStorage;
using application.Ports.Driven.Purchases;
using application.Ports.Driving.Subscriptions;
using application.Ports.Driving.Purchases;
using application.Ports.Driven.Payments;
using application.Ports.Driving.Payments;
using adapters.Driven.ExternalServices.Payments;
using application.UseCases.Payments;
using application.Ports.Driven.LinkSharing;
using application.Ports.Driving.LinkSharing;
using application.Ports.Driven.StorageNodes;
using application.Ports.Driving.StorageNodes;
using application.Ports.Driven.Trash;
using application.Ports.Driving.Trash;
using application.Ports.Driven.Settings;
using application.Ports.Driving.Settings;
using application.Ports.Driven.Permissions;
using application.Ports.Driving.Permissions;
using application.Ports.Driven.Notifications;
using application.Ports.Driving.Notifications;
using application.Ports.Driven.Roles;
using application.Ports.Driving.Roles;
using application.Ports.Driven.FileStorage;
using application.Ports.Driving.FileStorage.File;
using application.Ports.Driving.FileStorage.Folder;
using application.UseCases.Subscriptions;
using Microsoft.EntityFrameworkCore;
using adapters.Driven.ExternalServices.Auth;
using AutoMapper;
using application.Ports.Driven.Auth;
using application.Ports.Driving.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using adapters.Driven.Persistence.Repositories.Auth;
using Application.UseCases.Auth;
using Microsoft.Extensions.DependencyInjection;
using application.UseCases.Auth;
using api.Authorization;
using application.Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Stripe;




var builder = WebApplication.CreateBuilder(args);
const string frontendCorsPolicy = "FrontendCorsPolicy";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT access token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                "http://127.0.0.1:5173",
                "http://localhost:5173",
                "http://127.0.0.1:5175",
                "http://localhost:5175")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.FileRead, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new PermissionRequirement(AuthorizationPermissions.FileRead));
    });

    options.AddPolicy(AuthorizationPolicies.FileUpload, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new PermissionRequirement(AuthorizationPermissions.FileUpload));
    });

    options.AddPolicy(AuthorizationPolicies.FileDelete, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new PermissionRequirement(AuthorizationPermissions.FileDelete));
    });

    options.AddPolicy(AuthorizationPolicies.FileShare, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new PermissionRequirement(AuthorizationPermissions.FileShare));
    });

    options.AddPolicy(AuthorizationPolicies.FolderManage, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new PermissionRequirement(
            AuthorizationPermissions.FolderCreate,
            AuthorizationPermissions.FolderDelete,
            AuthorizationPermissions.FolderShare));
    });

    options.AddPolicy(AuthorizationPolicies.NodeManage, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new PermissionRequirement(AuthorizationPermissions.NodeManage));
    });

    options.AddPolicy(AuthorizationPolicies.Admin, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new PermissionRequirement(AuthorizationPermissions.SystemAdmin));
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ShcDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

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
builder.Services.AddScoped<ICreateCheckoutSessionUseCase, CreateCheckoutSessionUseCase>();
builder.Services.AddScoped<IProcessPaymentWebhookUseCase, ProcessPaymentWebhookUseCase>();
builder.Services.AddScoped<IPaymentGatewayService, StripePaymentGatewayService>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<ICreateSubscriptionPlanUseCase, CreateSubscriptionPlanUseCase>();
builder.Services.AddScoped<IListSubscriptionPlansUseCase, ListSubscriptionPlansUseCase>();
builder.Services.AddScoped<IGetSubscriptionPlanByIdUseCase, GetSubscriptionPlanByIdUseCase>();
builder.Services.AddScoped<IUpdateSubscriptionPlanUseCase, UpdateSubscriptionPlanUseCase>();
builder.Services.AddScoped<ICreateSubscriptionUseCase, CreateSubscriptionUseCase>();
builder.Services.AddScoped<IGetSubscriptionByIdUseCase, GetSubscriptionByIdUseCase>();
builder.Services.AddScoped<IGetUserSubscriptionsUseCase, GetUserSubscriptionsUseCase>();
builder.Services.AddScoped<IGetActiveUserSubscriptionUseCase, GetActiveUserSubscriptionUseCase>();
builder.Services.AddScoped<IGetSubscriptionEntitlementsUseCase, GetSubscriptionEntitlementsUseCase>();
builder.Services.AddScoped<IChangeSubscriptionPlanUseCase, ChangeSubscriptionPlanUseCase>();
builder.Services.AddScoped<ICancelSubscriptionUseCase, CancelSubscriptionUseCase>();
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
builder.Services.AddScoped<ITrashRepository, TrashRepository>();
builder.Services.AddScoped<ICreateTrashedItemUseCase, CreateTrashedItemUseCase>();
builder.Services.AddScoped<IGetTrashedItemByIdUseCase, GetTrashedItemByIdUseCase>();
builder.Services.AddScoped<IGetTrashedItemsByUserIdUseCase, GetTrashedItemsByUserIdUseCase>();
builder.Services.AddScoped<IRestoreTrashedItemUseCase, RestoreTrashedItemUseCase>();
builder.Services.AddScoped<IPermanentlyDeleteTrashedItemUseCase, PermanentlyDeleteTrashedItemUseCase>();
builder.Services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
builder.Services.AddScoped<IGetSystemSettingsUseCase, GetSystemSettingsUseCase>();
builder.Services.AddScoped<IListSystemSettingsUseCase, ListSystemSettingsUseCase>();
builder.Services.AddScoped<IUpdateSystemSettingsUseCase, UpdateSystemSettingsUseCase>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IGrantPermissionUseCase, GrantPermissionUseCase>();
builder.Services.AddScoped<IUpdatePermissionUseCase, UpdatePermissionUseCase>();
builder.Services.AddScoped<IRevokePermissionUseCase, RevokePermissionUseCase>();
builder.Services.AddScoped<ICheckPermissionUseCase, CheckPermissionUseCase>();
builder.Services.AddScoped<IGetPermissionsBySubjectUseCase, GetPermissionsBySubjectUseCase>();
builder.Services.AddScoped<IGetPermissionsByResourceUseCase, GetPermissionsByResourceUseCase>();
builder.Services.AddScoped<IListAuditLogsUseCase, ListAuditLogsUseCase>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ICreateNotificationUseCase, CreateNotificationUseCase>();
builder.Services.AddScoped<IGetNotificationByIdUseCase, GetNotificationByIdUseCase>();
builder.Services.AddScoped<IGetNotificationsByUserIdUseCase, GetNotificationsByUserIdUseCase>();
builder.Services.AddScoped<IGetUnreadNotificationsByUserIdUseCase, GetUnreadNotificationsByUserIdUseCase>();
builder.Services.AddScoped<IMarkNotificationAsReadUseCase, MarkNotificationAsReadUseCase>();
builder.Services.AddScoped<IMarkAllNotificationsAsReadUseCase, MarkAllNotificationsAsReadUseCase>();
builder.Services.AddScoped<IDeleteNotificationUseCase, DeleteNotificationUseCase>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ICreateRoleUseCase, CreateRoleUseCase>();
builder.Services.AddScoped<IGetRoleByIdUseCase, GetRoleByIdUseCase>();
builder.Services.AddScoped<IListRolesUseCase, ListRolesUseCase>();
builder.Services.AddScoped<IUpdateRoleUseCase, UpdateRoleUseCase>();
builder.Services.AddScoped<IDeleteRoleUseCase, DeleteRoleUseCase>();
builder.Services.AddScoped<IAssignRoleToUserUseCase, AssignRoleToUserUseCase>();
builder.Services.AddScoped<IRemoveRoleFromUserUseCase, RemoveRoleFromUserUseCase>();
builder.Services.AddScoped<IGetUserRolesUseCase, GetUserRolesUseCase>();
builder.Services.AddScoped<IGetRoleUsersUseCase, GetRoleUsersUseCase>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IFolderRepository, FolderRepository>();
builder.Services.AddScoped<IFileStorageRepository, FileStorageRepository>();
builder.Services.AddScoped<IStorageNodeAllocator, StorageNodeAllocator>();
builder.Services.AddScoped<IUploadFileUseCase, UploadFileUseCase>();
builder.Services.AddScoped<IGetFileByIdUseCase, GetFileByIdUseCase>();
builder.Services.AddScoped<ISearchFilesUseCase, SearchFilesUseCase>();
builder.Services.AddScoped<IDownloadFileUseCase, DownloadFileUseCase>();
builder.Services.AddScoped<IRenameFileUseCase, RenameFileUseCase>();
builder.Services.AddScoped<IMoveFileUseCase, MoveFileUseCase>();
builder.Services.AddScoped<IDeleteFileUseCase, DeleteFileUseCase>();
builder.Services.AddScoped<IShareFileUseCase, ShareFileUseCase>();
builder.Services.AddScoped<ICreateFolderUseCase, CreateFolderUseCase>();
builder.Services.AddScoped<IGetFolderByIdUseCase, GetFolderByIdUseCase>();
builder.Services.AddScoped<IListFolderContentUseCase, ListFolderContentUseCase>();
builder.Services.AddScoped<IRenameFolderUseCase, RenameFolderUseCase>();
builder.Services.AddScoped<IMoveFolderUseCase, MoveFolderUseCase>();
builder.Services.AddScoped<IDeleteFolderUseCase, DeleteFolderUseCase>();
builder.Services.AddScoped<IArchiveFolderUseCase, ArchiveFolderUseCase>();
builder.Services.AddScoped<IShareFolderUseCase, ShareFolderUseCase>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, OwnershipAuthorizationHandler>();
builder.Services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUserCredentialRepository, EfUserCredentialRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasherAdapter>();
builder.Services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
builder.Services.AddScoped<ILoginUserUseCase, LoginUserUseCase>();
builder.Services.AddScoped<ILogoutUserUseCase, LogoutUserUseCase>();
builder.Services.AddScoped<IRefreshTokenRepository, EfRefreshTokenRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors(frontendCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
