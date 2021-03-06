﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Toxomister : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            DevicePanel d = Properties.Graphics;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;
            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;

            switch (direction)
            {
                case 0:
                    break;
                case 1:
                    flipv = true;
                    break;
                /*case 2:
                    fliph = true;
                    break;
                case 3:
                    flipv = true;
                    fliph = true;
                    break;
                    */
            }

            var Animation = LoadAnimation(GetSetupAnimation(), d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

            Animation = LoadAnimation(GetSetupAnimation(), d, 1, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y + (flipv ? 45 : 0), Transparency, fliph, flipv);

        }

        public override string GetSetupAnimation()
        {
            return GetSpriteAnimationPath("/Toxomister.bin", "Toxomister", new string[] { "LRZ2", "LRZ1" });
        }

        public override string GetObjectName()
        {
            return "Toxomister";
        }
    }
}
