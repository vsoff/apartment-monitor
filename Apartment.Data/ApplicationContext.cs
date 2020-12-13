using System;
using Apartment.Data.Entities;
using Apartment.Options;
using Microsoft.EntityFrameworkCore;

namespace Apartment.Data
{
    public sealed class ApplicationContext : DbContext
    {
        private readonly string _connectionString;
        private readonly DataProviderType _providerType;

        public ApplicationContext(string connectionString, DataProviderType providerType)
        {
            _connectionString = connectionString;
            _providerType = providerType;

            if (Database.EnsureCreated())
                Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (_providerType)
            {
                case DataProviderType.MsSql:
                    optionsBuilder.UseSqlServer(_connectionString);
                    break;

                case DataProviderType.MySql:
                    optionsBuilder.UseMySql(_connectionString, builder => { builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null); });
                    break;

                default: throw new InvalidOperationException($"Неизвестный провайдер данных {_providerType}");
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemChangeEntity>().ToTable(nameof(ItemChangeEntity));
            modelBuilder.Entity<ApartmentEntity>().ToTable(ApartmentEntity.TableName);
            modelBuilder.Entity<ApartmentEntity>().HasIndex(u => u.ExternalId).IsUnique();
        }

        public DbSet<ItemChangeEntity> ItemChanges { get; set; }
        public DbSet<ApartmentEntity> Apartments { get; set; }
    }
}