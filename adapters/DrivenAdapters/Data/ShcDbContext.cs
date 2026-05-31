using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Auth;
using Domain.Entities.FileStorage;
using Domain.Entities.LinkSharing;
using Domain.Entities.Purchases;
using Domain.Entities.Roles;
using Domain.Entities.Settings;
using Domain.Entities.StorageNodes;
using Domain.Entities.Trash;
using Domain.Entities.Users;
using Domain.Entities.Users.Settings;
using Microsoft.EntityFrameworkCore;
using SHC.Domain.Entities.Permissions;

namespace adapters.DrivenAdapters.Data
{
    public class ShcDbContext : DbContext
    {
        public ShcDbContext(DbContextOptions<ShcDbContext> options) : base(options)
        {}

        //AUTH
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<UserCredentials> UserCredentials { get; set; } = null!;

        //FILE STORAGE
        public DbSet<FileItem> FileItems { get; set; } = null!;
        public DbSet<Folder> Folders { get; set; } = null!;

        //LINK SHARING
        public DbSet<SharedLink> SharedLinks { get; set; } = null!;

        //NOTIFICATIONS
        public DbSet<Notification> Notifications { get; set; } = null!;

        // PERMISSIONS
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;

        // PURCHASES & SUBSCRIPTIONS
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<Purchase> Purchases { get; set; } = null!;
        public DbSet<Subscription> Subscriptions { get; set; } = null!;
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; } = null!;
        public DbSet<UserSubscription> UserSubscriptions { get; set; } = null!;

        // ROLES
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;

        // SETTINGS
        public DbSet<SystemSetting> SystemSettings { get; set; } = null!;

        // STORAGE NODES
        public DbSet<StorageNode> StorageNodes { get; set; } = null!;

        // TRASH
        public DbSet<TrashedItem> TrashedItems { get; set; } = null!;

        // USERS & USER SETTINGS
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<NotificationSetting> NotificationSettings { get; set; } = null!;
        public DbSet<StorageSetting> StorageSettings { get; set; } = null!;
        public DbSet<PrivacySetting> PrivacySettings { get; set; } = null!;
        public DbSet<UiSetting> UiSettings { get; set; } = null!;
        public DbSet<UserSetting> UserSettings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
