﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedPlatformNode : LinkedRenderer
    {
        public override void Draw(DevicePanel d, RSDKv5.SceneEntity currentEntity, EditorEntity ObjectInstance)
        {
            ushort slotID = currentEntity.SlotID;
            ushort targetSlotID = (ushort)(currentEntity.SlotID + 1);

            //if (goProperty == 1 && destinationTag == 0) return; // probably just a destination

            // this is the start of a WarpDoor, find its partner(s)
            var nodePaths = currentEntity.Object.Entities.Where(e => e.SlotID == targetSlotID);

            if (nodePaths != null
                && nodePaths.Any())
            {
                ObjectInstance.DrawBase(d);
                // some destinations seem to be duplicated, so we must loop
                foreach (var tp in nodePaths)
                {
                    DrawCenteredLinkArrow(d, currentEntity, tp);
                }
            }
            else
            {
                ObjectInstance.DrawBase(d);
            }
        }

        public override string GetObjectName()
        {
            return "PlatformNode";
        }
    }
}