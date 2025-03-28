﻿// <auto-generated />
using System;
using HoaiNXRazorPages.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HoaiNXRazorPages.Infrastructure.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20250315163431_nextMigrate")]
    partial class nextMigrate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HoaiNXRazorPages.Domain.Entities.Category", b =>
                {
                    b.Property<short>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasColumnName("CategoryID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<short>("CategoryId"));

                    b.Property<string>("CategoryDesciption")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit");

                    b.Property<short?>("ParentCategoryId")
                        .HasColumnType("smallint")
                        .HasColumnName("ParentCategoryID");

                    b.HasKey("CategoryId");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Category", (string)null);
                });

            modelBuilder.Entity("HoaiNXRazorPages.Domain.Entities.NewsArticle", b =>
                {
                    b.Property<string>("NewsArticleId")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("NewsArticleID");

                    b.Property<short?>("CategoryId")
                        .HasColumnType("smallint")
                        .HasColumnName("CategoryID");

                    b.Property<short?>("CreatedById")
                        .HasColumnType("smallint")
                        .HasColumnName("CreatedByID");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Headline")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime");

                    b.Property<string>("NewsContent")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("NewsSource")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<bool?>("NewsStatus")
                        .HasColumnType("bit");

                    b.Property<string>("NewsTitle")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<short?>("UpdatedById")
                        .HasColumnType("smallint")
                        .HasColumnName("UpdatedByID");

                    b.HasKey("NewsArticleId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("CreatedById");

                    b.ToTable("NewsArticle", (string)null);
                });

            modelBuilder.Entity("HoaiNXRazorPages.Domain.Entities.SystemAccount", b =>
                {
                    b.Property<short>("AccountId")
                        .HasColumnType("smallint")
                        .HasColumnName("AccountID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<short>("AccountId"));

                    b.Property<string>("AccountEmail")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("nvarchar(70)");

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("AccountPassword")
                        .HasMaxLength(70)
                        .HasColumnType("nvarchar(70)");

                    b.Property<int?>("AccountRole")
                        .HasColumnType("int");

                    b.HasKey("AccountId");

                    b.ToTable("SystemAccount", (string)null);
                });

            modelBuilder.Entity("HoaiNXRazorPages.Domain.Entities.Tag", b =>
                {
                    b.Property<int>("TagId")
                        .HasColumnType("int")
                        .HasColumnName("TagID");

                    b.Property<string>("Note")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("TagName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("TagId")
                        .HasName("PK_HashTag");

                    b.ToTable("Tag", (string)null);
                });

            modelBuilder.Entity("NewsTag", b =>
                {
                    b.Property<string>("NewsArticleId")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("NewsArticleID");

                    b.Property<int>("TagId")
                        .HasColumnType("int")
                        .HasColumnName("TagID");

                    b.HasKey("NewsArticleId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("NewsTag", (string)null);
                });

            modelBuilder.Entity("HoaiNXRazorPages.Domain.Entities.Category", b =>
                {
                    b.HasOne("HoaiNXRazorPages.Domain.Entities.Category", "ParentCategory")
                        .WithMany("InverseParentCategory")
                        .HasForeignKey("ParentCategoryId")
                        .HasConstraintName("FK_Category_Category");

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("HoaiNXRazorPages.Domain.Entities.NewsArticle", b =>
                {
                    b.HasOne("HoaiNXRazorPages.Domain.Entities.Category", "Category")
                        .WithMany("NewsArticles")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_NewsArticle_Category");

                    b.HasOne("HoaiNXRazorPages.Domain.Entities.SystemAccount", "CreatedBy")
                        .WithMany("NewsArticles")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_NewsArticle_SystemAccount");

                    b.Navigation("Category");

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("NewsTag", b =>
                {
                    b.HasOne("HoaiNXRazorPages.Domain.Entities.NewsArticle", null)
                        .WithMany()
                        .HasForeignKey("NewsArticleId")
                        .IsRequired()
                        .HasConstraintName("FK_NewsTag_NewsArticle");

                    b.HasOne("HoaiNXRazorPages.Domain.Entities.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagId")
                        .IsRequired()
                        .HasConstraintName("FK_NewsTag_Tag");
                });

            modelBuilder.Entity("HoaiNXRazorPages.Domain.Entities.Category", b =>
                {
                    b.Navigation("InverseParentCategory");

                    b.Navigation("NewsArticles");
                });

            modelBuilder.Entity("HoaiNXRazorPages.Domain.Entities.SystemAccount", b =>
                {
                    b.Navigation("NewsArticles");
                });
#pragma warning restore 612, 618
        }
    }
}
