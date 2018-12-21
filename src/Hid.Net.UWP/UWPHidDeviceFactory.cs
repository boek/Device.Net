﻿using Device.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wde = Windows.Devices.Enumeration;

namespace Hid.Net.UWP
{
    public class UWPHidDeviceFactory : IDeviceFactory<UWPHidDevice>
    {
        public static void Register()
        {
            DeviceManager.Current.DeviceFactories.Add(new UWPHidDeviceFactory());
        }

        public UWPHidDevice GetDevice(DeviceDefinition deviceDefinition)
        {
            return new UWPHidDevice(deviceDefinition.DeviceId);
        }

        public async Task<IEnumerable<DeviceDefinition>> GetConnectedDeviceDefinitions(uint? vendorId, uint? productId)
        {
            var aqsFilter = $"System.Devices.InterfaceEnabled:=System.StructuredQueryType.Boolean#True AND System.DeviceInterface.Hid.VendorId:={vendorId} AND System.DeviceInterface.Hid.ProductId:={productId} ";

            var deviceInformationCollection = await wde.DeviceInformation.FindAllAsync(aqsFilter).AsTask();

            //TODO: return the vid/pid if we can get it from the properties. Also read/write buffer size

            var deviceDefinitions = deviceInformationCollection.Select(d => new DeviceDefinition { DeviceId = d.Id, DeviceType = DeviceType.Hid }).ToList();
            return deviceDefinitions;
        }
    }
}
