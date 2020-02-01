﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class AIZEggRobo : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("AIZEggRobo", d.DevicePanel, 0, -1, fliph, flipv, false);
            var editorAnimArms = Editor.Instance.EntityDrawing.LoadAnimation2("AIZEggRobo", d.DevicePanel, 1, -1, fliph, flipv, false);
            var editorAnimLegs = Editor.Instance.EntityDrawing.LoadAnimation2("AIZEggRobo", d.DevicePanel, 2, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimArms != null && editorAnimArms.Frames.Count != 0 && editorAnimLegs != null && editorAnimLegs.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frame2 = editorAnimArms.Frames[Animation.index];
                var frame3 = editorAnimLegs.Frames[Animation.index];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                    x + frame2.Frame.PivotX - (fliph ? (frame2.Frame.Width - editorAnimArms.Frames[0].Frame.Width) : 0),
                    y + frame2.Frame.PivotY + (flipv ? (frame2.Frame.Height - editorAnimArms.Frames[0].Frame.Height) : 0),
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame3),
                    x + frame3.Frame.PivotX - (fliph ? (frame3.Frame.Width - editorAnimLegs.Frames[0].Frame.Width) : 0),
                    y + frame3.Frame.PivotY + (flipv ? (frame3.Frame.Height - editorAnimLegs.Frames[0].Frame.Height) : 0),
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "AIZEggRobo";
        }
    }
}