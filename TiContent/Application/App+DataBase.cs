// ⠀
// App+DataBase.cs
// TiContent
// 
// Created by the_timick on 15.05.2025.
// ⠀

using Microsoft.EntityFrameworkCore;
using TiContent.Entities.HydraLinks;

namespace TiContent.Application;

public partial class App
{
    public sealed class AppDataBaseContext : DbContext
    {
        // Data
        
        public DbSet<HydraLinksEntity> HydraLinksItems => Set<HydraLinksEntity>();

        // LifeCycle
        
        public AppDataBaseContext() => Database.EnsureCreated();
    
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={AppConstants.FileNames.DataBaseFileName}");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HydraLinksEntity>();
            base.OnModelCreating(modelBuilder);
        }
    }
}