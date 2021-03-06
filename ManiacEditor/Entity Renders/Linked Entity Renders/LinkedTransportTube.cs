﻿using System.Linq;
using System.Drawing;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedTransportTube : LinkedRenderer
    {
        public override void Draw(Structures.LinkedEntityRenderProp properties)
        {
            byte TransportTubeType = properties.EditorObject.GetAttribute("type").ValueUInt8;
            ushort slotID = properties.EditorObject.SlotID;
            ushort targetSlotID = (ushort)(properties.EditorObject.SlotID + 1);

            if ((TransportTubeType == 2 || TransportTubeType == 4))
            {
                var transportTubePaths = Methods.Solution.CurrentSolution.Entities.Entities.Where(e => e.SlotID == targetSlotID && e.Name == "TransportTube");

                if (transportTubePaths != null && transportTubePaths.Any())
                {
                    foreach (var ttp in transportTubePaths)
                    {
                        int destinationType = ttp.GetAttribute("type").ValueUInt8;
                        if (destinationType == 3)
                        {
                            DrawLinkArrowTransportTubes(properties.Graphics, properties.EditorObject, ttp, 3, TransportTubeType);
                        }
                        else if (destinationType == 4)
                        {
                            DrawLinkArrowTransportTubes(properties.Graphics, properties.EditorObject, ttp, 4, TransportTubeType);
                        }
                        else if (destinationType == 2)
                        {
                            DrawLinkArrowTransportTubes(properties.Graphics, properties.EditorObject, ttp, 2, TransportTubeType);
                        }
                        else
                        {
                            DrawLinkArrowTransportTubes(properties.Graphics, properties.EditorObject, ttp, 1, TransportTubeType);
                        }

                    }
                }
            }

            properties.EditorObject.DrawBase(properties.Graphics);
        }

        public void DrawLinkArrowTransportTubes(DevicePanel Graphics, ManiacEditor.Classes.Scene.EditorEntity start, ManiacEditor.Classes.Scene.EditorEntity end, int destType, int sourceType)
        {
            Color color = Color.Transparent;
            switch (destType)
            {
                case 4:
                    color = Color.Yellow;
                    break;
                case 3:
                    color = Color.Red;
                    break;
            }
            if (sourceType == 2)
            {
                switch (destType)
                {
                    case 4:
                        color = Color.Green;
                        break;
                    case 3:
                        color = Color.Red;
                        break;
                }
            }
            int startX = start.Position.X.High;
            int startY = start.Position.Y.High;
            int endX = end.Position.X.High;
            int endY = end.Position.Y.High;

            int dx = endX - startX;
            int dy = endY - startY;

            int offsetX = 0;
            int offsetY = 0;
            int offsetDestinationX = 0;
            int offsetDestinationY = 0;

            Graphics.DrawArrow(startX + offsetX,
                        startY + offsetY,
                        end.Position.X.High + offsetDestinationX,
                        end.Position.Y.High + offsetDestinationY,
                        color);
        }

        public override string GetObjectName()
        {
            return "TransportTube";
        }
    }
}
