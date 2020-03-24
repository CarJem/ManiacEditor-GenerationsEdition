﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class BoundsMarker : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            var Animation = Methods.Entities.EntityDrawing.LoadAnimation(Properties.Graphics, "EditorIcons", 0, 2);
            DrawTexturePivotNormal(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);

        }

        public override string GetObjectName()
        {
            return "BoundsMarker";
        }
    }
}