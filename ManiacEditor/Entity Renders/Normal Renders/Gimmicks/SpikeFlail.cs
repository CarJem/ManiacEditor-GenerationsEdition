﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SpikeFlail : EntityRenderer
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
            int chainLength = (int)entity.attributesMap["chainLength"].ValueUInt8;            
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SpikeFlail", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnimBall = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SpikeFlail", d.DevicePanel, 1, 0, fliph, flipv, false);
            var editorAnimRing1 = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SpikeFlail", d.DevicePanel, 2, 0, fliph, flipv, false);
            var editorAnimRing2 = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SpikeFlail", d.DevicePanel, 3, 0, fliph, flipv, false);
            var editorAnimRing3 = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("SpikeFlail", d.DevicePanel, 4, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimBall != null && editorAnimBall.Frames.Count != 0 && editorAnimRing1 != null && editorAnimRing1.Frames.Count != 0 && editorAnimRing2 != null && editorAnimRing2.Frames.Count != 0 && editorAnimRing3 != null && editorAnimRing3.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameBall = editorAnimBall.Frames[0];
                var frameRing1 = editorAnimRing1.Frames[0];
                var frameRing2 = editorAnimRing2.Frames[0];
                var frameRing3 = editorAnimRing3.Frames[0];

                //Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
               
                if (chainLength >= 1)
                {
                    for (int i = 0; i < chainLength; i++)
                    {
                        var frameRingI = editorAnimRing1.Frames[0];
                        if (i + 1 % 1 == 0)
                        {
                            frameRingI = editorAnimRing1.Frames[0];
                        }
                        else if (i + 1 % 2 == 0)
                        {
                            frameRingI = editorAnimRing2.Frames[0];
                        }
                        else if (i + 1 % 3 == 0)
                        {
                            frameRingI = editorAnimRing3.Frames[0];
                        }
                        else
                        {
                            frameRingI = editorAnimRing1.Frames[0];
                        }

                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frameRingI),
                            x + frameRingI.Frame.PivotX + frame.Frame.Width + frameRingI.Frame.Width*(i),
                            y + frameRingI.Frame.PivotY,
                            frameRingI.Frame.Width, frameRingI.Frame.Height, false, Transparency);
                    }
                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frameBall),
                        x + frameBall.Frame.PivotX + frame.Frame.Width + frameRing1.Frame.Width*(chainLength+1),
                        y + frameBall.Frame.PivotY + (flipv ? (frameBall.Frame.Height - editorAnimBall.Frames[0].Frame.Height) : 0),
                        frameBall.Frame.Width, frameBall.Frame.Height, false, Transparency);
                }
            }
        }

        public override string GetObjectName()
        {
            return "SpikeFlail";
        }
    }
}
