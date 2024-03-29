﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using BottleCapApi.Models;

namespace BottleCapApi.Migrations
{
  [DbContext(typeof(DatabaseContext))]
  [Migration("20210417192734_AddedEnterpriseId")]
  partial class AddedEnterpriseId
  {
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
      modelBuilder
          .HasAnnotation("Relational:MaxIdentifierLength", 63)
          .HasAnnotation("ProductVersion", "5.0.5")
          .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

      modelBuilder.Entity("BottleCapApi.Models.Game", b =>
          {
            b.Property<int>("Id")
                      .ValueGeneratedOnAdd()
                      .HasColumnType("integer")
                      .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            b.Property<string>("ChannelName")
                      .HasColumnType("text");

            b.Property<string>("EnterpriseId")
                      .HasColumnType("text");

            b.Property<string>("SlackId")
                      .HasColumnType("text");

            b.HasKey("Id");

            b.ToTable("Games");
          });

      modelBuilder.Entity("BottleCapApi.Models.Player", b =>
          {
            b.Property<int>("Id")
                      .ValueGeneratedOnAdd()
                      .HasColumnType("integer")
                      .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            b.Property<int>("BottleCaps")
                      .HasColumnType("integer");

            b.Property<int>("GameId")
                      .HasColumnType("integer");

            b.Property<string>("Name")
                      .HasColumnType("text");

            b.Property<string>("SlackName")
                      .HasColumnType("text");

            b.HasKey("Id");

            b.HasIndex("GameId");

            b.ToTable("Players");
          });

      modelBuilder.Entity("BottleCapApi.Models.Player", b =>
          {
            b.HasOne("BottleCapApi.Models.Game", "Game")
                      .WithMany("Players")
                      .HasForeignKey("GameId")
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();

            b.Navigation("Game");
          });

      modelBuilder.Entity("BottleCapApi.Models.Game", b =>
          {
            b.Navigation("Players");
          });
#pragma warning restore 612, 618
    }
  }
}
