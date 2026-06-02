using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace adapters.DrivenAdapters.Data
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
        public DbSet<UserSetting> UserSettings { get; set; } = null!;

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
            modelBuilder.Entity<Permission>().HasKey(p => p.PermissionId);
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
                .WithOne()
                .HasForeignKey<UserSetting>(us => us.UserSettingId)
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

            // 🔗 Shared Links
            modelBuilder.Entity<SharedLink>()
                .HasOne(sl => sl.User)
                .WithMany()
                .HasForeignKey(sl => sl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Polymorphic target mapping
            // SharedLink → FileItem
            modelBuilder.Entity<SharedLink>()
                .HasOne<FileItem>() // no navigation property in SharedLink
                .WithMany()
                .HasForeignKey(sl => sl.TargetId)
                .OnDelete(DeleteBehavior.SetNull);

            // SharedLink → Folder
            modelBuilder.Entity<SharedLink>()
                .HasOne<Folder>() // no navigation property in SharedLink
                .WithMany()
                .HasForeignKey(sl => sl.TargetId)
                .OnDelete(DeleteBehavior.SetNull);


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
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(al => al.SubjectId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AuditLog>()
                .HasOne<Role>()
                .WithMany(r => r.AuditLogs)
                .HasForeignKey(al => al.SubjectId)
                .OnDelete(DeleteBehavior.SetNull);

            // 🔐 Permissions
            modelBuilder.Entity<Permission>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.SubjectId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Permission>()
                .HasOne<Role>()
                .WithMany(r => r.Permissions)
                .HasForeignKey(p => p.SubjectId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Permission>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey()
                .OnDelete(DeleteBehavior.SetNull);

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

            modelBuilder.Entity<Role>()
                .HasMany(r => r.AuditLogs)
                .WithOne()
                .HasForeignKey(al => al.SubjectId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Permissions)
                .WithOne()
                .HasForeignKey(p => p.SubjectId)
                .OnDelete(DeleteBehavior.SetNull);

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

        }
    }
}
