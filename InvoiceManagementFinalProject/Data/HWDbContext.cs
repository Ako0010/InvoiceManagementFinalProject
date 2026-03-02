using InvoiceManagementFinalProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Mail;

namespace InvoiceManagementFinalProject.Data;

public class HWDbContext : IdentityDbContext<AppUser>
{
    public HWDbContext(DbContextOptions options) 
        : base(options)
    {}

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceRow> InvoiceRows => Set<InvoiceRow>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<InvoiceAttachment> Attachments => Set<InvoiceAttachment>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(
            customer =>
            {
                customer.HasKey(e => e.Id);
                customer.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
                customer.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(200);
                customer.Property(e => e.Address)
                .HasMaxLength(500);
                customer.Property(e => e.PhoneNumber)
                .HasMaxLength(20);
                customer.Property(e => e.CreatedAt)
                .IsRequired();

                customer.HasOne(e => e.AppUser)
                 .WithMany()
                 .HasForeignKey(e => e.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            }
            );

        modelBuilder.Entity<Invoice>(
            invoice =>
            {
                invoice.HasKey(e => e.Id);
                invoice.Property(e => e.CustomerId)
                .IsRequired();
                invoice.Property(e => e.StartDate)
                .IsRequired();
                invoice.Property(e => e.EndDate)
                .IsRequired();
                invoice.Property(e => e.TotalSum)
                .IsRequired()
                .HasPrecision(18, 2);
                invoice.Property(e => e.Status)
                .IsRequired();
                invoice.Property(e => e.CreatedAt)
                .IsRequired();

                invoice.HasMany(i => i.InvoiceRows)
                .WithOne(r => r.Invoice)     
                .HasForeignKey(r => r.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);


                });
        modelBuilder.Entity<InvoiceRow>(
            row =>
            {
                row.HasKey(r => r.Id);
                row.Property(r => r.Service)
                       .IsRequired()
                       .HasMaxLength(200);
                row.Property(r => r.Quantity)
                       .IsRequired()
                       .HasPrecision(18, 2);
                row.Property(r => r.Amount)
                       .IsRequired()
                       .HasPrecision(18, 2);
                row.Property(r => r.Sum)
                        .IsRequired()
                        .HasPrecision(18, 2);

            }
        );
        modelBuilder.Entity<RefreshToken>(
           refresh =>
           {
               refresh.HasKey(rt => rt.Id);
               refresh.HasIndex(rt => rt.JwtId).IsUnique();
               refresh.Property(rt => rt.JwtId).IsRequired().HasMaxLength(64);
               refresh.Property(rt => rt.UserId).IsRequired().HasMaxLength(450);

               refresh.HasOne(rt => rt.User).WithMany()
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

           });
        modelBuilder.Entity<InvoiceAttachment>(
            attachment =>
            {
                attachment.HasKey(ta => ta.Id);

                attachment
                    .Property(ta => ta.OriginalFileName)
                    .IsRequired()
                    .HasMaxLength(500);

                attachment
                    .Property(ta => ta.StoredFileName)
                    .IsRequired()
                    .HasMaxLength(100);

                attachment
                    .Property(ta => ta.ContentType)
                    .IsRequired()
                    .HasMaxLength(200);

                attachment
                    .Property(ta => ta.UploadedUserId)
                    .IsRequired()
                    .HasMaxLength(450);

                attachment
                    .HasOne(ta => ta.Invoice)
                    .WithMany(t => t.Attachments)
                    .HasForeignKey(ta => ta.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);

                attachment
                    .HasOne(ta => ta.UploadedUser)
                    .WithMany()
                    .HasForeignKey(ta => ta.UploadedUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
    }
}
