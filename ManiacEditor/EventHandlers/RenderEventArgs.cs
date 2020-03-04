﻿using System;
using SharpDX.Direct3D9;

namespace ManiacEditor.EventHandlers
{
    public delegate void RenderEventHandler(object sender, DeviceEventArgs e);
    public delegate void CreateDeviceEventHandler(object sender, DeviceEventArgs e);

    public class DeviceEventArgs : EventArgs
    {
        private Device _device;

        public Device Device
        {
            get
            {
                return _device;
            }
        }

        public DeviceEventArgs(Device device)
        {
            _device = device;
        }


    }
}
