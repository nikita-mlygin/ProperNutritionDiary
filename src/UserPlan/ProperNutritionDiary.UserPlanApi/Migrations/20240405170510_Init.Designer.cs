﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProperNutritionDiary.UserPlanApi.Db;

#nullable disable

namespace ProperNutritionDiary.UserPlanApi.Migrations
{
    [DbContext(typeof(AppCtx))]
    [Migration("20240405170510_Init")]
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

            modelBuilder.Entity("ProperNutritionDiary.UserPlanApi.UserPlan.UserPlan", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DateEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateStart")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("UserPlans");
                });

            modelBuilder.Entity("ProperNutritionDiary.UserPlanApi.UserPlan.UserPlan", b =>
                {
                    b.OwnsOne("ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients.Macronutrients", "MacronutrientsGoal", b1 =>
                        {
                            b1.Property<Guid>("UserPlanId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<decimal>("Calories")
                                .HasColumnType("decimal(18,2)");

                            b1.Property<decimal>("Carbohydrates")
                                .HasColumnType("decimal(18,2)");

                            b1.Property<decimal>("Fats")
                                .HasColumnType("decimal(18,2)");

                            b1.Property<decimal>("Proteins")
                                .HasColumnType("decimal(18,2)");

                            b1.HasKey("UserPlanId");

                            b1.ToTable("UserPlans");

                            b1.WithOwner()
                                .HasForeignKey("UserPlanId");
                        });

                    b.Navigation("MacronutrientsGoal")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
