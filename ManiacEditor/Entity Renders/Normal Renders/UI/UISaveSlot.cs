﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UISaveSlot : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {

            //int frameID = (int)entity.attributesMap["listID"].ValueEnum;
            int type = (int)entity.attributesMap["type"].ValueEnum;
            string text = "Text" + Classes.Editor.SolutionState.CurrentLanguage;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation("SaveSelect", d.DevicePanel, 0, 0, false, false, false);
            var editorAnimBorder = Editor.Instance.EntityDrawing.LoadAnimation("SaveSelect", d.DevicePanel, 0, 1, false, false, false);
            var editorAnimBackground = Editor.Instance.EntityDrawing.LoadAnimation("SaveSelect", d.DevicePanel, 0, 2, false, false, false);
            var editorAnimActualRender = Editor.Instance.EntityDrawing.LoadAnimation("EditorUIRender", d.DevicePanel, 3, 0, false, false, false);
            var editorAnimActualRenderBorder = Editor.Instance.EntityDrawing.LoadAnimation("EditorUIRender", d.DevicePanel, 3, 1, false, false, false);
            var editorAnimText = Editor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, 2, 0, false, false, false);
            var editorAnimNoSave = Editor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, 2, 2, false, false, false);
            if (type == 1)
            {
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    var frame = editorAnim.Frames[Animation.index];
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                if (editorAnimBorder != null && editorAnimBorder.Frames.Count != 0)
                {
                    var frame = editorAnimBorder.Frames[Animation.index];
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                if (editorAnimBackground != null && editorAnimBackground.Frames.Count != 0)
                {
                    var frame = editorAnimBackground.Frames[Animation.index];
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                if (editorAnimText != null && editorAnimText.Frames.Count != 0)
                {
                    var frame = editorAnimText.Frames[Animation.index];
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                if (editorAnimNoSave != null && editorAnimNoSave.Frames.Count != 0 && type == 1)
                {
                    var frame = editorAnimNoSave.Frames[Animation.index];
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }
            else
            {
                if (editorAnimActualRender != null && editorAnimActualRender.Frames.Count != 0)
                {
                    var frame = editorAnimActualRender.Frames[Animation.index];
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    if (editorAnimBackground != null && editorAnimBackground.Frames.Count != 0)
                    {
                        var frame2 = editorAnimBackground.Frames[Animation.index];
                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2), x + frame2.Frame.PivotX, y + frame2.Frame.PivotY + (frame.Frame.PivotY / 2) - 6,
                            frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                    }
                    if (editorAnimText != null && editorAnimText.Frames.Count != 0)
                    {
                        var frame2 = editorAnimText.Frames[Animation.index];
                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2), x + frame2.Frame.PivotX, y + frame2.Frame.PivotY + (frame.Frame.PivotY / 2) - 6,
                            frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                    }
                }

                if (editorAnimActualRenderBorder != null && editorAnimActualRenderBorder.Frames.Count != 0)
                {
                    var frame = editorAnimActualRenderBorder.Frames[Animation.index];
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }



        }

        public override string GetObjectName()
        {
            return "UISaveSlot";
        }
    }
}