﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace ManiacEditor.Classes.Editor
{
    public class SolutionState
    {
        private static ManiacEditor.Controls.Editor.MainEditor Instance;
        public SolutionState(ManiacEditor.Controls.Editor.MainEditor instance)
        {
            Instance = instance;
        }

        #region Editor Status Variables

        public static bool IsEditing()
        {
            return IsTilesEdit() || IsEntitiesEdit() || IsChunksEdit();
        }
        public static bool IsSceneLoaded()
        {
            if (Classes.Editor.Solution.CurrentScene != null)
                return true;
            else
                return false;
        }
        public static bool IsTilesEdit()
        {
            return Classes.Editor.Solution.EditLayerA != null;
        }
        public static bool IsChunksEdit()
        {
            return Instance.EditorToolbar.ChunksToolButton.IsChecked.Value && Classes.Editor.Solution.EditLayerA != null;
        }
        public static bool IsEntitiesEdit()
        {
            return Instance.EditorToolbar.EditEntities.IsCheckedN.Value || Instance.EditorToolbar.EditEntities.IsCheckedA.Value || Instance.EditorToolbar.EditEntities.IsCheckedB.Value;
        }
        public static bool IsSelected(bool dualModeSelect = false)
        {
            if (IsTilesEdit())
            {

                bool SelectedA = Classes.Editor.Solution.EditLayerA?.SelectedTiles.Count > 0 || Classes.Editor.Solution.EditLayerA?.TempSelectionTiles.Count > 0;
                bool SelectedB = Classes.Editor.Solution.EditLayerB?.SelectedTiles.Count > 0 || Classes.Editor.Solution.EditLayerB?.TempSelectionTiles.Count > 0;
                if (dualModeSelect) return SelectedA && SelectedB;
                else return SelectedA || SelectedB;
            }
            else if (IsEntitiesEdit())
            {
                return Classes.Editor.Solution.Entities.IsSelected();
            }
            return false;
        }

        #endregion

        #region Dragging Variables

        public static int DraggedX { get; set; }
        public static int DraggedY { get; set; }
        public static bool DraggingSelection { get; set; } = false; //Determines if we are dragging a selection
        public static bool Dragged { get; set; } = false;
        public static bool StartDragged { get; set; } = false;

        #endregion

        #region Auto Scrolling Variables

        public static bool AutoScrolling { get; set; } = false; //Determines if the User is Scrolling
        public static bool AutoScrollingDragged { get; set; } = false;
        public static Point AutoScrollPosition { get; set; } //For Getting the Scroll Position

        #endregion

        #region Zooming Variables

        public static bool Zooming { get; set; } = false;  //Detects if we are zooming
        public static double Zoom { get; set; } = 1; //Double Value for Zoom Levels
        public static int ZoomLevel { get; set; } = 0; //Interger Value for Zoom Levels

        #endregion

        #region Screen Positon Variables
        public static int ScreenWidth { get; set; }
        public static int ScreenHeight { get; set; }
        public static int CustomX { get; set; } = 0;
        public static int CustomY { get; set; } = 0;
        #endregion

        #region View Position Controls

        public static int ViewPositionX { get => GetViewPositionX(); set => SetViewPositionX(value); }
        public static int ViewPositionY { get => GetViewPositionY(); set => SetViewPositionY(value); }


        public static int GetViewPositionX()
        {
            if (ManiacEditor.Controls.Editor.MainEditor.Instance.DeviceModel.hScrollBar1 != null)
            {
                int valueToReturn = 0;
                ManiacEditor.Controls.Editor.MainEditor.Instance.DeviceModel.hScrollBar1.Dispatcher.Invoke(() => 
                {
                    valueToReturn = (int)ManiacEditor.Controls.Editor.MainEditor.Instance.DeviceModel.hScrollBar1.Value;
                });
                return valueToReturn;
            }
            else return 0;
        }

        public static int GetViewPositionY()
        {
            if (ManiacEditor.Controls.Editor.MainEditor.Instance.DeviceModel.vScrollBar1 != null)
            {
                int valueToReturn = 0;
                ManiacEditor.Controls.Editor.MainEditor.Instance.DeviceModel.vScrollBar1.Dispatcher.Invoke(() =>
                {
                    valueToReturn = (int)ManiacEditor.Controls.Editor.MainEditor.Instance.DeviceModel.vScrollBar1.Value;
                });
                return valueToReturn;
            }
            else return 0;
        }

        public static void SetViewPositionX(int value)
        {
            if (ManiacEditor.Controls.Editor.MainEditor.Instance.DeviceModel.hScrollBar1 != null)
            {
                ManiacEditor.Controls.Editor.MainEditor.Instance.DeviceModel.hScrollBar1.Value = value;
            }
        }

        public static void SetViewPositionY(int value)
        {
            if (ManiacEditor.Controls.Editor.MainEditor.Instance.DeviceModel.vScrollBar1 != null)
            {
                ManiacEditor.Controls.Editor.MainEditor.Instance.DeviceModel.vScrollBar1.Value = value;
            }
        }

        #endregion

        #region Selection Regions Variables
        public static int RegionX1 { get; set; } = -1;
        public static int RegionY1 { get; set; } = -1;
        public static int RegionX2 { get; set; }
        public static int RegionY2 { get; set; }

        public static int TempSelectX1 { get; set; }
        public static int TempSelectX2 { get; set; }
        public static int TempSelectY1 { get; set; }
        public static int TempSelectY2 { get; set; }

        public static int LastX { get; set; }
        public static int LastY { get; set; }
        #endregion

        #region Tile Selection Variables

        public static int SelectedTilesCount { get; set; } //Used to get the Amount of Selected Tiles in a Selection
        public static int DeselectTilesCount { get; set; } //Used in combination with SelectedTilesCount to get the definitive amount of Selected Tiles
        public static int SelectedTileX { get; set; } = 0; //Used to get a single Selected Tile's X
        public static int SelectedTileY { get; set; } = 0; //Used to get a single Selected Tile's Y
        public static bool isTileDrawing { get; set; } = false;

        #endregion

        #region Old User State Model

        #region View Options

        public static bool EntitiesVisibileAboveAllLayers
        {
            get
            {
                return _EntitiesVisibileAboveAllLayers;
            }
            set
            {
                _EntitiesVisibileAboveAllLayers = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.SelectionBoxesAlwaysPrioritized.IsChecked = value;
            }
        }
        private static bool _EntitiesVisibileAboveAllLayers = false;

        public static bool PrioritizedEntityViewing
        {
            get
            {
                return _PrioritizedEntityViewing;
            }
            set
            {
                _PrioritizedEntityViewing = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.prioritizedViewingToolStripMenuItem.IsChecked = value;
            }
        }
        private static bool _PrioritizedEntityViewing = false;

        public static bool ParallaxAnimationChecked
        {
            get
            {
                return _ParallaxAnimationChecked;
            }
            set
            {
                _ParallaxAnimationChecked = value;
                Methods.Internal.UserInterface.UpdateControls();
            }
        }
        private static bool _ParallaxAnimationChecked = false;

        public static bool AllowAnimations
        {
            get
            {
                return _AllowAnimations;
            }
            set
            {
                _AllowAnimations = value;
                Methods.Internal.UserInterface.UpdateControls();
            }
        }
        private static bool _AllowAnimations = true;

        public static bool AllowSpriteAnimations
        {
            get
            {
                return _AllowSpriteAnimations;
            }
            set
            {
                _AllowSpriteAnimations = value;
            }
        }
        private static bool _AllowSpriteAnimations = true;

        public static bool AllowMovingPlatformAnimations
        {
            get
            {
                return _AllowMovingPlatformAnimations;
            }
            set
            {
                _AllowMovingPlatformAnimations = value;
            }
        }
        private static bool _AllowMovingPlatformAnimations = true;

        public static bool EntitySelectionBoxesAlwaysPrioritized
        {
            get
            {
                return _EntitySelectionBoxesAlwaysPrioritized;
            }
            set
            {
                _EntitySelectionBoxesAlwaysPrioritized = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.SelectionBoxesAlwaysPrioritized.IsChecked = value;
            }
        }
        private static bool _EntitySelectionBoxesAlwaysPrioritized = false;

        public static bool ShowTileID
        {
            get { return _ShowTileID; }
            set
            {
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.ShowTileIDButton.IsChecked = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.ReloadSpecificTextures(null, null);
                _ShowTileID = value;
            }
        }
        private static bool _ShowTileID;

        public static bool ShowGrid
        {
            get { return _ShowGrid; }
            set
            {
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.ShowGridToggleButton.IsChecked = value;
                _ShowGrid = value;
            }
        }
        private static bool _ShowGrid;

        public static bool UseEncoreColors
        {
            get { return _UseEncoreColors; }
            set
            {
                ManiacEditor.Controls.Editor.MainEditor.Instance.DisposeTextures();
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EncorePaletteButton.IsChecked = value;
                _UseEncoreColors = value;
                Classes.Editor.Solution.CurrentTiles?.Image.Reload((value ? ManiacEditor.Controls.Editor.MainEditor.Instance.EncorePalette[0] : null));
                ManiacEditor.Controls.Editor.MainEditor.Instance.TilesToolbar?.Reload((value ? ManiacEditor.Controls.Editor.MainEditor.Instance.EncorePalette[0] : null));
                ManiacEditor.Controls.Editor.MainEditor.Instance.EntityDrawing.ReleaseResources();
            }
        }
        private static bool _UseEncoreColors = false;

        public static bool ShowCollisionA
        {
            get { return _ShowCollisionA; }
            set
            {
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.ShowCollisionAButton.IsChecked = value;
                _ShowCollisionA = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.ShowCollisionBButton.IsChecked = false;
                _ShowCollisionB = false;
                ManiacEditor.Controls.Editor.MainEditor.Instance.ReloadSpecificTextures(null, null);
            }
        }
        private static bool _ShowCollisionA;

        public static bool ShowCollisionB
        {
            get { return _ShowCollisionB; }
            set
            {
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.ShowCollisionAButton.IsChecked = false;
                _ShowCollisionA = false;
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.ShowCollisionBButton.IsChecked = value;
                _ShowCollisionB = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.ReloadSpecificTextures(null, null);
            }
        }
        private static bool _ShowCollisionB;

        public static bool ShowParallaxSprites
        {
            get { return _ShowParallaxSprites; }
            set
            {
                _ShowParallaxSprites = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.showParallaxSpritesToolStripMenuItem.IsChecked = value;
            }
        }
        private static bool _ShowParallaxSprites = false;

        public static bool ApplyEditEntitiesTransparency
        {
            get { return _ApplyEditEntitiesTransparency; }
            set
            {
                _ApplyEditEntitiesTransparency = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.EditEntitiesTransparencyToggle.IsChecked = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorStatusBar.QuickEditEntitiesTransparentLayers.IsChecked = value;
            }
        }
        private static bool _ApplyEditEntitiesTransparency = false;

        public static bool ShowEntitySelectionBoxes
        {
            get { return _ShowEntitySelectionBoxes; }
            set
            {
                _ShowEntitySelectionBoxes = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.showEntitySelectionBoxesToolStripMenuItem.IsChecked = value;
            }
        }
        private static bool _ShowEntitySelectionBoxes = false;

        public static bool ShowEntityPathArrows
        {
            get
            {
                return _ShowEntityPathArrows;
            }
            set
            {
                _ShowEntityPathArrows = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.showEntityPathArrowsToolstripItem.IsChecked = value;
            }
        }
        private static bool _ShowEntityPathArrows = true;

        public static bool ShowWaterLevel
        {
            get
            {
                return _ShowWaterLevel;
            }
            set
            {
                _ShowWaterLevel = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.showWaterLevelToolStripMenuItem.IsChecked = value;
            }
        }
        private static bool _ShowWaterLevel = false;

        public static bool AlwaysShowWaterLevel
        {
            get
            {
                return _AlwaysShowWaterLevel;
            }
            set
            {
                _AlwaysShowWaterLevel = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.waterLevelAlwaysShowItem.IsChecked = value;
            }
        }
        private static bool _AlwaysShowWaterLevel = false;

        public static bool SizeWaterLevelwithBounds
        {
            get
            {
                return _SizeWaterLevelwithBounds;
            }
            set
            {
                _SizeWaterLevelwithBounds = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.sizeWithBoundsWhenNotSelectedToolStripMenuItem.IsChecked = value;
            }
        }
        private static bool _SizeWaterLevelwithBounds = false;

        public static bool ExtraLayersMoveToFront
        {
            get
            {
                return _ExtraLayersMoveToFront;
            }
            set
            {
                _ExtraLayersMoveToFront = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.moveExtraLayersToFrontToolStripMenuItem.IsChecked = value;
            }
        }
        private static bool _ExtraLayersMoveToFront = false;

        public static bool ShowFlippedTileHelper
        {
            get
            {
                return _ShowFlippedTileHelper;
            }
            set
            {
                _ShowFlippedTileHelper = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.ReloadSpecificTextures(null, null);
            }
        }
        private static bool _ShowFlippedTileHelper = false;

        #endregion

        #region Control Options

        public static int MagnetSize
        {
            get
            {
                return _MagnetSize;
            }
            set
            {
                bool isCustom = false;
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.x8ToolStripMenuItem.IsChecked = false;
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.x16ToolStripMenuItem1.IsChecked = false;
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.x32ToolStripMenuItem.IsChecked = false;
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.x64ToolStripMenuItem.IsChecked = false;
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.MagnetCustomSizeToolStripMenuItem.IsChecked = false;

                if (value == 8) ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.x8ToolStripMenuItem.IsChecked = true;
                else if (value == 16) ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.x16ToolStripMenuItem1.IsChecked = true;
                else if (value == 32) ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.x32ToolStripMenuItem.IsChecked = true;
                else if (value == 64) ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.x64ToolStripMenuItem.IsChecked = true;
                else if (value == -1)
                {
                    isCustom = true;
                    ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.MagnetCustomSizeToolStripMenuItem.IsChecked = true;
                }

                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.CustomMagnetLabel.Text = string.Format(ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.CustomMagnetLabel.Tag.ToString(), CustomMagnetSize);

                if (!isCustom) _MagnetSize = value;
                else _MagnetSize = CustomMagnetSize;
            }
        }
        private static int _MagnetSize = 16;
        public static int CustomMagnetSize = 16;

        public static bool CopyAir
        {
            get
            {
                return _CopyAir;
            }
            set
            {
                _CopyAir = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.copyAirToggle.IsChecked = value;
            }
        }
        private static bool _CopyAir = false;

        public static bool RightClicktoSwapSlotID
        {
            get
            {
                return _RightClicktoSwapSlotID;
            }
            set
            {
                _RightClicktoSwapSlotID = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.rightClicktoSwapSlotIDs.IsChecked = value;
            }
        }
        private static bool _RightClicktoSwapSlotID = false;

        public static int FasterNudgeAmount
        {
            get
            {
                return _FasterNudgeAmount;
            }
            set
            {
                _FasterNudgeAmount = value;
            }
        }
        private static int _FasterNudgeAmount = 5;

        public static bool UseMagnetMode
        {
            get { return _UseMagnetMode; }
            set
            {
                _UseMagnetMode = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.MagnetMode.IsChecked = value;
            }
        }
        private static bool _UseMagnetMode = false;

        public static bool UseMagnetXAxis
        {
            get { return _UseMagnetXAxis; }
            set
            {
                _UseMagnetXAxis = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.enableXAxisToolStripMenuItem.IsChecked = value;
            }
        }
        private static bool _UseMagnetXAxis = true;

        public static bool UseMagnetYAxis
        {
            get { return _UseMagnetYAxis; }
            set
            {
                _UseMagnetYAxis = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.enableYAxisToolStripMenuItem.IsChecked = value;
            }
        }
        private static bool _UseMagnetYAxis = true;

        public static bool EnableFasterNudge
        {
            get
            {
                return _EnableFasterNudge;
            }
            set
            {
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorStatusBar.nudgeFasterButton.IsChecked = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.nudgeSelectionFasterToolStripMenuItem.IsChecked = value;
                _EnableFasterNudge = value;
            }
        }
        private static bool _EnableFasterNudge = false;

        public static bool ScrollLocked
        {
            get
            {
                return _ScrollLocked;
            }
            set
            {
                _ScrollLocked = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorStatusBar.scrollLockButton.IsChecked = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.statusNAToolStripMenuItem.IsChecked = value;
            }
        }
        private static bool _ScrollLocked = true;

        #endregion

        #region Information Settings

        //TODO : May be Redundant
        public static bool QuitWithoutSavingWarningRequired { get; set; } = false;
        public static bool ShowingDataDirectory
        {
            get
            {
                return _ShowingDataDirectory;
            }
            set
            {
                {
                    _ShowingDataDirectory = value;
                }
            }

        }
        private static bool _ShowingDataDirectory = false;

        public static bool CountTilesSelectedInPixels
        {
            get
            {
                return _CountTilesSelectedInPixels;
            }
            set
            {
                _CountTilesSelectedInPixels = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorStatusBar.pixelModeButton.IsChecked = value;
                ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.pixelModeToolStripMenuItem.IsChecked = value;
            }
        }
        private static bool _CountTilesSelectedInPixels = false;


        #endregion

        #region Prefrence/Status Values

        public static int SelectedTileID { get; set; } = -1; //For Tile Maniac Intergration via Right Click in Editor View Panel
        public static string CurrentLanguage { get; set; } = "EN"; //Current Selected Language
        public static int ScrollDirection { get; set; } = 1;
        public static int PlayerBeingTracked { get; set; } = -1;
        public static int CurrentControllerButtons { get; set; } = 2; //For Setting the Menu Control Button Images.
        public static int LevelID { get; set; } = -1; //Self Explanatory
        public static int LastQuickButtonState { get; set; } = 0; //Gets the Last Quick Button State, so we can tell what action was used last

        #endregion



        #region Unoptimized Technical Options
        public static bool RemoveStageConfigEntriesAllowed { get => GetRemoveStageConfigEntriesAllowed(); set => SetRemoveStageConfigEntriesAllowed(value); }
        private static bool _RemoveStageConfigEntriesAllowed = true;
        private static bool GetRemoveStageConfigEntriesAllowed()
        {
            return _RemoveStageConfigEntriesAllowed;
        }
        private static void SetRemoveStageConfigEntriesAllowed(bool value)
        {
            _RemoveStageConfigEntriesAllowed = value;
        }
        public static bool IsConsoleWindowOpen { get => GetIsConsoleWindowOpen(); set => SetIsConsoleWindowOpen(value); }
        private static bool _IsConsoleWindowOpen = false;
        private static bool GetIsConsoleWindowOpen()
        {
            return _IsConsoleWindowOpen;
        }
        private static void SetIsConsoleWindowOpen(bool value)
        {
            _IsConsoleWindowOpen = value;
        }


        public static bool DataDirectoryReadOnlyMode { get => GetDataDirectoryReadOnlyMode(); set => SetDataDirectoryReadOnlyMode(value); }
        private static bool _DataDirectoryReadOnlyMode = false;
        private static bool GetDataDirectoryReadOnlyMode()
        {
            return _DataDirectoryReadOnlyMode;
        }
        private static void SetDataDirectoryReadOnlyMode(bool value)
        {
            _DataDirectoryReadOnlyMode = value;
        }

        #endregion

        #region Unoptimized Tool Modes
        public static void PointerMode(bool? value = null)
        {
            if (value != null) SetToolModes(0, value.Value);
            else SetToolModes(0, ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.PointerToolButton.IsChecked.Value);
        }
        public static void SelectionMode(bool? value = null)
        {
            if (value != null) SetToolModes(1, value.Value);
            else SetToolModes(1, ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.SelectToolButton.IsChecked.Value);
        }
        public static void DrawMode(bool? value = null)
        {
            if (value != null) SetToolModes(2, value.Value);
            else SetToolModes(2, ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked.Value);
        }
        public static void InteractionMode(bool? value = null)
        {
            SetToolModes(3, ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.InteractionToolButton.IsChecked.Value);
        }

        //Determines the Draw Tool's Brush Size (Tiles Only)
        public static int DrawBrushSize { get => GetDrawBrushSize(); set => SetDrawBrushSize(value); }
        private static int _DrawBrushSize = 1;

        private static int GetDrawBrushSize()
        {
            return _DrawBrushSize;
        }
        private static void SetDrawBrushSize(int value)
        {
            _DrawBrushSize = value;
        }



        public static void SplineMode(bool? value = null)
        {
            if (value != null) SetToolModes(4, value.Value);
            else SetToolModes(4, ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.SplineToolButton.IsChecked.Value);
        }
        public static void ChunksMode()
        {
            if (IsTilesEdit()) ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.ChunksToolButton.IsChecked ^= true;
            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void SetToolModes(int selectedID, bool value)
        {
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.PointerToolButton.IsChecked = (selectedID == 0 ? value : false);
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.SelectToolButton.IsChecked = (selectedID == 1 ? value : false);
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.DrawToolButton.IsChecked = (selectedID == 2 ? value : false);
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.InteractionToolButton.IsChecked = (selectedID == 3 ? value : false);
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.SplineToolButton.IsChecked = (selectedID == 4 ? value : false);
            Methods.Internal.UserInterface.UpdateControls();
        }
        #endregion

        #region Unoptimized Spline Options
        public class SplineOptions
        {
            //Determines the Spline Point Frequency
            public int SplineSize { get => GetSplineSize(); set => SetSplineSize(value); }
            private int _SplineSize = 128;
            private int GetSplineSize()
            {
                return _SplineSize;
            }
            private void SetSplineSize(int value)
            {
                _SplineSize = value;
                Methods.Internal.UserInterface.UpdateControls();
            }



            public bool SplineToolShowLines { get; set; } = true; //Self Explanatory
            public int SplineCurrentPointsDrawn { get; set; } = 0; //Self Explanatory
            public int SplineTotalNumberOfObjects { get; set; } = 0; //Self Explanatory
            public bool SplineToolShowPoints { get; set; } = true; //Self Explanatory
            public bool SplineToolShowObject { get; set; } = false; //Self Explanatory

            public bool SplineLineMode { get => GetSplineLineMode(); set => SetSplineLineMode(value); } //Self Explanatory
            public bool SplineOvalMode { get => GetSplineOvalMode(); set => SetSplineOvalMode(value); } //Self Explanatory

            private bool _SplineLineMode = false;
            private bool _SplineOvalMode = true;
            private bool GetSplineLineMode()
            {
                return _SplineLineMode;
            }
            private void SetSplineLineMode(bool value)
            {
                _SplineLineMode = value;
                UpdateSplineStates(1, value);
            }

            private bool GetSplineOvalMode()
            {
                return _SplineOvalMode;
            }
            private void SetSplineOvalMode(bool value)
            {
                _SplineOvalMode = value;
                UpdateSplineStates(0, value);
            }

            private void UpdateSplineStates(int mode, bool state)
            {
                if (mode == 1)
                {
                    if (state)
                    {
                        ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.SplineLineMode.IsChecked = true;
                        _SplineLineMode = true;
                        ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.SplineOvalMode.IsChecked = false;
                        _SplineOvalMode = false;
                    }
                    else
                    {
                        ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.SplineLineMode.IsChecked = false;
                        _SplineLineMode = false;
                        ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.SplineOvalMode.IsChecked = true;
                        _SplineOvalMode = true;
                    }
                }
                else
                {
                    if (state)
                    {
                        ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.SplineLineMode.IsChecked = false;
                        _SplineLineMode = false;
                        ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.SplineOvalMode.IsChecked = true;
                        _SplineOvalMode = true;
                    }
                    else
                    {
                        ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.SplineLineMode.IsChecked = true;
                        _SplineLineMode = true;
                        ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.SplineOvalMode.IsChecked = false;
                        _SplineOvalMode = false;
                    }
                }
            }

            public Classes.Editor.Scene.Sets.EditorEntity SplineObjectRenderingTemplate { get; set; }

            public string SplineRenderingObjectName { get => GetSplineRenderingObjectName(); }

            private string GetSplineRenderingObjectName()
            {
                return SplineObjectRenderingTemplate.Entity.Object.Name.Name;
            }




        }
        public static Dictionary<int, SplineOptions> SplineOptionsGroup { get => GetSplineOptionsGroup(); set => SetSplineOptionsGroup(value); }

        private static Dictionary<int, SplineOptions> _SplineOptionsGroup = new Dictionary<int, SplineOptions>();

        public static void AddSplineOptionsGroup(int splineID)
        {
            SplineOptionsGroup.Add(splineID, new SplineOptions());
        }

        private static Dictionary<int, SplineOptions> GetSplineOptionsGroup()
        {
            return _SplineOptionsGroup;
        }

        private static void SetSplineOptionsGroup(Dictionary<int, SplineOptions> value)
        {
            _SplineOptionsGroup = value;
        }


        public static bool AllowSplineOptionsUpdate = true;

        public enum SplineOption
        {
            Size,
            LineMode,
            OvalMode,
            ShowLines,
            ShowObjects,
            ShowPoints,
            SpawnObject
        }

        public static void AdjustSplineGroupOptions(SplineOption option, object value)
        {
            if (!AllowSplineOptionsUpdate) return;
            switch (option)
            {
                case SplineOption.Size:
                    if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineSize = (int)value;
                    else
                    {
                        SplineOptions options = new SplineOptions();
                        options.SplineSize = (int)value;
                        SplineOptionsGroup.Add(SelectedSplineID, options);
                    }
                    break;
                case SplineOption.LineMode:
                    if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineLineMode = (bool)value;
                    else
                    {
                        SplineOptions options = new SplineOptions();
                        options.SplineLineMode = (bool)value;
                        SplineOptionsGroup.Add(SelectedSplineID, options);
                    }
                    break;
                case SplineOption.OvalMode:
                    if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineOvalMode = (bool)value;
                    else
                    {
                        SplineOptions options = new SplineOptions();
                        options.SplineOvalMode = (bool)value;
                        SplineOptionsGroup.Add(SelectedSplineID, options);
                    }
                    break;
                case SplineOption.ShowLines:
                    if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineToolShowLines = (bool)value;
                    else
                    {
                        SplineOptions options = new SplineOptions();
                        options.SplineToolShowLines = (bool)value;
                        SplineOptionsGroup.Add(SelectedSplineID, options);
                    }
                    break;
                case SplineOption.ShowObjects:
                    if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineToolShowObject = (bool)value;
                    else
                    {
                        SplineOptions options = new SplineOptions();
                        options.SplineToolShowObject = (bool)value;
                        SplineOptionsGroup.Add(SelectedSplineID, options);
                    }
                    break;
                case SplineOption.ShowPoints:
                    if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineToolShowPoints = (bool)value;
                    else
                    {
                        SplineOptions options = new SplineOptions();
                        options.SplineToolShowPoints = (bool)value;
                        SplineOptionsGroup.Add(SelectedSplineID, options);
                    }
                    break;
                case SplineOption.SpawnObject:
                    if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineObjectRenderingTemplate = (Classes.Editor.Scene.Sets.EditorEntity)value;
                    else
                    {
                        SplineOptions options = new SplineOptions();
                        options.SplineObjectRenderingTemplate = (Classes.Editor.Scene.Sets.EditorEntity)value;
                        SplineOptionsGroup.Add(SelectedSplineID, options);
                    }
                    break;

            }
        }

        public static int SelectedSplineID { get => GetSplineSelectedID(); set => SetSplineSelectedID(value); }
        private static int _SelectedSplineID = 0;

        private static int GetSplineSelectedID()
        {
            return _SelectedSplineID;
        }
        private static void SetSplineSelectedID(int value)
        {
            _SelectedSplineID = value;
        }
        #endregion

        #region Unoptimized Multi Layer Edit Mode
        public static bool MultiLayerEditMode { get => GetMultiLayerEditMode(); set => SetMultiLayerEditMode(value); }
        private static bool _MultiLayerEditMode = true;
        private static bool _MultiLayerFirstTimeCheck = true;
        private static bool GetMultiLayerEditMode()
        {
            if (_MultiLayerFirstTimeCheck)
            {
                _MultiLayerFirstTimeCheck = false;
                SetMultiLayerEditMode(_MultiLayerEditMode);
            }

            return _MultiLayerEditMode;
        }
        private static void SetMultiLayerEditMode(bool value)
        {
            _MultiLayerEditMode = value;
            ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.multiLayerSelectionToolStripMenuItem.IsChecked = value;


            bool enabled = (value == true ? true : false);
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGLower.DualSelect = enabled;
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGLow.DualSelect = enabled;
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGHigh.DualSelect = enabled;
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGHigher.DualSelect = enabled;

            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGLower.SwapDefaultToA(!enabled);
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGLow.SwapDefaultToA(!enabled);
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGHigh.SwapDefaultToA(!enabled);
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGHigher.SwapDefaultToA(!enabled);

            foreach (var elb in ManiacEditor.Controls.Editor.MainEditor.Instance.ExtraLayerEditViewButtons.Values)
            {
                elb.DualSelect = enabled;
                elb.SwapDefaultToA(!enabled);
            }

            if (!enabled) Classes.Editor.Solution.EditLayerB = null;

            Methods.Internal.UserInterface.UpdateControls();
        }

        public static void UpdateMultiLayerSelectMode(bool updateControls = false)
        {
            bool enabled = (_MultiLayerEditMode == true ? true : false);
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGLower.DualSelect = enabled;
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGLow.DualSelect = enabled;
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGHigh.DualSelect = enabled;
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGHigher.DualSelect = enabled;

            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGLower.SwapDefaultToA(!enabled);
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGLow.SwapDefaultToA(!enabled);
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGHigh.SwapDefaultToA(!enabled);
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGHigher.SwapDefaultToA(!enabled);

            foreach (var elb in ManiacEditor.Controls.Editor.MainEditor.Instance.ExtraLayerEditViewButtons.Values)
            {
                elb.DualSelect = enabled;
                elb.SwapDefaultToA(!enabled);
            }

            if (!enabled) Classes.Editor.Solution.EditLayerB = null;

            if (updateControls) Methods.Internal.UserInterface.UpdateControls();
        }

        #endregion

        #region To-Remove Settings
        public static bool UseLargeDebugStats { get => GetUseLargeDebugStats(); set => SetUseLargeDebugStats(value); }
        private static bool _UseLargeDebugStats = false;
        private static bool GetUseLargeDebugStats()
        {
            return _UseLargeDebugStats;
        }
        private static void SetUseLargeDebugStats(bool value)
        {
            _UseLargeDebugStats = value;
            ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.useLargeTextToolStripMenuItem.IsChecked = value;
        }

        public static bool DebugStatsVisibleOnPanel { get => GetDebugStatsVisibleOnPanel(); set => SetDebugStatsVisibleOnPanel(value); }
        private static bool _DebugStatsVisibleOnPanel = false;
        private static bool GetDebugStatsVisibleOnPanel()
        {
            return _DebugStatsVisibleOnPanel;
        }
        private static void SetDebugStatsVisibleOnPanel(bool value)
        {
            _DebugStatsVisibleOnPanel = value;
            ManiacEditor.Controls.Editor.MainEditor.Instance.MenuBar.showStatsToolStripMenuItem.IsChecked = value;
            ManiacEditor.Controls.Editor.MainEditor.Instance.ViewPanel.InfoHUD.UpdatePopupVisibility();
        }
        #endregion

        #region Unoptimized Grid Size
        //Determines the Grid's Size
        public static int GridSize { get => GetGridSize(); set => SetGridSize(value); }
        private static int _GridSize = 16;
        public static int GridCustomSize { get => GetCustomSize(); set => ChangeCustomSize(value); }
        private static int _GridCustomSize = Core.Settings.MyDefaults.CustomGridSizeValue;

        private static void ChangeCustomSize(int value)
        {
            _GridCustomSize = value;
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.CustomGridLabel.Text = string.Format(ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.CustomGridLabel.Tag.ToString(), _GridCustomSize);
        }

        private static int GetCustomSize()
        {
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.CustomGridLabel.Text = string.Format(ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.CustomGridLabel.Tag.ToString(), _GridCustomSize);
            return _GridCustomSize;
        }

        private static int GetGridSize()
        {
            return _GridSize;
        }
        private static void SetGridSize(int value)
        {
            bool isCustom = false;
            _GridSize = value;

            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.Grid16x16SizeMenuItem.IsChecked = false;
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.Grid128x128SizeMenuItem.IsChecked = false;
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.Grid256x256SizeMenuItem.IsChecked = false;
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.GridCustomSizeMenuItem.IsChecked = false;

            if (value == 16) ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.Grid16x16SizeMenuItem.IsChecked = true;
            else if (value == 128) ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.Grid128x128SizeMenuItem.IsChecked = true;
            else if (value == 256) ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.Grid256x256SizeMenuItem.IsChecked = true;
            else if (value == -1)
            {
                isCustom = true;
                ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.GridCustomSizeMenuItem.IsChecked = true;
            }

            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.CustomGridLabel.Text = string.Format(ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.CustomGridLabel.Tag.ToString(), GridCustomSize);

            if (!isCustom) _GridSize = value;
            else _GridSize = GridCustomSize;
        }
        #endregion

        #region Unoptimized Collision View Mode (Colors)
        //Determines the Collision View Mode
        public static int CollisionPreset { get => GetCollisionPreset(); set => SetCollisionPreset(value); }
        private static int _CollisionPreset = 0;
        private static int GetCollisionPreset()
        {
            return _CollisionPreset;
        }
        private static void SetCollisionPreset(int value)
        {
            _CollisionPreset = value;

            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.invertedToolStripMenuItem.IsChecked = false;
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.customToolStripMenuItem1.IsChecked = false;
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.defaultToolStripMenuItem.IsChecked = false;

            if (value == 0) ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.defaultToolStripMenuItem.IsChecked = true;
            else if (value == 1) ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.invertedToolStripMenuItem.IsChecked = true;
            else if (value == 2) ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.customToolStripMenuItem1.IsChecked = true;


            ManiacEditor.Controls.Editor.MainEditor.Instance.ReloadSpecificTextures(null, null);
            ManiacEditor.Controls.Editor.MainEditor.Instance.RefreshCollisionColours(true);
        }



        public static System.Drawing.Color CollisionTOColour { get => GetCollisionTOColour(); set => SetCollisionTOColour(value); }
        private static System.Drawing.Color _CollisionTOColour = System.Drawing.Color.Yellow;
        private static System.Drawing.Color GetCollisionTOColour() { return _CollisionTOColour; }
        private static void SetCollisionTOColour(System.Drawing.Color value) { _CollisionTOColour = value; }

        public static System.Drawing.Color CollisionLRDColour { get => GetCollisionLRDColour(); set => SetCollisionLRDColour(value); }
        private static System.Drawing.Color _CollisionLRDColour = System.Drawing.Color.Red;
        private static System.Drawing.Color GetCollisionLRDColour() { return _CollisionLRDColour; }
        private static void SetCollisionLRDColour(System.Drawing.Color value) { _CollisionLRDColour = value; }

        public static System.Drawing.Color CollisionSAColour { get => GetCollisionSAColour(); set => SetCollisionSAColour(value); }
        private static System.Drawing.Color _CollisionSAColour = System.Drawing.Color.White;
        private static System.Drawing.Color GetCollisionSAColour() { return _CollisionSAColour; }
        private static void SetCollisionSAColour(System.Drawing.Color value) { _CollisionSAColour = value; }




        public static System.Drawing.Color waterColor { get => GetWaterEntityColor(); set => SetWaterEntityColor(value); }
        private static System.Drawing.Color _WaterEntityColor = System.Drawing.Color.Blue;
        private static System.Drawing.Color GetWaterEntityColor() { return _WaterEntityColor; }
        private static void SetWaterEntityColor(System.Drawing.Color value) { _WaterEntityColor = value; }

        public static System.Drawing.Color GridColor { get => GetGridColor(); set => SetGridColor(value); }
        private static System.Drawing.Color _GridColor = System.Drawing.Color.Red;
        private static System.Drawing.Color GetGridColor() { return _GridColor; }
        private static void SetGridColor(System.Drawing.Color value) { _GridColor = value; }
        #endregion

        #region To-Improve Implementation Variables
        public static bool AddStageConfigEntriesAllowed { get; set; } = true; //Self Explanatory
        public static bool isImportingObjects { get; set; } = false; //Determines if we are importing objects so we can disable all the other Scene Select Options
        public static bool EncorePaletteExists { get; set; } = false; // Determines if an Encore Pallete Exists

        #endregion


        public static bool Duplicate1
        {
            get
            {
                return _Duplicate1;
            }
            set
            {
                _Duplicate1 = value;
            }
        }
        private static bool _Duplicate1;



        #region Unoptimized Misc Stuff
        public static bool collisionOpacityChanged { get; set; } = false;
        public static int EncoreSetupType { get; set; } //Used to determine what kind of encore setup the stage uses
        public static string entitiesTextFilter { get; set; } = ""; //Used to hide objects that don't match the discription
        public static string LevelSelectCharS { get; set; } = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ*+,-./: \'\"";
        public static string MenuCharS { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ?!.";
        public static string MenuCharS_Small { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ?.:'\"!-,&¡<>¿"; //49 out of 121
        public static char[] MenuChar
        {
            get
            {
                return MenuCharS.ToCharArray();
            }
        }
        public static char[] LevelSelectChar
        {
            get
            {
                return LevelSelectCharS.ToCharArray();
            }
        }
        public static char[] MenuChar_Small
        {
            get
            {
                return MenuCharS_Small.ToCharArray();
            }
        }
        #endregion

        #endregion
    }
}
