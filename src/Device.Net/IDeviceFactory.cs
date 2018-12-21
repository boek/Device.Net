﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Device.Net
{
    public interface IDeviceFactory
    {
        Task<IEnumerable<DeviceDefinition>> GetConnectedDeviceDefinitions(uint? vendorId, uint? productId);
    }

    public interface IDeviceFactory<T> : IDeviceFactory where T : IDevice
    {
        T GetDevice(DeviceDefinition deviceDefinition);
    }
}
