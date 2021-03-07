using System;
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

        public virtual DbSet<Block> Blocks { get; set; }
        public virtual DbSet<BlockAction> BlockActions { get; set; }
        public virtual DbSet<BlockActionAttendance> BlockActionAttendances { get; set; }
        public virtual DbSet<BlockActionPeopleEnrollQueue> BlockActionPeopleEnrollQueues { get; set; }
        public virtual DbSet<BlockActionRestriction> BlockActionRestrictions { get; set; }
        public virtual DbSet<BlockRestriction> BlockRestrictions { get; set; }
        public virtual DbSet<BlockStagUserWhitelist> BlockStagUserWhitelists { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<SubjectInYear> SubjectInYears { get; set; }
        public virtual DbSet<SubjectInYearTerm> SubjectInYearTerms { get; set; }
        public virtual DbSet<TermStagConnection> TermStagConnections { get; set; }
        public virtual DbSet<User> Users { get; set; }

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

            modelBuilder.Entity<Block>(entity =>
            {
                entity.HasOne(d => d.SubjectInYearTerm)
                    .WithMany(p => p.Blocks)
                    .HasForeignKey(d => d.SubjectInYearTermId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Block_SubjectInYearTerm");
            });

            modelBuilder.Entity<BlockAction>(entity =>
            {
                entity.HasOne(d => d.Block)
                    .WithMany(p => p.BlockActions)
                    .HasForeignKey(d => d.BlockId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BlockAction_Block");
            });

            modelBuilder.Entity<BlockActionAttendance>(entity =>
            {
                entity.HasOne(d => d.Action)
                    .WithMany(p => p.BlockActionAttendances)
                    .HasForeignKey(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BlockActionAttendance_BlockAction");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.BlockActionAttendances)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_BlockActionAttendance_User");
            });

            modelBuilder.Entity<BlockActionPeopleEnrollQueue>(entity =>
            {
                entity.HasOne(d => d.Action)
                    .WithMany(p => p.BlockActionPeopleEnrollQueues)
                    .HasForeignKey(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BlockActionPeopleEnrollQueue_BlockAction");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.BlockActionPeopleEnrollQueues)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_BlockActionPeopleEnrollQueue_User");
            });

            modelBuilder.Entity<BlockActionRestriction>(entity =>
            {
                entity.Property(e => e.ActionId).ValueGeneratedNever();

                entity.HasOne(d => d.Action)
                    .WithOne(p => p.BlockActionRestriction)
                    .HasForeignKey<BlockActionRestriction>(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BlockActionRestriction_BlockAction");
            });

            modelBuilder.Entity<BlockRestriction>(entity =>
            {
                entity.Property(e => e.BlockId).ValueGeneratedNever();

                entity.HasOne(d => d.Block)
                    .WithOne(p => p.BlockRestriction)
                    .HasForeignKey<BlockRestriction>(d => d.BlockId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BlockRestriction_Block");
            });

            modelBuilder.Entity<BlockStagUserWhitelist>(entity =>
            {
                entity.HasOne(d => d.Block)
                    .WithMany(p => p.BlockStagUserWhitelists)
                    .HasForeignKey(d => d.BlockId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BlockStagUserWhitelist_Block");
            });

            modelBuilder.Entity<SubjectInYear>(entity =>
            {
                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.SubjectInYears)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubjectInYear_Subject");
            });

            modelBuilder.Entity<SubjectInYearTerm>(entity =>
            {
                entity.HasOne(d => d.SubjectInYear)
                    .WithMany(p => p.SubjectInYearTerms)
                    .HasForeignKey(d => d.SubjectInYearId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubjectInYearTerm_SubjectInYear");
            });

            modelBuilder.Entity<TermStagConnection>(entity =>
            {
                entity.HasOne(d => d.SubjectInYearTerm)
                    .WithMany(p => p.TermStagConnections)
                    .HasForeignKey(d => d.SubjectInYearTermId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TermStagConnection_SubjectInYearTerm");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
