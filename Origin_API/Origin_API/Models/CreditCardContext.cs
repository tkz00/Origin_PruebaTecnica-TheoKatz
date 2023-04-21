using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Origin_API.Models
{
    public partial class CreditCardContext : DbContext
    {
        public CreditCardContext()
        {
        }

        public CreditCardContext(DbContextOptions<CreditCardContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BalanceOperation> BalanceOperation { get; set; }
        public virtual DbSet<CreditCard> CreditCard { get; set; }
        public virtual DbSet<WithdrawOperation> WithdrawOperation { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=Origin_Test;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BalanceOperation>(entity =>
            {
                entity.ToTable("Balance_Operation");

                entity.HasIndex(e => e.CardId);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CardId).HasColumnName("card_id");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Card)
                    .WithMany(p => p.BalanceOperation)
                    .HasForeignKey(d => d.CardId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Balance_Operation_Credit_Card");
            });

            modelBuilder.Entity<CreditCard>(entity =>
            {
                entity.ToTable("Credit_Card");

                entity.HasIndex(e => e.CardNumber)
                    .HasName("UQ_Credit_Card_Card_Number")
                    .IsUnique();

                entity.HasIndex(e => e.Username)
                    .HasName("UQ_Credit_Card_Username")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Balance)
                    .HasColumnName("balance")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CardNumber)
                    .IsRequired()
                    .HasColumnName("card_number")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.ExpirationDate)
                    .HasColumnName("expiration_date")
                    .HasColumnType("date");

                entity.Property(e => e.FailedLoginAttempts).HasColumnName("failedLoginAttempts");

                entity.Property(e => e.LastFailedLoginTime).HasColumnType("datetime");

                entity.Property(e => e.Pin)
                    .IsRequired()
                    .HasColumnName("pin")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<WithdrawOperation>(entity =>
            {
                entity.ToTable("Withdraw_Operation");

                entity.HasIndex(e => e.CardId);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CardId).HasColumnName("card_id");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Card)
                    .WithMany(p => p.WithdrawOperation)
                    .HasForeignKey(d => d.CardId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Withdraw_Operation_Credit_Card");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
