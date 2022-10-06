﻿// <auto-generated />
using System;
using AzureIoTHub.Portal.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AzureIoTHub.Portal.Infrastructure.Migrations
{
    [DbContext(typeof(PortalDbContext))]
    [Migration("20221005134349_Add Concentrator")]
    partial class AddConcentrator
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.Concentrator", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ClientThumbprint")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DeviceType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsConnected")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("LoraRegion")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Concentrators");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.Device", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("DeviceModelId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsConnected")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("StatusUpdatedTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.DeviceModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<bool?>("ABPRelaxMode")
                        .HasColumnType("boolean");

                    b.Property<string>("AppEUI")
                        .HasColumnType("text");

                    b.Property<int>("ClassType")
                        .HasColumnType("integer");

                    b.Property<int?>("Deduplication")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool?>("Downlink")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsBuiltin")
                        .HasColumnType("boolean");

                    b.Property<int?>("KeepAliveTimeout")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("PreferredWindow")
                        .HasColumnType("integer");

                    b.Property<int?>("RXDelay")
                        .HasColumnType("integer");

                    b.Property<string>("SensorDecoder")
                        .HasColumnType("text");

                    b.Property<bool>("SupportLoRaFeatures")
                        .HasColumnType("boolean");

                    b.Property<bool?>("UseOTAA")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("DeviceModels");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.DeviceModelCommand", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<bool>("Confirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("DeviceModelId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Frame")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsBuiltin")
                        .HasColumnType("boolean");

                    b.Property<int>("Port")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("DeviceModelCommands");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.DeviceModelProperty", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsWritable")
                        .HasColumnType("boolean");

                    b.Property<string>("ModelId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<int>("PropertyType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("DeviceModelProperties");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.DeviceTag", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Required")
                        .HasColumnType("boolean");

                    b.Property<bool>("Searchable")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("DeviceTags");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.DeviceTagValue", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("DeviceId")
                        .HasColumnType("text");

                    b.Property<string>("LorawanDeviceId")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("LorawanDeviceId");

                    b.ToTable("DeviceTagValues");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.EdgeDeviceModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("EdgeDeviceModels");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.EdgeDeviceModelCommand", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("EdgeDeviceModelId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ModuleName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("EdgeDeviceModelCommands");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.LorawanDevice", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<bool?>("ABPRelaxMode")
                        .HasColumnType("boolean");

                    b.Property<bool>("AlreadyLoggedInOnce")
                        .HasColumnType("boolean");

                    b.Property<string>("AppEUI")
                        .HasColumnType("text");

                    b.Property<string>("AppKey")
                        .HasColumnType("text");

                    b.Property<string>("AppSKey")
                        .HasColumnType("text");

                    b.Property<int>("ClassType")
                        .HasColumnType("integer");

                    b.Property<string>("DataRate")
                        .HasColumnType("text");

                    b.Property<int>("Deduplication")
                        .HasColumnType("integer");

                    b.Property<string>("DevAddr")
                        .HasColumnType("text");

                    b.Property<string>("DeviceModelId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool?>("Downlink")
                        .HasColumnType("boolean");

                    b.Property<int?>("FCntDownStart")
                        .HasColumnType("integer");

                    b.Property<int?>("FCntResetCounter")
                        .HasColumnType("integer");

                    b.Property<int?>("FCntUpStart")
                        .HasColumnType("integer");

                    b.Property<string>("GatewayID")
                        .HasColumnType("text");

                    b.Property<bool>("IsConnected")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("boolean");

                    b.Property<int?>("KeepAliveTimeout")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NbRep")
                        .HasColumnType("text");

                    b.Property<string>("NwkSKey")
                        .HasColumnType("text");

                    b.Property<int>("PreferredWindow")
                        .HasColumnType("integer");

                    b.Property<int?>("RX1DROffset")
                        .HasColumnType("integer");

                    b.Property<int?>("RX2DataRate")
                        .HasColumnType("integer");

                    b.Property<int?>("RXDelay")
                        .HasColumnType("integer");

                    b.Property<string>("ReportedRX1DROffset")
                        .HasColumnType("text");

                    b.Property<string>("ReportedRX2DataRate")
                        .HasColumnType("text");

                    b.Property<string>("ReportedRXDelay")
                        .HasColumnType("text");

                    b.Property<string>("SensorDecoder")
                        .HasColumnType("text");

                    b.Property<DateTime>("StatusUpdatedTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool?>("Supports32BitFCnt")
                        .HasColumnType("boolean");

                    b.Property<string>("TxPower")
                        .HasColumnType("text");

                    b.Property<bool>("UseOTAA")
                        .HasColumnType("boolean");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("LorawanDevices");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.DeviceTagValue", b =>
                {
                    b.HasOne("AzureIoTHub.Portal.Domain.Entities.Device", null)
                        .WithMany("Tags")
                        .HasForeignKey("DeviceId");

                    b.HasOne("AzureIoTHub.Portal.Domain.Entities.LorawanDevice", null)
                        .WithMany("Tags")
                        .HasForeignKey("LorawanDeviceId");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.Device", b =>
                {
                    b.Navigation("Tags");
                });

            modelBuilder.Entity("AzureIoTHub.Portal.Domain.Entities.LorawanDevice", b =>
                {
                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}
