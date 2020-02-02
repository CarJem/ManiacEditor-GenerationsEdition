﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIResPicker : EntityRenderer
    {
        public override void Draw(Structures.EntityRenderProp properties)
        {
            Classes.Core.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Core.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            EditorAnimations Animation = properties.Animations;
            bool selected  = properties.isSelected;
            string text = "Text" + Classes.Core.SolutionState.CurrentLanguage;
            int arrowWidth = (int)entity.attributesMap["arrowWidth"].ValueEnum;
            if (arrowWidth != 0) arrowWidth /= 2;
            var leftArrow = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation("UIElements", d.DevicePanel, 2, 0, false, false, false);
            var rightArrow = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation("UIElements", d.DevicePanel, 2, 1, false, false, false);
            int width = (int)entity.attributesMap["size"].ValueVector2.X.High;
            int height = (int)entity.attributesMap["size"].ValueVector2.Y.High;
            double alignmentVal = 0;
            int align = (int)entity.attributesMap["align"].ValueEnum;
            switch (align)
            {
                case 0:
                    alignmentVal = -(width / 2) + 16;
                    break;
                case 1:
                    alignmentVal = (22 / 2);
                    break;
            }

			e.DrawUIButtonBack(d, x, y, width, height, 0, 0, Transparency);
            if (leftArrow != null && leftArrow.Frames.Count != 0)
            {
                var frame = leftArrow.Frames[Animation.index];
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX - arrowWidth + (int)alignmentVal, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (rightArrow != null && rightArrow.Frames.Count != 0)
            {
                var frame = rightArrow.Frames[Animation.index];
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + arrowWidth + (int)alignmentVal, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }


        }

        public override string GetObjectName()
        {
            return "UIResPicker";
        }
    }
}
