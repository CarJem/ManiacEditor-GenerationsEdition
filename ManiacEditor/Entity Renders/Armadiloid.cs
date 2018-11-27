﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Armadiloid : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueVar;
            bool fliph = false;
            bool flipv = false;
            var editorAnim = EditorEntity_ini.LoadAnimation2("Armadiloid", d, 0, 0, fliph, flipv, false);
            var editorAnimHead = EditorEntity_ini.LoadAnimation2("Armadiloid", d, 1, 0, fliph, flipv, false);
            var editorAnimBoost = EditorEntity_ini.LoadAnimation2("Armadiloid", d, 3, -1, fliph, flipv, false);
            var editorAnimRider = EditorEntity_ini.LoadAnimation2("Armadiloid", d, 4, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimHead != null && editorAnimHead.Frames.Count != 0 && editorAnimBoost != null && editorAnimBoost.Frames.Count != 0 && editorAnimRider != null && editorAnimRider.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameHead = editorAnimHead.Frames[0];
                var frameBoost = editorAnimBoost.Frames[Animation.index];
                var frameRider = editorAnimRider.Frames[Animation.index];

                if (type == 0)
                {
                    Animation.ProcessAnimation(frameBoost.Entry.FrameSpeed, frameBoost.Entry.Frames.Count, frameBoost.Frame.Duration);

                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);

                    d.DrawBitmap(frameHead.Texture,
                        x + frameHead.Frame.CenterX - (fliph ? (frameHead.Frame.Width - editorAnimHead.Frames[0].Frame.Width) : 0),
                        y + frameHead.Frame.CenterY + (flipv ? (frameHead.Frame.Height - editorAnimHead.Frames[0].Frame.Height) : 0),
                        frameHead.Frame.Width, frameHead.Frame.Height, false, Transparency);

                    d.DrawBitmap(frameBoost.Texture,
                        x + frameBoost.Frame.CenterX - (fliph ? (frameBoost.Frame.Width - editorAnimBoost.Frames[0].Frame.Width) : 0),
                        y + frameBoost.Frame.CenterY + (flipv ? (frameBoost.Frame.Height - editorAnimBoost.Frames[0].Frame.Height) : 0),
                        frameBoost.Frame.Width, frameBoost.Frame.Height, false, Transparency);
                }
                else if (type == 1)
                {
                    Animation.ProcessAnimation(frameRider.Entry.FrameSpeed, frameRider.Entry.Frames.Count, frameRider.Frame.Duration);

                    d.DrawBitmap(frameRider.Texture,
                        x + frameRider.Frame.CenterX - (fliph ? (frameRider.Frame.Width - editorAnimRider.Frames[0].Frame.Width) : 0),
                        y + frameRider.Frame.CenterY + (flipv ? (frameRider.Frame.Height - editorAnimRider.Frames[0].Frame.Height) : 0),
                        frameRider.Frame.Width, frameRider.Frame.Height, false, Transparency);
                }

            }
        }

        public override string GetObjectName()
        {
            return "Armadiloid";
        }
    }
}
