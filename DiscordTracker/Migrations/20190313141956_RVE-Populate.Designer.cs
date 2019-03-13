﻿// <auto-generated />
using System;
using DiscordTracker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiscordTracker.Migrations
{
    [DbContext(typeof(ApplicationDataContext))]
    [Migration("20190313141956_RVE-Populate")]
    partial class RVEPopulate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DiscordTracker.Data.DiscordUser", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("IRLName");

                    b.Property<bool>("IsAdmin");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("DiscordUser");
                });

            modelBuilder.Entity("DiscordTracker.Data.DiscordUserEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("EventTime");

                    b.Property<string>("Game");

                    b.Property<string>("Status");

                    b.Property<decimal>("UserId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("DiscordUserEvents");
                });

            modelBuilder.Entity("DiscordTracker.Data.DiscordVoiceChannel", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("DiscordVoiceChannel");
                });

            modelBuilder.Entity("DiscordTracker.Data.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal?>("ChannelId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<DateTime>("DateTime");

                    b.Property<string>("Message");

                    b.Property<decimal?>("UserId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.HasKey("Id");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("DiscordTracker.Data.Quote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("AddedById")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<decimal>("AuthorId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<DateTime>("Created");

                    b.Property<string>("QuoteText");

                    b.HasKey("Id");

                    b.HasIndex("AddedById");

                    b.HasIndex("AuthorId");

                    b.ToTable("Quotes");
                });

            modelBuilder.Entity("DiscordTracker.Data.RelatedVoiceEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("InitalEvent");

                    b.Property<bool>("IsPrimary");

                    b.Property<string>("SecondaryEvent");

                    b.HasKey("Id");

                    b.ToTable("RelatedVoiceEvents");
                });

            modelBuilder.Entity("DiscordTracker.Data.Setting", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("DiscordTracker.Data.VoiceEventLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("ChannelId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<DateTime>("Date");

                    b.Property<string>("EventType");

                    b.Property<decimal>("UserId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("UserId");

                    b.ToTable("VoiceEventLog");
                });

            modelBuilder.Entity("DiscordTracker.Data.DiscordUserEvent", b =>
                {
                    b.HasOne("DiscordTracker.Data.DiscordUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DiscordTracker.Data.Quote", b =>
                {
                    b.HasOne("DiscordTracker.Data.DiscordUser", "AddedBy")
                        .WithMany("QuotesSaved")
                        .HasForeignKey("AddedById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DiscordTracker.Data.DiscordUser", "Author")
                        .WithMany("QuotesAuthored")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DiscordTracker.Data.VoiceEventLog", b =>
                {
                    b.HasOne("DiscordTracker.Data.DiscordVoiceChannel", "Channel")
                        .WithMany()
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DiscordTracker.Data.DiscordUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
