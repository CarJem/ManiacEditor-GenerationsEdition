﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Iwamodoki : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Classes.Editor.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Editor.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            bool fliph = false;
            bool flipv = false;
            int type = (int)entity.attributesMap["type"].ValueEnum;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Iwamodoki", d.DevicePanel, type, 6, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && type >= 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "Iwamodoki";
        }
    }
}
