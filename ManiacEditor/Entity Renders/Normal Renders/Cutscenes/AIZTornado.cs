﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class AIZTornado : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Methods.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Scene.Sets.EditorEntity e = properties.EditorObject;
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
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("AIZTornado", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnim2 = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("AIZTornado", d.DevicePanel, 1, -1, fliph, flipv, false);
            var editorAnim3 = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("AIZTornado", d.DevicePanel, 2, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[Animation.index];
                var frame3 = editorAnim3.Frames[Animation.index];


                Animation.ProcessAnimation(frame2.Entry.SpeedMultiplyer, frame2.Entry.Frames.Count, frame2.Frame.Delay);
                Animation.ProcessAnimation2(frame3.Entry.SpeedMultiplyer, frame3.Entry.Frames.Count, frame3.Frame.Delay);

                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame3),
                    x + frame3.Frame.PivotX,
                    y + frame3.Frame.PivotY,
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame2),
                    x + frame2.Frame.PivotX,
                    y + frame2.Frame.PivotY,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "AIZTornado";
        }
    }
}
