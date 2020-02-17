﻿using System.IO;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ParallaxSprite : EntityRenderer
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
            int aniID = (int)entity.attributesMap["aniID"].ValueUInt8;
            int attribute = (int)entity.attributesMap["attribute"].ValueUInt8;
            RSDKv5.Position parallaxFactor = entity.attributesMap["parallaxFactor"].ValueVector2;
            RSDKv5.Position loopPoint = entity.attributesMap["loopPoint"].ValueVector2;

            /*
            if (Editor.Instance.UIModes.AnimationsEnabled && Editor.Instance.UIModes.ParallaxAnimationChecked)
            {
                EditorLayer layer = Classes.Edit.Scene.EditorSolution.Scene.AllLayers.ElementAtOrDefault(attribute);
                if (layer != null)
                {
                    int speed = (layer.RelativeSpeed == 0 ? 1 : layer.RelativeSpeed);
                    string groupKey = string.Format("{0},{1}", speed, layer.WidthPixels);
                    x = x - ManiacEditor.EditorAnimations.AnimationTiming[groupKey].FrameIndex;

                    int xBefore = x - ManiacEditor.EditorAnimations.AnimationTiming[groupKey].FrameIndex;
                    int offsetX = layer.WidthPixels - x;

                    if (x <= 0) x = loopPoint.X.High - offsetX;
                    if (y <= 0) y = loopPoint.Y.High;
                }
            }*/

            if (Animation.parallaxSprite == "")
            {
                Animation.parallaxSprite = GetParallaxPath(Controls.Editor.MainEditor.Instance);
            }

            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorIcons2", d.DevicePanel, 0, 12, fliph, flipv, false);
            if (Classes.Editor.SolutionState.ShowParallaxSprites)
            {
                editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2(Animation.parallaxSprite, d.DevicePanel, aniID, -1, fliph, flipv, false);
            }
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                if (Classes.Editor.SolutionState.ShowParallaxSprites)
                {
                    frame = editorAnim.Frames[Animation.index];
                }

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        private string GetParallaxPath (Controls.Editor.MainEditor EditorInstance)
        {
            string name = ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.Zone.Replace("\\", "");
            string zoneName = "";
            string binPath = "";
            string parallaxName = "";
            // Normal Check First
            zoneName = name;
            parallaxName = name + "Parallax";
            binPath = Path.Combine(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, "Sprites") + '\\' + zoneName + '\\' + parallaxName + ".bin";
            if (!File.Exists(binPath))
            {
                //Stick with the Zone Name, but ditch the last char for parallax
                zoneName = name;
                parallaxName = name.Substring(0, name.Length - 1) + "Parallax";
                binPath = Path.Combine(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, "Sprites") + '\\' + zoneName + '\\' + parallaxName + ".bin";
                if (!File.Exists(binPath))
                {
                    //Remove the Last Char of the Zone Name and Parallax but use "1" for the Zone Name
                    zoneName = name.Substring(0, name.Length - 1) + "1";
                    parallaxName = name.Substring(0, name.Length - 1) + "Parallax";
                    binPath = Path.Combine(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, "Sprites") + '\\' + zoneName + '\\' + parallaxName + ".bin";
                    if (!File.Exists(binPath))
                    {
                        //Remove the Last Char of the Zone Name and Parallax but use "2" for the Zone Name
                        zoneName = name.Substring(0, name.Length - 1) + "2";
                        parallaxName = name.Substring(0, name.Length - 1) + "Parallax";
                        binPath = Path.Combine(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, "Sprites") + '\\' + zoneName + '\\' + parallaxName + ".bin";
                        if (!File.Exists(binPath))
                        {
                            //Remove the Last Char of the Zone Name and Parallax
                            zoneName = name.Substring(0, name.Length - 1);
                            parallaxName = name.Substring(0, name.Length - 1) + "Parallax";
                            binPath = Path.Combine(ManiacEditor.Classes.Editor.SolutionPaths.CurrentSceneData.DataDirectory, "Sprites") + '\\' + zoneName + '\\' + parallaxName + ".bin";
                        }
                    }
                }
            }
            //Debug.Print(zoneName);
            //Debug.Print(binPath);
            //Debug.Print(parallaxName);
            return parallaxName;
        }

        public override string GetObjectName()
        {
            return "ParallaxSprite";
        }
    }
}
