// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Server.Services
{
    using System.Threading.Tasks;

    public interface ILoRaWANDeviceService
    {
        Task ExecuteLoRaWANCommand(string deviceId, string commandId);
    }
}
