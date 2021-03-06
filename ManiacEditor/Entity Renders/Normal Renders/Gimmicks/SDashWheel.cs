﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SDashWheel : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity entity = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            if (direction == 0) fliph = true;
            var editorAnim = LoadAnimation("SDashWheel", d, 0, 0);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency, fliph, flipv);
            var editorAnimKnob = LoadAnimation("SDashWheel", d, 2, 0);
            DrawTexturePivotNormal(d, editorAnimKnob, editorAnimKnob.RequestedAnimID, editorAnimKnob.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "SDashWheel";
        }
    }
}
