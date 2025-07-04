﻿// ⠀
// App+DataBase.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 03.06.2025.
// ⠀

using Microsoft.EntityFrameworkCore;
using TiContent.Foundation.Constants;
using TiContent.Foundation.Entities.DB;

namespace TiContent.UI.WinUI;

public partial class App
{
    public sealed partial class AppDataBaseContext : DbContext
    {
        // Tables

        public DbSet<DataBaseHistoryEntity> QueryHistoryItems => Set<DataBaseHistoryEntity>();
        public DbSet<DataBaseHydraLinkItemEntity> HydraLinksItems => Set<DataBaseHydraLinkItemEntity>();
        public DbSet<DataBaseHydraFilterItemEntity> HydraFiltersItems => Set<DataBaseHydraFilterItemEntity>();
        public DbSet<DataBaseImageEntity> ImageItems => Set<DataBaseImageEntity>();

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