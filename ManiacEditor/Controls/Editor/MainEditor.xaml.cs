﻿using IWshRuntimeLibrary;
using ManiacEditor.Actions;
using ManiacEditor.Entity_Renders;
using ManiacEditor.Controls;
using Microsoft.Win32;
using RSDKv5;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Clipboard = System.Windows.Clipboard;
using Color = System.Drawing.Color;
using DataObject = System.Windows.DataObject;
using File = System.IO.File;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using Path = System.IO.Path;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using ManiacEditor.Controls.Global;
using ManiacEditor.Enums;
using ManiacEditor.EventHandlers;
using ManiacEditor.Extensions;
using System.Windows.Forms.Integration;
using ManiacEditor.Controls.Editor;


namespace ManiacEditor.Controls.Editor
{
    public partial class MainEditor : Window
    {
        #region Definitions

        public static ManiacEditor.Controls.Editor.MainEditor Instance { get; set; }

        #region Editor Collections
        public List<string> ObjectList = new List<string>(); //All Gameconfig + Stageconfig Object names (Unused)
        public List<string> userDefinedSpritePaths = new List<string>();
        public Dictionary<string, string> userDefinedEntityRenderSwaps = new Dictionary<string, string>();
        public System.ComponentModel.BindingList<TextBlock> SplineSelectedObjectSpawnList = new System.ComponentModel.BindingList<TextBlock>();
        #endregion

        #region Internal/Public/Vital Classes
        internal Classes.Scene.EditorBackground EditBackground;
        public Classes.Scene.EditorChunks Chunks;
        public ManiacEditor.Controls.TileManiac.CollisionEditor TileManiacInstance = new ManiacEditor.Controls.TileManiac.CollisionEditor();
        #endregion

        #region Controls
        public ManiacEditor.Controls.Editor_Toolbars.TilesToolbar TilesToolbar = null;
        public ManiacEditor.Controls.Editor_Toolbars.EntitiesToolbar EntitiesToolbar = null;
        public ManiacEditor.Controls.Editor_Elements.StartScreen StartScreen { get; set; }
        #endregion

        #endregion

        #region Init
        public MainEditor()
        {
            PreInitalization();
            InitializeComponent();
            PostInitalization();

        }
        private void PreInitalization()
        {
            ManiacEditor.Methods.ProgramBase.Log.InfoFormat("Setting Up the Map Editor...");
            Instance = this;
            InitalizeInstances();
        }
        private void PostInitalization()
        {
            RuntimeInitalization();
            InitalizeSettings();
            InitilizeControls();
            LaunchInialization();
        }

        #endregion

        #region Init (Phases)

        private void LaunchInialization()
        {
            if (ManiacEditor.Properties.Settings.MyDevSettings.UseAutoForcefulStartup) Methods.Solution.SolutionLoader.OpenSceneForceFully();
        }

        private void RuntimeInitalization()
        {
            this.Title = String.Format("Maniac Editor - Generations Edition {0}", Methods.ProgramBase.GetCasualVersion());

            this.Activated += new System.EventHandler(this.Editor_Activated);

            Methods.Internal.Theming.SetTheme();
            ElementHost.EnableModelessKeyboardInterop(this);
            System.Windows.Application.Current.MainWindow = this;

            Extensions.ExternalExtensions.AllocConsole();
            Extensions.ExternalExtensions.HideConsoleWindow();
        }
        private void InitilizeControls()
        {
            StartScreen = new Editor_Elements.StartScreen();

            ViewPanel.UpdateInstance(this);
            ViewPanel.InfoHUD.UpdateInstance(this);
            ViewPanel.SplitContainer.UpdateInstance(this);

            ViewPanel.SharpPanel.UpdateGraphicsPanelControls();
            ViewPanel.SharpPanel.InitalizeGraphicsPanel();

            EditorToolbar.ExtraLayerEditViewButtons = new Dictionary<Controls.Global.EditLayerToggleButton, Controls.Global.EditLayerToggleButton>();
            EditorToolbar.ExtraLayerSeperators = new List<Separator>();

            MenuBar.UpdateInstance(this);
            StartScreen.UpdateInstance(this);

            Methods.Internal.UserInterface.UpdateControls();
            Methods.Internal.UserInterface.Misc.UpdateStartScreen(true, true);

            EditorStatusBar.UpdateFilterButtonApperance();
            Methods.Solution.SolutionState.RefreshCollisionColours();
        }
        private void InitalizeInstances()
        {
            Methods.Internal.Theming.UpdateInstance(this);
            Methods.Internal.Settings.UpdateInstance(this);
            Methods.Solution.CurrentSolution.UpdateInstance(this);
            Global.Controls.RetroEDTileList.UpdateInstance(this);
            Classes.Prefrences.RecentsRefrenceState.UpdateInstance(this);
            Methods.Solution.SolutionState.UpdateInstance(this);
            Editor_Elements.Toolbar.UpdateInstance(this);
            Methods.Entities.EntityDrawing.UpdateInstance(this);
            Methods.Entities.SplineSpawning.UpdateInstance(this);
            Methods.Solution.SolutionPaths.UpdateInstance(this);
            Classes.Prefrences.SceneCurrentSettings.UpdateInstance(this);
            Methods.Internal.UserInterface.UpdateInstance(this);
            Methods.Internal.Controls.UpdateInstance(this);
            Methods.Solution.SolutionLoader.UpdateInstance(this);
            Methods.ProgramLauncher.UpdateInstance(this);
            Methods.Runtime.GameHandler.UpdateInstance(this);
            Methods.Solution.SolutionActions.UpdateInstance(this);
            Methods.Draw.GraphicsTileDrawing.UpdateInstance(this);
            Methods.Layers.TileFindReplace.UpdateInstance(this);
        }
        private void InitalizeSettings()
        {
            Classes.Prefrences.SceneHistoryStorage.Initilize(this);
            Classes.Prefrences.DataStateHistoryStorage.Initilize(this);
            Methods.Internal.Settings.TryLoadSettings();
        }

        #endregion

        #region Editor Events
        private void Editor_Activated(object sender, EventArgs e)
        {
            ViewPanel.SharpPanel.GraphicPanel.Focus();
            if (TileManiacInstance.HasConfigBeenModified)
            {
                Methods.Internal.UserInterface.ReloadSpritesAndTextures();
            }

        }
        private void Editor_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Methods.ProgramLauncher.ManiaPalConnector != null) Methods.ProgramLauncher.ManiaPalConnector.Kill();

            try
            {
                Methods.Runtime.GameHandler.GameRunning = false;
                var mySettings = Properties.Settings.MySettings;
                ManiacEditor.Properties.Settings.MySettings.IsMaximized = WindowState == System.Windows.WindowState.Maximized;
                Properties.Settings.SaveAllSettings();
            }
            catch (Exception ex)
            {
                Debug.Write("Failed to write settings: " + ex);
            }

            ViewPanel.SharpPanel.Dispose();
            ViewPanel.SharpPanel.Host.Child.Dispose();



        }

        private bool IsViewPanelFocused()
        {
            bool TilesToolbarFocused = false;
            bool EntitiesToolbarFocused = false;
            bool MenuBarHasFocus = MenuBar.IsFocused || MenuBar.IsMouseOver;
            bool ViewPanelFocused = ViewPanel.IsFocused || ViewPanel.IsMouseOver;
            bool AreWeFocused = this.IsFocused || this.IsMouseOver;

            if (TilesToolbar != null) TilesToolbarFocused = TilesToolbar.IsFocused || TilesToolbar.IsMouseOver;
            if (EntitiesToolbar != null) EntitiesToolbarFocused = EntitiesToolbar.IsFocused || EntitiesToolbar.IsMouseOver;

            return ((ViewPanelFocused || AreWeFocused) && !MenuBarHasFocus && !TilesToolbarFocused && !EntitiesToolbarFocused);
        }

        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            var e2 = KeyEventExts.ToWinforms(e);
            if (e2 != null && IsViewPanelFocused())
            {
                Methods.Internal.Controls.GraphicPanel_OnKeyDown(sender, e2);
            }

        }
        private void Editor_KeyUp(object sender, KeyEventArgs e)
        {
            var e2 = KeyEventExts.ToWinforms(e);
            if (e2 != null && IsViewPanelFocused())
            {
                Methods.Internal.Controls.GraphicPanel_OnKeyUp(sender, e2);
            }

        }
        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void Editor_Resize(object sender, SizeChangedEventArgs e)
        {

        }
        public void Editor_Run()
        {
            Show();
            Focus();
            ViewPanel.SharpPanel.GraphicPanel.Show();
            ViewPanel.SharpPanel.GraphicPanel.Run();

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
        private void SelectConfigToolStripMenuItem_Click(MenuItem modConfig_CheckedItem)
        {
            var allItems = EditorToolbar.selectConfigToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
            foreach (var item in allItems)
            {
                item.IsChecked = false;
            }
            modConfig_CheckedItem.IsChecked = true;

        }
        #endregion
    }
}
