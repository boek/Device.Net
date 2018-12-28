﻿using Device.Net;
using Device.Net.Windows;
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using static Hid.Net.Windows.HidAPICalls;

namespace Hid.Net.Windows
{
    public class WindowsHidDeviceFactory : WindowsDeviceFactoryBase, IDeviceFactory
    {
        #region Public Override Properties
        public override DeviceType DeviceType => DeviceType.Hid;
        public override Guid ClassGuid { get; set; } = WindowsDeviceConstants.GUID_DEVINTERFACE_HID;
        #endregion

        #region Public Methods
        public IDevice GetDevice(DeviceDefinition deviceDefinition)
        {
            return deviceDefinition.DeviceType != DeviceType ? null : new WindowsHidDevice(deviceDefinition);
        }
        #endregion

        #region Private Static Methods
        protected override DeviceDefinition GetDeviceDefinition(string deviceId)
        {
            using (var safeFileHandle = APICalls.CreateFile(deviceId, APICalls.GenericRead | APICalls.GenericWrite, APICalls.FileShareRead | APICalls.FileShareWrite, IntPtr.Zero, APICalls.OpenExisting, 0, IntPtr.Zero))
            {
                var hidAttributes = new HidAttributes();
                var product = string.Empty;
                var serialNumber = string.Empty;

                var hidCollectionCapabilities = GetHidCapabilities(safeFileHandle);

                var pointerToBuffer = Marshal.AllocHGlobal(126);

                var manufacturer = GetManufacturerString(safeFileHandle);

                if (HidD_GetSerialNumberString(safeFileHandle, pointerToBuffer, 126))
                {
                    serialNumber = Marshal.PtrToStringUni(pointerToBuffer);
                }

                if (HidD_GetProductString(safeFileHandle, pointerToBuffer, 126))
                {
                    product = Marshal.PtrToStringUni(pointerToBuffer);
                }

                Marshal.FreeHGlobal(pointerToBuffer);

                //TODO: Deal with issues here

                var deviceInformation = new WindowsHidDeviceDefinition
                {
                    DeviceId = deviceId,
                    //TODO Is this the right way around?
                    WriteBufferSize = hidCollectionCapabilities.InputReportByteLength,
                    ReadBufferSize = hidCollectionCapabilities.OutputReportByteLength,
                    Manufacturer = manufacturer,
                    Product = product,
                    ProductId = (ushort)hidAttributes.ProductId,
                    SerialNumber = serialNumber,
                    Usage = hidCollectionCapabilities.Usage,
                    UsagePage = hidCollectionCapabilities.UsagePage,
                    VendorId = (ushort)hidAttributes.VendorId,
                    VersionNumber = (ushort)hidAttributes.VersionNumber,
                    DeviceType = DeviceType.Hid
                };

                return deviceInformation;
            }
        }
        #endregion

        #region Public Static Methods
        public static void Register()
        {
            DeviceManager.Current.DeviceFactories.Add(new WindowsHidDeviceFactory());
        }
        #endregion
    }
}
