// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Infrastructure
{
    using AzureIoTHub.Portal.Domain.Entities;
    using Microsoft.EntityFrameworkCore;


    public class PortalDbContext : DbContext
    {
        public DbSet<DeviceModelProperty> DeviceModelProperties { get; set; }
        public DbSet<DeviceTag> DeviceTags { get; set; }
        public DbSet<DeviceModelCommand> DeviceModelCommands { get; set; }
        public DbSet<DeviceModel> DeviceModels { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<LorawanDevice> LorawanDevices { get; set; }
        public DbSet<DeviceTagValue> DeviceTagValues { get; set; }
        public DbSet<EdgeDeviceModel> EdgeDeviceModels { get; set; }
        public DbSet<EdgeDeviceModelCommand> EdgeDeviceModelCommands { get; set; }
        public DbSet<Concentrator> Concentrators { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public PortalDbContext(DbContextOptions<PortalDbContext> options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
