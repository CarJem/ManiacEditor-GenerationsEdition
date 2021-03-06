﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class WarpDoor : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            bool fliph = false;
            bool flipv = false;

            var width = (int)(e.attributesMap["width"].ValueUInt32) - 1;
            var height = (int)(e.attributesMap["height"].ValueUInt32) - 1;
            var Animation = LoadAnimation("Global/PlaneSwitch.bin", d, 0, 0);
            bool wEven = width % 2 == 0;
            bool hEven = height % 2 == 0;
            for (int xx = 0; xx <= width; ++xx)
            {
                for (int yy = 0; yy <= height; ++yy)
                {
                    int pos_x = x + (wEven ? Animation.RequestedFrame.PivotX : -Animation.RequestedFrame.Width) + (-width / 2 + xx) * Animation.RequestedFrame.Width;
                    int pos_y = y + (hEven ? Animation.RequestedFrame.PivotY : -Animation.RequestedFrame.Height) + (-height / 2 + yy) * Animation.RequestedFrame.Height;

                    DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, pos_x, pos_y, Transparency, fliph, flipv);
                }
            }
        }

        public override string GetObjectName()
        {
            return "WarpDoor";
        }
    }
}
