﻿// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Server.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices;

    public class ConfigService : IConfigService
    {
        private readonly RegistryManager registryManager;

        public ConfigService(
            RegistryManager registry)
        {
            this.registryManager = registry;
        }

        public async Task<IEnumerable<Configuration>> GetIoTEdgeConfigurations()
        {
            var configurations = await this.registryManager.GetConfigurationsAsync(0);

            return configurations.Where(c => c.Content.ModulesContent.Any());
        }

        public async Task<IEnumerable<Configuration>> GetDevicesConfigurations()
        {
            var configurations = await this.registryManager.GetConfigurationsAsync(0);

            return configurations.Where(c => !c.Content.ModulesContent.Any());
        }

        public Task<Configuration> GetConfigItem(string id)
        {
            return this.registryManager.GetConfigurationAsync(id);
        }

        public async Task RolloutDeviceConfiguration(string deviceModel, Dictionary<string, object> desiredProperties)
        {
            await Task.CompletedTask;
        }
    }
}
