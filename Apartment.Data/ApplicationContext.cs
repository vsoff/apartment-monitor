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

        // TODO: Удалить потом. Добавил на время разработки, чтобы проводить миграции.
        public ApplicationContext() : this("Data Source=localhost;Initial Catalog=ApartmentsTest;Integrated Security=True;MultipleActiveResultSets=True", DataProviderType.MsSql)
        {
        }

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
            modelBuilder.Entity<RegionEntity>().ToTable(RegionEntity.TableName);
            modelBuilder.Entity<ItemChangeEntity>().ToTable(ItemChangeEntity.TableName);
            modelBuilder.Entity<ApartmentEntity>().ToTable(ApartmentEntity.TableName);
            modelBuilder.Entity<ApartmentEntity>().HasIndex(u => u.ExternalId).IsUnique();
        }

        public DbSet<ItemChangeEntity> ItemChanges { get; set; }
        public DbSet<ApartmentEntity> Apartments { get; set; }
        public DbSet<RegionEntity> Regions { get; set; }
    }
}