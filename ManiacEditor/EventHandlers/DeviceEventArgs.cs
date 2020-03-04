﻿using System;
using SFML.Graphics;

namespace ManiacEditor.EventHandlers
{
    public delegate void RenderEventHandler(object sender, DeviceEventArgs e);
    public delegate void CreateDeviceEventHandler(object sender, DeviceEventArgs e);

    public class DeviceEventArgs : EventArgs
    {
        private RenderWindow _device;

        public RenderWindow Device
        {
            get
            {
                return _device;
            }
        }

        public DeviceEventArgs(RenderWindow device)
        {
            _device = device;
        }


    }
}
