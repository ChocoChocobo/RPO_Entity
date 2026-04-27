using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RPO_Entity;

public partial class RpousersContext : DbContext
{
    public RpousersContext()
    {
        Database.EnsureCreated();
    }

    public RpousersContext(DbContextOptions<RpousersContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=rpousers.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
