﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class HCZOneWayDoor : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            int length = (int)(entity.attributesMap["length"].ValueVar) - 1;
            int orientation = (int)(entity.attributesMap["orientation"].ValueUInt8);
            bool fliph = false;
            bool flipv = false;
            int width = 0;
            int height = 0;
            switch (orientation) {
                case 0:
                    height = length;
                    break;
                case 1:
                    width = length;
                    break;
                case 2:
                    height = length;
                    break;
                case 3:
                    width = length;
                    break;
                default:
                    height = length;
                    break;
            }

            var editorAnim = e.LoadAnimation2("ButtonDoor", d, 0, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[e.index];
                e.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                bool wEven = width % 2 == 0;
                bool hEven = height % 2 == 0;
                for (int xx = 0; xx <= width; ++xx)
                {
                    for (int yy = 0; yy <= height; ++yy)
                    {
                        d.DrawBitmap(frame.Texture,
                            x + (wEven ? frame.Frame.CenterX : -frame.Frame.Width) + (-width / 2 + xx) * frame.Frame.Width,
                            y + (hEven ? frame.Frame.CenterY : -frame.Frame.Height) + (-height / 2 + yy) * frame.Frame.Height,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }
            }
        }

        public override string GetObjectName()
        {
            return "HCZOneWayDoor";
        }
    }
}