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
using ManiacEditor.Controls.Base.Controls;

using ManiacEditor.Controls.Utility;
using ManiacEditor.Controls.Utility.Editor;
using ManiacEditor.Controls.Utility.Object_ID_Repair_Tool;
using ManiacEditor.Controls.Utility.Object_Manager;
using ManiacEditor.Controls.Utility.Editor.Dev;
using ManiacEditor.Controls.Utility.Editor.Configuration;
using ManiacEditor.Controls.Utility.Editor.Options;

using ManiacEditor.Extensions;



namespace ManiacEditor.Controls.Base.Elements
{
    /// <summary>
    /// Interaction logic for Toolbar.xaml
    /// </summary>
    public partial class Toolbar : UserControl
    {
        public Toolbar()
        {
            InitializeComponent();

            EditFGLower.Click += EditFGLower_Click;
            EditFGLow.Click += EditFGLow_Click;
            EditFGHigh.Click += EditFGHigh_Click;
            EditFGHigher.Click += EditFGHigher_Click;

            EditFGLower.RightClick += EditFGLower_RightClick;
            EditFGLow.RightClick += EditFGLow_RightClick;
            EditFGHigh.RightClick += EditFGHigh_RightClick;
            EditFGHigher.RightClick += EditFGHigher_RightClick;
        }

        #region Action Events (MenuItems, Clicks, etc.)
        #region File Events
        private void NewSceneEvent(object sender, RoutedEventArgs e) { ManiacEditor.Controls.Base.MainEditor.Instance.FileHandler.NewScene(); }
        public void OpenSceneEvent(object sender, RoutedEventArgs e) { ManiacEditor.Controls.Base.MainEditor.Instance.FileHandler.OpenScene(); }
        public void SaveSceneEvent(object sender, RoutedEventArgs e) { ManiacEditor.Controls.Base.MainEditor.Instance.FileHandler.Save(); }
        #endregion

        #region Animations DropDown (WIP)
        private void MovingPlatformsObjectsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Classes.Core.SolutionState.AllowMovingPlatformAnimations == false)
            {
                movingPlatformsObjectsToolStripMenuItem.IsChecked = true;
                Classes.Core.SolutionState.AllowMovingPlatformAnimations = true;
            }
            else
            {
                movingPlatformsObjectsToolStripMenuItem.IsChecked = false;
                Classes.Core.SolutionState.AllowMovingPlatformAnimations = false;
            }

        }

        private void SpriteFramesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Classes.Core.SolutionState.AllowSpriteAnimations == false)
            {
                spriteFramesToolStripMenuItem.IsChecked = true;
                Classes.Core.SolutionState.AllowSpriteAnimations = true;
            }
            else
            {
                spriteFramesToolStripMenuItem.IsChecked = false;
                Classes.Core.SolutionState.AllowSpriteAnimations = false;
            }
        }

        private void ParallaxAnimationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Classes.Core.SolutionState.ParallaxAnimationChecked == false)
            {
                parallaxAnimationMenuItem.IsChecked = true;
                Classes.Core.SolutionState.ParallaxAnimationChecked = true;
            }
            else
            {
                parallaxAnimationMenuItem.IsChecked = false;
                Classes.Core.SolutionState.ParallaxAnimationChecked = false;
            }
        }

        #endregion
        #region Spline Tool Events
        private void SplineShowLineCheckboxCheckChanged(object sender, RoutedEventArgs e)
        {
            Classes.Core.SolutionState.AdjustSplineGroupOptions(Classes.Core.SolutionState.SplineOption.ShowLines, SplineShowLineCheckbox.IsChecked.Value);
        }

        private void SplineShowPointsCheckboxCheckChanged(object sender, RoutedEventArgs e)
        {
            Classes.Core.SolutionState.AdjustSplineGroupOptions(Classes.Core.SolutionState.SplineOption.ShowPoints, SplineShowPointsCheckbox.IsChecked.Value);
        }

        private void SplineShowObjectsCheckboxCheckChanged(object sender, RoutedEventArgs e)
        {
            Classes.Core.SolutionState.AdjustSplineGroupOptions(Classes.Core.SolutionState.SplineOption.ShowObjects, SplineShowObjectsCheckbox.IsChecked.Value);
        }

        bool AllowSplineFreqeunceUpdate = true;
        bool AllowSplineUpdateEvent = true;

        private void SplineOptionsIDChangedEvent(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (ManiacEditor.Controls.Base.MainEditor.Instance.UI != null && SplinePointSeperationSlider != null && SplinePointSeperationNUD != null && SplineGroupID != null && AllowSplineUpdateEvent)
            {
                SelectedSplineIDChangedEvent(SplineGroupID.Value.Value);
            }
        }

        public void SelectedSplineIDChangedEvent(int value)
        {
            AllowSplineUpdateEvent = false;
            Classes.Core.SolutionState.AllowSplineOptionsUpdate = false;
            SplineGroupID.Value = value;
            Classes.Core.SolutionState.SelectedSplineID = value;
            SplineSpawnID.Value = value;
            ManiacEditor.Controls.Base.MainEditor.Instance.UI.UpdateSplineSettings(value);
            Classes.Core.SolutionState.AllowSplineOptionsUpdate = true;
            AllowSplineUpdateEvent = true;

        }

        private void SplinePointFrequenceChangedEvent(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!Classes.Core.SolutionState.AllowSplineOptionsUpdate) return;
            if (ManiacEditor.Controls.Base.MainEditor.Instance.UI != null && SplinePointSeperationNUD != null && SplinePointSeperationSlider != null && AllowSplineFreqeunceUpdate)
            {
                AllowSplineFreqeunceUpdate = false;
                int size = (int)SplinePointSeperationNUD.Value;
                SplinePointSeperationSlider.Value = size;
                Classes.Core.SolutionState.AdjustSplineGroupOptions(Classes.Core.SolutionState.SplineOption.Size, size);
                AllowSplineFreqeunceUpdate = true;
            }
        }

        private void SplinePointFrequenceChangedEvent(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!Classes.Core.SolutionState.AllowSplineOptionsUpdate) return;
            if (ManiacEditor.Controls.Base.MainEditor.Instance.UI != null && SplinePointSeperationSlider != null && SplinePointSeperationNUD != null && AllowSplineFreqeunceUpdate)
            {
                AllowSplineFreqeunceUpdate = false;
                int size = (int)SplinePointSeperationSlider.Value;
                SplinePointSeperationNUD.Value = size;
                Classes.Core.SolutionState.AdjustSplineGroupOptions(Classes.Core.SolutionState.SplineOption.Size, size);
                AllowSplineFreqeunceUpdate = true;
            }
        }

        private void SplineLineMode_Click(object sender, RoutedEventArgs e)
        {
            Classes.Core.SolutionState.AdjustSplineGroupOptions(Classes.Core.SolutionState.SplineOption.LineMode, SplineLineMode.IsChecked.Value);
        }

        private void SplineOvalMode_Click(object sender, RoutedEventArgs e)
        {
            Classes.Core.SolutionState.AdjustSplineGroupOptions(Classes.Core.SolutionState.SplineOption.OvalMode, SplineOvalMode.IsChecked.Value);
        }
        private void SplineSpawnRender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Classes.Core.Solution.Entities != null && Classes.Core.SolutionState.AllowSplineOptionsUpdate)
            {
                var selectedItem = SelectedSplineRender.SelectedItem as TextBlock;
                if (selectedItem.Tag == null) return;
                if (selectedItem.Tag is RSDKv5.SceneObject)
                {
                    var obj = selectedItem.Tag as RSDKv5.SceneObject;
                    int splineID = Classes.Core.SolutionState.SelectedSplineID;
                    Classes.Core.SolutionState.AdjustSplineGroupOptions(Classes.Core.SolutionState.SplineOption.SpawnObject, Classes.Core.Solution.Entities.GenerateEditorEntity(new RSDKv5.SceneEntity(obj, 0)));
                    ManiacEditor.Controls.Base.MainEditor.Instance.EntitiesToolbar?.UpdateEntityProperties(new List<RSDKv5.SceneEntity>() { Classes.Core.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate.Entity });

                    if (Classes.Core.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate != null)
                        SplineRenderObjectName.Content = Classes.Core.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate.Entity.Object.Name.Name;
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

        #endregion
        #region Draw Tool Options Events
        bool AllowDrawBrushSizeChange = true;
        private void DrawToolSizeChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DrawToolSizeChanged();
        }

        private void DrawToolSizeChanged(bool wasSlider = false)
        {
            if (ManiacEditor.Controls.Base.MainEditor.Instance != null)
            {
                if (ManiacEditor.Controls.Base.MainEditor.Instance.UI != null && DrawTileSizeNUD != null && DrawTileSizeSlider != null && AllowDrawBrushSizeChange)
                {
                    AllowDrawBrushSizeChange = false;
                    int size = (wasSlider ? (int)DrawTileSizeSlider.Value : (int)DrawTileSizeNUD.Value);
                    DrawTileSizeSlider.Value = size;
                    DrawTileSizeNUD.Value = size;
                    Classes.Core.SolutionState.DrawBrushSize = size;
                    AllowDrawBrushSizeChange = true;
                }
            }
        }

        private void DrawToolSizeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DrawToolSizeChanged(true);
        }

        #endregion
        private void ToggleMagnetToolEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.UseMagnetMode ^= true; }
        private void UndoEvent(object sender, RoutedEventArgs e) { ManiacEditor.Controls.Base.MainEditor.Instance.EditorUndo(); }
        private void RedoEvent(object sender, RoutedEventArgs e) { ManiacEditor.Controls.Base.MainEditor.Instance.EditorRedo(); }
        private void ZoomInEvent(object sender, RoutedEventArgs e) { ManiacEditor.Controls.Base.MainEditor.Instance.UIEvents.ZoomIn(sender, e); }
        private void ZoomOutEvent(object sender, RoutedEventArgs e) { ManiacEditor.Controls.Base.MainEditor.Instance.UIEvents.ZoomOut(sender, e); }
        private void ToggleSelectToolEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.SelectionMode(); }
        private void TogglePointerToolEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.PointerMode(); }
        private void ToggleDrawToolEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.DrawMode(); }
        private void ToggleInteractionToolEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.InteractionMode(); }
        private void ToggleSplineToolEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.SplineMode(); }
        private void ToggleChunksToolEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.ChunksMode(); }
        public void ReloadToolStripButton_Click(object sender, RoutedEventArgs e) { ManiacEditor.Controls.Base.MainEditor.Instance.UI.ReloadSpritesAndTextures(); }
        public void ToggleSlotIDEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.ShowTileID ^= true; }
        private void FasterNudgeValueNUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e) { if (FasterNudgeValueNUD.Value != null) { Classes.Core.SolutionState.FasterNudgeAmount = FasterNudgeValueNUD.Value.Value; } }
        public void ApplyEditEntitiesTransparencyEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.ApplyEditEntitiesTransparency ^= true; }
        public void ShowCollisionAEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.ShowCollisionA ^= true; }
        public void ShowCollisionBEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.ShowCollisionB ^= true; }
        private void ShowFlippedTileHelperEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.ShowFlippedTileHelper ^= true; }
        public void EnableEncorePaletteEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.UseEncoreColors ^= true; }
        private void RunSceneEvent(object sender, RoutedEventArgs e) { ManiacEditor.Methods.GameHandler.RunScene(); }
        private void UseNormalCollisionEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.CollisionPreset = 0; }
        private void UseInvertedCollisionEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.CollisionPreset = 1; }
        private void UseCustomCollisionEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.CollisionPreset = 2; }


        #region Collision Slider Events
        private void CollisionOpacitySliderValueChangedEvent(object sender, RoutedPropertyChangedEventArgs<double> e) { ManiacEditor.Controls.Base.MainEditor.Instance.UIEvents?.CollisionOpacitySliderValueChanged(sender, e); }
        #endregion

        #region Magnet Events

        private void Magnet8x8Event(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.MagnetSize = 8; }
        private void Magnet16x16Event(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.MagnetSize = 16; }
        private void Magnet32x32Event(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.MagnetSize = 32; }
        private void Magnet64x64Event(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.MagnetSize = 64; }
        private void MagnetCustomEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.MagnetSize = -1; }
        private void CustomMagnetSizeAdjuster_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Classes.Core.SolutionState.CustomMagnetSize = CustomMagnetSizeAdjuster.Value.Value;
        }

        private void EnableMagnetXAxisLockEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.UseMagnetXAxis ^= true; }
        private void EnableMagnetYAxisLockEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.UseMagnetYAxis ^= true; }

        #endregion

        #region Grid Events
        public void ToggleGridEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.ShowGrid ^= true; }
        private void SetGrid16x16Event(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.GridSize = 16; }
        private void SetGrid128x128Event(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.GridSize = 128; }
        private void SetGrid256x256Event(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.GridSize = 256; }
        private void SetGridCustomSizeEvent(object sender, RoutedEventArgs e) { Classes.Core.SolutionState.GridSize = -1; }

        private void CustomGridSizeAdjuster_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Classes.Core.SolutionState.GridCustomSize = CustomGridSizeAdjuster.Value.Value;
            Classes.Core.SolutionState.GridSize = -1;
        }
        #endregion

        #region Apps
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

        #region Folders
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
        private void MoveThePlayerToHere(object sender, RoutedEventArgs e) { ManiacEditor.Methods.GameHandler.MoveThePlayerToHere(); }
        private void SetPlayerRespawnToHere(object sender, RoutedEventArgs e) { ManiacEditor.Methods.GameHandler.SetPlayerRespawnToHere(); }
        private void MoveCheckpoint(object sender, RoutedEventArgs e) { ManiacEditor.Methods.GameHandler.CheckpointSelected = true; }
        private void RemoveCheckpoint(object sender, RoutedEventArgs e) { ManiacEditor.Methods.GameHandler.UpdateCheckpoint(new System.Drawing.Point(0, 0), false); }
        private void AssetReset(object sender, RoutedEventArgs e) { ManiacEditor.Methods.GameHandler.AssetReset(); }
        private void RestartScene(object sender, RoutedEventArgs e) { ManiacEditor.Methods.GameHandler.RestartScene(); }
        private void TrackThePlayer(object sender, RoutedEventArgs e) { ManiacEditor.Methods.GameHandler.TrackthePlayer(sender, e); }
        private void UpdateInGameMenuItems(object sender, RoutedEventArgs e) { ManiacEditor.Methods.GameHandler.UpdateRunSceneDropdown(); }
        #endregion

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
            LayerShowButton_Click(ShowEntities, "Classes.Edit.Scene.EditorSolution.Entities");
        }
        private void ShowAnimations_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            toggle.IsChecked = !toggle.IsChecked.Value;
            LayerShowButton_Click(ShowAnimations, "Animations");
        }

        private void ShowAnimations_Checked(object sender, RoutedEventArgs e)
        {
            Classes.Core.SolutionState.AllowAnimations = true;
        }

        private void ShowAnimations_Unchecked(object sender, RoutedEventArgs e)
        {
            Classes.Core.SolutionState.AllowAnimations = false;
        }

        private void LayerEditButton_Click(EditLayerToggleButton button, MouseButton ClickType)
        {


            if (Classes.Core.SolutionState.MultiLayerEditMode)
            {
                if (button == EditEntities) EditEntitiesMode();
                else if (ClickType == MouseButton.Left) LayerA();
                else if (ClickType == MouseButton.Right) LayerB();
            }
            else
            {
                if (ClickType == MouseButton.Left) Normal();
            }
            ManiacEditor.Controls.Base.MainEditor.Instance.UI.UpdateControls();


            void EditEntitiesMode()
            {
                ManiacEditor.Controls.Base.MainEditor.Instance.Deselect(false);
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

                foreach (var elb in ManiacEditor.Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Values)
                {
                    elb.IsCheckedN = false;
                    elb.IsCheckedA = false;
                    elb.IsCheckedB = false;
                }

            }

            void Normal()
            {
                ManiacEditor.Controls.Base.MainEditor.Instance.Deselect(false);
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

                foreach (var elb in ManiacEditor.Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Values)
                {
                    if (elb != button) elb.IsCheckedN = false;
                }



            }

            void LayerA()
            {
                ManiacEditor.Controls.Base.MainEditor.Instance.Deselect(false);
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

                foreach (var elb in ManiacEditor.Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Values)
                {
                    if (elb != button) elb.IsCheckedA = false;
                }
            }
            void LayerB()
            {
                ManiacEditor.Controls.Base.MainEditor.Instance.Deselect(false);
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

                foreach (var elb in ManiacEditor.Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Values)
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
            foreach (Classes.Core.Scene.Sets.EditorLayer el in Classes.Core.Solution.CurrentScene.OtherLayers)
            {
                EditLayerToggleButton tsb = new EditLayerToggleButton()
                {
                    Text = el.Name,
                    LayerName = "Edit" + el.Name
                };
                LayerToolbar.Items.Add(tsb);
                tsb.DualSelect = true;
                tsb.TextForeground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(System.Drawing.Color.LawnGreen.A, System.Drawing.Color.LawnGreen.R, System.Drawing.Color.LawnGreen.G, System.Drawing.Color.LawnGreen.B));
                tsb.RightClick += AdHocLayerEdit_RightClick;
                tsb.IsLayerOptionsEnabled = true;

                tsb.Click += AdHocLayerEdit_Click;

                _extraLayerEditButtons.Add(tsb);
            }

            //EDIT BUTTONS SEPERATOR
            Separator tss = new Separator();
            LayerToolbar.Items.Add(tss);
            ManiacEditor.Controls.Base.MainEditor.Instance.ExtraLayerSeperators.Add(tss);

            //VIEW BUTTONS
            foreach (Classes.Core.Scene.Sets.EditorLayer el in Classes.Core.Solution.CurrentScene.OtherLayers)
            {
                EditLayerToggleButton tsb = new EditLayerToggleButton()
                {
                    Text = el.Name,
                    LayerName = "Show" + el.Name.Replace(" ", "")
                };
                LayerToolbar.Items.Insert(LayerToolbar.Items.IndexOf(extraViewLayersSeperator), tsb);
                tsb.TextForeground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, System.Drawing.Color.FromArgb(0x33AD35).R, System.Drawing.Color.FromArgb(0x33AD35).G, System.Drawing.Color.FromArgb(0x33AD35).B));
                tsb.IsLayerOptionsEnabled = true;


                _extraLayerViewButtons.Add(tsb);
            }

            //EDIT + VIEW BUTTONS LIST
            for (int i = 0; i < _extraLayerViewButtons.Count; i++)
            {
                ManiacEditor.Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Add(_extraLayerViewButtons[i], _extraLayerEditButtons[i]);
            }

            UpdateDualButtonsControlsForLayer(Classes.Core.Solution.FGLow, ShowFGLow, EditFGLow);
            UpdateDualButtonsControlsForLayer(Classes.Core.Solution.FGHigh, ShowFGHigh, EditFGHigh);
            UpdateDualButtonsControlsForLayer(Classes.Core.Solution.FGLower, ShowFGLower, EditFGLower);
            UpdateDualButtonsControlsForLayer(Classes.Core.Solution.FGHigher, ShowFGHigher, EditFGHigher);
        }
        public void TearDownExtraLayerButtons()
        {
            foreach (var elb in ManiacEditor.Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons)
            {
                LayerToolbar.Items.Remove(elb.Key);
                elb.Value.Click -= AdHocLayerEdit_Click;
                elb.Value.RightClick -= AdHocLayerEdit_RightClick;
                LayerToolbar.Items.Remove(elb.Value);
            }
            ManiacEditor.Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons.Clear();


            foreach (var els in ManiacEditor.Controls.Base.MainEditor.Instance.ExtraLayerSeperators)
            {
                LayerToolbar.Items.Remove(els);
            }
            ManiacEditor.Controls.Base.MainEditor.Instance.ExtraLayerSeperators.Clear();

        }
        /// <summary>
        /// Given a scene layer, configure the given visibiltiy and edit buttons which will control that layer.
        /// </summary>
        /// <param name="layer">The layer of the scene from which to extract a name.</param>
        /// <param name="visibilityButton">The button which controls the visibility of the layer.</param>
        /// <param name="editButton">The button which controls editing the layer.</param>
        private void UpdateDualButtonsControlsForLayer(Classes.Core.Scene.Sets.EditorLayer layer, ToggleButton visibilityButton, EditLayerToggleButton editButton)
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
            if (ClickType == MouseButton.Left && !Classes.Core.SolutionState.MultiLayerEditMode) Normal();
            else if (ClickType == MouseButton.Left && Classes.Core.SolutionState.MultiLayerEditMode) LayerA();
            else if (ClickType == MouseButton.Right && Classes.Core.SolutionState.MultiLayerEditMode) LayerB();

            void Normal()
            {
                EditLayerToggleButton tsb = sender as EditLayerToggleButton;
                ManiacEditor.Controls.Base.MainEditor.Instance.Deselect(false);
                if (tsb.IsCheckedN.Value)
                {
                    if (!ManiacEditor.Core.Settings.MySettings.KeepLayersVisible)
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

                    foreach (var elb in ManiacEditor.Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons)
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
                ManiacEditor.Controls.Base.MainEditor.Instance.Deselect(false);
                if (tsb.IsCheckedA.Value)
                {
                    if (!ManiacEditor.Core.Settings.MySettings.KeepLayersVisible)
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

                    foreach (var elb in ManiacEditor.Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons)
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
                ManiacEditor.Controls.Base.MainEditor.Instance.Deselect(false);
                if (tsb.IsCheckedB.Value)
                {
                    if (!ManiacEditor.Core.Settings.MySettings.KeepLayersVisible)
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

                    foreach (var elb in ManiacEditor.Controls.Base.MainEditor.Instance.ExtraLayerEditViewButtons)
                    {
                        if (elb.Value != tsb)
                        {
                            elb.Value.IsCheckedB = false;
                        }
                    }
                }
            }

            ManiacEditor.Controls.Base.MainEditor.Instance.UI.UpdateControls();
        }
        #endregion

        #region Mod Config List Stuff
        public MenuItem CreateModConfigMenuItem(int i)
        {
            MenuItem newItem = new MenuItem()
            {
                Header = ManiacEditor.Core.Settings.MySettings.ModLoaderConfigsNames[i],
                Tag = ManiacEditor.Core.Settings.MySettings.ModLoaderConfigs[i]
            };
            newItem.Click += ModConfigItemClicked;
            if (newItem.Tag.ToString() == ManiacEditor.Core.Settings.MySettings.LastModConfig) newItem.IsChecked = true;
            return newItem;
        }
        private void ModConfigItemClicked(object sender, RoutedEventArgs e)
        {
            var modConfig_CheckedItem = (sender as MenuItem);
            SelectConfigToolStripMenuItem_Click(modConfig_CheckedItem);
            ManiacEditor.Core.Settings.MySettings.LastModConfig = modConfig_CheckedItem.Tag.ToString();
        }
        public void EditConfigsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager configManager = new ConfigManager();
            configManager.Owner = Window.GetWindow(this);
            configManager.ShowDialog();

            // TODO: Fix NullReferenceException on Settings.mySettings.modConfigs
            selectConfigToolStripMenuItem.Items.Clear();
            for (int i = 0; i < ManiacEditor.Core.Settings.MySettings.ModLoaderConfigs.Count; i++)
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
                Classes.Core.SolutionState.GridColor = Extensions.Extensions.ColorConvertToDrawing(e.NewValue.Value);
            }
        }

        private void comboBox7_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            //Water Color
            if (e.NewValue.Value != null)
            {
                Classes.Core.SolutionState.waterColor = Extensions.Extensions.ColorConvertToDrawing(e.NewValue.Value);
            }
        }

        private void comboBox6_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            //Collision Solid(Top Only) Color
            if (e.NewValue.Value != null)
            {
                Classes.Core.SolutionState.CollisionTOColour = Extensions.Extensions.ColorConvertToDrawing(e.NewValue.Value);
                ManiacEditor.Controls.Base.MainEditor.Instance.RefreshCollisionColours(true);
            }
        }

        private void comboBox5_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            //Collision Solid(LRD) Color
            if (e.NewValue.Value != null)
            {
                Classes.Core.SolutionState.CollisionLRDColour = Extensions.Extensions.ColorConvertToDrawing(e.NewValue.Value);
                ManiacEditor.Controls.Base.MainEditor.Instance.RefreshCollisionColours(true);
            }
        }

        private void comboBox4_DropDown(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            //Collision Solid(All) Color
            if (e.NewValue.Value != null)
            {
                Classes.Core.SolutionState.CollisionSAColour = Extensions.Extensions.ColorConvertToDrawing(e.NewValue.Value);
                ManiacEditor.Controls.Base.MainEditor.Instance.RefreshCollisionColours(true);
            }
        }

        private void CollisionColorPickerClosed(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Controls.Base.MainEditor.Instance.ReloadSpecificTextures(sender, e);
            ManiacEditor.Controls.Base.MainEditor.Instance.RefreshCollisionColours(true);
        }


        #endregion

        public void UpdateTooltips()
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
            InteractionToolButton.ToolTip = "Interaction Tool";
            ShowCollisionAButton.ToolTip = "Show Collision Layer A" + KeyBindPraser("ShowPathA", true, true);
            ShowCollisionBButton.ToolTip = "Show Collision Layer B" + KeyBindPraser("ShowPathB", true, true);
            FlipAssistButton.ToolTip = "Show Flipped Tile Helper";
            ChunksToolButton.ToolTip = "Stamp Tool" + KeyBindPraser("StampTool", true);
            SplineToolButton.ToolTip = "Spline Tool" + KeyBindPraser("SplineTool", true);
            EncorePaletteButton.ToolTip = "Show Encore Colors";
            ShowTileIDButton.ToolTip = "Toggle Tile ID Visibility" + KeyBindPraser("ShowTileID", true, true);
            ShowGridButton.ToolTip = "Toggle Grid Visibility" + KeyBindPraser("ShowGrid", true, true);
        }

        public string KeyBindPraser(string keyRefrence, bool tooltip = false, bool nonRequiredBinding = false)
        {
            string nullString = (nonRequiredBinding ? "" : "N/A");
            if (nonRequiredBinding && tooltip) nullString = "None";
            List<string> keyBindList = new List<string>();
            List<string> keyBindModList = new List<string>();

            if (!Extensions.Extensions.KeyBindsSettingExists(keyRefrence)) return nullString;

            if (Properties.KeyBinds.Default == null) return nullString;

            var keybindDict = Properties.KeyBinds.Default[keyRefrence] as StringCollection;
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
    }
}