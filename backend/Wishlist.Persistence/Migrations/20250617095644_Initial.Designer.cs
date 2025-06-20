﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Wishlist.Persistence.Util;

#nullable disable

namespace Wishlist.Persistence.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20250617095644_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Wishlist")
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Wishlist.Persistence.Model.WishingList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.HasKey("Id")
                        .HasName("pk_wishing_list");

                    b.ToTable("wishing_list", "Wishlist");
                });

            modelBuilder.Entity("Wishlist.Persistence.Model.WishlistItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool>("IsDone")
                        .HasColumnType("boolean")
                        .HasColumnName("is_done");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int?>("WishlistId")
                        .HasColumnType("integer")
                        .HasColumnName("wishlist_id");

                    b.HasKey("Id")
                        .HasName("pk_wishlist_item");

                    b.HasIndex("WishlistId")
                        .HasDatabaseName("ix_wishlist_item_wishlist_id");

                    b.ToTable("wishlist_item", "Wishlist");
                });

            modelBuilder.Entity("Wishlist.Persistence.Model.WishlistItem", b =>
                {
                    b.HasOne("Wishlist.Persistence.Model.WishingList", "Wishlist")
                        .WithMany("Items")
                        .HasForeignKey("WishlistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_wishlist_item_wishing_list_wishlist_id");

                    b.Navigation("Wishlist");
                });

            modelBuilder.Entity("Wishlist.Persistence.Model.WishingList", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
