﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProperNutritionDiary.DiaryApi.Db;

#nullable disable

namespace ProperNutritionDiary.DiaryApi.Migrations
{
    [DbContext(typeof(AppCtx))]
    [Migration("20240405162824_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProperNutritionDiary.DiaryApi.Diary.Diary", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Diaries");
                });

            modelBuilder.Entity("ProperNutritionDiary.DiaryApi.Diary.DiaryEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ConsumptionTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DiaryEntryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Weight")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("DiaryEntryId");

                    b.ToTable("DiaryEntries");
                });

            modelBuilder.Entity("ProperNutritionDiary.DiaryApi.Diary.DiaryEntry", b =>
                {
                    b.HasOne("ProperNutritionDiary.DiaryApi.Diary.Diary", null)
                        .WithMany("DiaryEntries")
                        .HasForeignKey("DiaryEntryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients.Macronutrients", "Macronutrients", b1 =>
                        {
                            b1.Property<Guid>("DiaryEntryId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<decimal>("Calories")
                                .HasColumnType("decimal(18,2)");

                            b1.Property<decimal>("Carbohydrates")
                                .HasColumnType("decimal(18,2)");

                            b1.Property<decimal>("Fats")
                                .HasColumnType("decimal(18,2)");

                            b1.Property<decimal>("Proteins")
                                .HasColumnType("decimal(18,2)");

                            b1.HasKey("DiaryEntryId");

                            b1.ToTable("DiaryEntries");

                            b1.WithOwner()
                                .HasForeignKey("DiaryEntryId");
                        });

                    b.Navigation("Macronutrients")
                        .IsRequired();
                });

            modelBuilder.Entity("ProperNutritionDiary.DiaryApi.Diary.Diary", b =>
                {
                    b.Navigation("DiaryEntries");
                });
#pragma warning restore 612, 618
        }
    }
}
