// ⠀
// App+DataBase.cs
// TiContent.UI.WPF
//
// Created by the_timick on 15.05.2025.
// ⠀

using Microsoft.EntityFrameworkCore;
using TiContent.UI.WPF.Entities.DB;
using TiContent.UI.WPF.Entities.Legacy.HydraLinks;

namespace TiContent.UI.WPF.Application;

public partial class App
{
    public sealed class AppDataBaseContext : DbContext
    {
        // Data

        public DbSet<HydraLinksEntity> HydraLinksItems => Set<HydraLinksEntity>();

        public DbSet<ImageEntity> Images => Set<ImageEntity>();

        // LifeCycle

        public AppDataBaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={AppConstants.FileNames.DataBaseFileName}");
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }
    }
}
