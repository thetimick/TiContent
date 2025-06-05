// ⠀
// App+DataBase.cs
// TiContent.WinUI
// 
// Created by the_timick on 03.06.2025.
// ⠀

using Microsoft.EntityFrameworkCore;
using TiContent.Constants;
using TiContent.Entities.DB;

namespace TiContent.WinUI;

public partial class App
{
    public sealed partial class AppDataBaseContext : DbContext
    {
        // Data
        
        public DbSet<DataBaseHistoryEntity> QueryHistoryItems => Set<DataBaseHistoryEntity>();
        public DbSet<DataBaseHydraLinksEntity> HydraLinksItems => Set<DataBaseHydraLinksEntity>();
        
        // LifeCycle
        
        public AppDataBaseContext() => Database.EnsureCreated();
    
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={AppConstants.FileNames.DataBaseFileName}");
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }
    }
}