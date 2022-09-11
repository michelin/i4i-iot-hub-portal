// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Server.Mappers
{
    using AutoMapper;
    using Domain.Entities;
    using Models.v10.LoRaWAN;

    public class DeviceModelCommandProfile : Profile
    {
        public DeviceModelCommandProfile()
        {
            _ = CreateMap<DeviceModelCommandDto, DeviceModelCommand>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Name))
                .ReverseMap();
        }
    }
}
