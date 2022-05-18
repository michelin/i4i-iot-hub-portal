// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Server.Services
{
    using Azure.Data.Tables;
    using AzureIoTHub.Portal.Server.Factories;
    using AzureIoTHub.Portal.Server.Mappers;
    using AzureIoTHub.Portal.Models.v10;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure;
    using Exceptions;

    public class DeviceTagService : IDeviceTagService
    {
        /// <summary>
        /// The table client factory.
        /// </summary>
        private readonly ITableClientFactory tableClientFactory;

        /// <summary>
        /// The device tag mapper.
        /// </summary>
        private readonly IDeviceTagMapper deviceTagMapper;

        /// <summary>
        /// The default partition key in AzureDataTable
        /// </summary>
        public const string DefaultPartitionKey = "0";

        public DeviceTagService(IDeviceTagMapper deviceTagMapper, ITableClientFactory tableClientFactory)
        {
            this.deviceTagMapper = deviceTagMapper;
            this.tableClientFactory = tableClientFactory;
        }

        public IEnumerable<DeviceTag> GetAllTags()
        {
            var tagList = this.tableClientFactory
                            .GetDeviceTagSettings()
                            .Query<TableEntity>()
                            .Select(this.deviceTagMapper.GetDeviceTag);

            return tagList.ToList();
        }

        public IEnumerable<string> GetAllTagsNames()
        {
            try
            {
                var tagNameList = this.tableClientFactory
                    .GetDeviceTagSettings()
                    .Query<TableEntity>()
                    .Select(c => this.deviceTagMapper.GetDeviceTag(c).Name);

                return tagNameList.ToList();
            }
            catch (RequestFailedException e)
            {
                throw new InternalServerErrorException($"Unable to query device tags names: {e.Message}", e);
            }
        }

        public IEnumerable<string> GetAllSearchableTagsNames()
        {
            try
            {
                var tagNameList = this.tableClientFactory
                    .GetDeviceTagSettings()
                    .Query<TableEntity>()
                    .Where(c => this.deviceTagMapper.GetDeviceTag(c).Searchable)
                    .Select(c => this.deviceTagMapper.GetDeviceTag(c).Name);

                return tagNameList.ToList();
            }
            catch (RequestFailedException e)
            {
                throw new InternalServerErrorException($"Unable to query searchable device tags names: {e.Message}", e);
            }
        }

        public async Task UpdateTags(IEnumerable<DeviceTag> tags)
        {
            ArgumentNullException.ThrowIfNull(tags, nameof(tags));

            var query = this.tableClientFactory
                        .GetDeviceTagSettings()
                        .Query<TableEntity>();

            foreach (var item in query)
            {
                _ = await this.tableClientFactory
                    .GetDeviceTagSettings()
                    .DeleteEntityAsync(item.PartitionKey, item.RowKey);
            }

            foreach (var tag in tags)
            {
                var entity = new TableEntity()
                {
                    PartitionKey = DefaultPartitionKey,
                    RowKey = tag.Name
                };
                await SaveEntity(entity, tag);
            }
        }

        /// <summary>
        /// Saves the entity.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="tag">The device tag</param>
        private async Task SaveEntity(TableEntity entity, DeviceTag tag)
        {
            this.deviceTagMapper.UpdateTableEntity(entity, tag);
            _ = await this.tableClientFactory
                .GetDeviceTagSettings()
                .AddEntityAsync(entity);
        }
    }
}
