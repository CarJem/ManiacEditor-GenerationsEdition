﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using ManiacEditor.Actions;
using RSDKv5;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using ManiacEditor.Interfaces;
using Microsoft.Scripting.Utils;
using Microsoft.Win32;
using ManiacEditor.Entity_Renders;
using IWshRuntimeLibrary;
using System.Drawing;
using File = System.IO.File;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Point = System.Drawing.Point;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Path = System.IO.Path;
using System.Windows.Controls.Primitives;
using Rectangle = System.Drawing.Rectangle;
using MenuItem = System.Windows.Controls.MenuItem;
using Clipboard = System.Windows.Clipboard;
using DataObject = System.Windows.DataObject;
using MessageBox = RSDKrU.MessageBox;


namespace ManiacEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class Editor : Window
	{

        #region Definitions

		//Editor Paths
		public string DataDirectory; //Used to get the current Data Directory
		public string MasterDataDirectory = Environment.CurrentDirectory + "\\Data"; //Used as a way of allowing mods to not have to lug all the files in their folder just to load in Maniac.
		public IList<string> ResourcePackList = new List<string>();
        public string LoadedDataPack = "";
		public string[] EncorePalette = new string[6]; //Used to store the location of the encore palletes

		// Extra Layer Buttons
		public IDictionary<EditLayerToggleButton, EditLayerToggleButton> ExtraLayerEditViewButtons;
		private IList<Separator> ExtraLayerSeperators; //Used for Adding Extra Seperators along side Extra Edit/View Layer Buttons

		// Editor Collections
		public List<string> ObjectList = new List<string>(); //All Gameconfig + Stageconfig Object names (Unused)
		public List<Bitmap> CollisionLayerA = new List<Bitmap>(); //Collection of Collision Type A for the Loaded Scene
		public List<Bitmap> CollisionLayerB = new List<Bitmap>(); //Collection of Collision Type B for the Loaded Scene
		public List<string> entityRenderingObjects = EditorEntityDrawing.GetSpecialRenderList(1); //Used to get the Render List for Objects
		public List<string> renderOnScreenExlusions = EditorEntityDrawing.GetSpecialRenderList(0); //Used to get the Always Render List for Objects
		public IList<MenuItem> RecentDataItemsMenu; //Used to get items for the Data Directory Toolstrip Area
		private IList<MenuItem> RecentDataItemsButton; //Used to get items for the Data Directory Button Toolstrip
		public IList<SceneEntity> playerObjectPosition = new List<SceneEntity> { }; //Used to store the scenes current playerObjectPositions
		public List<string> userDefinedSpritePaths = new List<string>();
		public Dictionary<string, string> userDefinedEntityRenderSwaps = new Dictionary<string, string>();

        //Undo + Redo
        public Stack<IAction> UndoStack = new Stack<IAction>(); //Undo Actions Stack
        public Stack<IAction> RedoStack = new Stack<IAction>(); //Redo Actions Stack

        //Editor Layers
        internal EditorLayer FGHigher => EditorScene?.HighDetails;
		internal EditorLayer FGHigh => EditorScene?.ForegroundHigh;
		internal EditorLayer FGLow => EditorScene?.ForegroundLow;
		internal EditorLayer FGLower => EditorScene?.LowDetails;
		internal EditorLayer ScratchLayer => EditorScene?.Scratch;
        public EditorLayer EditLayerA { get; set; }
        public EditorLayer EditLayerB { get; set; }

        //Scene Width + Height (For Drawing)
        internal int SceneWidth => (EditorScene != null ? EditorScene.Layers.Max(sl => sl.Width) * 16 : 0);
		internal int SceneHeight => (EditorScene != null ? EditorScene.Layers.Max(sl => sl.Height) * 16 : 0);



		//Clipboards
		public Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> TilesClipboard;
		public Dictionary<Point, ushort> FindReplaceClipboard;
		public Dictionary<Point, ushort> TilesClipboardEditable;
		public List<EditorEntity> entitiesClipboard;

		//Collision Colours
		public Color CollisionAllSolid = Color.FromArgb(255, 255, 255, 255);
		public Color CollisionTopOnlySolid = Color.FromArgb(255, 255, 255, 255);
		public Color CollisionLRDSolid = Color.FromArgb(255, 255, 255, 0);

        //Internal/Public/Vital Classes
        public EditorTiles EditorTiles;
		public EditorScene EditorScene;
		public StageConfig StageConfig;
		public GameConfig GameConfig;
		public EditorUIControl UIControl;
		public EditorEntities Entities;
		internal EditorBackground BackgroundDX;
		public TilesToolbar TilesToolbar = null;
		public EntitiesToolbar EntitiesToolbar = null;
		public EditorEntityDrawing EntityDrawing;
		public EditorUpdater Updater;
		public TileConfig TileConfig;
		public EditorInGame InGame;
		public StartScreen StartScreen;
		public EditorStateModel StateModel;
		public UIText DebugTextHUD = new UIText();
		public EditorChunk Chunks;
		public EditorFormsModel FormsModel;
		public EditorDiscordRP Discord;
		public EditorUIEvents UIEvents;
		public EditorPath Paths;
		public EditorFileHandler FileHandler;
		public EditorDataPacks DataPacks;
		public EditorFindReplace FindAndReplace;
        public EditorZoomModel ZoomModel;
        public EditorTheming Theming;
        public EditorSettings Settings;
        public EditorManiacINI ManiacINI;
        public EditorUI UI;
        public EditorUIModes UIModes;
        public EditorLaunch Launcher;
        public ProcessMemory GameMemory = new ProcessMemory(); //Allows us to write hex codes like cheats, etc.
        public System.Windows.Forms.Integration.WindowsFormsHost FormsHost;
        public TileManiac.MainWindow TileManiacInstance = new TileManiac.MainWindow();

		// Stuff Used for Command Line Tool to Fix Duplicate Object ID's
		#region DLL Import Stuff
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
		private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

		[DllImport("USER32.DLL")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		private enum ShowWindowEnum
		{
			Hide = 0,
			ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
			Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
			Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
			Restore = 9, ShowDefault = 10, ForceMinimized = 11
		};

		public static void ShowConsoleWindow()
		{
			var handle = GetConsoleWindow();

			if (handle == IntPtr.Zero)
			{
				AllocConsole();
			}
			else
			{
				ShowWindow(handle, SW_SHOW);
			}
		}

		public static void HideConsoleWindow()
		{
			var handle = GetConsoleWindow();

			ShowWindow(handle, SW_HIDE);
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AllocConsole();

		[DllImport("kernel32.dll")]
		public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AttachConsole(int dwProcessId);

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		const int SW_HIDE = 0;
		const int SW_SHOW = 5;
		#endregion

		#endregion
		#region Editor Initalizing Methods
		public Editor(string dataDir = "", string scenePath = "", string modPath = "", int levelID = 0, bool ShortcutLaunch = false, int shortcutLaunchMode = 0, bool isEncoreMode = false, int X = 0, int Y = 0, double _ZoomedLevel = 0.0, int MegaManiacInstanceID = -1)
		{
			SystemEvents.PowerModeChanged += CheckDeviceState;
            Theming = new EditorTheming(this);
            Settings = new EditorSettings(this);
            UIModes = new EditorUIModes(this);

            Theming.UseDarkTheme_WPF(ManiacEditor.Settings.MySettings.NightMode);
			InitializeComponent();

            System.Windows.Application.Current.MainWindow = this;

            try
            {
                InitilizeEditor();
            }
            catch (Exception ex)
            {
                Debug.Print("Couldn't Initilize Editor!" + ex.ToString());
                throw ex;
            }
            try
            {
                Discord.InitDiscord();
            }
            catch (Exception ex)
            {
                Debug.Print("Discord RP couldn't start! Exception Error:" + ex.ToString());
            }

			if (ManiacEditor.Settings.MyDevSettings.DevAutoStart) OpenSceneForceFully();

			if (ShortcutLaunch)
			{
				try
				{
					if (dataDir != "" && scenePath != "") OpenSceneForceFully(dataDir, scenePath, modPath, levelID, isEncoreMode, X, Y);

					else if (dataDir != "") OpenSceneForceFully(dataDir);
				}
				catch
				{
					Debug.Print("Couldn't Force Open Maniac Editor with the Specificied Arguments!");
				}
			}
		}
		public void InitilizeEditor()
		{
			FormsModel = new EditorFormsModel(this);

			this.FormsModel.vScrollBar1.Scroll += new System.Windows.Controls.Primitives.ScrollEventHandler(this.VScrollBar1_Scroll);
			this.FormsModel.vScrollBar1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.VScrollBar1_ValueChanged);
			this.FormsModel.vScrollBar1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.VScrollBar1_Entered);
			this.FormsModel.hScrollBar1.Scroll += new System.Windows.Controls.Primitives.ScrollEventHandler(this.HScrollBar1_Scroll);
			this.FormsModel.hScrollBar1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.HScrollBar1_ValueChanged);
			this.FormsModel.hScrollBar1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.HScrollBar1_Entered);
			this.Activated += new System.EventHandler(this.Editor_Activated);
			this.FormsModel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Editor_KeyDown);
			this.FormsModel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Editor_KeyUp);

			this.FormsModel.GraphicPanel.OnRender += new ManiacEditor.RenderEventHandler(this.GraphicPanel_OnRender);
			this.FormsModel.GraphicPanel.OnCreateDevice += new ManiacEditor.CreateDeviceEventHandler(this.OnResetDevice);
			this.FormsModel.GraphicPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.GraphicPanel_DragDrop);
			this.FormsModel.GraphicPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.GraphicPanel_DragEnter);
			this.FormsModel.GraphicPanel.DragOver += new System.Windows.Forms.DragEventHandler(this.GraphicPanel_DragOver);
			this.FormsModel.GraphicPanel.DragLeave += new System.EventHandler(this.GraphicPanel_DragLeave);
			this.FormsModel.GraphicPanel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GraphicPanel_OnKeyDown);
			this.FormsModel.GraphicPanel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GraphicPanel_OnKeyUp);
			this.FormsModel.GraphicPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_MouseClick);
			this.FormsModel.GraphicPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_OnMouseDown);
			this.FormsModel.GraphicPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_OnMouseMove);
			this.FormsModel.GraphicPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_OnMouseUp);
			this.FormsModel.GraphicPanel.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.GraphicPanel_MouseWheel);

			ExtraLayerEditViewButtons = new Dictionary<EditLayerToggleButton, EditLayerToggleButton>();
			ExtraLayerSeperators = new List<Separator>();


			RecentDataItemsMenu = new List<MenuItem>();
			RecentDataItemsButton = new List<MenuItem>();
			UIModes.MenuChar = UIModes.MenuCharS.ToCharArray();
			UIModes.MenuChar_Small = UIModes.MenuCharS_Small.ToCharArray();
			UIModes.LevelSelectChar = UIModes.LevelSelectCharS.ToCharArray();
			InGame = new EditorInGame(this);
			EntityDrawing = new EditorEntityDrawing(this);
            StateModel = new EditorStateModel(this);
			UIControl = new EditorUIControl(this);
			StartScreen = new StartScreen(this);
			Discord = new EditorDiscordRP(this);
			Updater = new EditorUpdater();
			UIEvents = new EditorUIEvents(this);
			Paths = new EditorPath(this);
			FileHandler = new EditorFileHandler(this);
			DataPacks = new EditorDataPacks(this);
			FindAndReplace = new EditorFindReplace(this);
            ZoomModel = new EditorZoomModel(this);
            ManiacINI = new EditorManiacINI(this);
            Launcher = new EditorLaunch(this);
            UI = new EditorUI(this);



            this.Title = String.Format("Maniac Editor - Generations Edition {0}", Updater.GetVersion());
			FormsModel.GraphicPanel.Width = SystemInformation.PrimaryMonitorSize.Width;
			FormsModel.GraphicPanel.Height = SystemInformation.PrimaryMonitorSize.Height;

			ViewPanelContextMenu.Foreground = (SolidColorBrush)FindResource("NormalText");
			ViewPanelContextMenu.Background = (SolidColorBrush)FindResource("NormalBackground");


			EditFGLower.Click += EditFGLower_Click;
			EditFGLow.Click += EditFGLow_Click;
			EditFGHigh.Click += EditFGHigh_Click;
			EditFGHigher.Click += EditFGHigher_Click;

			EditFGLower.RightClick += EditFGLower_RightClick;
			EditFGLow.RightClick += EditFGLow_RightClick;
			EditFGHigh.RightClick += EditFGHigh_RightClick;
			EditFGHigher.RightClick += EditFGHigher_RightClick;

			AllocConsole();
			HideConsoleWindow();
			RefreshCollisionColours();
            ZoomModel.SetViewSize();
            UI.UpdateControls();
			Settings.TryLoadSettings();
			UpdateStartScreen(true, true);
		}
		#endregion
		#region Boolean States
		public bool IsEditing()
		{
			return IsTilesEdit() || IsEntitiesEdit() || IsChunksEdit();
		}
		public bool IsSceneLoaded()
		{
			if (EditorScene != null)
				return true;
			else
				return false;
		}
		public bool IsTilesEdit()
		{
			return EditLayerA != null;
		}
		public bool IsChunksEdit()
		{
			return ChunksToolButton.IsChecked.Value && EditLayerA != null;
		}
		public bool IsEntitiesEdit()
		{
			return EditEntities.IsCheckedN.Value;
		}
		public bool IsSelected()
		{
			if (IsTilesEdit())
			{
				bool SelectedA = EditLayerA?.SelectedTiles.Count > 0 || EditLayerA?.TempSelectionTiles.Count > 0;
				bool SelectedB = EditLayerB?.SelectedTiles.Count > 0 || EditLayerB?.TempSelectionTiles.Count > 0;
				return SelectedA || SelectedB;
			}
			else if (IsEntitiesEdit())
			{
				return Entities.IsSelected();
			}
			return false;
		}
		public bool CtrlPressed()
		{
			return System.Windows.Forms.Control.ModifierKeys.HasFlag(System.Windows.Forms.Keys.Control);
		}
		public bool ShiftPressed()
		{
			return System.Windows.Forms.Control.ModifierKeys.HasFlag(System.Windows.Forms.Keys.Shift);
		}
		public bool CanWriteFile(string fullFilePath)
		{
			if (!File.Exists(fullFilePath)) return true;

			if (File.GetAttributes(fullFilePath).HasFlag(FileAttributes.ReadOnly))
			{
				ShowError($"The file '{fullFilePath}' is Read Only.", "File is Read Only.");
				return false;
			}

			var result = RSDKrU.MessageBox.Show($"The file '{fullFilePath}' already exists. Overwrite?", "Overwrite?",
										 MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes) return true;

			return false;
		}
        #endregion
		#region Common Editor Functions
		public void EditorPlaceTile(Point position, int tile, EditorLayer layer)
		{
			Dictionary<Point, ushort> tiles = new Dictionary<Point, ushort>
			{
				[new Point(0, 0)] = (ushort)tile
			};
			layer.PasteFromClipboard(position, tiles);
		}
		public void DeleteSelected()
		{
			EditLayerA?.DeleteSelected();
			EditLayerB?.DeleteSelected();
			UI.UpdateEditLayerActions();

			if (IsEntitiesEdit())
			{
				Entities.DeleteSelected();
				UpdateLastEntityAction();
			}
		}
		public void UpdateLastEntityAction()
		{
			if (Entities.LastAction != null)
			{
				RedoStack.Clear();
				UndoStack.Push(Entities.LastAction);
				Entities.LastAction = null;
                UI.UpdateControls();
			}

		}
		public void FlipEntities(FlipDirection direction)
		{
			Dictionary<EditorEntity, Point> initalPos = new Dictionary<EditorEntity, Point>();
			Dictionary<EditorEntity, Point> postPos = new Dictionary<EditorEntity, Point>();
			foreach (EditorEntity e in Entities.SelectedEntities)
			{
				initalPos.Add(e, new Point(e.PositionX, e.PositionY));
			}
			Entities.Flip(direction);
			EntitiesToolbar.UpdateCurrentEntityProperites();
			foreach (EditorEntity e in Entities.SelectedEntities)
			{
				postPos.Add(e, new Point(e.PositionX, e.PositionY));
			}
			IAction action = new ActionMultipleMoveEntities(initalPos, postPos);
			UndoStack.Push(action);
			RedoStack.Clear();

		}
        /// <summary>
        /// Deselects all tiles and entities
        /// </summary>
        /// <param name="updateControls">Whether to update associated on-screen controls</param>
        public void Deselect(bool updateControls = true)
        {
            if (IsEditing())
            {
                EditLayerA?.Deselect();
                EditLayerB?.Deselect();

                if (IsEntitiesEdit()) Entities.Deselect();
                UI.SetSelectOnlyButtonsState(false);
                if (updateControls)
                    UI.UpdateEditLayerActions();
            }
        }
        public void EditorUndo()
        {
            if (UndoStack.Count > 0)
            {
                if (IsTilesEdit())
                {
                    // Deselect to apply the changes
                    Deselect();
                }
                else if (IsEntitiesEdit())
                {
                    if (UndoStack.Peek() is ActionAddDeleteEntities)
                    {
                        // deselect only if delete/create
                        Deselect();
                    }
                }
                IAction act = UndoStack.Pop();
                act.Undo();
                RedoStack.Push(act.Redo());
                if (IsEntitiesEdit() && IsSelected())
                {
                    // We need to update the properties of the selected entity
                    EntitiesToolbar.UpdateCurrentEntityProperites();
                }
            }
            UI.UpdateControls();
        }
        public void EditorRedo()
        {
            if (RedoStack.Count > 0)
            {
                IAction act = RedoStack.Pop();
                act.Undo();
                UndoStack.Push(act.Redo());
                if (IsEntitiesEdit() && IsSelected())
                {
                    // We need to update the properties of the selected entity
                    EntitiesToolbar.UpdateCurrentEntityProperites();
                }
            }
            UI.UpdateControls();
        }
        public void CopyTilesToClipboard(bool doNotUseWindowsClipboard = false)
        {
            bool hasMultipleValidLayers = EditLayerA != null && EditLayerB != null;
            if (!hasMultipleValidLayers)
            {
                Dictionary<Point, ushort> copyDataA = EditLayerA?.CopyToClipboard();
                Dictionary<Point, ushort> copyDataB = EditLayerB?.CopyToClipboard();
                Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> copyData = new Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>>(copyDataA, copyDataB);

                // Make a DataObject for the copied data and send it to the Windows clipboard for cross-instance copying
                if (!doNotUseWindowsClipboard)
                    Clipboard.SetDataObject(new DataObject("ManiacTiles", copyData), true);

                // Also copy to Maniac's clipboard in case it gets overwritten elsewhere
                TilesClipboard = copyData;
            }
            else if (hasMultipleValidLayers && UIModes.MultiLayerEditMode)
            {
                Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> copyData = EditorLayer.CopyMultiSelectionToClipboard(EditLayerA, EditLayerB);

                // Make a DataObject for the copied data and send it to the Windows clipboard for cross-instance copying
                if (!doNotUseWindowsClipboard)
                    Clipboard.SetDataObject(new DataObject("ManiacTiles", copyData), true);

                // Also copy to Maniac's clipboard in case it gets overwritten elsewhere
                TilesClipboard = copyData;
            }


        }
        public void CopyEntitiesToClipboard()
        {
            if (EntitiesToolbar.IsFocused == false)
            {
                // Windows Clipboard mode (WPF Current Breaks this Apparently)
                /*
				if (Settings.mySettings.EnableWindowsClipboard && !Settings.mySettings.ProhibitEntityUseOnExternalClipboard)
				{
					// Clone the entities and stow them here
					List<EditorEntity> copyData = entities.CopyToClipboard();

					// Prepare each Entity for the copy to release unnecessary data
					foreach (EditorEntity entity in copyData)
						entity.PrepareForExternalCopy();

					// Make a DataObject for the data and send it to the Windows clipboard for cross-instance copying
					Clipboard.SetDataObject(new DataObject("ManiacEntities", copyData), true);
				}*/

                // Local Clipboard mode
                {
                    // Clone the entities and stow them here
                    List<EditorEntity> copyData = Entities.CopyToClipboard();

                    // Send to Maniac's clipboard
                    entitiesClipboard = copyData;
                }
            }
        }
        public void MoveEntityOrTiles(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            int x = 0, y = 0;
            int modifier = (IsChunksEdit() ? 8 : 1);
            if (UIModes.UseMagnetMode)
            {
                switch (e.KeyData)
                {
                    case Keys.Up: y = (UIModes.UseMagnetYAxis ? -UIModes.MagnetSize : -1); break;
                    case Keys.Down: y = (UIModes.UseMagnetYAxis ? UIModes.MagnetSize : 1); break;
                    case Keys.Left: x = (UIModes.UseMagnetXAxis ? -UIModes.MagnetSize : -1); break;
                    case Keys.Right: x = (UIModes.UseMagnetXAxis ? UIModes.MagnetSize : 1); break;
                }
            }
            if (UIModes.EnableFasterNudge)
            {
                if (UIModes.UseMagnetMode)
                {
                    switch (e.KeyData)
                    {
                        case Keys.Up: y = (UIModes.UseMagnetYAxis ? -UIModes.MagnetSize * ManiacEditor.Settings.MyDefaults.FasterNudgeValue : -1 - ManiacEditor.Settings.MyDefaults.FasterNudgeValue); break;
                        case Keys.Down: y = (UIModes.UseMagnetYAxis ? UIModes.MagnetSize * ManiacEditor.Settings.MyDefaults.FasterNudgeValue : 1 + ManiacEditor.Settings.MyDefaults.FasterNudgeValue); break;
                        case Keys.Left: x = (UIModes.UseMagnetXAxis ? -UIModes.MagnetSize * ManiacEditor.Settings.MyDefaults.FasterNudgeValue : -1 - ManiacEditor.Settings.MyDefaults.FasterNudgeValue); break;
                        case Keys.Right: x = (UIModes.UseMagnetXAxis ? UIModes.MagnetSize * ManiacEditor.Settings.MyDefaults.FasterNudgeValue : 1 + ManiacEditor.Settings.MyDefaults.FasterNudgeValue); break;
                    }
                }
                else
                {
                    switch (e.KeyData)
                    {
                        case Keys.Up: y = (-1 - ManiacEditor.Settings.MyDefaults.FasterNudgeValue) * modifier; break;
                        case Keys.Down: y = (1 + ManiacEditor.Settings.MyDefaults.FasterNudgeValue) * modifier; break;
                        case Keys.Left: x = (-1 - ManiacEditor.Settings.MyDefaults.FasterNudgeValue) * modifier; break;
                        case Keys.Right: x = (1 + ManiacEditor.Settings.MyDefaults.FasterNudgeValue) * modifier; break;
                    }
                }

            }
            if (UIModes.UseMagnetMode == false && UIModes.EnableFasterNudge == false)
            {
                switch (e.KeyData)
                {
                    case Keys.Up: y = -1 * modifier; break;
                    case Keys.Down: y = 1 * modifier; break;
                    case Keys.Left: x = -1 * modifier; break;
                    case Keys.Right: x = 1 * modifier; break;
                }

            }
            EditLayerA?.MoveSelectedQuonta(new Point(x, y));
            EditLayerB?.MoveSelectedQuonta(new Point(x, y));

            UI.UpdateEditLayerActions();

            if (IsEntitiesEdit())
            {
                if (UIModes.UseMagnetMode)
                {
                    int xE = Entities.SelectedEntities[0].Entity.Position.X.High;
                    int yE = Entities.SelectedEntities[0].Entity.Position.Y.High;

                    if (xE % UIModes.MagnetSize != 0 && UIModes.UseMagnetXAxis)
                    {
                        int offsetX = x % UIModes.MagnetSize;
                        x -= offsetX;
                    }
                    if (yE % UIModes.MagnetSize != 0 && UIModes.UseMagnetYAxis)
                    {
                        int offsetY = y % UIModes.MagnetSize;
                        y -= offsetY;
                    }
                }


                Entities.MoveSelected(new Point(0, 0), new Point(x, y), false);
                EntitiesToolbar.UpdateCurrentEntityProperites();

                // Try to merge with last move
                if (UndoStack.Count > 0 && UndoStack.Peek() is ActionMoveEntities && (UndoStack.Peek() as ActionMoveEntities).UpdateFromKey(Entities.SelectedEntities.ToList(), new Point(x, y))) { }
                else
                {
                    UndoStack.Push(new ActionMoveEntities(Entities.SelectedEntities.ToList(), new Point(x, y), true));
                    RedoStack.Clear();
                    UI.UpdateControls();
                }
            }
        }
        public void CreateShortcut(string dataDir, string scenePath = "", string modPath = "", int X = 0, int Y = 0, bool isEncoreMode = false, int LevelSlotNum = -1, double ZoomedLevel = 0.0)
        {
            object shDesktop = (object)"Desktop";
            WshShell shell = new WshShell();
            string shortcutAddress = "";
            if (scenePath != "")
            {
                shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\" + "Scene Link" + " - Maniac.lnk";
            }
            else
            {
                shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\" + "Data Folder Link" + " - Maniac.lnk";
            }
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);

            string targetAddress = "\"" + Environment.CurrentDirectory + @"\ManiacEditor.exe" + "\"";
            string launchArguments = "";
            if (scenePath != "")
            {
                launchArguments = (dataDir != "" ? "DataDir=" + "\"" + dataDir + "\" " : "") + (scenePath != "" ? "ScenePath=" + "\"" + scenePath + "\" " : "") + (modPath != "" ? "ModPath=" + "\"" + modPath + "\" " : "") + (LevelSlotNum != -1 ? "LevelID=" + LevelSlotNum.ToString() + " " : "") + (isEncoreMode == true ? "EncoreMode=TRUE " : "") + (X != 0 ? "X=" + X.ToString() + " " : "") + (Y != 0 ? "Y=" + Y.ToString() + " " : "") + (ZoomedLevel != 0 ? "ZoomedLevel=" + ZoomedLevel.ToString() + " " : "");
            }
            else
            {
                launchArguments = (dataDir != "" ? "DataDir=" + "\"" + dataDir + "\" " : "");
            }

            shortcut.TargetPath = targetAddress;
            shortcut.Arguments = launchArguments;
            shortcut.WorkingDirectory = Environment.CurrentDirectory;
            shortcut.Save();
        }
        public void ShowError(string message, string title = "Error!")
        {
            RSDKrU.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public void GoToPosition(int x, int y, bool CenterCoords = true, bool ShortcutClear = false)
        {
            if (CenterCoords)
            {
                Rectangle r = FormsModel.GraphicPanel.GetScreen();
                int x2 = (int)(r.Width * StateModel.Zoom);
                int y2 = (int)(r.Height * StateModel.Zoom);

                int ResultX = (int)(x * StateModel.Zoom) - x2 / 2;
                int ResultY = (int)(y * StateModel.Zoom) - y2 / 2;

                if ((ResultX <= 0)) ResultX = 0;
                if ((ResultY <= 0)) ResultY = 0;


                StateModel.ShiftX = ResultX;
                StateModel.ShiftY = ResultY;
            }
            else
            {
                int ResultX = (int)(x * StateModel.Zoom);
                int ResultY = (int)(y * StateModel.Zoom);

                if ((ResultX <= 0)) ResultX = 0;
                if ((ResultY <= 0)) ResultY = 0;

                StateModel.ShiftX = ResultX;
                StateModel.ShiftY = ResultY;
            }


            if (ShortcutClear)
            {
                UIModes.ForceWarp = false;
                UIModes.TempWarpCoords = new Point(0, 0);
                UIModes.ShortcutHasZoom = false;
                UIModes.ShortcutZoomValue = 0.0;
            }

        }
        public void UnloadScene()
        {
            EditorScene?.Dispose();
            EditorScene = null;
            StageConfig = null;
            _levelIDLabel.Content = "Level ID: NULL";
            UIModes.LevelID = -1;
            UIModes.EncorePaletteExists = false;
            UIModes.EncoreSetupType = 0;
            playerObjectPosition = new List<SceneEntity> { };
            UIModes.INILayerNameHigher = "";
            UIModes.INILayerNameLower = "";
            ManiacINI.ClearSettings();
            userDefinedEntityRenderSwaps = new Dictionary<string, string>();
            userDefinedSpritePaths = new List<string>();
            EncorePaletteButton.IsChecked = false;
            Paths.UnloadScene();

            if (EditorTiles != null) EditorTiles.Dispose();
            EditorTiles = null;

            TearDownExtraLayerButtons();

            Background = null;

            Chunks = null;


            //foreach (EditorEntity entity in entitiesClipboard)
            //   entity.PrepareForExternalCopy();

            // Clear local clipboards
            TilesClipboard = null;
            entitiesClipboard = null;

            Entities = null;

            StateModel.Zoom = 1;
            StateModel.ZoomLevel = 0;

            UndoStack.Clear();
            RedoStack.Clear();

            EditFGLow.ClearCheckedItems();
            EditFGHigh.ClearCheckedItems();
            EditFGLower.ClearCheckedItems();
            EditFGHigher.ClearCheckedItems();
            EditEntities.ClearCheckedItems();

            ZoomModel.SetViewSize();

            UI.UpdateControls();

            // clear memory a little more aggressively 
            EntityDrawing.ReleaseResources();
            GC.Collect();

            CollisionLayerA.Clear();
            CollisionLayerB.Clear();
            TileConfig = null;

            UIModes.MenuChar = UIModes.MenuCharS.ToCharArray();
            UIModes.MenuChar_Small = UIModes.MenuCharS_Small.ToCharArray();
            UIModes.LevelSelectChar = UIModes.LevelSelectCharS.ToCharArray();

            UpdateStartScreen(true);
        }
        #endregion
        #region GameConfig/Data Folders
        public string GetDataDirectory()
		{
			using (var folderBrowserDialog = new FolderSelectDialog())
			{
				folderBrowserDialog.Title = "Select Data Folder";

				if (!folderBrowserDialog.ShowDialog())
					return null;

				return folderBrowserDialog.FileName;
			}
		}
		private MenuItem CreateDataDirectoryMenuLink(string target)
		{
			MenuItem newItem = new MenuItem();
			newItem.Header = target;
			newItem.Tag = target;
			newItem.Click += RecentDataDirectoryClicked;
			return newItem;
		}
		public bool SetGameConfig() { return Paths.SetGameConfig(); }
		public bool IsDataDirectoryValid(string directoryToCheck) { return Paths.IsDataDirectoryValid(directoryToCheck); }
		public void RecentDataDirectoryClicked(object sender, RoutedEventArgs e, String dataDirectory)
		{
			var dataDirectories = ManiacEditor.Settings.MySettings.DataDirectories;
            ManiacEditor.Settings.MyDefaults.SonicManiaPath = InGame.GamePath;
			if (IsDataDirectoryValid(dataDirectory))
			{
				ResetDataDirectoryToAndResetScene(dataDirectory);
			}
			else
			{
				MessageBox.Show($"The specified Data Directory {dataDirectory} is not valid.",
								"Invalid Data Directory!",
								MessageBoxButton.OK,
								MessageBoxImage.Error);
				dataDirectories.Remove(dataDirectory);
				RefreshDataDirectories(dataDirectories);

			}
            ManiacEditor.Settings.MySettings.Save();
		}
		public void ResetDataDirectoryToAndResetScene(string newDataDirectory, bool forceBrowse = false, bool forceSceneSelect = false)
		{
			if (FileHandler.AllowSceneUnloading() != true) return;
			UnloadScene();
            Settings.UseDefaultPrefrences();
			DataDirectory = newDataDirectory;
			AddRecentDataFolder(newDataDirectory);
			bool goodGameConfig = SetGameConfig();
			if (goodGameConfig == true)
			{
				if (forceBrowse) OpenScene(true);
				else if (forceSceneSelect) OpenScene(false);
				else OpenScene();

			}


		}
		public void RecentDataDirectoryClicked(object sender, RoutedEventArgs e)
		{
			var menuItem = sender as MenuItem;
			string dataDirectory = menuItem.Tag.ToString();
			var dataDirectories = ManiacEditor.Settings.MySettings.DataDirectories;
            ManiacEditor.Settings.MyDefaults.SonicManiaPath = InGame.GamePath;
			if (IsDataDirectoryValid(dataDirectory))
			{
				ResetDataDirectoryToAndResetScene(dataDirectory);
			}
			else
			{
				RSDKrU.MessageBox.Show($"The specified Data Directory {dataDirectory} is not valid.",
								"Invalid Data Directory!",
								MessageBoxButton.OK,
								MessageBoxImage.Error);
				dataDirectories.Remove(dataDirectory);
				RefreshDataDirectories(dataDirectories);

			}
            ManiacEditor.Settings.MySettings.Save();
		}

		/// <summary>
		/// Refreshes the Data directories displayed in the recent list under the File menu.
		/// </summary>
		/// <param name="settings">The settings file containing the </param>
		public void RefreshDataDirectories(StringCollection recentDataDirectories)
		{
			if (ManiacEditor.Settings.MySettings.DataDirectories?.Count > 0)
			{
                recentDataDirectoriesToolStripMenuItem.Visibility = Visibility.Collapsed;
                noRecentDataDirectoriesToolStripMenuItem.Visibility = Visibility.Collapsed;
                CleanUpRecentList();

				var startRecentItems = fileToolStripMenuItem.Items.IndexOf(recentDataDirectoriesToolStripMenuItem);
				var startRecentItemsButton = RecentDataDirectories_DropDown.Items.IndexOf(noRecentDataDirectoriesToolStripMenuItem);

				foreach (var dataDirectory in recentDataDirectories)
				{
                    RecentDataItemsMenu.Add(CreateDataDirectoryMenuLink(dataDirectory));
                    RecentDataItemsButton.Add(CreateDataDirectoryMenuLink(dataDirectory));

				}


				foreach (var menuItem in RecentDataItemsMenu.Reverse())
				{
                    fileToolStripMenuItem.Items.Insert(startRecentItems, menuItem);
				}

				foreach (var menuItem in RecentDataItemsButton.Reverse())
				{
                    RecentDataDirectories_DropDown.Items.Insert(startRecentItemsButton, menuItem);
				}
			}
			else
			{
                recentDataDirectoriesToolStripMenuItem.Visibility = Visibility.Visible;
                noRecentDataDirectoriesToolStripMenuItem.Visibility = Visibility.Visible;
			}



		}
		public void UpdateDataFolderLabel(object sender, RoutedEventArgs e)
		{
			string dataFolderTag_Normal = "Data Directory: {0}";

			_baseDataDirectoryLabel.Tag = dataFolderTag_Normal;
			UpdateDataFolderLabel();
			UIModes.ShowingDataDirectory = true;
		}
		private void UpdateDataFolderLabel(string dataDirectory = null)
		{
			if (dataDirectory != null) _baseDataDirectoryLabel.Content = string.Format(_baseDataDirectoryLabel.Tag.ToString(), dataDirectory);
			else _baseDataDirectoryLabel.Content = string.Format(_baseDataDirectoryLabel.Tag.ToString(), DataDirectory);
		}
		/// <summary>
		/// Removes any recent Data directories from the File menu.
		/// </summary>
		private void CleanUpRecentList()
		{
			foreach (var menuItem in RecentDataItemsMenu)
			{
				menuItem.Click -= RecentDataDirectoryClicked;
				fileToolStripMenuItem.Items.Remove(menuItem);
			}
			foreach (var menuItem in RecentDataItemsButton)
			{
				menuItem.Click -= RecentDataDirectoryClicked;
				RecentDataDirectories_DropDown.Items.Remove(menuItem);
			}
			RecentDataItemsMenu.Clear();
			RecentDataItemsButton.Clear();
		}
		public void AddRecentDataFolder(string dataDirectory)
		{
			try
			{
				var mySettings = Properties.Settings.Default;
				var dataDirectories = ManiacEditor.Settings.MySettings.DataDirectories;

				if (dataDirectories == null)
				{
					dataDirectories = new StringCollection();
                    ManiacEditor.Settings.MySettings.DataDirectories = dataDirectories;
				}

				if (dataDirectories.Contains(dataDirectory))
				{
					dataDirectories.Remove(dataDirectory);
				}

				if (dataDirectories.Count >= 10)
				{
					for (int i = 9; i < dataDirectories.Count; i++)
					{
						dataDirectories.RemoveAt(i);
					}
				}

				dataDirectories.Insert(0, dataDirectory);

                ManiacEditor.Settings.MySettings.Save();

				RefreshDataDirectories(dataDirectories);

				UpdateDataFolderLabel(dataDirectory);


			}
			catch (Exception ex)
			{
				Debug.Write("Failed to add data folder to recent list: " + ex);
			}
		}
		#endregion
		#region Open Scene Methods
		public void OpenSceneForceFully()
		{

			string dataDirectory = ManiacEditor.Settings.MyDevSettings.DevForceRestartData;
			DataDirectory = dataDirectory;
			string Result = ManiacEditor.Settings.MyDevSettings.DevForceRestartScene;
			int LevelID = ManiacEditor.Settings.MyDevSettings.DevForceRestartID;
			bool isEncore = ManiacEditor.Settings.MyDevSettings.DevForceRestartIsEncore;
			string CurrentZone = ManiacEditor.Settings.MyDevSettings.DevForceRestartCurrentZone;
			string CurrentName = ManiacEditor.Settings.MyDevSettings.DevForceRestartCurrentName;
			string CurrentSceneID = ManiacEditor.Settings.MyDevSettings.DevForceRestartSceneID;
			bool Browsed = ManiacEditor.Settings.MyDevSettings.DevForceRestartIsBrowsed;

			int x = ManiacEditor.Settings.MyDevSettings.DevForceRestartX;
			int y = ManiacEditor.Settings.MyDevSettings.DevForceRestartY;
			UIModes.TempWarpCoords = new Point(x, y);
			UIModes.ForceWarp = true;

			FileHandler.OpenSceneForcefully(dataDirectory, Result, LevelID, isEncore, CurrentZone, CurrentZone, CurrentSceneID, Browsed);
		}
		private void OpenSceneForceFully(string dataDir, string scenePath, string modPath, int levelID, bool isEncoreMode, int X, int Y, double _ZoomScale = 0.0, string SceneID = "", string Zone = "", string Name = "")
		{
			MessageBox.Show("These Kind of Shortcuts are Broken for now! SORRY!");

			/*
			string dataDirectory = dataDir;
			DataDirectory = dataDirectory;
			string Result = scenePath;
			int LevelID = levelID;
			bool isEncore = isEncoreMode;
			string CurrentZone = Zone;
			string CurrentName = Name;
			string CurrentSceneID = SceneID;
			bool Browsed = false;

			if (_ZoomScale != 0.0)
			{
				ShortcutZoomValue = _ZoomScale;
				ShortcutHasZoom = true;
			}
			TempWarpCoords = new Point(X, Y);
			ForceWarp = true;

			if (CurrentZone == "" || CurrentName == "" || CurrentSceneID == "")
			{
				MessageBox.Show("Shortcuts are Broken for now! SORRY!");
				return;
			}
			else
			{
				EditorSceneLoading.OpenSceneForcefully(dataDirectory, Result, LevelID, isEncore, CurrentZone, CurrentZone, CurrentSceneID, Browsed);
			}*/
		}
		private void OpenSceneForceFully(string dataDir)
		{
			DataDirectory = dataDir;
			FileHandler.OpenSceneForcefullyUsingSceneSelect(DataDirectory);
		}
		public void OpenScene(bool manual = false, string Result = null, int LevelID = -1, bool isEncore = false, bool modLoaded = false, string modDir = "")
		{
			FileHandler.OpenSceneUsingSceneSelect();
		}
        #endregion
        #region Main Events
        #region Editor Events
        private void Editor_Activated(object sender, EventArgs e)
        {
            FormsModel.GraphicPanel.Focus();
            if (TileManiacInstance.hasModified)
            {
                ReloadToolStripButton_Click(sender, null);
            }

        }
        private void Editor_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Launcher.ManiaPalConnector != null) Launcher.ManiaPalConnector.Kill();

            try
            {
                InGame.GameRunning = false;
                var mySettings = Properties.Settings.Default;
                ManiacEditor.Settings.MySettings.IsMaximized = WindowState == System.Windows.WindowState.Maximized;
                ManiacEditor.Settings.MySettings.Save();
                ManiacEditor.Settings.MyPerformance.Save();
                ManiacEditor.Settings.MyDefaults.Save();
                ManiacEditor.Settings.MyGameOptions.Save();
                ManiacEditor.Settings.MyDevSettings.Save();
                ManiacEditor.Settings.MyKeyBinds.Save();
            }
            catch (Exception ex)
            {
                Debug.Write("Failed to write settings: " + ex);
            }

            FormsModel.Dispose();
            //editorView = null;
            FormsHost.Child.Dispose();
            //host = null;



        }
        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            var e2 = KeyEventExts.ToWinforms(e);
            if (e2 != null)
            {
                UIControl.GraphicPanel_OnKeyDown(sender, e2);
            }

        }
        private void Editor_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!FormsModel.GraphicPanel.Focused)
            {
                UIControl.GraphicPanel_OnKeyDown(sender, e);
            }
        }
        private void Editor_KeyUp(object sender, KeyEventArgs e)
        {
            var e2 = KeyEventExts.ToWinforms(e);
            if (e2 != null)
            {
                UIControl.GraphicPanel_OnKeyUp(sender, e2);
            }

        }
        private void Editor_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!FormsModel.GraphicPanel.Focused)
            {
                UIControl.GraphicPanel_OnKeyUp(sender, e);
            }
        }
        public void Editor_Resize(object sender, RoutedEventArgs e) { ZoomModel.Resize(sender, e); }
        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            // Create the interop host control.
            FormsHost = new System.Windows.Forms.Integration.WindowsFormsHost();

            // Create the MaskedTextBox control.

            // Assign the MaskedTextBox control as the host control's child.
            FormsHost.Child = FormsModel;

            FormsHost.Foreground = (SolidColorBrush)FindResource("NormalText");

            // Add the interop host control to the Grid
            // control's collection of child controls.
            this.ViewPanelForm.Children.Add(FormsHost);

            FormsModel.GraphicPanel.Init(FormsModel);
        }
        #endregion
        #region Graphics Panel Events
        private void GraphicPanel_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Int32)) && IsTilesEdit())
            {
                Point rel = FormsModel.GraphicPanel.PointToScreen(Point.Empty);
                e.Effect = System.Windows.Forms.DragDropEffects.Move;
                EditLayerA?.StartDragOver(new Point((int)(((e.X - rel.X) + StateModel.ShiftX) / StateModel.Zoom), (int)(((e.Y - rel.Y) + StateModel.ShiftY) / StateModel.Zoom)), (ushort)TilesToolbar.SelectedTile);
                UI.UpdateEditLayerActions();
            }
            else
            {
                e.Effect = System.Windows.Forms.DragDropEffects.None;
            }
        }
        private void GraphicPanel_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Int32)) && IsTilesEdit())
            {
                Point rel = FormsModel.GraphicPanel.PointToScreen(Point.Empty);
                EditLayerA?.DragOver(new Point((int)(((e.X - rel.X) + StateModel.ShiftX) / StateModel.Zoom), (int)(((e.Y - rel.Y) + StateModel.ShiftY) / StateModel.Zoom)), (ushort)TilesToolbar.SelectedTile);
                FormsModel.GraphicPanel.Render();

            }
        }
        private void GraphicPanel_DragLeave(object sender, EventArgs e)
        {
            EditLayerA?.EndDragOver(true);
            FormsModel.GraphicPanel.Render();
        }
        private void GraphicPanel_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            EditLayerA?.EndDragOver(false);
        }
        public void GraphicPanel_OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            UIControl.GraphicPanel_OnKeyDown(sender, e);
        }
        public void GraphicPanel_OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            UIControl.GraphicPanel_OnKeyUp(sender, e);
        }
        #endregion
        #region Mouse Actions Event Handlers
        private void GraphicPanel_OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) { UIControl.MouseMove(sender, e); }
        private void GraphicPanel_OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { UIControl.MouseDown(sender, e); }
        private void GraphicPanel_OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { UIControl.MouseUp(sender, e); }
        private void GraphicPanel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) { UIControl.MouseWheel(sender, e); }
        private void GraphicPanel_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) { UIControl.MouseClick(sender, e); }
        #endregion
        #region Splitter Events
        private void Spliter_DragDelta(object sender, DragDeltaEventArgs e) { ZoomModel.Resize(sender, e); }
        private void Spliter_SizeChanged(object sender, SizeChangedEventArgs e) { ZoomModel.SetZoomLevel(StateModel.ZoomLevel, new System.Drawing.Point(StateModel.ShiftX, StateModel.ShiftY), 0.0, false); }
        #endregion
        #region Scrollbar Events
        private void VScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) { ZoomModel.VScrollBar1_Scroll(sender, e); }
        private void HScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) { ZoomModel.HScrollBar1_Scroll(sender, e); }
        private void VScrollBar1_ValueChanged(object sender, RoutedEventArgs e) { ZoomModel.VScrollBar1_ValueChanged(sender, e); }
        private void HScrollBar1_ValueChanged(object sender, RoutedEventArgs e) { ZoomModel.HScrollBar1_ValueChanged(sender, e); }
        private void VScrollBar1_Entered(object sender, EventArgs e) { ZoomModel.VScrollBar1_Entered(sender, e); }
        private void HScrollBar1_Entered(object sender, EventArgs e) { ZoomModel.HScrollBar1_Entered(sender, e); }
        #endregion

        public void OnResetDevice(object sender, DeviceEventArgs e)
		{
			Device device = e.Device;
		}
		public void CheckDeviceState(object sender, PowerModeChangedEventArgs e)
		{
			switch (e.Mode)
			{
				case PowerModes.Suspend:
					SetDeviceSleepState(false);
					break;
				case PowerModes.Resume:
					SetDeviceSleepState(true);
					break;
			}
		}
		private void GraphicPanel_OnRender(object sender, DeviceEventArgs e)
		{
			// hmm, if I call refresh when I update the values, for some reason it will stop to render until I stop calling refrsh
			// So I will refresh it here

			bool showEntities = ShowEntities.IsChecked.Value && !EditEntities.IsCheckedAll;
			bool showEntitiesEditing = EditEntities.IsCheckedAll;

			bool PriorityMode = UIModes.PrioritizedEntityViewing;
            bool AboveAllMode = UIModes.EntitiesVisibileAboveAllLayers;


			if (EntitiesToolbar?.NeedRefresh ?? false) EntitiesToolbar.PropertiesRefresh();
            if (EditorScene != null)
            {
                if (!IsTilesEdit())
                    BackgroundDX.Draw(FormsModel.GraphicPanel);
                if (IsTilesEdit())
                {
                    if (ManiacEditor.Settings.MyPerformance.ShowEditLayerBackground == true)
                    {
                        BackgroundDX.DrawEdit(FormsModel.GraphicPanel);
                    }
                }


                // Future Implementation

                /*
                List<int> layerDrawingOrder = new List<int> { };
                var allLayers = EditorScene.AllLayers;
                foreach (var layer in allLayers)
                {
                    layerDrawingOrder.Add(layer.Layer.UnknownByte2);
                }
                layerDrawingOrder.Sort();
                for (int i = 0; i < layerDrawingOrder.Count; i++)
                {
                    DrawLayers(layerDrawingOrder[i]);
                }


                DrawLayers();
                */


                if (UIModes.DebugStatsVisibleOnPanel && EditorScene != null)
                {
                    Point point = new Point((short)(15), (short)(15));

                    DebugTextHUD.DrawEditorHUDText(this, FormsModel.GraphicPanel, point.X, point.Y, StateModel.GetDataFolder(), true, 255, 15);
                    DebugTextHUD.DrawEditorHUDText(this, FormsModel.GraphicPanel, point.X, point.Y + 12 * 1, StateModel.GetMasterDataFolder(), true, 255, 22);
                    DebugTextHUD.DrawEditorHUDText(this, FormsModel.GraphicPanel, point.X, point.Y + 12 * 2, StateModel.GetScenePath(), true, 255, 11);
                    DebugTextHUD.DrawEditorHUDText(this, FormsModel.GraphicPanel, point.X, point.Y + 12 * 3, StateModel.GetSceneFilePath(), true, 255, 12);
                    DebugTextHUD.DrawEditorHUDText(this, FormsModel.GraphicPanel, point.X, point.Y + 12 * 4, StateModel.GetZoom(), true, 255, 11);
                    DebugTextHUD.DrawEditorHUDText(this, FormsModel.GraphicPanel, point.X, point.Y + 12 * 5, StateModel.GetSetupObject(), true, 255, 13);
                    DebugTextHUD.DrawEditorHUDText(this, FormsModel.GraphicPanel, point.X, point.Y + 12 * 6, StateModel.GetSelectedZone(), true, 255, 14);

                    DebugTextHUD.DrawEditorHUDText(this, FormsModel.GraphicPanel, point.X, point.Y + 12 * 8, "Use " + UIControl.KeyBindPraser("StatusBoxToggle") + " to Toggle this Information", true, 255, UIControl.KeyBindPraser("StatusBoxToggle").Length, 4);
                }



                if (EditorScene.OtherLayers.Contains(EditLayerA)) EditLayerA.Draw(FormsModel.GraphicPanel);

                if (!UIModes.ExtraLayersMoveToFront)
                {

                    foreach (var elb in ExtraLayerEditViewButtons)
                    {
                        if (elb.Key.IsCheckedAll || elb.Value.IsCheckedAll)
                        {
                            var _extraViewLayer = EditorScene.OtherLayers.Single(el => el.Name.Equals(elb.Key.Text));
                            _extraViewLayer.Draw(FormsModel.GraphicPanel);
                        }
                    }
                }

                if (ShowFGLower.IsChecked.Value || EditFGLower.IsCheckedAll) FGLower.Draw(FormsModel.GraphicPanel);
                if (ShowFGLow.IsChecked.Value || EditFGLow.IsCheckedAll) FGLow.Draw(FormsModel.GraphicPanel);


                if (showEntities && !AboveAllMode)
                {
                    if (PriorityMode)
                    {
                        Entities.DrawPriority(FormsModel.GraphicPanel, -1);
                        Entities.DrawPriority(FormsModel.GraphicPanel, 0);
                        Entities.DrawPriority(FormsModel.GraphicPanel, 1);
                    }
                    else
                    {
                        Entities.Draw(FormsModel.GraphicPanel);
                    }
                }

                if (ShowFGHigh.IsChecked.Value || EditFGHigh.IsCheckedAll)
                    FGHigh.Draw(FormsModel.GraphicPanel);


                if (showEntities && PriorityMode && !AboveAllMode)
                {
                    Entities.DrawPriority(FormsModel.GraphicPanel, 2);
                    Entities.DrawPriority(FormsModel.GraphicPanel, 3);
                }

                if (ShowFGHigher.IsChecked.Value || EditFGHigher.IsCheckedAll)
                    FGHigher.Draw(FormsModel.GraphicPanel);

                if (UIModes.ExtraLayersMoveToFront)
                {
                    foreach (var elb in ExtraLayerEditViewButtons)
                    {
                        if (elb.Value.IsCheckedAll || elb.Key.IsCheckedAll)
                        {
                            var _extraViewLayer = EditorScene.OtherLayers.Single(el => el.Name.Equals(elb.Key.Text));
                            _extraViewLayer.Draw(FormsModel.GraphicPanel);
                        }
                    }
                }

                if (showEntitiesEditing || AboveAllMode)
                {
                    if (PriorityMode)
                    {
                        Entities.DrawPriority(FormsModel.GraphicPanel, -1);
                        Entities.DrawPriority(FormsModel.GraphicPanel, 0);
                        Entities.DrawPriority(FormsModel.GraphicPanel, 1);
                        Entities.DrawPriority(FormsModel.GraphicPanel, 2);
                        Entities.DrawPriority(FormsModel.GraphicPanel, 3);
                    }
                    else
                    {
                        Entities.Draw(FormsModel.GraphicPanel);
                    }
                }

                if (UIModes.EntitySelectionBoxesAlwaysPrioritized && (showEntities || showEntitiesEditing))
                {
                    Entities.DrawSelectionBoxes(FormsModel.GraphicPanel);
                }

			}

			if (StateModel.draggingSelection)
			{
				int bound_x1 = (int)(StateModel.selectingX / StateModel.Zoom); int bound_x2 = (int)(StateModel.lastX / StateModel.Zoom);
				int bound_y1 = (int)(StateModel.selectingY / StateModel.Zoom); int bound_y2 = (int)(StateModel.lastY / StateModel.Zoom);
				if (bound_x1 != bound_x2 && bound_y1 != bound_y2)
				{
					if (bound_x1 > bound_x2)
					{
						bound_x1 = (int)(StateModel.lastX / StateModel.Zoom);
						bound_x2 = (int)(StateModel.selectingX / StateModel.Zoom);
					}
					if (bound_y1 > bound_y2)
					{
						bound_y1 = (int)(StateModel.lastY / StateModel.Zoom);
						bound_y2 = (int)(StateModel.selectingY / StateModel.Zoom);
					}


				}

				FormsModel.GraphicPanel.DrawRectangle(bound_x1, bound_y1, bound_x2, bound_y2, Color.FromArgb(100, Color.Purple));
				FormsModel.GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x2, bound_y1, Color.Purple);
				FormsModel.GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x1, bound_y2, Color.Purple);
				FormsModel.GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x2, bound_y1, Color.Purple);
				FormsModel.GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x1, bound_y2, Color.Purple);
			}
			else
			{
				StateModel.select_x1 = 0; StateModel.select_x2 = 0; StateModel.select_y1 = 0; StateModel.select_y2 = 0;
			}

			if (UIModes.ShowGrid && EditorScene != null)
				BackgroundDX.DrawGrid(FormsModel.GraphicPanel);

			if (InGame.GameRunning)
			{
				InGame.DrawGameElements(FormsModel.GraphicPanel);

				if (InGame.PlayerSelected)
				{
					InGame.MovePlayer(new Point(StateModel.lastX, StateModel.lastY), StateModel.Zoom, InGame.SelectedPlayer);
				}
				if (InGame.CheckpointSelected)
				{
					Point clicked_point = new Point((int)(StateModel.lastX / StateModel.Zoom), (int)(StateModel.lastY / StateModel.Zoom));
					InGame.UpdateCheckpoint(clicked_point);
				}
			}

			if (StateModel.scrolling)
			{
				if (FormsModel.vScrollBar1.IsVisible && FormsModel.hScrollBar1.IsVisible) FormsModel.GraphicPanel.Draw2DCursor(StateModel.scrollPosition.X, StateModel.scrollPosition.Y);
				else if (FormsModel.vScrollBar1.IsVisible) FormsModel.GraphicPanel.DrawVertCursor(StateModel.scrollPosition.X, StateModel.scrollPosition.Y);
				else if (FormsModel.hScrollBar1.IsVisible) FormsModel.GraphicPanel.DrawHorizCursor(StateModel.scrollPosition.X, StateModel.scrollPosition.Y);
			}
			if (UIModes.ForceWarp)
			{
				if (UIModes.ShortcutHasZoom) ZoomModel.SetZoomLevel(0, UIModes.TempWarpCoords, UIModes.ShortcutZoomValue);
				else ZoomModel.SetZoomLevel(ManiacEditor.Settings.MyDevSettings.DevForceRestartZoomLevel, UIModes.TempWarpCoords);
				GoToPosition(UIModes.TempWarpCoords.X, UIModes.TempWarpCoords.Y, false, true);
                ZoomModel.SetViewSize((int)(SceneWidth * StateModel.Zoom), (int)(SceneHeight * StateModel.Zoom));

			}
		}
		public void DrawLayers(int drawOrder = 0)
		{
			var _extraViewLayer = EditorScene.LayerByDrawingOrder.FirstOrDefault(el => el.Layer.DrawingOrder.Equals(drawOrder));
			_extraViewLayer.Draw(FormsModel.GraphicPanel);
		}
		public void Run()
		{
			Show();
			Focus();
			FormsModel.Show();
			FormsModel.GraphicPanel.Run();

		}
        public void UpdateStartScreen(bool visible, bool firstLoad = false)
        {
            if (firstLoad)
            {
                Thread thread = new Thread(() => {
                    Updater.CheckforUpdates(true, true);
                    EditorUIModes.UpdateUpdaterMessage = true;
                });
                thread.Start();
                this.OverlayPanel.Children.Add(StartScreen);
                StartScreen.SelectScreen.ReloadQuickPanel();
                this.ViewPanelForm.Visibility = Visibility.Hidden;


            }
            if (visible)
            {
                StartScreen.Visibility = Visibility.Visible;
                StartScreen.SelectScreen.ReloadQuickPanel();
                this.ViewPanelForm.Visibility = Visibility.Hidden;
            }
            else
            {
                StartScreen.Visibility = Visibility.Hidden;
                StartScreen.SelectScreen.ReloadQuickPanel();
                this.ViewPanelForm.Visibility = Visibility.Visible;
            }

        }

        #endregion
        #region Asset Reloading
        public void ReloadSpecificTextures(object sender, RoutedEventArgs e)
		{
			try
			{
				// release all our resources, and force a reload of the tiles
				// Entities should take care of themselves
				DisposeTextures();

				if (UIModes.UseEncoreColors)
				{
                    if (EditorTiles != null) EditorTiles.StageTiles?.Image.Reload(EncorePalette[0]);
				}
				else
				{
                    if (EditorTiles != null) EditorTiles.StageTiles?.Image.Reload();
				}

			}
			catch (Exception ex)
			{
				RSDKrU.MessageBox.Show(ex.Message);
			}
		}
		public void DisposeTextures()
		{
            if (EditorScene != null)
            {
                // Make sure to dispose the textures of the extra layers too
                if (EditorTiles != null) EditorTiles?.DisposeTextures();
                if (FGHigh != null) FGHigh.DisposeTextures();
                if (FGLow != null) FGLow.DisposeTextures();
                if (FGHigher != null) FGHigher.DisposeTextures();
                if (FGLower != null) FGLower.DisposeTextures();

                foreach (var el in EditorScene.OtherLayers)
                {
                    el.DisposeTextures();
                }
            }
		}
		public void RefreshCollisionColours(bool RefreshMasks = false)
		{
			if (EditorScene != null && CollisionLayerA != null && CollisionLayerB != null)
			{
				switch (UIModes.CollisionPreset)
				{
					case 2:
						CollisionAllSolid = Properties.Defaults.Default.CollisionSAColour;
						CollisionTopOnlySolid = Properties.Defaults.Default.CollisionTOColour;
						CollisionLRDSolid = Properties.Defaults.Default.CollisionLRDColour;
						break;
					case 1:
						CollisionAllSolid = Color.Black;
						CollisionTopOnlySolid = Color.Yellow;
						CollisionLRDSolid = Color.Red;
						break;
					case 0:
						CollisionAllSolid = Color.White;
						CollisionTopOnlySolid = Color.Yellow;
						CollisionLRDSolid = Color.Red;
						break;
				}




				if (RefreshMasks)
				{
					CollisionLayerA.Clear();
					CollisionLayerB.Clear();

					for (int i = 0; i < 1024; i++)
					{
						CollisionLayerA.Add(EditorTiles.StageTiles.Config.CollisionPath1[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionAllSolid));
						CollisionLayerB.Add(EditorTiles.StageTiles.Config.CollisionPath2[i].DrawCMask(Color.FromArgb(0, 0, 0, 0), CollisionAllSolid));
					}
				}
			}

		}
		#endregion
		#region Backup Tool Methods

		public void BackupScene()
		{
			UIModes.BackupType = 1;
			BackupEvent(null, null);
			UIModes.BackupType = 0;
		}
		public void BackupSceneBeforeCrash()
		{
			UIModes.BackupType = 2;
			BackupEvent(null, null);
			UIModes.BackupType = 0;
		}
		public void AutoBackupScene()
		{
			UIModes.BackupType = 3;
			BackupEvent(null, null);
			UIModes.BackupType = 0;
		}
		public void BackupTool(object sender, RoutedEventArgs e)
		{

		}

		#endregion
		#region Get + Set Methods
		public Rectangle GetScreen()
		{
			if (ManiacEditor.Settings.MySettings.EntityFreeCam) return new Rectangle(StateModel.CustomX, StateModel.CustomY, FormsModel.mainPanel.Width, FormsModel.mainPanel.Height);
			else return new Rectangle(StateModel.ShiftX, StateModel.ShiftY, FormsModel.mainPanel.Width, FormsModel.mainPanel.Height);
		}
		public double GetZoom()
		{
			return StateModel.Zoom;
		}
		private void SetDeviceSleepState(bool state)
		{
			FormsModel.GraphicPanel.bRender = state;
			if (state == true)
			{
				ReloadToolStripButton_Click(null, null);
			}
		}
		public Scene GetSceneSelection()
		{
			string selectedScene;

			ManiacEditor.Interfaces.SceneSelectWindow select = new ManiacEditor.Interfaces.SceneSelectWindow(GameConfig, this);
			select.Owner = Window.GetWindow(this);
			select.ShowDialog();
			if (select.SceneSelect.Result == null)
				return null;
			selectedScene = select.SceneSelect.Result;

			if (!File.Exists(selectedScene))
			{
				string[] splitted = selectedScene.Split('\\');

				string part1 = splitted[0];
				string part2 = splitted[1];

				selectedScene = Path.Combine(DataDirectory, "Stages", part1, part2);
			}
			return new Scene(selectedScene);
		}
		#endregion
        #region Action Events (MenuItems, Clicks, etc.)
        #region File Events
        private void NewSceneEvent(object sender, RoutedEventArgs e) { FileHandler.NewScene(); }
        public void OpenSceneEvent(object sender, RoutedEventArgs e) { FileHandler.OpenScene(); }
        public void OpenDataDirectoryEvent(object sender, RoutedEventArgs e) { FileHandler.OpenDataDirectory(); }
        public void SaveSceneEvent(object sender, RoutedEventArgs e) { FileHandler.Save(); }
        private void ExitEditorEvent(object sender, RoutedEventArgs e) { Close(); }
        private void ExportAsPNGEvent(object sender, RoutedEventArgs e) { FileHandler.ExportAsPNG(); }
        private void ExportLayersAsPNGEvent(object sender, RoutedEventArgs e) { FileHandler.ExportLayersAsPNG(); }
        public void SaveSceneAsEvent(object sender, RoutedEventArgs e) { FileHandler.SaveAs(); }
        private void BackupEvent(object sender, RoutedEventArgs e) { UIEvents.BackupToolStripMenuItem_Click(sender, e); }
        private void RecoverEvent(object sender, RoutedEventArgs e) { UIEvents.BackupRecoverButton_Click(sender, e); }
        public void UnloadSceneEvent(object sender, RoutedEventArgs e) { FileHandler.UnloadScene(); }
        private void BackupStageConfigEvent(object sender, RoutedEventArgs e) { UIEvents.StageConfigBackup(sender, e); }
        private void BackupSceneEvent(object sender, RoutedEventArgs e) { UIEvents.SceneBackup(sender, e); }
        #endregion
        #region Edit Events
        public void PasteToChunksEvent(object sender, RoutedEventArgs e) { UIEvents.PasteToChunks(); }
        public void SelectAllEvent(object sender, RoutedEventArgs e) { UIEvents.SelectAll(); }
        public void CutEvent(object sender, RoutedEventArgs e) { UIEvents.Cut(); }
        public void CopyEvent(object sender, RoutedEventArgs e) { UIEvents.Copy(); }
        public void PasteEvent(object sender, RoutedEventArgs e) { UIEvents.Paste(); }
        public void DuplicateEvent(object sender, RoutedEventArgs e) { UIEvents.Duplicate(); }
        private void DeleteEvent(object sender, RoutedEventArgs e) { UIEvents.Delete(); }
        public void FlipVerticalEvent(object sender, RoutedEventArgs e) { UIEvents.FlipVertical(); }
        public void FlipHorizontalEvent(object sender, RoutedEventArgs e) { UIEvents.FlipHorizontal(); }
        public void FlipVerticalIndividualEvent(object sender, RoutedEventArgs e) { UIEvents.FlipVerticalIndividual(); }
        public void FlipHorizontalIndividualEvent(object sender, RoutedEventArgs e) { UIEvents.FlipHorizontalIndividual(); }
        #endregion
        #region Developer Stuff (WIP)
        private void DeveloperTerminalEvent(object sender, RoutedEventArgs e) { Launcher.DevTerm(); }
        private void MD5GeneratorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.MD5GeneratorToolStripMenuItem_Click(sender, e); }
        private void FindAndReplaceToolEvent(object sender, RoutedEventArgs e) { UIEvents.FindAndReplaceTool(sender, e); }
        private void ConsoleWindowToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.ConsoleWindowToolStripMenuItem_Click(sender, e); }
        private void SaveForForceOpenOnStartupToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.SaveForForceOpenOnStartupToolStripMenuItem_Click(sender, e); }
        private void LeftToolbarToggleDev_Click(object sender, RoutedEventArgs e) { UI.UpdateToolbars(false, true); }
        private void RightToolbarToggleDev_Click(object sender, RoutedEventArgs e) { UI.UpdateToolbars(true, true); }
        private void EnableAllButtonsToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.EnableAllButtonsToolStripMenuItem_Click(sender, e); }
        #endregion
        #region Animations DropDown (WIP)
        private void MovingPlatformsObjectsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (UIModes.MovingPlatformsChecked == false)
            {
                movingPlatformsObjectsToolStripMenuItem.IsChecked = true;
                UIModes.MovingPlatformsChecked = true;
            }
            else
            {
                movingPlatformsObjectsToolStripMenuItem.IsChecked = false;
                UIModes.MovingPlatformsChecked = false;
            }

        }

        private void SpriteFramesToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (UIModes.AnnimationsChecked == false)
            {
                spriteFramesToolStripMenuItem.IsChecked = true;
                UIModes.AnnimationsChecked = true;
            }
            else
            {
                spriteFramesToolStripMenuItem.IsChecked = false;
                UIModes.AnnimationsChecked = false;
            }
        }

        #endregion
        private void GoToPlayerSpawnEvent(object sender, RoutedEventArgs e) { UIEvents.PlayerSpawnToolStripMenuItem_Click(sender, e); }
        public void GoToPositionEvent(object sender, RoutedEventArgs e) { UIEvents.GoToPosition(sender, e); }
        private void ToggleMagnetToolEvent(object sender, RoutedEventArgs e) { UIModes.UseMagnetMode ^= true; }
        private void OpenDataDirectoryMenuButton(object sender, RoutedEventArgs e) { UIEvents.OpenDataDirectoryMenuButton(sender, e); }
        private void UndoEvent(object sender, RoutedEventArgs e) { EditorUndo(); }
		private void RedoEvent(object sender, RoutedEventArgs e) { EditorRedo(); }
		private void ZoomInEvent(object sender, RoutedEventArgs e) { UIEvents.ZoomIn(sender, e); }
		private void ZoomOutEvent(object sender, RoutedEventArgs e) { UIEvents.ZoomOut(sender, e); }
		private void ToggleSelectToolEvent(object sender, RoutedEventArgs e) { UIModes.SelectionMode(); }
		private void TogglePointerToolEvent(object sender, RoutedEventArgs e) { UIModes.PointerMode(); }
		private void ToggleDrawToolEvent(object sender, RoutedEventArgs e) { UIModes.DrawMode(); }
		private void ToggleInteractionToolEvent(object sender, RoutedEventArgs e) { UIModes.InteractionMode(); }
		private void ToggleChunksToolEvent(object sender, RoutedEventArgs e) { UIModes.ChunksMode(); }
		public void ReloadToolStripButton_Click(object sender, RoutedEventArgs e) { UI.ReloadSpritesAndTextures(); }
		public void ToggleSlotIDEvent(object sender, RoutedEventArgs e) { UIModes.ShowTileID ^= true; }
        private void TogglePixelModeEvent(object sender, RoutedEventArgs e) { UIModes.EnablePixelCountMode ^= true; }
        public void ToggleScrollLockEvent(object sender, RoutedEventArgs e) { UIModes.ScrollLocked ^= true; }
        public void ToggleFasterNudgeEvent(object sender, RoutedEventArgs e) { UIModes.EnableFasterNudge ^= true; }
        public void ApplyEditEntitiesTransparencyEvent(object sender, RoutedEventArgs e) { UIModes.ApplyEditEntitiesTransparency ^= true; }
        public void ShowCollisionAEvent(object sender, RoutedEventArgs e) { UIModes.ShowCollisionA ^= true; }
        public void ShowCollisionBEvent(object sender, RoutedEventArgs e) { UIModes.ShowCollisionB ^= true; }
        private void ShowFlippedTileHelperEvent(object sender, RoutedEventArgs e) { UIModes.ShowFlippedTileHelper ^= true; }
        public void EnableEncorePaletteEvent(object sender, RoutedEventArgs e) { UIModes.UseEncoreColors ^= true; }
        private void RunSceneEvent(object sender, RoutedEventArgs e) { InGame.RunScene(); }
        public void ToggleDebugHUDEvent(object sender, RoutedEventArgs e) { UIModes.DebugStatsVisibleOnPanel ^= true; }
        private void ResetZoomLevelEvent(object sender, RoutedEventArgs e) { ZoomModel.SetZoomLevel(0, new Point(0, 0)); }
        private void UseLargeDebugHUDText(object sender, RoutedEventArgs e) { UIModes.UseLargeDebugStats ^= true; }
        public void MenuButtonChangedEvent(object sender, RoutedEventArgs e) { UIEvents.SetMenuButtonType(sender, e); }
        public void MenuButtonChangedEvent(string tag) { UIEvents.SetMenuButtonType(tag); }
        private void ShowEntitiesAboveAllOtherLayersToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIModes.EntitiesVisibileAboveAllLayers ^= true; }
        private void EntitySelectionBoxesAlwaysPrioritizedEvent(object sender, RoutedEventArgs e) { UIModes.EntitySelectionBoxesAlwaysPrioritized ^= true; }
        private void PrioritizedEntityViewingEvent(object sender, RoutedEventArgs e) { UIModes.PrioritizedEntityViewing ^= true; }
        private void SetEncorePalleteEvent(object sender, RoutedEventArgs e) { UIEvents.SetEncorePallete(sender); }
        public void SetEncorePalleteEvent(object sender = null, string path = "") { UIEvents.SetEncorePallete(sender, path); }
        private void MoveExtraLayersToFrontEvent(object sender, RoutedEventArgs e) { UIModes.ExtraLayersMoveToFront ^= true; }
        private void EntityFilterTextChangedEvent(object sender, TextChangedEventArgs e) { UIEvents.EntityFilterTextChanged(sender, e); }
        private void ShowEntitySelectionBoxesEvent(object sender, RoutedEventArgs e) { UIModes.ShowEntitySelectionBoxes ^= true; }
        private void ShowWaterLevelEvent(object sender, RoutedEventArgs e) { UIModes.ShowWaterLevel ^= true; }
        private void AlwaysShowWaterLevelEvent(object sender, RoutedEventArgs e) { UIModes.AlwaysShowWaterLevel ^= true; }
        private void SortSelectedSlotIDsEvent(object sender, RoutedEventArgs e) { Entities.OrderSelectedSlotIDs(); }
        private void SortSelectedSlotIDsOptimizedEvent(object sender, RoutedEventArgs e) { Entities.OrderSelectedSlotIDs(true); }
        private void WaterSizeWithBoundsEvent(object sender, RoutedEventArgs e) { UIModes.SizeWaterLevelwithBounds ^= true; }
        private void SwapEncoreManiaEntityVisibilityEvent(object sender, RoutedEventArgs e) { UIEvents.SwapEncoreManiaEntityVisibility(); }
        private void ShowParallaxSpritesEvent(object sender, RoutedEventArgs e) { UIModes.ShowParallaxSprites ^= true; }
        private void SetScrollDirectionEvent(object sender, RoutedEventArgs e) { UIEvents.SetScrollLockDirection(); }
        private void ShowEntityPathArrowsEvent(object sender, RoutedEventArgs e) { UIModes.ShowEntityPathArrows ^= true; }
        private void MenuLanguageChangedEvent(object sender, RoutedEventArgs e) { UIEvents.MenuLanguageChanged(sender, e); }
        public void QuickButtonClickEvent(object sender, RoutedEventArgs e)
        {
            if (sender == MoreSettingsButton)
            {
                switch (UIModes.LastQuickButtonState)
                {
                    case 1:
                        UIEvents.SetScrollLockDirection();
                        break;
                    case 2:
                        UIModes.ApplyEditEntitiesTransparency ^= true;
                        break;
                    case 3:
                        UIEvents.SwapEncoreManiaEntityVisibility();
                        break;
                    default:
                        UIModes.LastQuickButtonState = 1;
                        UIEvents.SetScrollLockDirection();
                        break;
                }
            }
            else if (sender == QuickSwapScrollDirection)
            {
                UIModes.LastQuickButtonState = 1;
                UIEvents.SetScrollLockDirection();
            }
            else if (sender == QuickSwapEncoreManiaEntitVisibility)
            {
                UIModes.LastQuickButtonState = 3;
                UIEvents.SwapEncoreManiaEntityVisibility();
            }
            else if (sender == QuickEditEntitiesTransparentLayers)
            {
                UIModes.LastQuickButtonState = 2;
                UIModes.ApplyEditEntitiesTransparency ^= true;
            }

        }
        private void OptimizeEntitySlotIDsEvent(object sender, RoutedEventArgs e) { if (EditorScene != null) Entities.OptimizeAllSlotIDs(); }
        private void ToggleRightClickSlotIDSwapEvent(object sender, RoutedEventArgs e) { UIModes.RightClicktoSwapSlotID ^= true; }
        private void ToggleCopyAirEvent(object sender, RoutedEventArgs e) { UIModes.CopyAir ^= true; }
        private void ChangeLevelIDEvent(object sender, RoutedEventArgs e) { UIEvents.ChangeLevelID(sender, e); }
        private void ToggleMultiLayerSelectEvent(object sender, RoutedEventArgs e) { UIModes.MultiLayerEditMode ^= true; }
        private void MakeDataFolderShortcutEvent(object sender, RoutedEventArgs e) { UIEvents.MakeShortcutForDataFolderOnly(sender, e); }
        private void MakeShortcutWithCurrentCoordinatesEvent(object sender, RoutedEventArgs e) { UIEvents.MakeShortcutWithCurrentCoordinatesToolStripMenuItem_Click(sender, e); }
        private void MakeShortcutWithoutCurrentCoordinatesEvent(object sender, RoutedEventArgs e) { UIEvents.MakeShortcutWithoutCurrentCoordinatesToolStripMenuItem_Click(sender, e); }
        private void SoundLooperToolStripMenuItem_Click(object sender, RoutedEventArgs e) { UIEvents.SoundLooperToolStripMenuItem_Click(sender, e); }
        private void FindUnusedTiles(object sender, RoutedEventArgs e) { FindAndReplace.FindUnusedTiles(); }
        private void UseNormalCollisionEvent(object sender, RoutedEventArgs e) { UIModes.CollisionPreset = 0; }
        private void UseInvertedCollisionEvent(object sender, RoutedEventArgs e) { UIModes.CollisionPreset = 1; }
        private void UseCustomCollisionEvent(object sender, RoutedEventArgs e) { UIModes.CollisionPreset = 2; }

        #region Collision Slider Events
        private void CollisionOpacitySliderDragCompletedEvent(object sender, DragCompletedEventArgs e) { UIEvents.CollisionOpacitySliderDragEnd(sender, e); }
        private void CollisionOpacitySliderLostFocusEvent(object sender, RoutedEventArgs e) { UIEvents.CollisionOpacitySliderLostFocus(sender, e); }
        private void CollisionOpacitySliderValueChangedEvent(object sender, RoutedPropertyChangedEventArgs<double> e) { UIEvents?.CollisionOpacitySliderValueChanged(sender, e); }
        #endregion

        #region Magnet Events

        private void Magnet8x8Event(object sender, RoutedEventArgs e) { UIModes.MagnetSize = 8; }
        private void Magnet16x16Event(object sender, RoutedEventArgs e) { UIModes.MagnetSize = 16; }
        private void Magnet32x32Event(object sender, RoutedEventArgs e) { UIModes.MagnetSize = 32; }
        private void Magnet64x64Event(object sender, RoutedEventArgs e) { UIModes.MagnetSize = 64; }
        private void EnableMagnetXAxisLockEvent(object sender, RoutedEventArgs e) { UIModes.UseMagnetXAxis ^= true; }
        private void EnableMagnetYAxisLockEvent(object sender, RoutedEventArgs e) { UIModes.UseMagnetYAxis ^= true; }

        #endregion

        #region Grid Events
        public void ToggleGridEvent(object sender, RoutedEventArgs e) { UIModes.ShowGrid ^= true; }
        private void SetGrid16x16Event(object sender, RoutedEventArgs e) { UIModes.GridSize = 16; }
        private void SetGrid128x128Event(object sender, RoutedEventArgs e) { UIModes.GridSize = 128; }
        private void SetGrid256x256Event(object sender, RoutedEventArgs e) { UIModes.GridSize = 128; }
        private void SetGridCustomSizeEvent(object sender, RoutedEventArgs e) { UIModes.GridSize = Properties.Defaults.Default.CustomGridSizeValue; }
        #endregion

        #region Apps
        private void TileManiacEditTileEvent(object sender, RoutedEventArgs e) { Launcher.TileManiacIntergration(); }
        private void OpenModManagerToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Launcher.ManiaModManager(); }
        private void TileManiacNormal(object sender, RoutedEventArgs e) { Launcher.TileManiacNormal(); }
        private void InsanicManiacToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Launcher.InsanicManiac(); }
        private void RSDKAnnimationEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Launcher.RSDKAnnimationEditor(); }
        private void RenderListManagerToolstripMenuItem_Click(object sender, RoutedEventArgs e) { Launcher.RenderListManager(); }
        private void ColorPaletteEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Launcher.ManiaPal(sender, e); }
        private void ManiaPalMenuItem_SubmenuOpened(object sender, RoutedEventArgs e) { Launcher.ManiaPalSubmenuOpened(sender, e); }
        private void DuplicateObjectIDHealerToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Launcher.DuplicateObjectIDHealer(); }
        #endregion

        #region Folders
        private void OpenSceneFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Launcher.OpenSceneFolder(); }
        private void OpenDataDirectoryFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Launcher.OpenDataDirectory(); }
        private void OpenSonicManiaFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Launcher.OpenSonicManiaFolder(); }
        private void OpenASavedPlaceToolStripMenuItem_DropDownOpening(object sender, RoutedEventArgs e) { Launcher.OpenASavedPlaceDropDownOpening(sender, e); }
        private void OpenASavedPlaceToolStripMenuItem_DropDownClosed(object sender, RoutedEventArgs e) { Launcher.OpenASavedPlaceDropDownClosed(sender, e); }
        #endregion

        #region Settings and Other Menu Events
        public void AboutScreenEvent(object sender, RoutedEventArgs e) { Launcher.AboutScreen(); }
        public void ImportObjectsToolStripMenuItem_Click(object sender, RoutedEventArgs e, Window window = null) { Launcher.ImportObjectsToolStripMenuItem_Click(sender, e); }
        public void ImportSoundsEvent(object sender, RoutedEventArgs e) { Launcher.ImportSoundsToolStripMenuItem_Click(sender, e); }
        public void ImportSoundsEvent(object sender, RoutedEventArgs e, Window window = null) { Launcher.ImportSounds(sender, e, window); }
        private void LayerManagerEvent(object sender, RoutedEventArgs e) { Launcher.LayerManager(sender, e); }
        private void ManiacINIEditorEvent(object sender, RoutedEventArgs e) { Launcher.ManiacINIEditor(sender, e); }
        private void ChangePrimaryBackgroundColorEvent(object sender, RoutedEventArgs e) { Launcher.ChangePrimaryBackgroundColor(sender, e); }
        private void ChangeSecondaryBackgroundColorEvent(object sender, RoutedEventArgs e) { Launcher.ChangeSecondaryBackgroundColor(sender, e); }
        public void ObjectManagerEvent(object sender, RoutedEventArgs e) { Launcher.ObjectManager(); }
        private void InGameOptionsMenuEvent(object sender, RoutedEventArgs e) { Launcher.InGameSettings(); }
        private void WikiLinkEvent(object sender, RoutedEventArgs e) { Launcher.WikiLink(); }
        public void OptionsMenuEvent(object sender, RoutedEventArgs e) { Launcher.OptionsMenu(); }
        private void ControlsMenuEvent(object sender, RoutedEventArgs e) { Launcher.ControlMenu(); }
        #endregion

        #region Game Running Events
        private void MoveThePlayerToHere(object sender, RoutedEventArgs e) { InGame.MoveThePlayerToHere(); }
        private void SetPlayerRespawnToHere(object sender, RoutedEventArgs e) { InGame.SetPlayerRespawnToHere(); }
        private void MoveCheckpoint(object sender, RoutedEventArgs e) { InGame.CheckpointSelected = true; }
        private void RemoveCheckpoint(object sender, RoutedEventArgs e) { InGame.UpdateCheckpoint(new Point(0, 0), false); }
        private void AssetReset(object sender, RoutedEventArgs e) { InGame.AssetReset(); }
        private void RestartScene(object sender, RoutedEventArgs e) { InGame.RestartScene(); }
        private void TrackThePlayer(object sender, RoutedEventArgs e) { InGame.TrackthePlayer(sender, e); }
        private void UpdateInGameMenuItems(object sender, RoutedEventArgs e) { InGame.UpdateRunSceneDropdown(); }
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
			LayerShowButton_Click(ShowEntities, "Entities");
		}
		private void ShowAnimations_Click(object sender, RoutedEventArgs e)
		{
			ToggleButton toggle = sender as ToggleButton;
			toggle.IsChecked = !toggle.IsChecked.Value;
			LayerShowButton_Click(ShowAnimations, "Animations");
		}
		private void LayerEditButton_Click(EditLayerToggleButton button, MouseButton ClickType)
		{
			if (UIModes.MultiLayerEditMode)
			{
				if (ClickType == MouseButton.Left) LayerA();
				else if (ClickType == MouseButton.Right) LayerB();
			}
			else
			{
				if (ClickType == MouseButton.Left) Normal();
			}
            UI.UpdateControls();



			void Normal()
			{
				Deselect(false);
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
					elb.IsCheckedN = false;
				}



			}
			void LayerA()
			{
				Deselect(false);
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
					EditEntities.IsCheckedA = false;
					button.IsCheckedA = true;
				}

				foreach (var elb in ExtraLayerEditViewButtons.Values)
				{
					elb.IsCheckedA = false;
				}
			}
			void LayerB()
			{
				Deselect(false);
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
					EditEntities.IsCheckedB = false;
					button.IsCheckedB = true;
				}

				foreach (var elb in ExtraLayerEditViewButtons.Values)
				{
					elb.IsCheckedB = false;
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
			foreach (EditorLayer el in EditorScene.OtherLayers)
			{
				EditLayerToggleButton tsb = new EditLayerToggleButton()
				{
					Text = el.Name,
                    LayerName = "Edit" + el.Name
				};
				LayerToolbar.Items.Add(tsb);
				tsb.TextForeground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Color.LawnGreen.A, Color.LawnGreen.R, Color.LawnGreen.G, Color.LawnGreen.B));
				tsb.RightClick += AdHocLayerEdit_RightClick;
				tsb.Click += AdHocLayerEdit_Click;

				_extraLayerEditButtons.Add(tsb);
			}

			//EDIT BUTTONS SEPERATOR
			Separator tss = new Separator();
			LayerToolbar.Items.Add(tss);
			ExtraLayerSeperators.Add(tss);

			//VIEW BUTTONS
			foreach (EditorLayer el in EditorScene.OtherLayers)
			{
				EditLayerToggleButton tsb = new EditLayerToggleButton()
				{
					Text = el.Name,
                    LayerName = "Show" + el.Name.Replace(" ", "")
				};
				LayerToolbar.Items.Insert(LayerToolbar.Items.IndexOf(extraViewLayersSeperator), tsb);
				tsb.TextForeground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, Color.FromArgb(0x33AD35).R, Color.FromArgb(0x33AD35).G, Color.FromArgb(0x33AD35).B));

				_extraLayerViewButtons.Add(tsb);
			}

			//EDIT + VIEW BUTTONS LIST
			for (int i = 0; i < _extraLayerViewButtons.Count; i++)
			{
				ExtraLayerEditViewButtons.Add(_extraLayerViewButtons[i], _extraLayerEditButtons[i]);
			}

			UpdateDualButtonsControlsForLayer(FGLow, ShowFGLow, EditFGLow);
			UpdateDualButtonsControlsForLayer(FGHigh, ShowFGHigh, EditFGHigh);
			UpdateDualButtonsControlsForLayer(FGLower, ShowFGLower, EditFGLower);
			UpdateDualButtonsControlsForLayer(FGHigher, ShowFGHigher, EditFGHigher);
		}
		public void TearDownExtraLayerButtons()
		{
			foreach (var elb in ExtraLayerEditViewButtons)
			{
				LayerToolbar.Items.Remove(elb.Key);
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
		private void UpdateDualButtonsControlsForLayer(EditorLayer layer, ToggleButton visibilityButton, EditLayerToggleButton editButton)
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
			if (ClickType == MouseButton.Left && !UIModes.MultiLayerEditMode) Normal();
			else if (ClickType == MouseButton.Left && UIModes.MultiLayerEditMode) LayerA();
			else if (ClickType == MouseButton.Right && UIModes.MultiLayerEditMode) LayerB();

			void Normal()
			{
				EditLayerToggleButton tsb = sender as EditLayerToggleButton;
				Deselect(false);
				if (tsb.IsCheckedN.Value)
				{
					if (!ManiacEditor.Settings.MySettings.KeepLayersVisible)
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
				Deselect(false);
				if (tsb.IsCheckedA.Value)
				{
					if (!ManiacEditor.Settings.MySettings.KeepLayersVisible)
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
				Deselect(false);
				if (tsb.IsCheckedB.Value)
				{
					if (!ManiacEditor.Settings.MySettings.KeepLayersVisible)
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

            UI.UpdateControls();
		}
        #endregion
        #region Mod Config List Stuff
        public MenuItem CreateModConfigMenuItem(int i)
        {
            MenuItem newItem = new MenuItem()
            {
                Header = ManiacEditor.Settings.MySettings.ModLoaderConfigsNames[i],
                Tag = ManiacEditor.Settings.MySettings.ModLoaderConfigs[i]
            };
            newItem.Click += ModConfigItemClicked;
            if (newItem.Tag.ToString() == ManiacEditor.Settings.MySettings.LastModConfig) newItem.IsChecked = true;
            return newItem;
        }
        private void ModConfigItemClicked(object sender, RoutedEventArgs e)
        {
            var modConfig_CheckedItem = (sender as MenuItem);
            SelectConfigToolStripMenuItem_Click(modConfig_CheckedItem);
            ManiacEditor.Settings.MySettings.LastModConfig = modConfig_CheckedItem.Tag.ToString();
        }
        public void EditConfigsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Interfaces.WPF_UI.ConfigManager configManager = new Interfaces.WPF_UI.ConfigManager();
            configManager.Owner = GetWindow(this);
            configManager.ShowDialog();

            // TODO: Fix NullReferenceException on Settings.mySettings.modConfigs
            selectConfigToolStripMenuItem.Items.Clear();
            for (int i = 0; i < ManiacEditor.Settings.MySettings.ModLoaderConfigs.Count; i++)
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
    }
}