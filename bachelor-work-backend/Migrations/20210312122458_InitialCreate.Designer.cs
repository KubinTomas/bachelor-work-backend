﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using bachelor_work_backend.Database;

namespace bachelor_work_backend.Migrations
{
    [DbContext(typeof(BachContext))]
    [Migration("20210312122458_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("bachelor_work_backend.Database.Block", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id")
                        .UseIdentityColumn();

                    b.Property<DateTime>("DateIn")
                        .HasColumnType("datetime")
                        .HasColumnName("dateIn");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("isActive");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("name");

                    b.Property<int>("SubjectInYearTermId")
                        .HasColumnType("int")
                        .HasColumnName("subjectInYearTermId");

                    b.Property<string>("UcitIdno")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("ucitIdno");

                    b.HasKey("Id");

                    b.HasIndex("SubjectInYearTermId");

                    b.ToTable("Block");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.BlockAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id")
                        .UseIdentityColumn();

                    b.Property<DateTime?>("AttendanceAllowEndDate")
                        .HasColumnType("datetime")
                        .HasColumnName("attendanceAllowEndDate");

                    b.Property<DateTime?>("AttendanceAllowStartDate")
                        .HasColumnType("datetime")
                        .HasColumnName("attendanceAllowStartDate");

                    b.Property<DateTime>("AttendanceSignOffEndDate")
                        .HasColumnType("datetime")
                        .HasColumnName("attendanceSignOffEndDate");

                    b.Property<int>("BlockId")
                        .HasColumnType("int")
                        .HasColumnName("blockId");

                    b.Property<string>("Color")
                        .HasMaxLength(7)
                        .HasColumnType("nvarchar(7)")
                        .HasColumnName("color");

                    b.Property<DateTime>("DateIn")
                        .HasColumnType("datetime")
                        .HasColumnName("dateIn");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)")
                        .HasColumnName("description");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime")
                        .HasColumnName("endDate");

                    b.Property<int>("GroupId")
                        .HasColumnType("int")
                        .HasColumnName("groupId");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("isActive");

                    b.Property<string>("Location")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("location");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("name");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime")
                        .HasColumnName("startDate");

                    b.Property<string>("UcitIdno")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("ucitIdno");

                    b.Property<bool>("Visible")
                        .HasColumnType("bit")
                        .HasColumnName("visible");

                    b.HasKey("Id");

                    b.HasIndex("BlockId");

                    b.ToTable("BlockAction");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.BlockActionAttendance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id")
                        .UseIdentityColumn();

                    b.Property<int>("ActionId")
                        .HasColumnType("int")
                        .HasColumnName("actionId");

                    b.Property<DateTime>("DateIn")
                        .HasColumnType("datetime")
                        .HasColumnName("dateIn");

                    b.Property<DateTime?>("EvaluationDate")
                        .HasColumnType("datetime")
                        .HasColumnName("evaluationDate");

                    b.Property<bool>("Fulfilled")
                        .HasColumnType("bit")
                        .HasColumnName("fulfilled");

                    b.Property<string>("StudentOsCislo")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("studentOsCislo");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("userId");

                    b.HasKey("Id");

                    b.HasIndex("ActionId");

                    b.HasIndex("UserId");

                    b.ToTable("BlockActionAttendance");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.BlockActionPeopleEnrollQueue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id")
                        .UseIdentityColumn();

                    b.Property<int>("ActionId")
                        .HasColumnType("int")
                        .HasColumnName("actionId");

                    b.Property<DateTime>("DateIn")
                        .HasColumnType("datetime")
                        .HasColumnName("dateIn");

                    b.Property<string>("StudentOsCislo")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("studentOsCislo");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("userId");

                    b.HasKey("Id");

                    b.HasIndex("ActionId");

                    b.HasIndex("UserId");

                    b.ToTable("BlockActionPeopleEnrollQueue");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.BlockActionRestriction", b =>
                {
                    b.Property<int>("ActionId")
                        .HasColumnType("int")
                        .HasColumnName("actionId");

                    b.Property<bool>("AllowExternalUsers")
                        .HasColumnType("bit")
                        .HasColumnName("allowExternalUsers");

                    b.Property<bool>("AllowOnlyStudentsOnWhiteList")
                        .HasColumnType("bit")
                        .HasColumnName("allowOnlyStudentsOnWhiteList");

                    b.Property<int>("MaxCapacity")
                        .HasColumnType("int")
                        .HasColumnName("maxCapacity");

                    b.HasKey("ActionId");

                    b.ToTable("BlockActionRestriction");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.BlockRestriction", b =>
                {
                    b.Property<int>("BlockId")
                        .HasColumnType("int")
                        .HasColumnName("blockId");

                    b.Property<int>("ActionAttendLimit")
                        .HasColumnType("int")
                        .HasColumnName("actionAttendLimit");

                    b.Property<bool>("AllowExternalUsers")
                        .HasColumnType("bit")
                        .HasColumnName("allowExternalUsers");

                    b.Property<bool>("AllowOnlyStudentsOnWhiteList")
                        .HasColumnType("bit")
                        .HasColumnName("allowOnlyStudentsOnWhiteList");

                    b.HasKey("BlockId");

                    b.ToTable("BlockRestriction");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.BlockStagUserWhitelist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id")
                        .UseIdentityColumn();

                    b.Property<int>("BlockId")
                        .HasColumnType("int")
                        .HasColumnName("blockId");

                    b.Property<string>("StudentOsCislo")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("studentOsCislo");

                    b.HasKey("Id");

                    b.HasIndex("BlockId");

                    b.ToTable("BlockStagUserWhitelist");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.Subject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id")
                        .UseIdentityColumn();

                    b.Property<DateTime>("DateIn")
                        .HasColumnType("datetime")
                        .HasColumnName("dateIn");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)")
                        .HasColumnName("description");

                    b.Property<string>("Fakulta")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("fakulta");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("isActive");

                    b.Property<string>("Katedra")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("katedra");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("name");

                    b.Property<string>("UcitIdno")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("ucitIdno");

                    b.HasKey("Id");

                    b.ToTable("Subject");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.SubjectInYear", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id")
                        .UseIdentityColumn();

                    b.Property<DateTime>("DateIn")
                        .HasColumnType("datetime")
                        .HasColumnName("dateIn");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)")
                        .HasColumnName("description");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("isActive");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("name");

                    b.Property<int>("SubjectId")
                        .HasColumnType("int")
                        .HasColumnName("subjectId");

                    b.Property<string>("UcitIdno")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("ucitIdno");

                    b.Property<string>("Year")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("year");

                    b.HasKey("Id");

                    b.HasIndex("SubjectId");

                    b.ToTable("SubjectInYear");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.SubjectInYearTerm", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id")
                        .UseIdentityColumn();

                    b.Property<DateTime>("DateIn")
                        .HasColumnType("datetime")
                        .HasColumnName("dateIn");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("isActive");

                    b.Property<int>("SubjectInYearId")
                        .HasColumnType("int")
                        .HasColumnName("subjectInYearId");

                    b.Property<string>("Term")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("term");

                    b.Property<string>("UcitIdno")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("ucitIdno");

                    b.HasKey("Id");

                    b.HasIndex("SubjectInYearId");

                    b.ToTable("SubjectInYearTerm");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.TermStagConnection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id")
                        .UseIdentityColumn();

                    b.Property<DateTime>("DateIn")
                        .HasColumnType("datetime")
                        .HasColumnName("dateIn");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("department");

                    b.Property<int>("SubjectInYearTermId")
                        .HasColumnType("int")
                        .HasColumnName("subjectInYearTermId");

                    b.Property<string>("Term")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("term");

                    b.Property<string>("UcitIdno")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("ucitIdno");

                    b.Property<string>("Year")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("year");

                    b.Property<string>("ZkrPredm")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("zkrPredm");

                    b.HasKey("Id");

                    b.HasIndex("SubjectInYearTermId");

                    b.ToTable("TermStagConnection");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id")
                        .UseIdentityColumn();

                    b.Property<bool>("Confirmed")
                        .HasColumnType("bit")
                        .HasColumnName("confirmed");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("email");

                    b.Property<string>("Guid")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("guid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("password");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("surname");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.UserPasswordRecovery", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id")
                        .UseIdentityColumn();

                    b.Property<string>("Guid")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("guid");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("userId");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("datetime")
                        .HasColumnName("validUntil");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserPasswordRecovery");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.Block", b =>
                {
                    b.HasOne("bachelor_work_backend.Database.SubjectInYearTerm", "SubjectInYearTerm")
                        .WithMany("Blocks")
                        .HasForeignKey("SubjectInYearTermId")
                        .HasConstraintName("FK_Block_SubjectInYearTerm")
                        .IsRequired();

                    b.Navigation("SubjectInYearTerm");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.BlockAction", b =>
                {
                    b.HasOne("bachelor_work_backend.Database.Block", "Block")
                        .WithMany("BlockActions")
                        .HasForeignKey("BlockId")
                        .HasConstraintName("FK_BlockAction_Block")
                        .IsRequired();

                    b.Navigation("Block");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.BlockActionAttendance", b =>
                {
                    b.HasOne("bachelor_work_backend.Database.BlockAction", "Action")
                        .WithMany("BlockActionAttendances")
                        .HasForeignKey("ActionId")
                        .HasConstraintName("FK_BlockActionAttendance_BlockAction")
                        .IsRequired();

                    b.HasOne("bachelor_work_backend.Database.User", "User")
                        .WithMany("BlockActionAttendances")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_BlockActionAttendance_User");

                    b.Navigation("Action");

                    b.Navigation("User");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.BlockActionPeopleEnrollQueue", b =>
                {
                    b.HasOne("bachelor_work_backend.Database.BlockAction", "Action")
                        .WithMany("BlockActionPeopleEnrollQueues")
                        .HasForeignKey("ActionId")
                        .HasConstraintName("FK_BlockActionPeopleEnrollQueue_BlockAction")
                        .IsRequired();

                    b.HasOne("bachelor_work_backend.Database.User", "User")
                        .WithMany("BlockActionPeopleEnrollQueues")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_BlockActionPeopleEnrollQueue_User");

                    b.Navigation("Action");

                    b.Navigation("User");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.BlockActionRestriction", b =>
                {
                    b.HasOne("bachelor_work_backend.Database.BlockAction", "Action")
                        .WithOne("BlockActionRestriction")
                        .HasForeignKey("bachelor_work_backend.Database.BlockActionRestriction", "ActionId")
                        .HasConstraintName("FK_BlockActionRestriction_BlockAction")
                        .IsRequired();

                    b.Navigation("Action");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.BlockRestriction", b =>
                {
                    b.HasOne("bachelor_work_backend.Database.Block", "Block")
                        .WithOne("BlockRestriction")
                        .HasForeignKey("bachelor_work_backend.Database.BlockRestriction", "BlockId")
                        .HasConstraintName("FK_BlockRestriction_Block")
                        .IsRequired();

                    b.Navigation("Block");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.BlockStagUserWhitelist", b =>
                {
                    b.HasOne("bachelor_work_backend.Database.Block", "Block")
                        .WithMany("BlockStagUserWhitelists")
                        .HasForeignKey("BlockId")
                        .HasConstraintName("FK_BlockStagUserWhitelist_Block")
                        .IsRequired();

                    b.Navigation("Block");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.SubjectInYear", b =>
                {
                    b.HasOne("bachelor_work_backend.Database.Subject", "Subject")
                        .WithMany("SubjectInYears")
                        .HasForeignKey("SubjectId")
                        .HasConstraintName("FK_SubjectInYear_Subject")
                        .IsRequired();

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.SubjectInYearTerm", b =>
                {
                    b.HasOne("bachelor_work_backend.Database.SubjectInYear", "SubjectInYear")
                        .WithMany("SubjectInYearTerms")
                        .HasForeignKey("SubjectInYearId")
                        .HasConstraintName("FK_SubjectInYearTerm_SubjectInYear")
                        .IsRequired();

                    b.Navigation("SubjectInYear");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.TermStagConnection", b =>
                {
                    b.HasOne("bachelor_work_backend.Database.SubjectInYearTerm", "SubjectInYearTerm")
                        .WithMany("TermStagConnections")
                        .HasForeignKey("SubjectInYearTermId")
                        .HasConstraintName("FK_TermStagConnection_SubjectInYearTerm")
                        .IsRequired();

                    b.Navigation("SubjectInYearTerm");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.UserPasswordRecovery", b =>
                {
                    b.HasOne("bachelor_work_backend.Database.User", "User")
                        .WithMany("UserPasswordRecoveries")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserPasswordRecovery_User")
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.Block", b =>
                {
                    b.Navigation("BlockActions");

                    b.Navigation("BlockRestriction");

                    b.Navigation("BlockStagUserWhitelists");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.BlockAction", b =>
                {
                    b.Navigation("BlockActionAttendances");

                    b.Navigation("BlockActionPeopleEnrollQueues");

                    b.Navigation("BlockActionRestriction");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.Subject", b =>
                {
                    b.Navigation("SubjectInYears");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.SubjectInYear", b =>
                {
                    b.Navigation("SubjectInYearTerms");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.SubjectInYearTerm", b =>
                {
                    b.Navigation("Blocks");

                    b.Navigation("TermStagConnections");
                });

            modelBuilder.Entity("bachelor_work_backend.Database.User", b =>
                {
                    b.Navigation("BlockActionAttendances");

                    b.Navigation("BlockActionPeopleEnrollQueues");

                    b.Navigation("UserPasswordRecoveries");
                });
#pragma warning restore 612, 618
        }
    }
}
