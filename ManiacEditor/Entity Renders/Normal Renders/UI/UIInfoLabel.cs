﻿using RSDKv5;
using SharpDX;

namespace ManiacEditor.Entity_Renders
{
    public class UIInfoLabel : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;

            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            string text = e.attributesMap["text"].ValueString;
            int width = (int)e.attributesMap["size"].ValueVector2.X.High;
            int height = (int)e.attributesMap["size"].ValueVector2.Y.High;
            int spacingAmount = 0;
            int fullWidth = 0;

            if (width == 0) width = 1;
            foreach (char symb in text)
            {
                var editorAnim = GetFrameID(d, symb);
                fullWidth = fullWidth + editorAnim.RequestedFrame.Width;
            }

            int x2 = x - (fullWidth / 2) + fullWidth % 2;
            int y2 = y - 5;

            d.DrawQuad(x - (width / 2) - height, y - (height / 2), x + (width / 2) + height, y + (height / 2), System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), 0);

            foreach (char symb in text)
            {
                var editorAnim = GetFrameID(d, symb);
                Point Offset = GetFrame(editorAnim.RequestedFrame);
                DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x2 + spacingAmount + Offset.X, y2 + Offset.Y, Transparency);
                spacingAmount = spacingAmount + editorAnim.RequestedFrame.Width;
            }

            
        }


        Point GetFrame(RSDKv5.Animation.AnimationEntry.Frame Frame)
        {
            int AdditionalX = 0;
            int AdditionalY = 0;
            if (Frame.PivotX > 0)
            {
                AdditionalX = Frame.PivotX;
            }
            else
            {
                AdditionalX = -Frame.PivotX;
            }
            if (Frame.PivotY > 0)
            {
                AdditionalY = -Frame.PivotY;
            }
            else
            {
                AdditionalY = Frame.PivotY;
            }
            return new Point(AdditionalX, 4);
        }

        public Methods.Drawing.ObjectDrawing.EditorAnimation GetFrameID(DevicePanel d, char letter)
        {
            var editorAnim = LoadAnimation("UI/UIElements.bin", d, 4, 0);
            for (int i = 0; i < editorAnim.RequestedAnimation.Frames.Count; i++)
            {
                editorAnim = LoadAnimation("UI/UIElements.bin", d, 4, i);
                if ((double)editorAnim.RequestedFrame.ID == (double)letter) return editorAnim;
            }

            editorAnim = LoadAnimation("UI/SmallFont.bin", d, 0, 0);
            for (int i = 0; i < editorAnim.RequestedAnimation.Frames.Count; i++)
            {
                editorAnim = LoadAnimation("UI/SmallFont.bin", d, 0, i);
                if ((double)editorAnim.RequestedFrame.ID == (double)letter) return editorAnim;
            }

            return editorAnim;

        }

        public override string GetObjectName()
        {
            return "UIInfoLabel";
        }
    }
}
