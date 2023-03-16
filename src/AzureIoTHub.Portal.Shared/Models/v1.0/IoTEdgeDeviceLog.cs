// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Models.v10
{
    using System;

    public class IoTEdgeDeviceLog
    {
        public string Id { get; set; } = default!;
        public string Text { get; set; } = default!;
        public int LogLevel { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
