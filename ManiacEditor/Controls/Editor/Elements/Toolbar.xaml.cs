﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Specialized;
using ManiacEditor.Controls.Global;

using ManiacEditor.Controls.Utility;
using ManiacEditor.Controls.Utility.Editors;
using ManiacEditor.Controls.Utility.Object_ID_Repair_Tool;
using ManiacEditor.Controls.Utility.Object_Manager;
using ManiacEditor.Controls.Utility.Editors.Dev;
using ManiacEditor.Controls.Utility.Editors.Configuration;

using ManiacEditor.Extensions;



namespace ManiacEditor.Controls.Editor.Elements
{
    /// <summary>
    /// Interaction logic for Toolbar.xaml
    /// </summary>
    public partial class Toolbar : UserControl
    {
        #region Variables
        private bool HasFullyInitialized { get; set; } = false;

        public bool AllowNUDUpdate { get; set; } = true;

        #region Extra Layer Buttons
        public IDictionary<Controls.Global.EditLayerToggleButton, Controls.Global.EditLayerToggleButton> ExtraLayerEditViewButtons { get; set; }
        public IList<Separator> ExtraLayerSeperators { get; set; }
        #endregion

        #endregion

        #region Init
        public Toolbar()
        {
            InitializeComponent();

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                EditFGLower.Click += EditFGLower_Click;
                EditFGLow.Click += EditFGLow_Click;
                EditFGHigh.Click += EditFGHigh_Click;
                EditFGHigher.Click += EditFGHigher_Click;

                EditFGLower.RightClick += EditFGLower_RightClick;
                EditFGLow.RightClick += EditFGLow_RightClick;
                EditFGHigh.RightClick += EditFGHigh_RightClick;
                EditFGHigher.RightClick += EditFGHigher_RightClick;
                HasFullyInitialized = true;
            }

        }

        private static MainEditor Instance;
        public static void UpdateInstance(MainEditor _instance)
        {
            Instance = _instance;
        }
        #endregion

        #region File Events
        private void NewSceneEvent(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Editor.SolutionLoader.NewScene(); }
        public void OpenSceneEvent(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Editor.SolutionLoader.OpenScene(); }
        public void SaveSceneEvent(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Editor.SolutionLoader.Save(); }
        #endregion

        #region Animation Events
        private void MovingPlatformsObjectsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Methods.Editor.SolutionState.AllowMovingPlatformAnimations == false)
            {
                movingPlatformsObjectsToolStripMenuItem.IsChecked = true;
                Methods.Editor.SolutionState.AllowMovingPlatformAnimations = true;
            }
            else
            {
                movingPlatformsObjectsToolStripMenuItem.IsChecked = false;
                Methods.Editor.SolutionState.AllowMovingPlatformAnimations = false;
            }

        }
        private void SpriteFramesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Methods.Editor.SolutionState.AllowSpriteAnimations == false)
            {
                spriteFramesToolStripMenuItem.IsChecked = true;
                Methods.Editor.SolutionState.AllowSpriteAnimations = true;
            }
            else
            {
                spriteFramesToolStripMenuItem.IsChecked = false;
                Methods.Editor.SolutionState.AllowSpriteAnimations = false;
            }
        }
        private void ParallaxAnimationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Methods.Editor.SolutionState.ParallaxAnimationChecked == false)
            {
                parallaxAnimationMenuItem.IsChecked = true;
                Methods.Editor.SolutionState.ParallaxAnimationChecked = true;
            }
            else
            {
                parallaxAnimationMenuItem.IsChecked = false;
                Methods.Editor.SolutionState.ParallaxAnimationChecked = false;
            }
        }

        #endregion

        #region Apps Item Events
        private void TileManiacEditTileEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.TileManiacIntergration(); }
        private void RSDKUnpackerEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.RSDKUnpacker(); }
        private void SonicManiaHeadless(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.SonicManiaHeadless(); }
        private void MenuAppsCheatEngine_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.CheatEngine(); }
        private void ModManager(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ManiaModManager(); }
        private void TileManiacNormal(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.TileManiacNormal(); }
        private void InsanicManiacToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.InsanicManiac(); }
        private void RSDKAnnimationEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.RSDKAnnimationEditor(); }
        private void RenderListManagerToolstripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.RenderListManager(); }
        private void ColorPaletteEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ManiaPal(sender, e); }
        private void ManiaPalMenuItem_SubmenuOpened(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ManiaPalSubmenuOpened(sender, e); }
        private void DuplicateObjectIDHealerToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.DuplicateObjectIDHealer(); }
        #endregion

        #region Folder Item Events
        private void OpenSceneFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenSceneFolder(); }
        private void OpenManiacEditorFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenManiacEditorFolder(); }
        private void OpenManiacEditorFixedSettingsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenManiacEditorFixedSettingsFolder(); }
        private void OpenManiacEditorPortableSettingsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenManiacEditorPortableSettingsFolder(); }
        private void OpenDataDirectoryFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenDataDirectory(); }
        private void OpenSonicManiaFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenSonicManiaFolder(); }
        private void OpenASavedPlaceToolStripMenuItem_DropDownOpening(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenASavedPlaceDropDownOpening(sender, e); }
        private void OpenASavedPlaceToolStripMenuItem_DropDownClosed(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenASavedPlaceDropDownClosed(sender, e); }
        private void OpenAResourcePackFolderToolStripMenuItem_DropDownOpening(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenAResourcePackFolderDropDownOpening(sender, e); }
        private void OpenAResourcePackFolderToolStripMenuItem_DropDownClosed(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenAResourcePackFolderDropDownClosed(sender, e); }
        #endregion

        #region Common Tool Events

        #region General Events
        private void UndoEvent(object sender, RoutedEventArgs e) { Methods.Editor.EditorActions.EditorUndo(); }
        private void RedoEvent(object sender, RoutedEventArgs e) { Methods.Editor.EditorActions.EditorRedo(); }
        private void ZoomInEvent(object sender, RoutedEventArgs e) { Methods.Editor.EditorActions.ZoomIn(); }
        private void ZoomOutEvent(object sender, RoutedEventArgs e) { Methods.Editor.EditorActions.ZoomOut(); }
        #endregion

        #region Global Tools
        private void ToggleSelectToolEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.SelectionMode(); }
        private void TogglePointerToolEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.PointerMode(); }
        private void ToggleDrawToolEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.DrawMode(); }
        private void ToggleSplineToolEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.SplineMode(); }
        private void ToggleChunksToolEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.ChunksMode(true); }
        #endregion

        #region Spline Tool Events
        private void SplineShowLineCheckboxCheckChanged(object sender, RoutedEventArgs e)
        {
            Methods.Editor.SolutionState.AdjustSplineGroupOptions(Methods.Editor.SolutionState.SplineOption.ShowLines, SplineShowLineCheckbox.IsChecked.Value);
        }
        private void SplineShowPointsCheckboxCheckChanged(object sender, RoutedEventArgs e)
        {
            Methods.Editor.SolutionState.AdjustSplineGroupOptions(Methods.Editor.SolutionState.SplineOption.ShowPoints, SplineShowPointsCheckbox.IsChecked.Value);
        }
        private void SplineShowObjectsCheckboxCheckChanged(object sender, RoutedEventArgs e)
        {
            Methods.Editor.SolutionState.AdjustSplineGroupOptions(Methods.Editor.SolutionState.SplineOption.ShowObjects, SplineShowObjectsCheckbox.IsChecked.Value);
        }

        bool AllowSplineFreqeunceUpdate = true;
        bool AllowSplineUpdateEvent = true;

        private void SplineOptionsIDChangedEvent(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (SplinePointSeperationSlider != null && SplinePointSeperationNUD != null && SplineGroupID != null && AllowSplineUpdateEvent)
            {
                SelectedSplineIDChangedEvent(SplineGroupID.Value.Value);
            }
        }
        public void SelectedSplineIDChangedEvent(int value)
        {
            AllowSplineUpdateEvent = false;
            Methods.Editor.SolutionState.AllowSplineOptionsUpdate = false;
            SplineGroupID.Value = value;
            Methods.Editor.SolutionState.SelectedSplineID = value;
            SplineSpawnID.Value = value;
            Methods.Internal.UserInterface.SplineControls.UpdateSplineSettings(value);
            Methods.Editor.SolutionState.AllowSplineOptionsUpdate = true;
            AllowSplineUpdateEvent = true;

        }
        private async void SplinePointFrequenceChangedEvent(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (HasFullyInitialized)
            {
                if (!Methods.Editor.SolutionState.AllowSplineOptionsUpdate) return;
                if (SplinePointSeperationNUD != null && SplinePointSeperationSlider != null && AllowSplineFreqeunceUpdate)
                {
                    AllowSplineFreqeunceUpdate = false;
                    int size = (int)SplinePointSeperationNUD.Value;
                    SplinePointSeperationSlider.Value = size;
                    await Task.Run(() => Methods.Editor.SolutionState.AdjustSplineGroupOptions(Methods.Editor.SolutionState.SplineOption.Size, size));
                    AllowSplineFreqeunceUpdate = true;
                }
            }
        }
        private async void SplinePointFrequenceChangedEvent(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (HasFullyInitialized)
            {
                if (!Methods.Editor.SolutionState.AllowSplineOptionsUpdate) return;
                if (SplinePointSeperationSlider != null && SplinePointSeperationNUD != null && AllowSplineFreqeunceUpdate)
                {
                    AllowSplineFreqeunceUpdate = false;
                    int size = (int)SplinePointSeperationSlider.Value;
                    SplinePointSeperationNUD.Value = size;
                    await Task.Run(() => Methods.Editor.SolutionState.AdjustSplineGroupOptions(Methods.Editor.SolutionState.SplineOption.Size, size));
                    AllowSplineFreqeunceUpdate = true;
                }
            }
        }
        private void SplineLineMode_Click(object sender, RoutedEventArgs e)
        {
            Methods.Editor.SolutionState.AdjustSplineGroupOptions(Methods.Editor.SolutionState.SplineOption.LineMode, SplineLineMode.IsChecked.Value);
        }
        private void SplineOvalMode_Click(object sender, RoutedEventArgs e)
        {
            Methods.Editor.SolutionState.AdjustSplineGroupOptions(Methods.Editor.SolutionState.SplineOption.OvalMode, SplineOvalMode.IsChecked.Value);
        }
        private void SplineSpawnRender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Methods.Editor.Solution.Entities != null && Methods.Editor.SolutionState.AllowSplineOptionsUpdate)
            {
                var selectedItem = SelectedSplineRender.SelectedItem as TextBlock;
                if (selectedItem.Tag == null) return;
                if (selectedItem.Tag is RSDKv5.SceneObject)
                {
                    var obj = selectedItem.Tag as RSDKv5.SceneObject;
                    int splineID = Methods.Editor.SolutionState.SelectedSplineID;
                    Methods.Editor.SolutionState.AdjustSplineGroupOptions(Methods.Editor.SolutionState.SplineOption.SpawnObject, Methods.Editor.Solution.Entities.GenerateEditorEntity(new RSDKv5.SceneEntity(obj, 0)));
                    Instance.EntitiesToolbar?.UpdateToolbar(new List<Classes.Scene.EditorEntity>() { Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate });

                    if (Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate != null)
                        SplineRenderObjectName.Content = Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate.Object.Name.Name;
                    else
                        SplineRenderObjectName.Content = "None";

                }

            }
        }
        private void SplineRenderObjectName_Click(object sender, RoutedEventArgs e)
        {
            if (!SelectedSplineRender.IsDropDownOpen) SelectedSplineRender.IsDropDownOpen = true;
            else SelectedSplineRender.IsDropDownOpen = false;
        }
        private async void RenderSelectedSpline_Click(object sender, RoutedEventArgs e)
        {
            if (Methods.Editor.SolutionState.SplineOptionsGroup[Methods.Editor.SolutionState.SelectedSplineID].SplineObjectRenderingTemplate != null && Methods.Editor.SolutionState.SplineOptionsGroup[Methods.Editor.SolutionState.SelectedSplineID].SplineTotalNumberOfObjects >= 2)
            {
                await Task.Run(() => Methods.Entities.SplineSpawning.RenderSplineByID(Methods.Editor.SolutionState.SelectedSplineID));
                Methods.Internal.UserInterface.SplineControls.UpdateSplineToolbox();
                Methods.Internal.UserInterface.UpdateControls();
            }

        }

        #endregion

        #region Draw Tool Size Events
        bool AllowDrawBrushSizeChange = true;
        private void DrawToolSizeChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DrawToolSizeChanged();
        }
        private void DrawToolSizeChanged(bool wasSlider = false)
        {
            if (Instance != null)
            {
                if (DrawTileSizeNUD != null && DrawTileSizeSlider != null && AllowDrawBrushSizeChange)
                {
                    AllowDrawBrushSizeChange = false;
                    int size = (wasSlider ? (int)DrawTileSizeSlider.Value : (int)DrawTileSizeNUD.Value);
                    DrawTileSizeSlider.Value = size;
                    DrawTileSizeNUD.Value = size;
                    Methods.Editor.SolutionState.DrawBrushSize = size;
                    AllowDrawBrushSizeChange = true;
                }
            }
        }
        private void DrawToolSizeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DrawToolSizeChanged(true);
        }

        #endregion

        #region Collision Events
        public void ShowCollisionAEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.ShowCollisionA ^= true; }
        public void ShowCollisionBEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.ShowCollisionB ^= true; }
        private void UseNormalCollisionEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.CollisionPreset = 0; }
        private void UseInvertedCollisionEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.CollisionPreset = 1; }
        private void UseCustomCollisionEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.CollisionPreset = 2; }
        private void CollisionOpacitySliderValueChangedEvent(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Instance != null) Instance.RefreshCollisionColours(true);
        }
        #endregion

        #region Magnet Events

        private void ToggleMagnetToolEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.UseMagnetMode ^= true; }
        private void Magnet8x8Event(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.MagnetSize = 8; }
        private void Magnet16x16Event(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.MagnetSize = 16; }
        private void Magnet32x32Event(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.MagnetSize = 32; }
        private void Magnet64x64Event(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.MagnetSize = 64; }
        private void MagnetCustomEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.MagnetSize = -1; }
        private void CustomMagnetSizeAdjuster_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (HasFullyInitialized && AllowNUDUpdate)
            {
                Methods.Editor.SolutionState.CustomMagnetSize = CustomMagnetSizeAdjuster.Value.Value;
            }
        }

        private void EnableMagnetXAxisLockEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.UseMagnetXAxis ^= true; }
        private void EnableMagnetYAxisLockEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.UseMagnetYAxis ^= true; }

        #endregion

        #region Grid Events
        public void ToggleGridEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.ShowGrid ^= true; }
        private void SetGrid16x16Event(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.GridSize = 16; }
        private void SetGrid128x128Event(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.GridSize = 128; }
        private void SetGrid256x256Event(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.GridSize = 256; }
        private void SetGridCustomSizeEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.GridSize = -1; }

        private void CustomGridSizeAdjuster_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (HasFullyInitialized && AllowNUDUpdate)
            {
                Methods.Editor.SolutionState.GridCustomSize = CustomGridSizeAdjuster.Value.Value;
                Methods.Editor.SolutionState.GridSize = -1;
            }

        }
        #endregion

        #region Misc Events

        public void ReloadToolStripButton_Click(object sender, RoutedEventArgs e) { Methods.Internal.UserInterface.ReloadSpritesAndTextures(); }
        public void ToggleShowTileIDEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.ShowTileID ^= true; }
        private void FasterNudgeValueNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e) { if (FasterNudgeValueNUD.Value != null) { Methods.Editor.SolutionState.FasterNudgeAmount = FasterNudgeValueNUD.Value.Value; } }
        private void ShowFlippedTileHelperEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.ShowFlippedTileHelper ^= true; }
        public void EnableEncorePaletteEvent(object sender, RoutedEventArgs e) { Methods.Editor.SolutionState.UseEncoreColors ^= true; }
        private void RunSceneEvent(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.RunScene(); }

        #endregion

        #endregion

        #region Settings and Other Menu Events
        public void AboutScreenEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.AboutScreen(); }
        public void ImportObjectsToolStripMenuItem_Click(Window window = null) { Methods.ProgramLauncher.ImportObjectsToolStripMenuItem_Click(window); }
        public void ImportObjectsWithMegaList(Window window = null) { Methods.ProgramLauncher.ImportObjectsWithMegaList(window); }
        public void ImportSoundsEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ImportSoundsToolStripMenuItem_Click(sender, e); }
        public void ImportSoundsEvent(Window window = null) { Methods.ProgramLauncher.ImportSounds(window); }
        private void LayerManagerEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.LayerManager(sender, e); }
        private void ManiacINIEditorEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ManiacINIEditor(sender, e); }
        private void ChangePrimaryBackgroundColorEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ChangePrimaryBackgroundColor(sender, e); }
        private void ChangeSecondaryBackgroundColorEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ChangeSecondaryBackgroundColor(sender, e); }
        public void ObjectManagerEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ObjectManager(); }
        private void InGameOptionsMenuEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.InGameSettings(); }
        private void WikiLinkEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.WikiLink(); }
        public void OptionsMenuEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OptionsMenu(); }
        private void ControlsMenuEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ControlMenu(); }
        #endregion

        #region Game Running Events
        private void MoveThePlayerToHere(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.MoveThePlayerToHere(); }
        private void SetPlayerRespawnToHere(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.SetPlayerRespawnToHere(); }
        private void MoveCheckpoint(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.CheckpointSelected = true; }
        private void RemoveCheckpoint(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.UpdateCheckpoint(new System.Drawing.Point(0, 0), false); }
        private void AssetReset(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.AssetReset(); }
        private void RestartScene(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.RestartScene(); }
        private void TrackThePlayer(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.TrackthePlayer(sender, e); }
        private void UpdateInGameMenuItems(object sender, RoutedEventArgs e) { ManiacEditor.Methods.Runtime.GameHandler.UpdateRunSceneDropdown(); }
        #endregion

        #region Layer Toolbar Events
        private void LayerShowButton_Click(ToggleButton button, string desc)
        {
            if (button.IsChecked.Value)
            {
                button.IsChecked = false;
                button.ToolTip = "Show " + desc;
            }
            else
            {
                button.IsChecked = true;
                button.ToolTip = "Hide " + desc;
            }
        }
        private void ShowFGLow_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            toggle.IsChecked = !toggle.IsChecked.Value;
            LayerShowButton_Click(ShowFGLow, "Layer FG Low");
        }
        private void ShowFGHigh_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            toggle.IsChecked = !toggle.IsChecked.Value;
            LayerShowButton_Click(ShowFGHigh, "Layer FG High");
        }
        private void ShowFGHigher_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            toggle.IsChecked = !toggle.IsChecked.Value;
            LayerShowButton_Click(ShowFGHigher, "Layer FG Higher");
        }
        private void ShowFGLower_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            toggle.IsChecked = !toggle.IsChecked.Value;
            LayerShowButton_Click(ShowFGLower, "Layer FG Lower");
        }
        private void ShowEntities_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            toggle.IsChecked = !toggle.IsChecked.Value;
            LayerShowButton_Click(ShowEntities, "Entities");
        }
        private void ShowAnimations_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            toggle.IsChecked = !toggle.IsChecked.Value;
            LayerShowButton_Click(ShowAnimations, "Animations");
        }

        private void ShowAnimations_Checked(object sender, RoutedEventArgs e)
        {
            Methods.Editor.SolutionState.AllowAnimations = true;
        }

        private void ShowAnimations_Unchecked(object sender, RoutedEventArgs e)
        {
            Methods.Editor.SolutionState.AllowAnimations = false;
        }

        private void LayerEditButton_Click(EditLayerToggleButton button, MouseButton ClickType)
        {
            if (Methods.Editor.SolutionState.MultiLayerEditMode)
            {
                if (button == EditEntities) EditEntitiesMode();
                else if (ClickType == MouseButton.Left) LayerA();
                else if (ClickType == MouseButton.Right) LayerB();
            }
            else
            {
                if (ClickType == MouseButton.Left) Normal();
            }
            Methods.Internal.UserInterface.UpdateControls();


            void EditEntitiesMode()
            {
                Methods.Editor.EditorActions.Deselect(false);
                if (!button.IsCheckedN.Value)
                {
                    button.IsCheckedN = false;
                }
                else
                {

                    EditEntities.IsCheckedN = true;

                    EditFGLow.IsCheckedN = false;
                    EditFGHigh.IsCheckedN = false;
                    EditFGLower.IsCheckedN = false;
                    EditFGHigher.IsCheckedN = false;
                    EditFGLow.IsCheckedA = false;
                    EditFGHigh.IsCheckedA = false;
                    EditFGLower.IsCheckedA = false;
                    EditFGHigher.IsCheckedA = false;
                    EditFGLow.IsCheckedB = false;
                    EditFGHigh.IsCheckedB = false;
                    EditFGLower.IsCheckedB = false;
                    EditFGHigher.IsCheckedB = false;
                }

                foreach (var elb in ExtraLayerEditViewButtons.Values)
                {
                    elb.IsCheckedN = false;
                    elb.IsCheckedA = false;
                    elb.IsCheckedB = false;
                }

            }

            void Normal()
            {
                Methods.Editor.EditorActions.Deselect(false);
                if (!button.IsCheckedN.Value)
                {
                    button.IsCheckedN = false;
                }
                else
                {
                    EditFGLow.IsCheckedN = false;
                    EditFGHigh.IsCheckedN = false;
                    EditFGLower.IsCheckedN = false;
                    EditFGHigher.IsCheckedN = false;
                    EditEntities.IsCheckedN = false;
                    button.IsCheckedN = true;
                }

                foreach (var elb in ExtraLayerEditViewButtons.Values)
                {
                    if (elb != button) elb.IsCheckedN = false;
                }



            }

            void LayerA()
            {
                Methods.Editor.EditorActions.Deselect(false);
                if (!button.IsCheckedA.Value)
                {
                    button.IsCheckedA = false;
                }
                else
                {
                    EditFGLow.IsCheckedA = false;
                    EditFGHigh.IsCheckedA = false;
                    EditFGLower.IsCheckedA = false;
                    EditFGHigher.IsCheckedA = false;
                    EditEntities.IsCheckedN = false;
                    button.IsCheckedA = true;
                }

                foreach (var elb in ExtraLayerEditViewButtons.Values)
                {
                    if (elb != button) elb.IsCheckedA = false;
                }
            }
            void LayerB()
            {
                Methods.Editor.EditorActions.Deselect(false);
                if (!button.IsCheckedB.Value)
                {
                    button.IsCheckedB = false;
                }
                else
                {
                    EditFGLow.IsCheckedB = false;
                    EditFGHigh.IsCheckedB = false;
                    EditFGLower.IsCheckedB = false;
                    EditFGHigher.IsCheckedB = false;
                    EditEntities.IsCheckedN = false;
                    button.IsCheckedB = true;
                }

                foreach (var elb in ExtraLayerEditViewButtons.Values)
                {
                    if (elb != button) elb.IsCheckedB = false;
                }
            }
        }
        private void EditFGLow_Click(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditFGLow, MouseButton.Left);
        }
        private void EditFGLow_RightClick(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditFGLow, MouseButton.Right);
        }
        private void EditFGHigh_Click(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditFGHigh, MouseButton.Left);
        }
        private void EditFGHigh_RightClick(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditFGHigh, MouseButton.Right);
        }
        private void EditFGLower_Click(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditFGLower, MouseButton.Left);
        }
        private void EditFGLower_RightClick(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditFGLower, MouseButton.Right);
        }
        private void EditFGHigher_Click(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditFGHigher, MouseButton.Left);
        }
        private void EditFGHigher_RightClick(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditFGHigher, MouseButton.Right);
        }
        private void EditEntities_Click(object sender, RoutedEventArgs e)
        {
            EditLayerToggleButton toggle = sender as EditLayerToggleButton;
            LayerEditButton_Click(EditEntities, MouseButton.Left);
        }
        public void SetupLayerButtons()
        {
            TearDownExtraLayerButtons();
            IList<EditLayerToggleButton> _extraLayerEditButtons = new List<EditLayerToggleButton>(); //Used for Extra Layer Edit Buttons
            IList<EditLayerToggleButton> _extraLayerViewButtons = new List<EditLayerToggleButton>(); //Used for Extra Layer View Buttons

            //EDIT BUTTONS
            foreach (Classes.Scene.EditorLayer el in Methods.Editor.Solution.CurrentScene.OtherLayers)
            {
                EditLayerToggleButton tsb = new EditLayerToggleButton()
                {
                    Text = el.Name,
                    LayerName = "Edit" + el.Name
                };
                LayerToolbar.Items.Add(tsb);
                tsb.DualSelect = true;
                tsb.TextForeground = Methods.Internal.Theming.GetSCBResource("Maniac_ExtraEditLayer_LabelText");
                tsb.RightClick += AdHocLayerEdit_RightClick;
                tsb.IsLayerOptionsEnabled = true;

                tsb.Click += AdHocLayerEdit_Click;

                _extraLayerEditButtons.Add(tsb);
            }

            //EDIT BUTTONS SEPERATOR
            Separator tss = new Separator();
            LayerToolbar.Items.Add(tss);
            ExtraLayerSeperators.Add(tss);

            //VIEW BUTTONS
            foreach (Classes.Scene.EditorLayer el in Methods.Editor.Solution.CurrentScene.OtherLayers)
            {
                EditLayerToggleButton tsb = new EditLayerToggleButton()
                {
                    Text = el.Name,
                    LayerName = "Show" + el.Name.Replace(" ", "")
                };
                LayerToolbar.Items.Insert(LayerToolbar.Items.IndexOf(extraViewLayersSeperator), tsb);
                tsb.TextForeground = Methods.Internal.Theming.GetSCBResource("Maniac_ExtraViewLayer_LabelText");
                tsb.IsLayerOptionsEnabled = true;
                tsb.IsLayerControlsHidden = true;


                _extraLayerViewButtons.Add(tsb);
            }

            //EDIT + VIEW BUTTONS LIST
            for (int i = 0; i < _extraLayerViewButtons.Count; i++)
            {
                ExtraLayerEditViewButtons.Add(_extraLayerViewButtons[i], _extraLayerEditButtons[i]);
            }

            UpdateDualButtonsControlsForLayer(Methods.Editor.Solution.FGLow, ShowFGLow, EditFGLow);
            UpdateDualButtonsControlsForLayer(Methods.Editor.Solution.FGHigh, ShowFGHigh, EditFGHigh);
            UpdateDualButtonsControlsForLayer(Methods.Editor.Solution.FGLower, ShowFGLower, EditFGLower);
            UpdateDualButtonsControlsForLayer(Methods.Editor.Solution.FGHigher, ShowFGHigher, EditFGHigher);
        }
        public void TearDownExtraLayerButtons()
        {
            foreach (var elb in ExtraLayerEditViewButtons)
            {
                LayerToolbar.Items.Remove(elb.Key);
                elb.Value.Click -= AdHocLayerEdit_Click;
                elb.Value.RightClick -= AdHocLayerEdit_RightClick;
                LayerToolbar.Items.Remove(elb.Value);
            }
            ExtraLayerEditViewButtons.Clear();


            foreach (var els in ExtraLayerSeperators)
            {
                LayerToolbar.Items.Remove(els);
            }
            ExtraLayerSeperators.Clear();

        }
        /// <summary>
        /// Given a scene layer, configure the given visibiltiy and edit buttons which will control that layer.
        /// </summary>
        /// <param name="layer">The layer of the scene from which to extract a name.</param>
        /// <param name="visibilityButton">The button which controls the visibility of the layer.</param>
        /// <param name="editButton">The button which controls editing the layer.</param>
        private void UpdateDualButtonsControlsForLayer(Classes.Scene.EditorLayer layer, ToggleButton visibilityButton, EditLayerToggleButton editButton)
        {
            bool layerValid = layer != null;
            visibilityButton.IsChecked = layerValid;
            if (layerValid)
            {
                string name = layer.Name;
                visibilityButton.Content = name;
                editButton.Text = name.ToString();
            }
        }
        private void AdHocLayerEdit_RightClick(object sender, RoutedEventArgs e)
        {
            AdHocLayerEdit(sender, MouseButton.Right);
        }
        private void AdHocLayerEdit_Click(object sender, RoutedEventArgs e)
        {
            AdHocLayerEdit(sender, MouseButton.Left);
        }
        private void AdHocLayerEdit(object sender, MouseButton ClickType)
        {
            if (ClickType == MouseButton.Left && !Methods.Editor.SolutionState.MultiLayerEditMode) Normal();
            else if (ClickType == MouseButton.Left && Methods.Editor.SolutionState.MultiLayerEditMode) LayerA();
            else if (ClickType == MouseButton.Right && Methods.Editor.SolutionState.MultiLayerEditMode) LayerB();

            Methods.Internal.UserInterface.UpdateControls();

            void Normal()
            {
                EditLayerToggleButton tsb = sender as EditLayerToggleButton;
                Methods.Editor.EditorActions.Deselect(false);
                if (tsb.IsCheckedN.Value)
                {
                    if (!ManiacEditor.Properties.Settings.MySettings.KeepLayersVisible)
                    {
                        ShowFGLow.IsChecked = false;
                        ShowFGHigh.IsChecked = false;
                        ShowFGLower.IsChecked = false;
                        ShowFGHigher.IsChecked = false;
                    }
                    EditFGLow.ClearCheckedItems(3);
                    EditFGHigh.ClearCheckedItems(3);
                    EditFGLower.ClearCheckedItems(3);
                    EditFGHigher.ClearCheckedItems(3);
                    EditEntities.ClearCheckedItems(3);

                    foreach (var elb in ExtraLayerEditViewButtons)
                    {
                        if (elb.Value != tsb)
                        {
                            elb.Value.IsCheckedN = false;
                        }
                    }
                }
            }
            void LayerA()
            {
                EditLayerToggleButton tsb = sender as EditLayerToggleButton;
                Methods.Editor.EditorActions.Deselect(false);
                if (tsb.IsCheckedA.Value)
                {
                    if (!ManiacEditor.Properties.Settings.MySettings.KeepLayersVisible)
                    {
                        ShowFGLow.IsChecked = false;
                        ShowFGHigh.IsChecked = false;
                        ShowFGLower.IsChecked = false;
                        ShowFGHigher.IsChecked = false;
                    }
                    EditFGLow.ClearCheckedItems(1);
                    EditFGHigh.ClearCheckedItems(1);
                    EditFGLower.ClearCheckedItems(1);
                    EditFGHigher.ClearCheckedItems(1);
                    EditEntities.ClearCheckedItems(1);

                    foreach (var elb in ExtraLayerEditViewButtons)
                    {
                        if (elb.Value != tsb)
                        {
                            elb.Value.IsCheckedA = false;
                        }
                    }
                }
            }
            void LayerB()
            {
                EditLayerToggleButton tsb = sender as EditLayerToggleButton;
                Methods.Editor.EditorActions.Deselect(false);
                if (tsb.IsCheckedB.Value)
                {
                    if (!ManiacEditor.Properties.Settings.MySettings.KeepLayersVisible)
                    {
                        ShowFGLow.IsChecked = false;
                        ShowFGHigh.IsChecked = false;
                        ShowFGLower.IsChecked = false;
                        ShowFGHigher.IsChecked = false;
                    }
                    EditFGLow.ClearCheckedItems(2);
                    EditFGHigh.ClearCheckedItems(2);
                    EditFGLower.ClearCheckedItems(2);
                    EditFGHigher.ClearCheckedItems(2);
                    EditEntities.ClearCheckedItems(2);

                    foreach (var elb in ExtraLayerEditViewButtons)
                    {
                        if (elb.Value != tsb)
                        {
                            elb.Value.IsCheckedB = false;
                        }
                    }
                }
            }
        }
        #endregion

        #region Mod Config List Stuff
        public MenuItem CreateModConfigMenuItem(int i)
        {
            MenuItem newItem = new MenuItem()
            {
                Header = ManiacEditor.Properties.Settings.MySettings.ModLoaderConfigsNames[i],
                Tag = ManiacEditor.Properties.Settings.MySettings.ModLoaderConfigs[i]
            };
            newItem.Click += ModConfigItemClicked;
            if (newItem.Tag.ToString() == ManiacEditor.Properties.Settings.MySettings.LastModConfig) newItem.IsChecked = true;
            return newItem;
        }
        private void ModConfigItemClicked(object sender, RoutedEventArgs e)
        {
            var modConfig_CheckedItem = (sender as MenuItem);
            SelectConfigToolStripMenuItem_Click(modConfig_CheckedItem);
            ManiacEditor.Properties.Settings.MySettings.LastModConfig = modConfig_CheckedItem.Tag.ToString();
        }
        public void EditConfigsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager configManager = new ConfigManager();
            configManager.Owner = Window.GetWindow(this);
            configManager.ShowDialog();

            selectConfigToolStripMenuItem.Items.Clear();
            if (ManiacEditor.Properties.Settings.MySettings.ModLoaderConfigs == null) ManiacEditor.Properties.Settings.MySettings.ModLoaderConfigs = new StringCollection();
            for (int i = 0; i < ManiacEditor.Properties.Settings.MySettings.ModLoaderConfigs.Count; i++)
            {
                selectConfigToolStripMenuItem.Items.Add(CreateModConfigMenuItem(i));
            }
        }
        private void SelectConfigToolStripMenuItem_Click(MenuItem modConfig_CheckedItem)
        {
            var allItems = selectConfigToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allItems)
            {
                item.IsChecked = false;
            }
            modConfig_CheckedItem.IsChecked = true;

        }
        #endregion

        #region Custom Color Picker Events
        private void comboBox8_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            //Grid Default Color
            if (e.NewValue.Value != null)
            {
                Methods.Editor.SolutionState.GridColor = Extensions.Extensions.ColorConvertToDrawing(e.NewValue.Value);
            }
        }
        private void comboBox7_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            //Water Color
            if (e.NewValue.Value != null)
            {
                Methods.Editor.SolutionState.waterColor = Extensions.Extensions.ColorConvertToDrawing(e.NewValue.Value);
            }
        }
        private void comboBox6_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            //Collision Solid(Top Only) Color
            if (e.NewValue.Value != null && Instance != null)
            {
                Methods.Editor.SolutionState.CollisionTOColour = Extensions.Extensions.ColorConvertToDrawing(e.NewValue.Value);
                Instance.RefreshCollisionColours(true);
            }
        }
        private void comboBox5_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            //Collision Solid(LRD) Color
            if (e.NewValue.Value != null && Instance != null)
            {
                Methods.Editor.SolutionState.CollisionLRDColour = Extensions.Extensions.ColorConvertToDrawing(e.NewValue.Value);
                Instance.RefreshCollisionColours(true);
            }
        }
        private void comboBox4_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            //Collision Solid(All) Color
            if (e.NewValue.Value != null && Instance != null)
            {
                Methods.Editor.SolutionState.CollisionSAColour = Extensions.Extensions.ColorConvertToDrawing(e.NewValue.Value);
                Instance.RefreshCollisionColours(true);
            }
        }
        private void CollisionColorPickerClosed(object sender, RoutedEventArgs e)
        {
            Instance.ReloadSpecificTextures(sender, e);
            Instance.RefreshCollisionColours(true);
        }


        #endregion

        #region Generated Items


        #endregion

        #region UI Refresh

        public void UpdateGameRunningButton(bool enabled = true)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                RunSceneButton.IsEnabled = enabled;
                RunSceneDropDown.IsEnabled = enabled && RunSceneButton.IsEnabled;

                if (Methods.Runtime.GameHandler.GameRunning || System.Diagnostics.Process.GetProcessesByName("SonicMania").FirstOrDefault() != null)
                {
                    if (Methods.Runtime.GameHandler.GameRunning) RunSceneIcon.Fill = System.Windows.Media.Brushes.Blue;
                    else RunSceneIcon.Fill = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    RunSceneIcon.Fill = System.Windows.Media.Brushes.Gray;
                }
            }));

        }
        public void SetEditButtonsState(bool enabled)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                EditFGLow.IsEnabled = enabled && Methods.Editor.Solution.FGLow != null;
                EditFGHigh.IsEnabled = enabled && Methods.Editor.Solution.FGHigh != null;
                EditFGLower.IsEnabled = enabled && Methods.Editor.Solution.FGLower != null;
                EditFGHigher.IsEnabled = enabled && Methods.Editor.Solution.FGHigher != null;
                EditEntities.IsEnabled = enabled;

                EditFGLow.IsCheckedA = enabled && EditFGLow.IsCheckedA.Value;
                EditFGHigh.IsCheckedA = enabled && EditFGHigh.IsCheckedA.Value;
                EditFGLower.IsCheckedA = enabled && EditFGLower.IsCheckedA.Value;
                EditFGHigher.IsCheckedA = enabled && EditFGHigher.IsCheckedA.Value;

                EditFGLow.IsCheckedB = enabled && EditFGLow.IsCheckedB.Value;
                EditFGHigh.IsCheckedB = enabled && EditFGHigh.IsCheckedB.Value;
                EditFGLower.IsCheckedB = enabled && EditFGLower.IsCheckedB.Value;
                EditFGHigher.IsCheckedB = enabled && EditFGHigher.IsCheckedB.Value;

                foreach (var layerButtons in ExtraLayerEditViewButtons)
                {
                    layerButtons.Value.IsCheckedA = layerButtons.Value.IsCheckedA.Value && enabled;
                    layerButtons.Value.IsCheckedB = layerButtons.Value.IsCheckedB.Value && enabled;
                }

                EditEntities.IsCheckedN = enabled && EditEntities.IsCheckedN.Value;

                SetLayerEditButtonsState(enabled);

                MagnetMode.IsEnabled = enabled && ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit();
                MagnetMode.IsChecked = Methods.Editor.SolutionState.UseMagnetMode && ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit();
                MagnetModeSplitButton.IsEnabled = enabled && ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit();
                Methods.Editor.SolutionState.UseMagnetMode = ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit() && MagnetMode.IsChecked.Value;



                UndoButton.IsEnabled = enabled && Actions.UndoRedoModel.UndoStack.Count > 0;
                RedoButton.IsEnabled = enabled && Actions.UndoRedoModel.RedoStack.Count > 0;



                PointerToolButton.IsEnabled = enabled;
                SelectToolButton.IsEnabled = enabled && ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit();

                DrawToolButton.IsEnabled = enabled && ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit() || ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit();
                DrawToolDropdown.IsEnabled = enabled && ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit() || ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit();

                ChunksToolButton.IsEnabled = enabled && ManiacEditor.Methods.Editor.SolutionState.IsTilesEdit();

                SplineToolButton.IsEnabled = enabled && ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit();
                SplineToolDropdown.IsEnabled = enabled && ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit();

                SplineToolButton.IsChecked = SplineToolButton.IsChecked.Value && ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit();

                bool isAnyOtherToolChecked()
                {
                    bool isPointer = (bool)PointerToolButton.IsChecked.Value;
                    bool isSelect = (bool)SelectToolButton.IsChecked.Value;
                    bool isDraw = (bool)DrawToolButton.IsChecked.Value;
                    bool isSpline = (bool)SplineToolButton.IsChecked.Value;

                    if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit())
                    {
                        if (isDraw || isSpline)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (isDraw || isSelect)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }


                PointerToolButton.IsChecked = isAnyOtherToolChecked();
                ChunksToolButton.IsChecked = (bool)ChunksToolButton.IsChecked && !ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit();
                if (Instance.TilesToolbar != null)
                {
                    if (ChunksToolButton.IsChecked.Value) Instance.TilesToolbar.TabControl.SelectedIndex = 1;
                    else Instance.TilesToolbar.TabControl.SelectedIndex = 0;
                }

                ShowGridButton.IsEnabled = enabled && Methods.Editor.Solution.StageConfig != null;
                ShowCollisionAButton.IsEnabled = enabled && Methods.Editor.Solution.TileConfig != null;
                ShowCollisionBButton.IsEnabled = enabled && Methods.Editor.Solution.TileConfig != null;
                ShowTileIDButton.IsEnabled = enabled && Methods.Editor.Solution.StageConfig != null;
                EncorePaletteButton.IsEnabled = enabled && Methods.Editor.SolutionState.EncorePaletteExists;
                FlipAssistButton.IsEnabled = enabled;
            }));
        }
        private void SetLayerEditButtonsState(bool enabled)
        {
            if (!Methods.Editor.SolutionState.MultiLayerEditMode)
            {
                if (enabled && EditFGLow.IsCheckedN.Value) Methods.Editor.Solution.EditLayerA = Methods.Editor.Solution.FGLow;
                else if (enabled && EditFGHigh.IsCheckedN.Value) Methods.Editor.Solution.EditLayerA = Methods.Editor.Solution.FGHigh;
                else if (enabled && EditFGHigher.IsCheckedN.Value) Methods.Editor.Solution.EditLayerA = Methods.Editor.Solution.FGHigher;
                else if (enabled && EditFGLower.IsCheckedN.Value) Methods.Editor.Solution.EditLayerA = Methods.Editor.Solution.FGLower;
                else if (enabled && ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedN.Value))
                {
                    var selectedExtraLayerButton = ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedN.Value);
                    var editorLayer = Methods.Editor.Solution.CurrentScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Methods.Editor.Solution.EditLayerA = editorLayer;
                }
                else Methods.Editor.Solution.EditLayerA = null;
            }
            else
            {
                SetEditLayerA();
                SetEditLayerB();
            }

            void SetEditLayerA()
            {
                if (enabled && EditFGLow.IsCheckedA.Value) Methods.Editor.Solution.EditLayerA = Methods.Editor.Solution.FGLow;
                else if (enabled && EditFGHigh.IsCheckedA.Value) Methods.Editor.Solution.EditLayerA = Methods.Editor.Solution.FGHigh;
                else if (enabled && EditFGHigher.IsCheckedA.Value) Methods.Editor.Solution.EditLayerA = Methods.Editor.Solution.FGHigher;
                else if (enabled && EditFGLower.IsCheckedA.Value) Methods.Editor.Solution.EditLayerA = Methods.Editor.Solution.FGLower;
                else if (enabled && ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedA.Value))
                {

                    var selectedExtraLayerButton = ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedA.Value);
                    int index = ExtraLayerEditViewButtons.IndexOf(selectedExtraLayerButton);
                    var editorLayer = Methods.Editor.Solution.CurrentScene.OtherLayers.ElementAt(index);

                    Methods.Editor.Solution.EditLayerA = editorLayer;
                }
                else Methods.Editor.Solution.EditLayerA = null;
            }
            void SetEditLayerB()
            {
                if (enabled && EditFGLow.IsCheckedB.Value) Methods.Editor.Solution.EditLayerB = Methods.Editor.Solution.FGLow;
                else if (enabled && EditFGHigh.IsCheckedB.Value) Methods.Editor.Solution.EditLayerB = Methods.Editor.Solution.FGHigh;
                else if (enabled && EditFGHigher.IsCheckedB.Value) Methods.Editor.Solution.EditLayerB = Methods.Editor.Solution.FGHigher;
                else if (enabled && EditFGLower.IsCheckedB.Value) Methods.Editor.Solution.EditLayerB = Methods.Editor.Solution.FGLower;
                else if (enabled && ExtraLayerEditViewButtons.Any(elb => elb.Value.IsCheckedB.Value))
                {
                    var selectedExtraLayerButton = ExtraLayerEditViewButtons.Single(elb => elb.Value.IsCheckedB.Value);
                    var editorLayer = Methods.Editor.Solution.CurrentScene.OtherLayers.Single(el => el.Name.Equals(selectedExtraLayerButton.Value.Text));

                    Methods.Editor.Solution.EditLayerB = editorLayer;
                }
                else Methods.Editor.Solution.EditLayerB = null;
            }

        }
        public void SetSceneOnlyButtonsState(bool enabled)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                ShowFGHigh.IsEnabled = enabled && Methods.Editor.Solution.FGHigh != null;
                ShowFGLow.IsEnabled = enabled && Methods.Editor.Solution.FGLow != null;
                ShowFGHigher.IsEnabled = enabled && Methods.Editor.Solution.FGHigher != null;
                ShowFGLower.IsEnabled = enabled && Methods.Editor.Solution.FGLower != null;
                ShowEntities.IsEnabled = enabled;

                ShowGridToggleButton.IsEnabled = enabled;
                ShowGridButton.IsEnabled = enabled;
                CollisionSettingsDropdown.IsEnabled = enabled;
                OtherDropdown.IsEnabled = enabled;

                ReloadButton.IsEnabled = enabled;

                Save.IsEnabled = enabled;

                if (Properties.Settings.MyPerformance.ReduceZoom)
                {
                    ZoomInButton.IsEnabled = enabled && Methods.Editor.SolutionState.ZoomLevel < 5;
                    ZoomOutButton.IsEnabled = enabled && Methods.Editor.SolutionState.ZoomLevel > -2;
                }
                else
                {
                    ZoomInButton.IsEnabled = enabled && Methods.Editor.SolutionState.ZoomLevel < 5;
                    ZoomOutButton.IsEnabled = enabled && Methods.Editor.SolutionState.ZoomLevel > -5;
                }
            }));
            UpdateGameRunningButton(enabled);
        }
        public void UpdateTooltips()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                New.ToolTip = "New Scene" + KeyBindPraser("New", true);
                Open.ToolTip = "Open Scene" + KeyBindPraser("Open", true);
                Save.ToolTip = "Save Scene" + KeyBindPraser("_Save", true);
                RunSceneButton.ToolTip = "Run Scene" + KeyBindPraser("RunScene", true, true);
                ReloadButton.ToolTip = "Reload Tiles and Sprites" + KeyBindPraser("RefreshResources", true, true);
                PointerToolButton.ToolTip = "Pointer Tool" + KeyBindPraser("PointerTool", true);
                MagnetMode.ToolTip = "Magnet Mode" + KeyBindPraser("MagnetTool", true);
                ZoomInButton.ToolTip = "Zoom In (Ctrl + Wheel Up)";
                ZoomOutButton.ToolTip = "Zoom In (Ctrl + Wheel Down)";
                SelectToolButton.ToolTip = "Selection Tool" + KeyBindPraser("SelectTool", true);
                DrawToolButton.ToolTip = "Draw Tool" + KeyBindPraser("DrawTool", true);
                ShowCollisionAButton.ToolTip = "Show Collision Layer A" + KeyBindPraser("ShowPathA", true, true);
                ShowCollisionBButton.ToolTip = "Show Collision Layer B" + KeyBindPraser("ShowPathB", true, true);
                FlipAssistButton.ToolTip = "Show Flipped Tile Helper";
                ChunksToolButton.ToolTip = "Stamp Tool" + KeyBindPraser("StampTool", true);
                SplineToolButton.ToolTip = "Spline Tool" + KeyBindPraser("SplineTool", true);
                EncorePaletteButton.ToolTip = "Show Encore Colors";
                ShowTileIDButton.ToolTip = "Toggle Tile ID Visibility" + KeyBindPraser("ShowTileID", true, true);
                ShowGridButton.ToolTip = "Toggle Grid Visibility" + KeyBindPraser("ShowGrid", true, true);
            }));

        }
        public string KeyBindPraser(string keyRefrence, bool tooltip = false, bool nonRequiredBinding = false)
        {
            string nullString = (nonRequiredBinding ? "" : "N/A");
            if (nonRequiredBinding && tooltip) nullString = "None";
            List<string> keyBindList = new List<string>();
            List<string> keyBindModList = new List<string>();

            if (!Extensions.Extensions.KeyBindsSettingExists(keyRefrence)) return nullString;

            if (Properties.Settings.MyKeyBinds == null) return nullString;

            var keybindDict = Properties.Settings.MyKeyBinds.GetInput(keyRefrence) as List<string>;
            if (keybindDict != null)
            {
                keyBindList = keybindDict.Cast<string>().ToList();
            }
            else
            {
                return nullString;
            }

            if (keyBindList == null)
            {
                return nullString;
            }

            if (keyBindList.Count > 1)
            {
                string keyBindLister = "";
                foreach (string key in keyBindList)
                {
                    keyBindLister += String.Format("({0}) ", key);
                }
                if (tooltip) return String.Format(" ({0})", keyBindLister);
                else return keyBindLister;
            }
            else if ((keyBindList.Count == 1) && keyBindList[0] != "None")
            {
                if (tooltip) return String.Format(" ({0})", keyBindList[0]);
                else return keyBindList[0];
            }
            else
            {
                return nullString;
            }


        }

        #endregion

        #region Context Menu Click
        private void ContextMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = (sender as Button);
            if (!btn.ContextMenu.IsOpen)
            {
                //Open the Context menu when Button is clicked
                btn.ContextMenu.IsEnabled = true;
                btn.ContextMenu.PlacementTarget = (sender as Button);
                btn.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                btn.ContextMenu.IsOpen = true;

            }
        }
        #endregion


    }
}
