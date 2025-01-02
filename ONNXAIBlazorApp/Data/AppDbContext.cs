using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ONNXAIBlazorApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONNXAIBlazorApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ExcelDataDto> ExcelData { get; set; }

        // SQLite connection string (local database)
        private readonly string _dbPath;

        public AppDbContext(string dbPath)
        {
            _dbPath = dbPath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // SQLite database connection
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExcelDataDto>().HasKey(x => x.Id); // Primary key
        }

    }
}
