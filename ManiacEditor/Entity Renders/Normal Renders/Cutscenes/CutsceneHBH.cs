﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class CutsceneHBH : EntityRenderer
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
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            int characterID = (int)entity.attributesMap["characterID"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            string sprite = "";
            int animID = 0;
            bool multiFrame = false;
            switch (characterID)
            {
                case 0:
                    sprite = "Gunner";
                    animID = 0;
                    break;
                case 1:
                    sprite = "Shinobi";
                    animID = 0;
                    break;
                case 2:
                    sprite = "Mystic";
                    animID = 0;
                    break;
                case 3:
                    sprite = "Rider";
                    animID = 0;
                    break;
                case 4:
                    sprite = "King";
                    animID = 0;
                    multiFrame = true;
                    break;
                case 5:
                    sprite = "Rogues";
                    animID = 0;
                    break;
                case 6:
                    sprite = "Rogues";
                    animID = 5;
                    break;
                case 7:
                    sprite = "Rogues";
                    animID = 12;
                    break;
                case 8:
                    sprite = "DamagedKing";
                    animID = 0;
                    multiFrame = true;
                    break;
                case 9:
                    sprite = "HBHPile";
                    animID = 0;
                    break;
                default:
                    sprite = "Gunner";
                    animID = 0;
                    break;
            }
            if (direction == 1)
            {
                fliph = true;
            }
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2(sprite, d.DevicePanel, animID, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (multiFrame)
            {
                string sprite2 = "";
                int animID2 = 0;
                switch (characterID)
                {
                    case 4:
                        sprite2 = "King";
                        animID2 = 14;
                        break;
                    case 8:
                        sprite = "DamagedKing";
                        animID2 = 1;
                        break;
                }
                var editorAnim2 = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2(sprite2, d.DevicePanel, animID2, -1, fliph, flipv, false);
                if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
                {
                    var frame = editorAnim2.Frames[Animation.index];

                    Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }

        }

        public override string GetObjectName()
        {
            return "CutsceneHBH";
        }
    }
}
