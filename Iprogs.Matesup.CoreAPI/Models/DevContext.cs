using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class DevContext : DbContext
{
    public DevContext()
    {
    }

    public DevContext(DbContextOptions<DevContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActiveUsers> ActiveUsers { get; set; }

    public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }

    public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }

    public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }

    public virtual DbSet<ChatMaster> ChatMaster { get; set; }

    public virtual DbSet<ChatMasterAttachments> ChatMasterAttachments { get; set; }

    public virtual DbSet<ChatRoomBlockedUsers> ChatRoomBlockedUsers { get; set; }

    public virtual DbSet<ChatRoomMaster> ChatRoomMaster { get; set; }

    public virtual DbSet<ChatRoomUserMapping> ChatRoomUserMapping { get; set; }

    public virtual DbSet<ErrorLogs> ErrorLogs { get; set; }

    public virtual DbSet<FollowersMaster> FollowersMaster { get; set; }

    public virtual DbSet<Logs> Logs { get; set; }

    public virtual DbSet<LookupChatRoomPrivacy> LookupChatRoomPrivacy { get; set; }

    public virtual DbSet<LookupChatRoomType> LookupChatRoomType { get; set; }

    public virtual DbSet<LookupCity> LookupCity { get; set; }

    public virtual DbSet<LookupCountry> LookupCountry { get; set; }

    public virtual DbSet<LookupGender> LookupGender { get; set; }

    public virtual DbSet<LookupRelationshipStatus> LookupRelationshipStatus { get; set; }

    public virtual DbSet<LookupState> LookupState { get; set; }

    public virtual DbSet<MegaPhoneMaster> MegaPhoneMaster { get; set; }

    public virtual DbSet<UserInterestedIn> UserInterestedIn { get; set; }

    public virtual DbSet<UserPics> UserPics { get; set; }

    public virtual DbSet<UserProfileMaster> UserProfileMaster { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActiveUsers>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.LastActive)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastLogin)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastNotification)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithOne(p => p.ActiveUsers)
                .HasForeignKey<ActiveUsers>(d => d.UserId)
                .HasConstraintName("FK__ActiveUse__UserI__078C1F06");
        });

        modelBuilder.Entity<AspNetRoleClaims>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetRoles>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetUserClaims>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogins>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserTokens>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUsers>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Role).WithMany(p => p.User)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRoles",
                    r => r.HasOne<AspNetRoles>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUsers>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<ChatMaster>(entity =>
        {
            entity.Property(e => e.SentOn).HasColumnType("datetime");

            entity.HasOne(d => d.ChatRoom).WithMany(p => p.ChatMaster)
                .HasForeignKey(d => d.ChatRoomId)
                .HasConstraintName("FK__ChatMaste__ChatR__2A4B4B5E");

            entity.HasOne(d => d.ReplyToNavigation).WithMany(p => p.InverseReplyToNavigation)
                .HasForeignKey(d => d.ReplyTo)
                .HasConstraintName("FK__ChatMaste__Reply__5AEE82B9");

            entity.HasOne(d => d.User).WithMany(p => p.ChatMaster)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ChatMaste__UserI__0880433F");
        });

        modelBuilder.Entity<ChatMasterAttachments>(entity =>
        {
            entity.Property(e => e.AttachmentName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FileExtn)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FileName).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Chat).WithMany(p => p.ChatMasterAttachments)
                .HasForeignKey(d => d.ChatId)
                .HasConstraintName("FK__ChatMaste__ChatI__71D1E811");
        });

        modelBuilder.Entity<ChatRoomBlockedUsers>(entity =>
        {
            entity.HasIndex(e => new { e.ChatRoomId, e.BlockedUserId, e.BlockedBy }, "UQ__ChatRoom__2CB1E193B46AABBA").IsUnique();

            entity.Property(e => e.BlockedOn).HasColumnType("datetime");

            entity.HasOne(d => d.BlockedByNavigation).WithMany(p => p.ChatRoomBlockedUsersBlockedByNavigation)
                .HasForeignKey(d => d.BlockedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatRoomB__Block__0E391C95");

            entity.HasOne(d => d.BlockedUser).WithMany(p => p.ChatRoomBlockedUsersBlockedUser)
                .HasForeignKey(d => d.BlockedUserId)
                .HasConstraintName("FK__ChatRoomB__Block__0C50D423");

            entity.HasOne(d => d.ChatRoom).WithMany(p => p.ChatRoomBlockedUsers)
                .HasForeignKey(d => d.ChatRoomId)
                .HasConstraintName("FK__ChatRoomB__ChatR__1EA48E88");
        });

        modelBuilder.Entity<ChatRoomMaster>(entity =>
        {
            entity.HasIndex(e => e.ChatRoomName, "UQ__ChatRoom__38EF6358F5EEB815").IsUnique();

            entity.Property(e => e.ChatRoomName).HasMaxLength(200);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastMessageOn).HasColumnType("datetime");
            entity.Property(e => e.PasswordChangedOn)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.ChatRoomPrivacyNavigation).WithMany(p => p.ChatRoomMaster)
                .HasForeignKey(d => d.ChatRoomPrivacy)
                .HasConstraintName("FK__ChatRoomM__ChatR__208CD6FA");

            entity.HasOne(d => d.ChatRoomTypeNavigation).WithMany(p => p.ChatRoomMaster)
                .HasForeignKey(d => d.ChatRoomType)
                .HasConstraintName("FK__ChatRoomM__ChatR__1F98B2C1");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ChatRoomMasterCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatRoomM__Creat__11158940");

            entity.HasOne(d => d.RoomOwner).WithMany(p => p.ChatRoomMasterRoomOwner)
                .HasForeignKey(d => d.RoomOwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatRoomM__RoomO__1209AD79");
        });

        modelBuilder.Entity<ChatRoomUserMapping>(entity =>
        {
            entity.Property(e => e.LastSeen).HasColumnType("datetime");

            entity.HasOne(d => d.ChatRoom).WithMany(p => p.ChatRoomUserMapping)
                .HasForeignKey(d => d.ChatRoomId)
                .HasConstraintName("FK__ChatRoomU__ChatR__2F10007B");

            entity.HasOne(d => d.User).WithMany(p => p.ChatRoomUserMapping)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ChatRoomU__UserI__12FDD1B2");
        });

        modelBuilder.Entity<ErrorLogs>(entity =>
        {
            entity.Property(e => e.Category)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<FollowersMaster>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.FollowerUserId }, "UQ__Follower__DCC5BA51A71646F7").IsUnique();

            entity.Property(e => e.FollowedOn)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.FollowerUser).WithMany(p => p.FollowersMasterFollowerUser)
                .HasForeignKey(d => d.FollowerUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Followers__Follo__25518C17");

            entity.HasOne(d => d.User).WithMany(p => p.FollowersMasterUser)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Followers__UserI__2645B050");
        });

        modelBuilder.Entity<Logs>(entity =>
        {
            entity.Property(e => e.Category)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<LookupChatRoomPrivacy>(entity =>
        {
            entity.Property(e => e.ChatRoomPrivacy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<LookupChatRoomType>(entity =>
        {
            entity.Property(e => e.ChatRoomType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<LookupCity>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_CityMaster");

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.State).WithMany(p => p.LookupCity)
                .HasForeignKey(d => d.StateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CityMaster_StateMaster");
        });

        modelBuilder.Entity<LookupCountry>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_CountryMaster");

            entity.Property(e => e.CountryCode)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<LookupGender>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_LookupGenderMaster");

            entity.Property(e => e.Gender)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.GenderIcon)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<LookupRelationshipStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_LookupRelationshipStatusMaster");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RelationshipStatus)
                .HasMaxLength(25)
                .IsUnicode(false);
        });

        modelBuilder.Entity<LookupState>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_StateMaster");

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Country).WithMany(p => p.LookupState)
                .HasForeignKey(d => d.CountryID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StateMaster_CountryMaster");
        });

        modelBuilder.Entity<MegaPhoneMaster>(entity =>
        {
            entity.Property(e => e.SentOn).HasColumnType("datetime");

            entity.HasOne(d => d.ChatRoom).WithMany(p => p.MegaPhoneMasterChatRoom)
                .HasForeignKey(d => d.ChatRoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MegaPhone__ChatR__29221CFB");

            entity.HasOne(d => d.NewChatRoom).WithMany(p => p.MegaPhoneMasterNewChatRoom)
                .HasForeignKey(d => d.NewChatRoomId)
                .HasConstraintName("FK__MegaPhone__NewCh__2A164134");

            entity.HasOne(d => d.User).WithMany(p => p.MegaPhoneMaster)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MegaPhone__UserI__2B0A656D");
        });

        modelBuilder.Entity<UserInterestedIn>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.UserInterestedIn)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserInter__UserI__13F1F5EB");
        });

        modelBuilder.Entity<UserPics>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.CoverPicName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ProfilePicName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithOne(p => p.UserPics)
                .HasForeignKey<UserPics>(d => d.UserId)
                .HasConstraintName("FK__UserPics__UserId__16CE6296");
        });

        modelBuilder.Entity<UserProfileMaster>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.HasIndex(e => e.NickName, "UQ__UserProf__01E67C8B047CC8A7").IsUnique();

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.NickName).HasMaxLength(100);

            entity.HasOne(d => d.CityNavigation).WithMany(p => p.UserProfileMaster)
                .HasForeignKey(d => d.City)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__UserProfil__City__2645B050");

            entity.HasOne(d => d.CountryNavigation).WithMany(p => p.UserProfileMaster)
                .HasForeignKey(d => d.Country)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__UserProfi__Count__282DF8C2");

            entity.HasOne(d => d.GenderNavigation).WithMany(p => p.UserProfileMaster)
                .HasForeignKey(d => d.Gender)
                .HasConstraintName("FK__UserProfi__Gende__25518C17");

            entity.HasOne(d => d.RelationshipStatusNavigation).WithMany(p => p.UserProfileMaster)
                .HasForeignKey(d => d.RelationshipStatus)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__UserProfi__Relat__2BFE89A6");

            entity.HasOne(d => d.StateNavigation).WithMany(p => p.UserProfileMaster)
                .HasForeignKey(d => d.State)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__UserProfi__State__2739D489");

            entity.HasOne(d => d.User).WithOne(p => p.UserProfileMaster)
                .HasForeignKey<UserProfileMaster>(d => d.UserId)
                .HasConstraintName("FK__UserProfi__UserI__3E52440B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
