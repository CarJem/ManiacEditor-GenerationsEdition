﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using SFML.System;
using System.Windows.Data;

namespace ManiacEditor.Methods.Solution
{
    public static class SolutionState 
    {
        public static StateModel Main { get; set; } = new StateModel();
        public class StateModel : Xe.Tools.Wpf.BaseNotifyPropertyChanged
        {
            #region Init
            private ManiacEditor.Controls.Editor.MainEditor Instance;
            public void UpdateInstance(ManiacEditor.Controls.Editor.MainEditor instance)
            {
                Instance = instance;
            }
            #endregion

            #region Binding Based Future

            public void UpdatePastableBindings()
            {
                bool WindowsClipboardState;
                bool WindowsEntityClipboardState;

                bool HasCopyDataTiles;
                bool HasCopyDataEntities;

                try
                {
                    //Doing this too often seems to cause a lot of grief for the app;
                    //Should be relocated and stored as a bool....
                    WindowsClipboardState = (_TilesEditing ? System.Windows.Clipboard.ContainsData("ManiacTiles") : false);
                    WindowsEntityClipboardState = (_EntitiesEditing ? System.Windows.Clipboard.ContainsData("ManiacEntities") : false);
                }
                catch
                {
                    WindowsClipboardState = false;
                    WindowsEntityClipboardState = false;
                }

                HasCopyDataTiles = Methods.Solution.SolutionClipboard.TilesClipboard != null || WindowsClipboardState == true;
                HasCopyDataEntities = Methods.Solution.SolutionClipboard.ObjectsClipboard != null || WindowsEntityClipboardState == true;

                if (HasCopyDataTiles && _TilesEditing)
                {
                    IsPastable = true;
                    IsChunkPastable = true;
                }
                else if (HasCopyDataEntities && _EntitiesEditing)
                {
                    IsPastable = true;
                    IsChunkPastable = false;
                }
                else
                {
                    IsPastable = false;
                    IsChunkPastable = false;
                }
            }
            public void UpdateBindings()
            {
                if (Instance == null) return;

                SceneLoaded = Methods.Solution.CurrentSolution.CurrentScene == null ? false : true;

                TilesEditing = Methods.Solution.CurrentSolution.EditLayerA != null;
                EntitiesEditing = Instance.EditorToolbar.EditEntities.IsCheckedN.Value || Instance.EditorToolbar.EditEntities.IsCheckedA.Value || Instance.EditorToolbar.EditEntities.IsCheckedB.Value;

                StageConfigLoaded = Methods.Solution.CurrentSolution.StageConfig != null;
                TileConfigLoaded = Methods.Solution.CurrentSolution.TileConfig != null;

                CanUndo = _SceneLoaded && Actions.UndoRedoModel.UndoStack.Count > 0;
                CanRedo = _SceneLoaded && Actions.UndoRedoModel.RedoStack.Count > 0;

                if (_TilesEditing && _SceneLoaded) IsSelected = Methods.Solution.SolutionMultiLayer.IsSelected();
                else if (_EntitiesEditing && _SceneLoaded) IsSelected = Methods.Solution.CurrentSolution.Entities.IsAnythingSelected();
                else IsSelected = false;

                IsEditing = _TilesEditing || _EntitiesEditing || IsChunksEdit();

                if (_EntitiesEditing && _IsSelected) CanFlip = true;
                else if (_TilesEditing) CanFlip = true;
                else CanFlip = false;

                if (_TilesEditing) CanFlipAlt = true;
                else CanFlipAlt = false;

                if (Properties.Settings.MyPerformance.ReduceZoom)
                {
                    CanZoomIn = _SceneLoaded && Methods.Solution.SolutionState.Main.ZoomLevel < 5;
                    CanZoomOut = _SceneLoaded && Methods.Solution.SolutionState.Main.ZoomLevel > -2;
                }
                else
                {
                    CanZoomIn = _SceneLoaded && Methods.Solution.SolutionState.Main.ZoomLevel < 5;
                    CanZoomOut = _SceneLoaded && Methods.Solution.SolutionState.Main.ZoomLevel > -5;
                }

                UpdatePastableBindings();
            }

            private bool _SceneLoaded = false;
            public bool SceneLoaded
            {
                get
                {
                    return _SceneLoaded;
                }
                set
                {
                    _SceneLoaded = value;
                    OnPropertyChanged(nameof(SceneLoaded));
                }
            }
            public bool IsSceneLoaded()
            {
                UpdateBindings();
                return _SceneLoaded;
            }

            private bool _IsEncorePaletteLoaded = false;
            public bool IsEncorePaletteLoaded
            {
                get
                {
                    return _IsEncorePaletteLoaded;
                }
                set
                {
                    _IsEncorePaletteLoaded = value;
                    OnPropertyChanged(nameof(IsEncorePaletteLoaded));
                }
            }

            private bool _TileConfigLoaded = false;
            public bool TileConfigLoaded
            {
                get
                {
                    return _TileConfigLoaded;
                }
                set
                {
                    _TileConfigLoaded = value;
                    OnPropertyChanged(nameof(TileConfigLoaded));
                }
            }

            private bool _StageConfigLoaded = false;
            public bool StageConfigLoaded
            {
                get
                {
                    return _StageConfigLoaded;
                }
                set
                {
                    _StageConfigLoaded = value;
                    OnPropertyChanged(nameof(StageConfigLoaded));
                }
            }

            private bool _TilesEditing = false;
            public bool TilesEditing
            {
                get
                {
                    return _TilesEditing;
                }
                set
                {
                    _TilesEditing = value;
                    OnPropertyChanged(nameof(TilesEditing));
                }
            }
            public bool IsTilesEdit()
            {
                UpdateBindings();
                return _TilesEditing;
            }

            private bool _EntitiesEditing = false;
            public bool EntitiesEditing
            {
                get
                {
                    return _EntitiesEditing;
                }
                set
                {
                    _EntitiesEditing = value;
                    OnPropertyChanged(nameof(EntitiesEditing));
                }
            }
            public bool IsEntitiesEdit()
            {
                UpdateBindings();
                return _EntitiesEditing;
            }

            private bool _CanUndo = false;
            public bool CanUndo
            {
                get
                {
                    return _CanUndo;
                }
                set
                {
                    _CanUndo = value;
                    OnPropertyChanged(nameof(CanUndo));
                }
            }

            private bool _CanRedo = false;
            public bool CanRedo
            {
                get
                {
                    return _CanRedo;
                }
                set
                {
                    _CanRedo = value;
                    OnPropertyChanged(nameof(CanRedo));
                }
            }

            private bool _IsSelected = false;
            public bool IsSelected
            {
                get
                {
                    return _IsSelected;
                }
                set
                {
                    _IsSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }

            private bool _IsEditing = false;
            public bool IsEditing
            {
                get
                {
                    return _IsEditing;
                }
                set
                {
                    _IsEditing = value;
                    OnPropertyChanged(nameof(IsEditing));
                }
            }

            private bool _CanFlip = false;
            public bool CanFlip
            {
                get
                {
                    return _CanFlip;
                }
                set
                {
                    _CanFlip = value;
                    OnPropertyChanged(nameof(CanFlip));
                }
            }

            private bool _CanFlipAlt = false;
            public bool CanFlipAlt
            {
                get
                {
                    return _CanFlipAlt;
                }
                set
                {
                    _CanFlipAlt = value;
                    OnPropertyChanged(nameof(CanFlipAlt));
                }
            }

            private bool _IsPastable = false;
            public bool IsPastable
            {
                get
                {
                    return _IsPastable;
                }
                set
                {
                    _IsPastable = value;
                    OnPropertyChanged(nameof(IsPastable));
                }
            }

            private bool _IsChunkPastable = false;
            public bool IsChunkPastable
            {
                get
                {
                    return _IsChunkPastable;
                }
                set
                {
                    _IsChunkPastable = value;
                    OnPropertyChanged(nameof(IsChunkPastable));
                }
            }

            private bool _CanZoomOut = false;
            public bool CanZoomOut
            {
                get
                {
                    return _CanZoomOut;
                }
                set
                {
                    _CanZoomOut = value;
                    OnPropertyChanged(nameof(CanZoomOut));
                }
            }

            private bool _CanZoomIn = false;
            public bool CanZoomIn
            {
                get
                {
                    return _CanZoomIn;
                }
                set
                {
                    _CanZoomIn = value;
                    OnPropertyChanged(nameof(CanZoomIn));
                }
            }

            #endregion

            #region Methods



            public bool IsChunksEdit()
            {
                return Instance.EditorToolbar.ChunksToolButton.IsChecked.Value && Methods.Solution.CurrentSolution.EditLayerA != null;
            }

            public bool AnyDragged()
            {
                return ((DraggingSelection || Dragged) && !AutoScrolling);
            }

            #endregion

            #region Status Variables (Binding Based)

            private int _DraggedX;
            private int _DraggedY;
            private bool _DraggingSelection = false; //Determines if we are dragging a selection
            private bool _Dragged = false;
            private bool _StartDragged = false;
            private bool _AutoScrolling = false; //Determines if the User is Scrolling
            private Point _AutoScrollPosition;//For Getting the Scroll Position
            private Vector2f _ActualAutoScrollPosition;//For Getting the Scroll Position
            private bool _Zooming = false;  //Detects if we are zooming
            private double _Zoom = 1;
            private int _ZoomLevel = 1;
            private bool _UnlockCamera = false;
            private int _ViewPositionX = 0;
            private int _ViewPositionY = 0;
            private int _LastX = 0;
            private int _LastY = 0;
            private int _TempSelectX1 = 0;
            private int _TempSelectX2 = 0;
            private int _TempSelectY1 = 0;
            private int _TempSelectY2 = 0;


            public int LastX
            {
                get
                {
                    return _LastX;
                }
                set
                {
                    _LastX = value;
                    OnPropertyChanged(nameof(LastX));
                    UpdateStatusLabels();
                }
            }
            public int LastY
            {
                get
                {
                    return _LastY;
                }
                set
                {
                    _LastY = value;
                    OnPropertyChanged(nameof(LastY));
                    UpdateStatusLabels();
                }
            }
            public int DraggedX
            {
                get
                {
                    return _DraggedX;
                }
                set
                {
                    _DraggedX = value;
                    OnPropertyChanged(nameof(DraggedX));
                }
            }
            public int DraggedY
            {
                get
                {
                    return _DraggedY;
                }
                set
                {
                    _DraggedY = value;
                    OnPropertyChanged(nameof(DraggedY));
                }
            }
            public bool DraggingSelection
            {
                get
                {
                    return _DraggingSelection;
                }
                set
                {
                    _DraggingSelection = value;
                    OnPropertyChanged(nameof(DraggingSelection));
                }
            }
            public bool Dragged
            {
                get
                {
                    return _Dragged;
                }
                set
                {
                    _Dragged = value;
                    OnPropertyChanged(nameof(Dragged));
                }
            }
            public bool StartDragged
            {
                get
                {
                    return _StartDragged;
                }
                set
                {
                    _StartDragged = value;
                    OnPropertyChanged(nameof(StartDragged));
                }
            }
            public bool AutoScrolling
            {
                get
                {
                    return _AutoScrolling;
                }
                set
                {
                    _AutoScrolling = value;
                    OnPropertyChanged(nameof(AutoScrolling));
                }
            }
            public Point AutoScrollPosition
            {
                get
                {
                    return _AutoScrollPosition;
                }
                set
                {
                    _AutoScrollPosition = value;
                    OnPropertyChanged(nameof(AutoScrollPosition));
                }
            }
            public bool Zooming
            {
                get
                {
                    return _Zooming;
                }
                set
                {
                    _Zooming = value;
                    OnPropertyChanged(nameof(Zooming));
                }
            }
            public double Zoom
            {
                get
                {
                    return _Zoom;
                }
                set
                {
                    
                    _Zoom = value;
                    OnPropertyChanged(nameof(Zoom));
                    UpdateStatusLabels();

                }
            }
            public int ZoomLevel
            {
                get
                {
                    return _ZoomLevel;
                }
                set
                {
                    
                    _ZoomLevel = value;
                    OnPropertyChanged(nameof(ZoomLevel));
                    
                }
            }
            public int TempSelectX1
            {
                get
                {
                    return _TempSelectX1;
                }
                set
                {

                    _TempSelectX1 = value;
                    OnPropertyChanged(nameof(TempSelectX1));
                    UpdateStatusLabels();
                }
            }
            public int TempSelectX2
            {
                get
                {
                    return _TempSelectX2;
                }
                set
                {

                    _TempSelectX2 = value;
                    OnPropertyChanged(nameof(TempSelectX2));
                    UpdateStatusLabels();
                }
            }
            public int TempSelectY1
            {
                get
                {
                    return _TempSelectY1;
                }
                set
                {

                    _TempSelectY1 = value;
                    OnPropertyChanged(nameof(TempSelectY1));
                    UpdateStatusLabels();

                }
            }
            public int TempSelectY2
            {
                get
                {
                    return _TempSelectY2;
                }
                set
                {

                    _TempSelectY2 = value;
                    OnPropertyChanged(nameof(TempSelectY2));
                    UpdateStatusLabels();
                }
            }
            public int ViewPositionX
            {
                get
                {
                    return _ViewPositionX;
                }
                private set
                {
                    SetViewPositionX(value);
                }
            }
            public void SetViewPositionY(int value)
            {
                _ViewPositionY = value;
                ManiacEditor.Methods.Drawing.ObjectDrawing.RequestEntityVisiblityRefresh(true);
                OnPropertyChanged(nameof(ViewPositionY));
                UpdateStatusLabels();

                if (Methods.Solution.SolutionState.Main.AnyDragged()) Instance.ViewPanel.SharpPanel.GraphicPanel.OnMouseMoveEventCreate();
                Instance.ViewPanel.SharpPanel.UpdateGraphicsPanelControls();
            }
            public void SetViewPositionX(int value)
            {
                _ViewPositionX = value;
                ManiacEditor.Methods.Drawing.ObjectDrawing.RequestEntityVisiblityRefresh(true);
                OnPropertyChanged(nameof(ViewPositionX));
                UpdateStatusLabels();

                if (Methods.Solution.SolutionState.Main.AnyDragged()) Instance.ViewPanel.SharpPanel.GraphicPanel.OnMouseMoveEventCreate();
                Instance.ViewPanel.SharpPanel.UpdateGraphicsPanelControls();
            }
            public int ViewPositionY
            {
                get
                {
                    return _ViewPositionY;
                }
                private set
                {
                    SetViewPositionY(value);
                }
            }
            public bool UnlockCamera
            {
                get
                {
                    return _UnlockCamera;
                }
                set
                {
                    _UnlockCamera = value;
                    OnPropertyChanged(nameof(UnlockCamera));
                    Instance.MenuBar.UnlockCameraToolStripMenuItem.IsChecked = value;
                    ViewPositionX = 0;
                    ViewPositionY = 0;
                    Instance.ViewPanel.SharpPanel.UpdateGraphicsPanelControls();
                }
            }
            public Enums.Axis ScrollDirection { get; set; } = Enums.Axis.Y;


            #region Not Ready Yet
            public Point GetLastXY()
            {
                return new Point((int)(LastX), (int)(LastY));
            }

            public void UpdateLastXY(int x, int y)
            {
                LastX = x;
                LastY = y;
            }

            public int RegionX1 { get; set; } = -1;
            public int RegionY1 { get; set; } = -1;
            public int RegionX2 { get; set; }
            public int RegionY2 { get; set; }




            #endregion

            #endregion

            #region Status Bar Labels (Binding Based)

            private void UpdateStatusLabels()
            {
                string __MousePositionString;
                string __ZoomLevelString;
                string __ScrollDirectionString;
                string __SelectionSizeTooltipString;
                string __SelectionSizeString;
                string __SelectionBoxSizeString;
                string __SelectedTilePositionString;
                string __SelectedTilePositionTooltipString;

                __MousePositionString = "X: " + _LastX + " Y: " + _LastY;
                //if (CountTilesSelectedInPixels) MousePositionString  = "X: " + (int)(LastX) + " Y: " + (int)(LastY);
                //else MousePositionString = "X: " + (int)((LastX) / 16) + " Y: " + (int)((LastY) / 16);

                __ZoomLevelString = "Zoom Value: " + _Zoom.ToString();

                __ScrollDirectionString = "Scroll Direction: " + (ScrollDirection == (int)Enums.Axis.X ? "X" : "Y") + (ScrollLocked ? " (Locked)" : "");

                if (Methods.Solution.SolutionState.Main.CountTilesSelectedInPixels == false) __SelectionSizeTooltipString = "The Size of the Selection";
                __SelectionSizeTooltipString = "The Length of all the Tiles (by Pixels) in the Selection";

                if (Methods.Solution.SolutionState.Main.CountTilesSelectedInPixels == false) __SelectionSizeString = "Amount of Tiles in Selection: " + (Methods.Solution.SolutionState.Main.SelectedTilesCount);
                else __SelectionSizeString = "Length of Pixels in Selection: " + Methods.Solution.SolutionState.Main.SelectedTilesCount * 16;

                __SelectionBoxSizeString = "Selection Box Size: X: " + (_TempSelectX2 - _TempSelectX1) + ", Y: " + (_TempSelectY2 - _TempSelectY1);

                if (Methods.Solution.SolutionState.Main.CountTilesSelectedInPixels == false) __SelectedTilePositionString = "Selected Tile Position: X: " + (int)SelectedTileX + ", Y: " + (int)SelectedTileY;
                else __SelectedTilePositionString = "Selected Tile Pixel Position: " + "X: " + (int)SelectedTileX * 16 + ", Y: " + (int)SelectedTileY * 16;

                if (Methods.Solution.SolutionState.Main.CountTilesSelectedInPixels == false) __SelectedTilePositionTooltipString = "The Position of the Selected Tile";
                else __SelectedTilePositionTooltipString = "The Pixel Position of the Selected Tile";

                if (__MousePositionString != MousePositionString) MousePositionString = __MousePositionString;
                if (__ZoomLevelString != ZoomLevelString) ZoomLevelString = __ZoomLevelString;
                if (__ScrollDirectionString != ScrollDirectionString) ScrollDirectionString = __ScrollDirectionString;
                if (__SelectionSizeTooltipString != SelectionSizeTooltipString) SelectionSizeTooltipString = __SelectionSizeTooltipString;
                if (__SelectionSizeString != SelectionSizeString) SelectionSizeString = __SelectionSizeString;
                if (__SelectionBoxSizeString != SelectionBoxSizeString) SelectionBoxSizeString = __SelectionBoxSizeString;
                if (__SelectedTilePositionString != SelectedTilePositionString) SelectedTilePositionString = __SelectedTilePositionString;
                if (__SelectedTilePositionTooltipString != SelectedTilePositionTooltipString) SelectedTilePositionTooltipString = __SelectedTilePositionTooltipString;
            }



            private string _MousePositionString = string.Empty;
            public string MousePositionString
            {
                get
                {
                    return _MousePositionString;
                }
                set
                {
                    _MousePositionString = value;
                    OnPropertyChanged(nameof(MousePositionString));
                }
            }


            private string _ZoomLevelString = string.Empty;
            public string ZoomLevelString
            {
                get
                {
                    return _ZoomLevelString;
                }
                set
                {
                    _ZoomLevelString = value;
                    OnPropertyChanged(nameof(ZoomLevelString));
                }
            }


            private string _ScrollDirectionString = string.Empty;
            public string ScrollDirectionString
            {
                get
                {
                    return _ScrollDirectionString;
                }
                set
                {
                    _ScrollDirectionString = value;
                    OnPropertyChanged(nameof(ScrollDirectionString));
                }
            }


            private string _SelectionSizeTooltipString = string.Empty;
            public string SelectionSizeTooltipString
            {
                get
                {
                    return _SelectionSizeTooltipString;
                }
                set
                {
                    _SelectionSizeTooltipString = value;
                    OnPropertyChanged(nameof(SelectionSizeTooltipString));
                }
            }


            private string _SelectionSizeString = string.Empty;
            public string SelectionSizeString
            {
                get
                {
                    return _SelectionSizeString;
                }
                set
                {
                    _SelectionSizeString = value;
                    OnPropertyChanged(nameof(SelectionSizeString));
                }
            }


            private string _SelectionBoxSizeString = string.Empty;
            public string SelectionBoxSizeString
            {
                get 
                {
                    return _SelectionBoxSizeString;
                }
                set
                {
                    _SelectionBoxSizeString = value;
                    OnPropertyChanged(nameof(SelectionBoxSizeString));
                }
            }


            private string _SelectedTilePositionString = string.Empty;
            public string SelectedTilePositionString
            {
                get
                {
                    return _SelectedTilePositionString;
                }
                set
                {
                    _SelectedTilePositionString = value;
                    OnPropertyChanged(nameof(SelectedTilePositionString));
                }
            }


            private string _SelectedTilePositionTooltipString = string.Empty;
            public string SelectedTilePositionTooltipString
            {
                get
                {
                    return _SelectedTilePositionTooltipString;
                }
                set
                {
                    _SelectedTilePositionTooltipString = value;
                    OnPropertyChanged(nameof(SelectedTilePositionTooltipString));
                }
            }

            #endregion

            #region Unomptimized Section

            #region Tile Selection Variables

            public int SelectedTilesCount
            {
                get
                {
                    int tileCount = 0;

                    foreach (var layer in Methods.Solution.CurrentSolution.EditLayers)
                    {
                        int deselectAmount = layer.TempSelectionDeselectTiles.Count;
                        int selectedAmount = layer.SelectedTiles.Count + layer.TempSelectionTiles.Count - deselectAmount;
                        tileCount += selectedAmount;
                    }

                    return tileCount;
                }
            }
            public int SelectedTileX
            {
                get
                {
                    int lastTileX = -1;
                    foreach (var layer in Methods.Solution.CurrentSolution.EditLayers)
                    {
                        int layerlastPosX = layer.LastSelectedPositionX;
                        if (layerlastPosX != -1) lastTileX = layerlastPosX;
                    }
                    return lastTileX;
                }
            }
            public int SelectedTileY
            {
                get
                {
                    int lastTileY = -1;
                    foreach (var layer in Methods.Solution.CurrentSolution.EditLayers)
                    {
                        int layerlastPosY = layer.LastSelectedPositionY;
                        if (layerlastPosY != -1) lastTileY = layerlastPosY;
                    }
                    return lastTileY;
                }
            }
            public bool isTileDrawing { get; set; } = false;

            #endregion

            #region Grid View Variables
            public bool ShowGrid
            {
                get { return _ShowGrid; }
                set
                {
                    if (CurrentSolution.CurrentScene == null) value = false;
                    Instance.EditorToolbar.ShowGridToggleButton.IsChecked = value;
                    _ShowGrid = value;
                }
            }
            private bool _ShowGrid;
            public int GridSize { get => GetGridSize(); set => SetGridSize(value); }
            private int _GridSize = 16;
            public int GridCustomSize { get => GetCustomSize(); set => ChangeCustomSize(value); }
            private int _GridCustomSize = Properties.Settings.MyDefaults.CustomGridSizeValue;

            private void ChangeCustomSize(int value)
            {
                _GridCustomSize = value;
                Instance.EditorToolbar.CustomGridSizeLabel.Text = string.Format(Instance.EditorToolbar.CustomGridSizeLabel.Tag.ToString(), _GridCustomSize);
                Instance.EditorToolbar.AllowNUDUpdate = false;
                Instance.EditorToolbar.CustomGridSizeAdjuster.Value = _GridCustomSize;
                Instance.EditorToolbar.AllowNUDUpdate = true;
            }

            private int GetCustomSize()
            {
                Instance.EditorToolbar.CustomGridSizeLabel.Text = string.Format(Instance.EditorToolbar.CustomGridSizeLabel.Tag.ToString(), _GridCustomSize);
                return _GridCustomSize;
            }

            private int GetGridSize()
            {
                return _GridSize;
            }
            private void SetGridSize(int value)
            {
                bool isCustom = false;
                _GridSize = value;

                Instance.EditorToolbar.Grid16x16SizeMenuItem.IsChecked = false;
                Instance.EditorToolbar.Grid128x128SizeMenuItem.IsChecked = false;
                Instance.EditorToolbar.Grid256x256SizeMenuItem.IsChecked = false;
                Instance.EditorToolbar.GridCustomSizeMenuItem.IsChecked = false;

                if (value == 16) Instance.EditorToolbar.Grid16x16SizeMenuItem.IsChecked = true;
                else if (value == 128) Instance.EditorToolbar.Grid128x128SizeMenuItem.IsChecked = true;
                else if (value == 256) Instance.EditorToolbar.Grid256x256SizeMenuItem.IsChecked = true;
                else if (value == -1)
                {
                    isCustom = true;
                    Instance.EditorToolbar.GridCustomSizeMenuItem.IsChecked = true;
                }

                Instance.EditorToolbar.CustomGridSizeLabel.Text = string.Format(Instance.EditorToolbar.CustomGridSizeLabel.Tag.ToString(), GridCustomSize);

                if (!isCustom) _GridSize = value;
                else _GridSize = GridCustomSize;
            }

            public Color GridColor { get => GetGridColor(); set => SetGridColor(value); }
            private Color _GridColor = Color.Red;
            private Color GetGridColor() { return _GridColor; }
            private void SetGridColor(Color value) { _GridColor = value; }
            #endregion

            #region General Options Variables
            public bool CopyAir
            {
                get
                {
                    return _CopyAir;
                }
                set
                {
                    _CopyAir = value;
                    Instance.MenuBar.copyAirToggle.IsChecked = value;
                }
            }
            private bool _CopyAir = false;

            public int FasterNudgeAmount
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
            private int _FasterNudgeAmount = 5;



            public bool EnableFasterNudge
            {
                get
                {
                    return _EnableFasterNudge;
                }
                set
                {
                    Instance.EditorStatusBar.nudgeFasterButton.IsChecked = value;
                    Instance.MenuBar.nudgeSelectionFasterToolStripMenuItem.IsChecked = value;
                    _EnableFasterNudge = value;
                }
            }
            private bool _EnableFasterNudge = false;

            public bool ScrollLocked
            {
                get
                {
                    return _ScrollLocked;
                }
                set
                {
                    _ScrollLocked = value;
                    Instance.EditorStatusBar.scrollLockButton.IsChecked = value;
                    Instance.MenuBar.statusNAToolStripMenuItem.IsChecked = value;
                }
            }
            private bool _ScrollLocked = true;
            #endregion

            #region Tool Mode Methods
            public void PointerToolMode(bool? value = null)
            {
                if (value != null) SetToolModes(0, value.Value);
                else SetToolModes(0, Instance.EditorToolbar.PointerToolButton.IsChecked.Value);
            }
            public void SelectionToolMode(bool? value = null)
            {
                if (value != null) SetToolModes(1, value.Value);
                else SetToolModes(1, IsSelectMode());
            }
            public void DrawToolMode(bool? value = null)
            {
                if (value != null) SetToolModes(2, value.Value);
                else SetToolModes(2, IsDrawMode());
            }

            //Determines the Draw Tool's Brush Size (Tiles Only)
            public int DrawBrushSize { get => GetDrawBrushSize(); set => SetDrawBrushSize(value); }
            private int _DrawBrushSize = 1;

            private int GetDrawBrushSize()
            {
                return _DrawBrushSize;
            }
            private void SetDrawBrushSize(int value)
            {
                _DrawBrushSize = value;
            }
            public void SplineMode(bool? value = null)
            {
                if (value != null) SetToolModes(4, value.Value);
                else SetToolModes(4, Instance.EditorToolbar.SplineToolButton.IsChecked.Value);
            }
            public void ChunksMode(bool isClick = false)
            {
                if (!isClick)
                {
                    bool isEnabled = Instance.EditorToolbar.ChunksToolButton.IsChecked.Value;
                    if (isEnabled) Instance.EditorToolbar.ChunksToolButton.IsChecked = true;
                    else Instance.EditorToolbar.ChunksToolButton.IsChecked = false;
                }

                Methods.Internal.UserInterface.UpdateControls();
            }
            public void SetToolModes(int selectedID, bool value)
            {
                Instance.EditorToolbar.PointerToolButton.IsChecked = (selectedID == 0 ? value : false);
                Instance.EditorToolbar.SelectToolButton.IsChecked = (selectedID == 1 ? value : false);
                Instance.EditorToolbar.DrawToolButton.IsChecked = (selectedID == 2 ? value : false);
                Instance.EditorToolbar.SplineToolButton.IsChecked = (selectedID == 4 ? value : false);
                Methods.Internal.UserInterface.UpdateControls();
            }

            public bool IsPointerMode()
            {
                return Instance.EditorToolbar.PointerToolButton.IsChecked.Value;
            }
            public bool IsSelectMode()
            {
                return Instance.EditorToolbar.SelectToolButton.IsChecked.Value;
            }
            public bool IsSplineMode()
            {
                return Instance.EditorToolbar.SplineToolButton.IsChecked.Value;
            }
            public bool IsChunkMode()
            {
                return Instance.EditorToolbar.ChunksToolButton.IsChecked.Value;
            }
            public bool IsDrawMode()
            {
                return Instance.EditorToolbar.DrawToolButton.IsChecked.Value;
            }

            #endregion

            #region Magnet Mode Methods

            public int MagnetSize
            {
                get
                {
                    return _MagnetSize;
                }
                set
                {
                    bool isCustom = false;
                    Instance.EditorToolbar.Magnet8x8ModeMenuItem.IsChecked = false;
                    Instance.EditorToolbar.Magnet16x16ModeMenuItem.IsChecked = false;
                    Instance.EditorToolbar.Magnet32x32ModeMenuItem.IsChecked = false;
                    Instance.EditorToolbar.Magnet64x64ModeMenuItem.IsChecked = false;
                    Instance.EditorToolbar.MagnetCustomModeMenuItem.IsChecked = false;

                    if (value == 8) Instance.EditorToolbar.Magnet8x8ModeMenuItem.IsChecked = true;
                    else if (value == 16) Instance.EditorToolbar.Magnet16x16ModeMenuItem.IsChecked = true;
                    else if (value == 32) Instance.EditorToolbar.Magnet32x32ModeMenuItem.IsChecked = true;
                    else if (value == 64) Instance.EditorToolbar.Magnet64x64ModeMenuItem.IsChecked = true;
                    else if (value == -1)
                    {
                        isCustom = true;
                        Instance.EditorToolbar.MagnetCustomModeMenuItem.IsChecked = true;
                    }

                    Instance.EditorToolbar.CustomMagnetSizeLabel.Text = string.Format(Instance.EditorToolbar.CustomMagnetSizeLabel.Tag.ToString(), _CustomMagnetSize);

                    if (!isCustom) _MagnetSize = value;
                    else _MagnetSize = _CustomMagnetSize;
                }
            }
            private int _MagnetSize = 16;
            public int CustomMagnetSize
            {
                get
                {
                    return _CustomMagnetSize;
                }
                set
                {
                    if (Instance != null)
                    {
                        _CustomMagnetSize = value;
                        MagnetSize = -1;
                        Instance.EditorToolbar.AllowNUDUpdate = false;
                        Instance.EditorToolbar.CustomMagnetSizeAdjuster.Value = _CustomMagnetSize;
                        Instance.EditorToolbar.AllowNUDUpdate = true;
                    }
                }
            }
            private int _CustomMagnetSize = 16;
            public bool UseMagnetXAxis
            {
                get 
                {
                    if (Instance == null) return false;
                    return Instance.EditorToolbar.MagnetXAxisLockMenuItem.IsChecked; 
                }
                set
                {
                    Instance.EditorToolbar.MagnetXAxisLockMenuItem.IsChecked = value;
                }
            }
            public bool UseMagnetYAxis
            {
                get
                {
                    if (Instance == null) return false;
                    return Instance.EditorToolbar.MagnetXAxisLockMenuItem.IsChecked;
                }
                set
                {
                    if (Instance == null) return;
                    Instance.EditorToolbar.MagnetXAxisLockMenuItem.IsChecked = value;
                }
            }
            public bool UseMagnetMode
            {
                get 
                {
                    if (Instance == null) return false;
                    return Instance.EditorToolbar.MagnetMode.IsChecked.Value; 
                }
                set
                {
                    if (Instance == null) return;
                    if (value == Instance.EditorToolbar.MagnetMode.IsChecked.Value) return;
                    Instance.EditorToolbar.MagnetMode.IsChecked = value;
                }
            }

            #endregion

            #region Wand Mode Methods

            public bool UseMagicSelectWand
            {
                get
                {
                    if (Instance == null) return false;
                    return Instance.EditorToolbar.WandToolButton.IsChecked.Value;
                }
                set
                {
                    if (Instance == null) return;
                    if (value == Instance.EditorToolbar.WandToolButton.IsChecked.Value) return;
                    Instance.EditorToolbar.WandToolButton.IsChecked = value;
                }
            }
            public bool? MagicSelectWand_SolidTop_A
            {
                get
                {
                    if (Instance == null) return false;
                    return Instance.EditorToolbar.WandFlipSolidTopACheckbox.IsChecked;
                }
            }
            public bool? MagicSelectWand_SolidTop_B
            {
                get
                {
                    if (Instance == null) return false;
                    return Instance.EditorToolbar.WandFlipSolidTopBCheckbox.IsChecked;
                }
            }
            public bool? MagicSelectWand_SolidLRB_A
            {
                get
                {
                    if (Instance == null) return false;
                    return Instance.EditorToolbar.WandFlipSolidLrbACheckbox.IsChecked;
                }
            }
            public bool? MagicSelectWand_SolidLRB_B
            {
                get
                {
                    if (Instance == null) return false;
                    return Instance.EditorToolbar.WandFlipSolidLrbBCheckbox.IsChecked;
                }
            }
            public bool? MagicSelectWand_FlipX
            {
                get
                {
                    if (Instance == null) return false;
                    return Instance.EditorToolbar.WandFlipXCheckbox.IsChecked;
                }
            }
            public bool? MagicSelectWand_FlipY
            {
                get
                {
                    if (Instance == null) return false;
                    return Instance.EditorToolbar.WandFlipYCheckbox.IsChecked;
                }
            }
            public int? MagicSelectWand_Index
            {
                get
                {
                    if (Instance == null) return 0;
                    return Instance.EditorToolbar.WandSelectTileList.SelectedIndex;
                }
            }

            public Structures.TileSelectSpecifics GetMagicWandSelectSpecifics()
            {
                return new Structures.TileSelectSpecifics(MagicSelectWand_Index, MagicSelectWand_FlipX, MagicSelectWand_FlipY, MagicSelectWand_SolidTop_A, MagicSelectWand_SolidTop_B, MagicSelectWand_SolidLRB_A, MagicSelectWand_SolidLRB_B);
            }

            #endregion

            #region Collision View Variables

            public bool ShowCollisionA
            {
                get { return _ShowCollisionA; }
                set
                {
                    Instance.EditorToolbar.ShowCollisionAButton.IsChecked = value;
                    _ShowCollisionA = value;
                    Instance.EditorToolbar.ShowCollisionBButton.IsChecked = false;
                    _ShowCollisionB = false;

                    if (value == true)
                    {
                        if (ShowTileID) ShowTileID = false;
                        if (ShowFlippedTileHelper) ShowFlippedTileHelper = false;
                    }
                    Methods.Internal.UserInterface.ReloadSpritesAndTextures();
                }
            }
            private bool _ShowCollisionA;

            public bool ShowCollisionB
            {
                get { return _ShowCollisionB; }
                set
                {
                    Instance.EditorToolbar.ShowCollisionAButton.IsChecked = false;
                    _ShowCollisionA = false;
                    Instance.EditorToolbar.ShowCollisionBButton.IsChecked = value;
                    _ShowCollisionB = value;

                    if (value == true)
                    {
                        if (ShowTileID) ShowTileID = false;
                        if (ShowFlippedTileHelper) ShowFlippedTileHelper = false;
                    }
                    Methods.Internal.UserInterface.ReloadSpritesAndTextures();
                }
            }
            private bool _ShowCollisionB;

            #endregion

            #region Scene View Variables

            public bool ShowTileID
            {
                get { return _ShowTileID; }
                set
                {
                    Instance.EditorToolbar.ShowTileIDButton.IsChecked = value;
                    _ShowTileID = value;
                    if (value == true)
                    {
                        if (ShowCollisionA) ShowCollisionA = false;
                        if (ShowCollisionB) ShowCollisionB = false;
                        if (ShowFlippedTileHelper) ShowFlippedTileHelper = false;
                    }
                    Methods.Internal.UserInterface.ReloadSpritesAndTextures();
                }
            }
            private bool _ShowTileID;
            public bool UseEncoreColors
            {
                get { return _UseEncoreColors; }
                set
                {
                    Instance.EditorToolbar.EncorePaletteButton.IsChecked = value;
                    _UseEncoreColors = value;
                    Methods.Internal.UserInterface.ReloadSpritesAndTextures();
                }
            }
            private bool _UseEncoreColors = false;
            public bool ExtraLayersMoveToFront
            {
                get
                {
                    return _ExtraLayersMoveToFront;
                }
                set
                {
                    _ExtraLayersMoveToFront = value;
                    Instance.MenuBar.moveExtraLayersToFrontToolStripMenuItem.IsChecked = value;
                }
            }
            private bool _ExtraLayersMoveToFront = false;
            public bool ShowFlippedTileHelper
            {
                get
                {
                    return _ShowFlippedTileHelper;
                }
                set
                {
                    _ShowFlippedTileHelper = value;
                    Instance.EditorToolbar.FlipAssistButton.IsChecked = value;
                    if (value == true)
                    {
                        if (ShowCollisionA) ShowCollisionA = false;
                        if (ShowCollisionB) ShowCollisionB = false;
                        if (ShowTileID) ShowTileID = false;
                    }
                    Methods.Internal.UserInterface.ReloadSpritesAndTextures();
                }
            }
            private bool _ShowFlippedTileHelper = false;

            #endregion

            #region Object View Variables

            public bool EntitiesVisibileAboveAllLayers
            {
                get
                {
                    return _EntitiesVisibileAboveAllLayers;
                }
                set
                {
                    _EntitiesVisibileAboveAllLayers = value;
                    Instance.MenuBar.showEntitiesAboveAllOtherLayersToolStripMenuItem.IsChecked = value;
                }
            }
            private bool _EntitiesVisibileAboveAllLayers = false;
            public bool ShowParallaxSprites
            {
                get { return _ShowParallaxSprites; }
                set
                {
                    _ShowParallaxSprites = value;
                    Instance.MenuBar.showParallaxSpritesToolStripMenuItem.IsChecked = value;
                }
            }
            private bool _ShowParallaxSprites = false;
            public bool ApplyEditEntitiesTransparency
            {
                get { return _ApplyEditEntitiesTransparency; }
                set
                {
                    _ApplyEditEntitiesTransparency = value;
                    Instance.MenuBar.EditEntitiesTransparencyToggle.IsChecked = value;
                    Instance.EditorStatusBar.QuickEditEntitiesTransparentLayers.IsChecked = value;
                }
            }
            private bool _ApplyEditEntitiesTransparency = false;
            public bool ShowEntitySelectionBoxes
            {
                get { return _ShowEntitySelectionBoxes; }
                set
                {
                    _ShowEntitySelectionBoxes = value;
                    Instance.MenuBar.showEntitySelectionBoxesToolStripMenuItem.IsChecked = value;
                }
            }
            private bool _ShowEntitySelectionBoxes = false;
            public bool ShowEntityPathArrows
            {
                get
                {
                    return _ShowEntityPathArrows;
                }
                set
                {
                    _ShowEntityPathArrows = value;
                    Instance.MenuBar.showEntityPathArrowsToolstripItem.IsChecked = value;
                }
            }
            private bool _ShowEntityPathArrows = true;
            public bool ShowWaterLevel
            {
                get
                {
                    return _ShowWaterLevel;
                }
                set
                {
                    _ShowWaterLevel = value;
                    Instance.MenuBar.showWaterLevelToolStripMenuItem.IsChecked = value;
                }
            }
            private bool _ShowWaterLevel = false;
            public bool AlwaysShowWaterLevel
            {
                get
                {
                    return _AlwaysShowWaterLevel;
                }
                set
                {
                    _AlwaysShowWaterLevel = value;
                    Instance.MenuBar.waterLevelAlwaysShowItem.IsChecked = value;
                }
            }
            private bool _AlwaysShowWaterLevel = false;
            public bool SizeWaterLevelwithBounds
            {
                get
                {
                    return _SizeWaterLevelwithBounds;
                }
                set
                {
                    _SizeWaterLevelwithBounds = value;
                    Instance.MenuBar.sizeWithBoundsWhenNotSelectedToolStripMenuItem.IsChecked = value;
                }
            }
            private bool _SizeWaterLevelwithBounds = false;

            public Color SelectedWaterColor { get; set; }

            public Color waterColor { get => GetWaterEntityColor(); set => SetWaterEntityColor(value); }
            private Color _WaterEntityColor = Color.Blue;
            private Color GetWaterEntityColor() { return _WaterEntityColor; }
            private void SetWaterEntityColor(Color value) { _WaterEntityColor = value; }
            public string CurrentManiaUILanguage { get; set; } = "EN";
            public int CurrentManiaUIControllerType { get; set; } = 2;

            #endregion

            #region Editor View Variables
            public int LastSelectedTileID { get; set; } = -1;
            public int CurrentPlayerBeingTracked { get; set; } = -1;
            public int LastQuickButtonState { get; set; } = 0;
            public bool CountTilesSelectedInPixels
            {
                get
                {
                    return _CountTilesSelectedInPixels;
                }
                set
                {
                    _CountTilesSelectedInPixels = value;
                    Instance.EditorStatusBar.EnablePixelModeButton.IsChecked = value;
                    Instance.MenuBar.EnablePixelModeMenuItem.IsChecked = value;
                }
            }
            private bool _CountTilesSelectedInPixels = false;

            #endregion

            #region Unoptimized Spline Options
            public class SplineOptions
            {
                //Determines the Spline Point Frequency

                private StateModel ParentModel { get; set; }

                public int SplineSize { get => GetSplineSize(); set => SetSplineSize(value); }
                private int _SplineSize = 128;
                private int GetSplineSize()
                {
                    return _SplineSize;
                }
                private void SetSplineSize(int value)
                {
                    _SplineSize = value;
                    ParentModel.Instance.Dispatcher.Invoke(() => Methods.Internal.UserInterface.UpdateControls());
                }

                public SplineOptions(StateModel Parent)
                {
                    ParentModel = Parent;
                }

                public bool SplineToolShowLines { get; set; } = true; //Self Explanatory
                public int SplineNumberOfObjectsRendered { get; set; } = 0; //Self Explanatory
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
                            ParentModel.Instance.EditorToolbar.SplineLineMode.IsChecked = true;
                            _SplineLineMode = true;
                            ParentModel.Instance.EditorToolbar.SplineOvalMode.IsChecked = false;
                            _SplineOvalMode = false;
                        }
                        else
                        {
                            ParentModel.Instance.EditorToolbar.SplineLineMode.IsChecked = false;
                            _SplineLineMode = false;
                            ParentModel.Instance.EditorToolbar.SplineOvalMode.IsChecked = true;
                            _SplineOvalMode = true;
                        }
                    }
                    else
                    {
                        if (state)
                        {
                            ParentModel.Instance.EditorToolbar.SplineLineMode.IsChecked = false;
                            _SplineLineMode = false;
                            ParentModel.Instance.EditorToolbar.SplineOvalMode.IsChecked = true;
                            _SplineOvalMode = true;
                        }
                        else
                        {
                            ParentModel.Instance.EditorToolbar.SplineLineMode.IsChecked = true;
                            _SplineLineMode = true;
                            ParentModel.Instance.EditorToolbar.SplineOvalMode.IsChecked = false;
                            _SplineOvalMode = false;
                        }
                    }
                }

                public Classes.Scene.EditorEntity SplineObjectRenderingTemplate { get; set; }

                public string SplineRenderingObjectName { get => GetSplineRenderingObjectName(); }

                private string GetSplineRenderingObjectName()
                {
                    return SplineObjectRenderingTemplate.Object.Name.Name;
                }




            }
            public Dictionary<int, SplineOptions> SplineOptionsGroup { get => GetSplineOptionsGroup(); set => SetSplineOptionsGroup(value); }

            private Dictionary<int, SplineOptions> _SplineOptionsGroup = new Dictionary<int, SplineOptions>();

            public void AddSplineOptionsGroup(int splineID)
            {
                SplineOptionsGroup.Add(splineID, new SplineOptions(this));
            }

            private Dictionary<int, SplineOptions> GetSplineOptionsGroup()
            {
                return _SplineOptionsGroup;
            }

            private void SetSplineOptionsGroup(Dictionary<int, SplineOptions> value)
            {
                _SplineOptionsGroup = value;
            }


            public bool AllowSplineOptionsUpdate = true;

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

            public void AdjustSplineGroupOptions(SplineOption option, object value)
            {
                if (!AllowSplineOptionsUpdate) return;
                switch (option)
                {
                    case SplineOption.Size:
                        if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineSize = (int)value;
                        else
                        {
                            SplineOptions options = new SplineOptions(this);
                            options.SplineSize = (int)value;
                            SplineOptionsGroup.Add(SelectedSplineID, options);
                        }
                        break;
                    case SplineOption.LineMode:
                        if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineLineMode = (bool)value;
                        else
                        {
                            SplineOptions options = new SplineOptions(this);
                            options.SplineLineMode = (bool)value;
                            SplineOptionsGroup.Add(SelectedSplineID, options);
                        }
                        break;
                    case SplineOption.OvalMode:
                        if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineOvalMode = (bool)value;
                        else
                        {
                            SplineOptions options = new SplineOptions(this);
                            options.SplineOvalMode = (bool)value;
                            SplineOptionsGroup.Add(SelectedSplineID, options);
                        }
                        break;
                    case SplineOption.ShowLines:
                        if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineToolShowLines = (bool)value;
                        else
                        {
                            SplineOptions options = new SplineOptions(this);
                            options.SplineToolShowLines = (bool)value;
                            SplineOptionsGroup.Add(SelectedSplineID, options);
                        }
                        break;
                    case SplineOption.ShowObjects:
                        if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineToolShowObject = (bool)value;
                        else
                        {
                            SplineOptions options = new SplineOptions(this);
                            options.SplineToolShowObject = (bool)value;
                            SplineOptionsGroup.Add(SelectedSplineID, options);
                        }
                        break;
                    case SplineOption.ShowPoints:
                        if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineToolShowPoints = (bool)value;
                        else
                        {
                            SplineOptions options = new SplineOptions(this);
                            options.SplineToolShowPoints = (bool)value;
                            SplineOptionsGroup.Add(SelectedSplineID, options);
                        }
                        break;
                    case SplineOption.SpawnObject:
                        if (SplineOptionsGroup.ContainsKey(SelectedSplineID)) SplineOptionsGroup[SelectedSplineID].SplineObjectRenderingTemplate = (Classes.Scene.EditorEntity)value;
                        else
                        {
                            SplineOptions options = new SplineOptions(this);
                            options.SplineObjectRenderingTemplate = (Classes.Scene.EditorEntity)value;
                            SplineOptionsGroup.Add(SelectedSplineID, options);
                        }
                        break;

                }
            }

            public int SelectedSplineID { get => GetSplineSelectedID(); set => SetSplineSelectedID(value); }
            private int _SelectedSplineID = 0;
            private int GetSplineSelectedID()
            {
                return _SelectedSplineID;
            }
            private void SetSplineSelectedID(int value)
            {
                _SelectedSplineID = value;
            }
            #endregion

            #region Collision View Variables
            public int CollisionPreset { get => GetCollisionPreset(); set => SetCollisionPreset(value); }
            private int _CollisionPreset = 0;
            private int GetCollisionPreset()
            {
                return _CollisionPreset;
            }
            private void SetCollisionPreset(int value)
            {
                _CollisionPreset = value;

                Instance.EditorToolbar.CollisionInvertedMenuItem.IsChecked = false;
                Instance.EditorToolbar.CollisionCustomMenuItem.IsChecked = false;
                Instance.EditorToolbar.CollisionDefaultMenuItem.IsChecked = false;

                if (value == 0) Instance.EditorToolbar.CollisionDefaultMenuItem.IsChecked = true;
                else if (value == 1) Instance.EditorToolbar.CollisionInvertedMenuItem.IsChecked = true;
                else if (value == 2) Instance.EditorToolbar.CollisionCustomMenuItem.IsChecked = true;

                RefreshCollisionColours();
            }

            #region Collision Colors
            public Color Custom_CollisionAllSolid_Color { get; set; } = Color.White;
            public Color Custom_CollisionTopOnlySolid_Color { get; set; } = Color.Yellow;
            public Color Custom_CollisionLRDSolid_Color { get; set; } = Color.Red;


            public Color CollisionAllSolid_Color { get; set; } = Color.White;
            public Color CollisionTopOnlySolid_Color { get; set; } = Color.Yellow;
            public Color CollisionLRDSolid_Color { get; set; } = Color.Red;

            #endregion


            public void RefreshCollisionColours()
            {
                try
                {
                    if (Methods.Solution.CurrentSolution.CurrentScene != null && Methods.Solution.CurrentSolution.CurrentTiles != null)
                    {
                        switch (CollisionPreset)
                        {
                            case 2:
                                CollisionAllSolid_Color = Custom_CollisionAllSolid_Color;
                                CollisionTopOnlySolid_Color = Custom_CollisionTopOnlySolid_Color;
                                CollisionLRDSolid_Color = Custom_CollisionLRDSolid_Color;
                                break;
                            case 1:
                                CollisionAllSolid_Color = Color.Black;
                                CollisionTopOnlySolid_Color = Color.Yellow;
                                CollisionLRDSolid_Color = Color.Red;
                                break;
                            case 0:
                                CollisionAllSolid_Color = Color.White;
                                CollisionTopOnlySolid_Color = Color.Yellow;
                                CollisionLRDSolid_Color = Color.Red;
                                break;
                        }
                    }
                }
                catch
                {

                }


            }
            #endregion

            #region Misc



            #region Unoptimized Technical Options
            public bool IsConsoleWindowOpen { get => GetIsConsoleWindowOpen(); set => SetIsConsoleWindowOpen(value); }
            private bool _IsConsoleWindowOpen = false;
            private bool GetIsConsoleWindowOpen()
            {
                return _IsConsoleWindowOpen;
            }
            private void SetIsConsoleWindowOpen(bool value)
            {
                _IsConsoleWindowOpen = value;
            }
            public bool DataDirectoryReadOnlyMode { get => GetDataDirectoryReadOnlyMode(); set => SetDataDirectoryReadOnlyMode(value); }
            private bool _DataDirectoryReadOnlyMode = false;
            private bool GetDataDirectoryReadOnlyMode()
            {
                return _DataDirectoryReadOnlyMode;
            }
            private void SetDataDirectoryReadOnlyMode(bool value)
            {
                _DataDirectoryReadOnlyMode = value;
            }

            #endregion

            #region To-Remove Settings

            public bool DebugStatsVisibleOnPanel { get => GetDebugStatsVisibleOnPanel(); set => SetDebugStatsVisibleOnPanel(value); }
            private bool _DebugStatsVisibleOnPanel = false;
            private bool GetDebugStatsVisibleOnPanel()
            {
                return _DebugStatsVisibleOnPanel;
            }
            private void SetDebugStatsVisibleOnPanel(bool value)
            {
                _DebugStatsVisibleOnPanel = value;
                Instance.MenuBar.showStatsToolStripMenuItem.IsChecked = value;
                Instance.ViewPanel.InfoHUD.UpdatePopupSize();
            }
            #endregion

            #region To-Improve Implementation Variables
            public bool AddStageConfigEntriesAllowed { get; set; } = true; //Self Explanatory

            #endregion

            public bool Duplicate1
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
            private bool _Duplicate1;

            #region Unoptimized Misc Stuff
            public int EncoreSetupType { get; set; } //Used to determine what kind of encore setup the stage uses
            public string ObjectFilter { get; set; } = ""; //Used to hide objects that don't match the discription
            #endregion

            #endregion

            #endregion
        }
    }

    public class SolutionStateBindingExtension : Binding
    {
        public SolutionStateBindingExtension()
        {
            Initialize();
        }

        public SolutionStateBindingExtension(string path) : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Source = SolutionState.Main;
            this.Mode = BindingMode.TwoWay;
        }
    }
}
