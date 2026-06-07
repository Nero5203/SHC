using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using application.Common.Authorization;
using Domain.Entities.Authorization;
using Domain.Entities.AI;
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
using domain.Entities.FileStorage;

namespace adapters.Driven.Persistence.Data
{
    public class ShcDbContext : DbContext
    {
        public ShcDbContext(DbContextOptions<ShcDbContext> options) : base(options)
        {}

        //AUTH
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<UserCredential> UserCredentials { get; set; } = null!;

        //FILE STORAGE
        public DbSet<FileItem> FileItems { get; set; } = null!;
        public DbSet<Folder> Folders { get; set; } = null!;

        //LINK SHARING
        public DbSet<SharedLink> SharedLinks { get; set; } = null!;

        //NOTIFICATIONS
        public DbSet<Notification> Notifications { get; set; } = null!;

        // PERMISSIONS
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<SHC.Domain.Entities.Permissions.Permission> Permissions { get; set; } = null!;
        public DbSet<Domain.Entities.Authorization.Permission> AuthorizationPermissionCatalog { get; set; } = null!;
        public DbSet<RolePermission> RolePermissions { get; set; } = null!;

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
        public DbSet<UserSetting> UserSettings { get; set; } = null!;
        public DbSet<AIFileInsight> AIFileInsights { get; set; } = null!;
        public DbSet<AISuggestion> AISuggestions { get; set; } = null!;
        public DbSet<FileActivity> FileActivities { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RefreshToken>().HasKey(rt => rt.RefreshTokenId);
            modelBuilder.Entity<UserCredential>().HasKey(uc => uc.UserCredentialId);
            modelBuilder.Entity<FileItem>().HasKey(fl => fl.FileItemId);
            modelBuilder.Entity<Folder>().HasKey(f => f.FolderId);
            modelBuilder.Entity<SharedLink>().HasKey(sl => sl.SharedLinkId);
            modelBuilder.Entity<Notification>().HasKey(n => n.NotificationId);
            modelBuilder.Entity<AuditLog>().HasKey(al => al.AuditLogId);
            modelBuilder.Entity<SHC.Domain.Entities.Permissions.Permission>().HasKey(p => p.PermissionId);
            modelBuilder.Entity<Domain.Entities.Authorization.Permission>().HasKey(p => p.PermissionId);
            modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });
            modelBuilder.Entity<Invoice>().HasKey(i => i.InvoiceId);
            modelBuilder.Entity<Purchase>().HasKey(p => p.PurchaseId);
            modelBuilder.Entity<Subscription>().HasKey(s => s.SubscriptionId);
            modelBuilder.Entity<SubscriptionPlan>().HasKey(sp => sp.SubscriptionPlanId);
            modelBuilder.Entity<UserSubscription>().HasKey(us => new { us.UserId, us.SubscriptionId });
            modelBuilder.Entity<Role>().HasKey(r => r.RoleId);
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<SystemSetting>().HasKey(ss => ss.SystemSettingId);
            modelBuilder.Entity<StorageNode>().HasKey(sn => sn.StorageNodeId);
            modelBuilder.Entity<TrashedItem>().HasKey(ti => ti.TrashedItemId);
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<UserSetting>().HasKey(us => us.UserSettingId);
            modelBuilder.Entity<UserSetting>().OwnsOne(x => x.UiSettings);
            modelBuilder.Entity<UserSetting>().OwnsOne(x => x.StorageSettings);
            modelBuilder.Entity<UserSetting>().OwnsOne(x => x.NotificationSettings);
            modelBuilder.Entity<UserSetting>().OwnsOne(x => x.PrivacySettings);

            // 🔐 Auth & Security
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserCredential>()
                .HasOne(uc => uc.User)
                .WithOne(u => u.UserCredentials)
                .HasForeignKey<UserCredential>(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ⚙️ User Settings
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserSettings)
                .WithOne(us => us.User)
                .HasForeignKey<UserSetting>(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 📁 Storage: Folders & Files
            modelBuilder.Entity<Folder>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Folder>()
                .HasOne(f => f.ParentFolder)
                .WithMany(f => f.SubFolders)
                .HasForeignKey(f => f.ParentFolderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FileItem>()
                .HasOne(fi => fi.Folder)
                .WithMany(f => f.FileItems)
                .HasForeignKey(fi => fi.FolderId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<FileItem>()
                .HasOne(fi => fi.User)
                .WithMany()
                .HasForeignKey(fi => fi.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FileItem>()
                .HasOne(fi => fi.StorageNode)
                .WithMany(sn => sn.FileItems)
                .HasForeignKey(fi => fi.StorageNodeId)
                .OnDelete(DeleteBehavior.Cascade);

    
            modelBuilder.Entity<FileItem>()
                .HasMany(fi => fi.AISuggestions)
                .WithOne(ai => ai.FileItem)
                .HasForeignKey(ai => ai.FileItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FileItem>()
                .HasOne(fi => fi.AIFileInsight)
                .WithOne(ai => ai.FileItem)
                .HasForeignKey<AIFileInsight>(ai => ai.FileItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SharedLink>()
                .HasKey(sl => sl.SharedLinkId);

            modelBuilder.Entity<SharedLink>()
                .Property(sl => sl.TargetId)
                .IsRequired();

            modelBuilder.Entity<SharedLink>()
                .Property(sl => sl.TargetType)
                .IsRequired();

            modelBuilder.Entity<SharedLink>()
                .HasOne(sl => sl.User)
                .WithMany()
                .HasForeignKey(sl => sl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔔 Notifications
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.FileItems)
                .WithMany(fi => fi.Notifications)
                .HasForeignKey(n => n.FileId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Folder)
                .WithMany(f => f.Notifications)
                .HasForeignKey(n => n.FolderId)
                .OnDelete(DeleteBehavior.SetNull);

            // 🧾 Audit Logs

            modelBuilder.Entity<AuditLog>()
                .HasKey(al => al.AuditLogId);

            modelBuilder.Entity<AuditLog>()
                .Property(al => al.SubjectId)
                .IsRequired();

            modelBuilder.Entity<AuditLog>()
                .Property(al => al.SubjectType)
                .IsRequired();

            // 🔐 Permissions

            modelBuilder.Entity<SHC.Domain.Entities.Permissions.Permission>()
                .HasKey(p => p.PermissionId);

            modelBuilder.Entity<SHC.Domain.Entities.Permissions.Permission>()
                .Property(p => p.SubjectId)
                .IsRequired();

            modelBuilder.Entity<SHC.Domain.Entities.Permissions.Permission>()
                .Property(p => p.SubjectType)
                .IsRequired();

            modelBuilder.Entity<Domain.Entities.Authorization.Permission>()
                .ToTable("AuthorizationPermissions");

            modelBuilder.Entity<Domain.Entities.Authorization.Permission>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(128);

            modelBuilder.Entity<Domain.Entities.Authorization.Permission>()
                .Property(p => p.Description)
                .HasMaxLength(512);

            modelBuilder.Entity<Domain.Entities.Authorization.Permission>()
                .HasIndex(p => p.Name)
                .IsUnique();

            modelBuilder.Entity<RolePermission>()
                .ToTable("RolePermissions");

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // 💳 Purchases & Invoices
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Purchase)
                .WithOne(p => p.Invoice)
                .HasForeignKey<Invoice>(i => i.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.User)
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 📦 Subscriptions
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.SubscriptionPlan)
                .WithMany(sp => sp.Subscriptions)
                .HasForeignKey(s => s.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Subscription)
                .WithMany(s => s.Purchases)
                .HasForeignKey(p => p.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserSubscription>()
                .HasKey(us => new { us.UserId, us.SubscriptionId }); // composite key

            modelBuilder.Entity<UserSubscription>()
                .HasOne(us => us.User)
                .WithMany()
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserSubscription>()
                .HasOne(us => us.Subscription)
                .WithMany(s => s.UserSubscriptions)
                .HasForeignKey(us => us.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);


            // 👥 Roles
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            SeedAuthorization(modelBuilder);

            
            // ⚙️ System Settings
            modelBuilder.Entity<SystemSetting>()
                .Property(ss => ss.AllowedFileExtensions)
                .HasMaxLength(512);

            modelBuilder.Entity<SystemSetting>()
                .Property(ss => ss.RegistrationMode)
                .HasConversion<int>();

            modelBuilder.Entity<SystemSetting>()
                .Property(ss => ss.CreatedAt)
                .IsRequired();

            modelBuilder.Entity<SystemSetting>()
                .Property(ss => ss.UpdatedAt)
                .IsRequired();

            // 🗄️ Storage Nodes
            modelBuilder.Entity<StorageNode>()
                .Property(sn => sn.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<StorageNode>()
                .Property(sn => sn.Hostname)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<StorageNode>()
                .Property(sn => sn.IpAddress)
                .IsRequired()
                .HasMaxLength(45);

            modelBuilder.Entity<StorageNode>()
                .Property(sn => sn.Status)
                .HasConversion<int>();

            modelBuilder.Entity<StorageNode>()
                .HasMany(sn => sn.FileItems)
                .WithOne(fi => fi.StorageNode)
                .HasForeignKey(fi => fi.StorageNodeId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🗑️ Trash
            modelBuilder.Entity<TrashedItem>()
                .Property(ti => ti.ItemType)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<TrashedItem>()
                .Property(ti => ti.Name)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<TrashedItem>()
                .Property(ti => ti.OriginalPath)
                .HasMaxLength(512);

            modelBuilder.Entity<TrashedItem>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(ti => ti.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FileActivity>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(fa => fa.UserId);

            modelBuilder.Entity<FileActivity>()
                .HasOne<FileItem>()
                .WithMany()
                .HasForeignKey(fa => fa.FileItemId);

            modelBuilder.Entity<AISuggestion>()
                .HasOne(ai => ai.User)
                .WithMany()
                .HasForeignKey(ai => ai.UserId)
                .OnDelete(DeleteBehavior.Cascade);


        }

        private static void SeedAuthorization(ModelBuilder modelBuilder)
        {
            var createdAt = new DateTime(2026, 6, 5, 0, 0, 0, DateTimeKind.Utc);

            var adminRoleId = Guid.Parse("10000000-0000-0000-0000-000000000001");
            var userRoleId = Guid.Parse("10000000-0000-0000-0000-000000000002");
            var storageNodeOwnerRoleId = Guid.Parse("10000000-0000-0000-0000-000000000003");

            modelBuilder.Entity<Role>().HasData(
                new { RoleId = adminRoleId, Name = AuthorizationRoles.Admin, Description = "Full platform administrator.", CreatedAt = createdAt },
                new { RoleId = userRoleId, Name = AuthorizationRoles.User, Description = "Default cloud user.", CreatedAt = createdAt },
                new { RoleId = storageNodeOwnerRoleId, Name = AuthorizationRoles.StorageNodeOwner, Description = "Storage node owner and operator.", CreatedAt = createdAt });

            var permissions = new (Guid Id, string Name, string Description)[]
            {
                (Guid.Parse("20000000-0000-0000-0000-000000000001"), AuthorizationPermissions.FileRead, "Read file metadata and content."),
                (Guid.Parse("20000000-0000-0000-0000-000000000002"), AuthorizationPermissions.FileUpload, "Upload files."),
                (Guid.Parse("20000000-0000-0000-0000-000000000003"), AuthorizationPermissions.FileDelete, "Delete files."),
                (Guid.Parse("20000000-0000-0000-0000-000000000004"), AuthorizationPermissions.FileShare, "Share files."),
                (Guid.Parse("20000000-0000-0000-0000-000000000005"), AuthorizationPermissions.FolderCreate, "Create folders."),
                (Guid.Parse("20000000-0000-0000-0000-000000000006"), AuthorizationPermissions.FolderDelete, "Delete folders."),
                (Guid.Parse("20000000-0000-0000-0000-000000000007"), AuthorizationPermissions.FolderShare, "Share folders."),
                (Guid.Parse("20000000-0000-0000-0000-000000000008"), AuthorizationPermissions.NodeRegister, "Register storage nodes."),
                (Guid.Parse("20000000-0000-0000-0000-000000000009"), AuthorizationPermissions.NodeManage, "Manage storage nodes."),
                (Guid.Parse("20000000-0000-0000-0000-000000000010"), AuthorizationPermissions.UserManage, "Manage users."),
                (Guid.Parse("20000000-0000-0000-0000-000000000011"), AuthorizationPermissions.SystemAdmin, "Administer system settings.")
            };

            modelBuilder.Entity<Domain.Entities.Authorization.Permission>().HasData(
                permissions.Select(p => new { PermissionId = p.Id, p.Name, p.Description }));

            var rolePermissions = new List<object>();

            rolePermissions.AddRange(permissions.Select(p => new { RoleId = adminRoleId, PermissionId = p.Id }));

            rolePermissions.AddRange(permissions
                .Where(p => p.Name is AuthorizationPermissions.FileRead
                    or AuthorizationPermissions.FileUpload
                    or AuthorizationPermissions.FileShare
                    or AuthorizationPermissions.FolderCreate
                    or AuthorizationPermissions.FolderShare)
                .Select(p => new { RoleId = userRoleId, PermissionId = p.Id }));

            rolePermissions.AddRange(permissions
                .Where(p => p.Name is AuthorizationPermissions.NodeRegister or AuthorizationPermissions.NodeManage)
                .Select(p => new { RoleId = storageNodeOwnerRoleId, PermissionId = p.Id }));

            modelBuilder.Entity<RolePermission>().HasData(rolePermissions);
        }
    }
}
