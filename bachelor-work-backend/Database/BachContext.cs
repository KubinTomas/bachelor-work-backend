﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace bachelor_work_backend.Database
{
    public partial class BachContext : DbContext
    {
        public BachContext()
        {
        }

        public BachContext(DbContextOptions<BachContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Subject> Subjects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-I36NBNO;initial catalog=BachelorkWorkDb;Integrated Security=True;ConnectRetryCount=0");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.Property(e => e.Fakulta).IsFixedLength(true);

                entity.Property(e => e.Katedra).IsFixedLength(true);

                entity.Property(e => e.Name).IsFixedLength(true);

                entity.Property(e => e.UcitIdno).IsFixedLength(true);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
