﻿using Device.Net.LibUsb.MacOS;
using LibUsbDotNet;
using LibUsbDotNet.LudnMonoLibUsb;
using LibUsbDotNet.Main;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Device.Net.LibUsb
{
    public abstract class LibUsbDeviceFactoryBase : IDeviceFactory
    {
        public abstract DeviceType DeviceType { get; }

        public async Task<IEnumerable<ConnectedDeviceDefinition>> GetConnectedDeviceDefinitionsAsync(FilterDeviceDefinition deviceDefinition)
        {
            return await Task.Run(() =>
            {
                UsbDeviceFinder usbDeviceFinder = null;

                if (deviceDefinition.VendorId.HasValue)
                {
                    if (deviceDefinition.ProductId.HasValue)
                    {
                        usbDeviceFinder = new UsbDeviceFinder((int)deviceDefinition.VendorId.Value, (int)deviceDefinition.ProductId.Value);
                    }
                    else
                    {
                        usbDeviceFinder = new UsbDeviceFinder((int)deviceDefinition.VendorId.Value);
                    }
                }

                var usbDevice = UsbDevice.OpenUsbDevice(usbDeviceFinder);

                var retVal = new List<ConnectedDeviceDefinition>();

                if (usbDevice != null)
                {
                    uint vendorId;
                    uint productId;

                    if (usbDevice is MonoUsbDevice monoUsbDevice)
                    {
                        vendorId = (uint)monoUsbDevice.Profile.DeviceDescriptor.VendorID;
                        productId = (uint)monoUsbDevice.Profile.DeviceDescriptor.ProductID;
                    }
                    else
                    {
                        vendorId = (uint)usbDevice.UsbRegistryInfo.Vid;
                        productId = (uint)usbDevice.UsbRegistryInfo.Pid;
                    }

                    retVal.Add(new ConnectedDeviceDefinition(usbDevice.DevicePath) { VendorId = vendorId, ProductId = productId });
                    usbDevice.Close();
                    return retVal;
                }

                return retVal;
            });
        }

        public IDevice GetDevice(ConnectedDeviceDefinition deviceDefinition)
        {
            var usbDeviceFinder = new UsbDeviceFinder((int)deviceDefinition.VendorId.Value, (int)deviceDefinition.ProductId.Value);
            var usbDevice = UsbDevice.OpenUsbDevice(usbDeviceFinder);
            return usbDevice != null ? new LibUsbDevice(usbDevice, 3000) : null;
        }
    }
}