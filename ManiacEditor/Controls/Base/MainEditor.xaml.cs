﻿using IWshRuntimeLibrary;
using ManiacEditor.Actions;
using ManiacEditor.Entity_Renders;
using ManiacEditor.Controls;
using Microsoft.Scripting.Utils;
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
using ManiacEditor.Controls.Base.Controls;
using ManiacEditor.Enums;
using ManiacEditor.Event_Handlers;
using ManiacEditor.Extensions;


namespace ManiacEditor.Controls.Base
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainEditor : Window
	{
        #region Classical Regions
        #region Definitions
        public static ManiacEditor.Controls.Base.MainEditor Instance;

		//Editor Paths
		public string DataDirectory; //Used to get the current Data Directory
		public string MasterDataDirectory = Environment.CurrentDirectory + "\\Data"; //Used as a way of allowing mods to not have to lug all the files in their folder just to load in Maniac.
		public IList<string> ResourcePackList { get; set; } = new List<string>();
        public string LoadedDataPack = "";
		public string[] EncorePalette = new string[6]; //Used to store the location of the encore palletes

		// Extra Layer Buttons
		public IDictionary<EditLayerToggleButton, EditLayerToggleButton> ExtraLayerEditViewButtons;
		public IList<Separator> ExtraLayerSeperators; //Used for Adding Extra Seperators along side Extra Edit/View Layer Buttons

		// Editor Collections
		public List<string> ObjectList = new List<string>(); //All Gameconfig + Stageconfig Object names (Unused)
        public IList<Tuple<MenuItem, MenuItem>> RecentSceneItems;
        public IList<Tuple<MenuItem, MenuItem>> RecentDataSourceItems;
		public List<string> userDefinedSpritePaths = new List<string>();
		public Dictionary<string, string> userDefinedEntityRenderSwaps = new Dictionary<string, string>();
        public System.ComponentModel.BindingList<TextBlock> SplineSelectedObjectSpawnList = new System.ComponentModel.BindingList<TextBlock>();
        public System.Timers.Timer Timer = new System.Timers.Timer();

        //Undo + Redo
        public Stack<IAction> UndoStack { get; set; } = new Stack<IAction>(); //Undo Actions Stack
        public Stack<IAction> RedoStack = new Stack<IAction>(); //Redo Actions Stack

		//Clipboards
		public Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> TilesClipboard;
		public Dictionary<Point, ushort> FindReplaceClipboard;
		public Dictionary<Point, ushort> TilesClipboardEditable;
		public List<Classes.Editor.Scene.Sets.EditorEntity> entitiesClipboard;

        //Collision Colours
        public Color CollisionAllSolid = Color.White;
        public Color CollisionTopOnlySolid = Color.Yellow;
        public Color CollisionLRDSolid = Color.Red;

        //Internal/Public/Vital Classes

        public EditorControl EditorControls;

		internal Classes.Editor.Scene.EditorBackground BackgroundDX;
		public ManiacEditor.Controls.Base.Toolbars.TilesToolbar.TilesToolbar TilesToolbar = null;
		public ManiacEditor.Controls.Base.Toolbars.EntitiesToolbar.EntitiesToolbar EntitiesToolbar = null;
		public Methods.Entities.EntityDrawing EntityDrawing;
		public ManiacEditor.Controls.Base.Elements.StartScreen StartScreen;
		public Classes.Editor.SolutionState StateModel;
		public Classes.Editor.Scene.EditorChunks Chunks;
		public GraphicsModel DeviceModel;
		public EditorUIEvents UIEvents;
		public Classes.Editor.Scene.EditorPath Paths;
		public ManiacEditor.Classes.Editor.SolutionLoader FileHandler;
		public Methods.Layers.TileFindReplace FindAndReplace;
        public EditorZoomModel ZoomModel;
        public EditorUI UI;
        public Core.ProcessMemory GameMemory = new Core.ProcessMemory(); //Allows us to write hex codes like cheats, etc.
        public System.Windows.Forms.Integration.WindowsFormsHost DeviceHost;
        public ManiacEditor.Controls.TileManiac.CollisionEditor TileManiacInstance = new ManiacEditor.Controls.TileManiac.CollisionEditor();

		// Stuff Used for Command Line Tool to Fix Duplicate Object ID's
		#region DLL Import Stuff
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
		private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

		[DllImport("USER32.DLL")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr CloseClipboard();

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
		public MainEditor(string dataDir = "", string scenePath = "", string modPath = "", int levelID = 0, bool ShortcutLaunch = false, int shortcutLaunchMode = 0, bool isEncoreMode = false, int X = 0, int Y = 0, double _ZoomedLevel = 0.0, int MegaManiacInstanceID = -1)
		{

            ManiacEditor.Methods.ProgramBase.Log.InfoFormat("Setting Up the Map Editor...");

            Methods.Internal.Theming.UpdateInstance(this);
            Methods.Internal.Settings.UpdateInstance(this);

            Methods.Internal.Theming.UseDarkTheme_WPF(ManiacEditor.Core.Settings.MySettings.NightMode);
            Instance = this;
            InitializeComponent();

            Timer.Interval = 1;
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();



            System.Windows.Application.Current.MainWindow = this;

            try
            {
                InitilizeEditor();
            }
            catch (Exception ex)
            {
                Debug.Print("Couldn't Initialize Editor!" + ex.ToString());
                throw ex;
            }

			if (ManiacEditor.Core.Settings.MyDevSettings.DevAutoStart) OpenSceneForceFully();

			if (ShortcutLaunch)
			{
				try
				{
					if (dataDir != "" && scenePath != "") OpenSceneForceFully(dataDir, scenePath, modPath, levelID, isEncoreMode, X, Y);

					else if (dataDir != "") OpenSceneForceFully(dataDir);
				}
				catch
				{
					Debug.Print("Couldn't Force Open Maniac Editor with the Specified Arguments!");
				}
			}
		}

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Classes.Editor.Solution.CurrentScene != null)
            {
                foreach (var layer in Classes.Editor.Solution.CurrentScene.AllLayers)
                {
                    layer.UpdateLayerScrollIndex();
                }
            }
        }

        public void InitilizeEditor()
		{
			DeviceModel = new GraphicsModel(this);

			this.Activated += new System.EventHandler(this.Editor_Activated);


			ExtraLayerEditViewButtons = new Dictionary<EditLayerToggleButton, EditLayerToggleButton>();
			ExtraLayerSeperators = new List<Separator>();

            RecentSceneItems = new List<Tuple<MenuItem, MenuItem>>();
            RecentDataSourceItems = new List<Tuple<MenuItem, MenuItem>>();
            Methods.GameHandler.UpdateInstance(this);
			EntityDrawing = new Methods.Entities.EntityDrawing(this);
            StateModel = new Classes.Editor.SolutionState(this);
            EditorControls = new EditorControl();
            StartScreen = new ManiacEditor.Controls.Base.Elements.StartScreen(this);
			UIEvents = new EditorUIEvents(this);
			Paths = new Classes.Editor.Scene.EditorPath(this);
			FileHandler = new ManiacEditor.Classes.Editor.SolutionLoader(this);
			Methods.Prefrences.DataPackStorage.Initilize(this);
			FindAndReplace = new Methods.Layers.TileFindReplace(this);
            ZoomModel = new EditorZoomModel(this);
            Methods.Prefrences.SceneCurrentSettings.UpdateInstance(this);
            Methods.ProgramLauncher.UpdateInstance(this);
            UI = new EditorUI();
            Methods.Prefrences.SceneHistoryStorage.Initilize(this);
            ManiacEditor.Methods.Prefrences.DataStateHistoryStorage.Initilize(this);

            EditorStatusBar.UpdateFilterButtonApperance(true);

            this.Title = String.Format("Maniac Editor - Generations Edition {0}", Methods.ProgramBase.GetCasualVersion());

			ViewPanelContextMenu.Foreground = (SolidColorBrush)FindResource("NormalText");
			ViewPanelContextMenu.Background = (SolidColorBrush)FindResource("NormalBackground");




			AllocConsole();
			HideConsoleWindow();
			RefreshCollisionColours();
            ZoomModel.SetViewSize();
            UI.UpdateControls();
			Methods.Internal.Settings.TryLoadSettings();

            
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
			if (Classes.Editor.Solution.CurrentScene != null)
				return true;
			else
				return false;
		}
		public bool IsTilesEdit()
		{
			return Classes.Editor.Solution.EditLayerA != null;
		}
		public bool IsChunksEdit()
		{
			return EditorToolbar.ChunksToolButton.IsChecked.Value && Classes.Editor.Solution.EditLayerA != null;
		}
        public bool IsEntitiesEdit()
        {
            return EditorToolbar.EditEntities.IsCheckedN.Value || EditorToolbar.EditEntities.IsCheckedA.Value || EditorToolbar.EditEntities.IsCheckedB.Value;
		}
		public bool IsSelected(bool dualModeSelect = false)
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

			var result = System.Windows.MessageBox.Show($"The file '{fullFilePath}' already exists. Overwrite?", "Overwrite?",
										 MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes) return true;

			return false;
		}
        #endregion
		#region Common Editor Functions
		public void EditorPlaceTile(Point position, int tile, Classes.Editor.Scene.Sets.EditorLayer layer, bool isDrawing = false)
		{
            if (isDrawing)
            {
                double offset = (Classes.Editor.SolutionState.DrawBrushSize / 2) * Classes.Editor.Constants.TILE_SIZE;
                Point finalPosition = new Point((int)(position.X - offset), (int)(position.Y - offset));
                Dictionary<Point, ushort> tiles = new Dictionary<Point, ushort>();
                for (int x = 0; x < Classes.Editor.SolutionState.DrawBrushSize; x++)
                {
                    for (int y = 0; y < Classes.Editor.SolutionState.DrawBrushSize; y++)
                    {
                        if (!tiles.ContainsKey(new Point(x, y))) tiles.Add(new Point(x, y), (ushort)tile);
                    }
                }
                layer.DrawAsBrush(finalPosition, tiles);
            }
            else
            {
                Dictionary<Point, ushort> tiles = new Dictionary<Point, ushort>
                {
                    [new Point(0, 0)] = (ushort)tile
                };
                layer.PasteFromClipboard(position, tiles);
            }
		}
		public void DeleteSelected()
		{
            Classes.Editor.Solution.EditLayerA?.DeleteSelected();
            Classes.Editor.Solution.EditLayerB?.DeleteSelected();
			UI.UpdateEditLayerActions();

			if (IsEntitiesEdit())
			{
				Classes.Editor.Solution.Entities.DeleteSelected();
				UpdateLastEntityAction();
			}
		}
        public void UpdateLastEntityAction()
        {
            if (Classes.Editor.Solution.Entities.LastAction != null || Classes.Editor.Solution.Entities.LastActionInternal != null) RedoStack.Clear();
            if (Classes.Editor.Solution.Entities.LastAction != null)
			{
				UndoStack.Push(Classes.Editor.Solution.Entities.LastAction);
				Classes.Editor.Solution.Entities.LastAction = null;
			}
            if (Classes.Editor.Solution.Entities.LastActionInternal != null)
            {
                UndoStack.Push(Classes.Editor.Solution.Entities.LastActionInternal);
                Classes.Editor.Solution.Entities.LastActionInternal = null;
            }
            if (Classes.Editor.Solution.Entities.LastAction != null || Classes.Editor.Solution.Entities.LastActionInternal != null) UI.UpdateControls();

        }
		public void FlipEntities(FlipDirection direction)
		{
			Dictionary<Classes.Editor.Scene.Sets.EditorEntity, Point> initalPos = new Dictionary<Classes.Editor.Scene.Sets.EditorEntity, Point>();
			Dictionary<Classes.Editor.Scene.Sets.EditorEntity, Point> postPos = new Dictionary<Classes.Editor.Scene.Sets.EditorEntity, Point>();
			foreach (Classes.Editor.Scene.Sets.EditorEntity e in Classes.Editor.Solution.Entities.SelectedEntities)
			{
				initalPos.Add(e, new Point(e.PositionX, e.PositionY));
			}
			Classes.Editor.Solution.Entities.Flip(direction);
			EntitiesToolbar.UpdateCurrentEntityProperites();
			foreach (Classes.Editor.Scene.Sets.EditorEntity e in Classes.Editor.Solution.Entities.SelectedEntities)
			{
				postPos.Add(e, new Point(e.PositionX, e.PositionY));
			}
			IAction action = new ActionMultipleMoveEntities(initalPos, postPos);
			UndoStack.Push(action);
			RedoStack.Clear();

		}
        /// <summary>
        /// Deselects all tiles and Classes.Edit.Scene.EditorSolution.Entities
        /// </summary>
        /// <param name="updateControls">Whether to update associated on-screen controls</param>
        public void Deselect(bool updateControls = true)
        {
            if (IsEditing())
            {
                Classes.Editor.Solution.EditLayerA?.Deselect();
                Classes.Editor.Solution.EditLayerB?.Deselect();

                if (IsEntitiesEdit()) Classes.Editor.Solution.Entities.Deselect();
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
            DeviceModel.GraphicPanel.Render();
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
            DeviceModel.GraphicPanel.Render();
            UI.UpdateControls();
        }
        public void CopyTilesToClipboard(bool doNotUseWindowsClipboard = false)
        {
            bool hasMultipleValidLayers = Classes.Editor.Solution.EditLayerA != null && Classes.Editor.Solution.EditLayerB != null;
            if (!hasMultipleValidLayers)
            {
                Dictionary<Point, ushort> copyDataA = Classes.Editor.Solution.EditLayerA?.CopyToClipboard();
                Dictionary<Point, ushort> copyDataB = Classes.Editor.Solution.EditLayerB?.CopyToClipboard();
                Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> copyData = new Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>>(copyDataA, copyDataB);

                // Make a DataObject for the copied data and send it to the Windows clipboard for cross-instance copying
                if (!doNotUseWindowsClipboard)
                    Clipboard.SetDataObject(new DataObject("ManiacTiles", copyData), true);

                // Also copy to Maniac's clipboard in case it gets overwritten elsewhere
                TilesClipboard = copyData;
            }
            else if (hasMultipleValidLayers && Classes.Editor.SolutionState.MultiLayerEditMode)
            {
                Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> copyData = Classes.Editor.Scene.Sets.EditorLayer.CopyMultiSelectionToClipboard(Classes.Editor.Solution.EditLayerA, Classes.Editor.Solution.EditLayerB);

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
                List<Classes.Editor.Scene.Sets.EditorEntity> copyData = Classes.Editor.Solution.Entities.CopyToClipboard();

                /*
                // Prepare each Entity for the copy to release unnecessary data
                foreach (Classes.Edit.Scene.Sets.EditorEntity entity in copyData)
                   entity.PrepareForExternalCopy();

                CloseClipboard();

                // Make a DataObject for the data and send it to the Windows clipboard for cross-instance copying
                Clipboard.SetDataObject(new DataObject("ManiacEntities", copyData));*/

                // Send to Maniac's clipboard
                entitiesClipboard = copyData;
            }
        }

        public void PasteEntitiesToClipboard()
        {
            if (EntitiesToolbar.IsFocused.Equals(false))
            {
                try
                {

                    // check if there are Classes.Edit.Scene.EditorSolution.Entities on the Windows clipboard; if so, use those
                    if (System.Windows.Clipboard.ContainsData("ManiacEntities"))
                    {
                        Classes.Editor.Solution.Entities.PasteFromClipboard(new Point((int)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom)), (List<Classes.Editor.Scene.Sets.EditorEntity>)System.Windows.Clipboard.GetDataObject().GetData("ManiacEntities"));
                        UpdateLastEntityAction();
                    }

                    // if there's none, use the internal clipboard
                    else if (entitiesClipboard != null)
                    {
                        Classes.Editor.Solution.Entities.PasteFromClipboard(new Point((int)(Classes.Editor.SolutionState.LastX / Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.SolutionState.LastY / Classes.Editor.SolutionState.Zoom)), entitiesClipboard);
                        UpdateLastEntityAction();
                    }
                }
                catch (Classes.Editor.Scene.EditorEntities.TooManyEntitiesException)
                {
                    System.Windows.MessageBox.Show("Too many Classes.Edit.Scene.EditorSolution.Entities! (limit: 2048)");
                    return;
                }
                UI.UpdateEntitiesToolbarList();
                UI.SetSelectOnlyButtonsState();
            }
        }

        public void MoveEntityOrTiles(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            int x = 0, y = 0;
            int modifier = (IsChunksEdit() ? 8 : 1);
            if (Classes.Editor.SolutionState.UseMagnetMode)
            {
                switch (e.KeyData)
                {
                    case Keys.Up: y = (Classes.Editor.SolutionState.UseMagnetYAxis ? -Classes.Editor.SolutionState.MagnetSize : -1); break;
                    case Keys.Down: y = (Classes.Editor.SolutionState.UseMagnetYAxis ? Classes.Editor.SolutionState.MagnetSize : 1); break;
                    case Keys.Left: x = (Classes.Editor.SolutionState.UseMagnetXAxis ? -Classes.Editor.SolutionState.MagnetSize : -1); break;
                    case Keys.Right: x = (Classes.Editor.SolutionState.UseMagnetXAxis ? Classes.Editor.SolutionState.MagnetSize : 1); break;
                }
            }
            if (Classes.Editor.SolutionState.EnableFasterNudge)
            {
                if (Classes.Editor.SolutionState.UseMagnetMode)
                {
                    switch (e.KeyData)
                    {
                        case Keys.Up: y = (Classes.Editor.SolutionState.UseMagnetYAxis ? -Classes.Editor.SolutionState.MagnetSize * Classes.Editor.SolutionState.FasterNudgeAmount : -1 - Classes.Editor.SolutionState.FasterNudgeAmount); break;
                        case Keys.Down: y = (Classes.Editor.SolutionState.UseMagnetYAxis ? Classes.Editor.SolutionState.MagnetSize * Classes.Editor.SolutionState.FasterNudgeAmount : 1 + Classes.Editor.SolutionState.FasterNudgeAmount); break;
                        case Keys.Left: x = (Classes.Editor.SolutionState.UseMagnetXAxis ? -Classes.Editor.SolutionState.MagnetSize * Classes.Editor.SolutionState.FasterNudgeAmount : -1 - Classes.Editor.SolutionState.FasterNudgeAmount); break;
                        case Keys.Right: x = (Classes.Editor.SolutionState.UseMagnetXAxis ? Classes.Editor.SolutionState.MagnetSize * Classes.Editor.SolutionState.FasterNudgeAmount : 1 + Classes.Editor.SolutionState.FasterNudgeAmount); break;
                    }
                }
                else
                {
                    if (IsChunksEdit())
                    {
                        switch (e.KeyData)
                        {
                            case Keys.Up: y = -1 * modifier; break;
                            case Keys.Down: y = 1 * modifier; break;
                            case Keys.Left: x = -1 * modifier; break;
                            case Keys.Right: x = 1 * modifier; break;
                        }
                    }
                    else
                    {
                        switch (e.KeyData)
                        {
                            case Keys.Up: y = (-1 - Classes.Editor.SolutionState.FasterNudgeAmount) * modifier; break;
                            case Keys.Down: y = (1 + Classes.Editor.SolutionState.FasterNudgeAmount) * modifier; break;
                            case Keys.Left: x = (-1 - Classes.Editor.SolutionState.FasterNudgeAmount) * modifier; break;
                            case Keys.Right: x = (1 + Classes.Editor.SolutionState.FasterNudgeAmount) * modifier; break;
                        }
                    }

                }

            }
            if (Classes.Editor.SolutionState.UseMagnetMode == false && Classes.Editor.SolutionState.EnableFasterNudge == false)
            {
                switch (e.KeyData)
                {
                    case Keys.Up: y = -1 * modifier; break;
                    case Keys.Down: y = 1 * modifier; break;
                    case Keys.Left: x = -1 * modifier; break;
                    case Keys.Right: x = 1 * modifier; break;
                }

            }
            Classes.Editor.Solution.EditLayerA?.MoveSelectedQuonta(new Point(x, y));
            Classes.Editor.Solution.EditLayerB?.MoveSelectedQuonta(new Point(x, y));

            UI.UpdateEditLayerActions();

            if (IsEntitiesEdit())
            {
                if (Classes.Editor.SolutionState.UseMagnetMode)
                {
                    int xE = Classes.Editor.Solution.Entities.SelectedEntities[0].Entity.Position.X.High;
                    int yE = Classes.Editor.Solution.Entities.SelectedEntities[0].Entity.Position.Y.High;

                    if (xE % Classes.Editor.SolutionState.MagnetSize != 0 && Classes.Editor.SolutionState.UseMagnetXAxis)
                    {
                        int offsetX = x % Classes.Editor.SolutionState.MagnetSize;
                        x -= offsetX;
                    }
                    if (yE % Classes.Editor.SolutionState.MagnetSize != 0 && Classes.Editor.SolutionState.UseMagnetYAxis)
                    {
                        int offsetY = y % Classes.Editor.SolutionState.MagnetSize;
                        y -= offsetY;
                    }
                }


                Classes.Editor.Solution.Entities.MoveSelected(new Point(0, 0), new Point(x, y), false);
                EntitiesToolbar.UpdateCurrentEntityProperites();

                // Try to merge with last move
                List<Classes.Editor.Scene.Sets.EditorEntity> SelectedList = Classes.Editor.Solution.Entities.SelectedEntities.ToList();
                List<Classes.Editor.Scene.Sets.EditorEntity> SelectedInternalList = Classes.Editor.Solution.Entities.SelectedInternalEntities.ToList();
                bool selectedActionsState = UndoStack.Count > 0 && UndoStack.Peek() is ActionMoveEntities && (UndoStack.Peek() as ActionMoveEntities).UpdateFromKey(SelectedList, new Point(x, y));
                bool selectedInternalActionsState = UndoStack.Count > 0 && UndoStack.Peek() is ActionMoveEntities && (UndoStack.Peek() as ActionMoveEntities).UpdateFromKey(SelectedInternalList, new Point(x, y));

                if (selectedActionsState || selectedInternalActionsState) { }
                else
                {
                    if (SelectedList.Count != 0) UndoStack.Push(new ActionMoveEntities(SelectedList, new Point(x, y), true));
                    if (SelectedInternalList.Count != 0) UndoStack.Push(new ActionMoveEntities(SelectedInternalList, new Point(x, y), true));

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
            System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public void GoToPosition(int x, int y, bool CenterCoords = true, bool ShortcutClear = false)
        {
            if (CenterCoords)
            {
                Rectangle r = DeviceModel.GraphicPanel.GetScreen();
                int x2 = (int)(r.Width * Classes.Editor.SolutionState.Zoom);
                int y2 = (int)(r.Height * Classes.Editor.SolutionState.Zoom);

                int ResultX = (int)(x * Classes.Editor.SolutionState.Zoom) - x2 / 2;
                int ResultY = (int)(y * Classes.Editor.SolutionState.Zoom) - y2 / 2;

                if ((ResultX <= 0)) ResultX = 0;
                if ((ResultY <= 0)) ResultY = 0;


                Classes.Editor.SolutionState.ViewPositionX = ResultX;
                Classes.Editor.SolutionState.ViewPositionY = ResultY;
            }
            else
            {
                int ResultX = (int)(x * Classes.Editor.SolutionState.Zoom);
                int ResultY = (int)(y * Classes.Editor.SolutionState.Zoom);

                if ((ResultX <= 0)) ResultX = 0;
                if ((ResultY <= 0)) ResultY = 0;

                Classes.Editor.SolutionState.ViewPositionX = ResultX;
                Classes.Editor.SolutionState.ViewPositionY = ResultY;
            }
        }

        #endregion
        #region GameConfig/Data Folders
        public string GetDataDirectory()
		{
			using (var folderBrowserDialog = new GenerationsLib.Core.FolderSelectDialog())
			{
				folderBrowserDialog.Title = "Select Data Folder";

				if (!folderBrowserDialog.ShowDialog())
					return null;

				return folderBrowserDialog.FileName;
			}
		}
		public bool SetGameConfig() { return Paths.SetGameConfig(); }
		public bool IsDataDirectoryValid(string directoryToCheck) { return Paths.IsDataDirectoryValid(directoryToCheck); }
        #endregion
        #region Open Scene Methods
        public void OpenSceneForceFully()
		{
			string dataDirectory = ManiacEditor.Core.Settings.MyDevSettings.DevForceRestartData;
			if (dataDirectory != null) DataDirectory = dataDirectory;
			string Result = ManiacEditor.Core.Settings.MyDevSettings.DevForceRestartScene;
			int LevelID = ManiacEditor.Core.Settings.MyDevSettings.DevForceRestartID;
			bool isEncore = ManiacEditor.Core.Settings.MyDevSettings.DevForceRestartIsEncore;
			string CurrentZone = ManiacEditor.Core.Settings.MyDevSettings.DevForceRestartCurrentZone;
			string CurrentName = ManiacEditor.Core.Settings.MyDevSettings.DevForceRestartCurrentName;
			string CurrentSceneID = ManiacEditor.Core.Settings.MyDevSettings.DevForceRestartSceneID;
			bool Browsed = ManiacEditor.Core.Settings.MyDevSettings.DevForceRestartIsBrowsed;
            IList<string> DevResourcePacks = new List<string>();
            if (ManiacEditor.Core.Settings.MyDevSettings.DevForceRestartResourcePacks != null) DevResourcePacks = ManiacEditor.Core.Settings.MyDevSettings.DevForceRestartResourcePacks.Cast<string>().ToList();
            int x = ManiacEditor.Core.Settings.MyDevSettings.DevForceRestartX;
			int y = ManiacEditor.Core.Settings.MyDevSettings.DevForceRestartY;

			FileHandler.OpenSceneFromSaveState(dataDirectory, Result, LevelID, isEncore, CurrentZone, CurrentZone, CurrentSceneID, Browsed, DevResourcePacks);

            GoToPosition(x, y, true);

        }
		private void OpenSceneForceFully(string dataDir, string scenePath, string modPath, int levelID, bool isEncoreMode, int X, int Y, double _ZoomScale = 0.0, string SceneID = "", string Zone = "", string Name = "")
		{
            System.Windows.MessageBox.Show("These Kind of Shortcuts are Broken for now! SORRY!");

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
			FileHandler.OpenSceneSelectWithPrefrences(DataDirectory);
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
            DeviceModel.GraphicPanel.Focus();
            if (TileManiacInstance.hasModified)
            {
                ReloadToolStripButton_Click(sender, null);
            }

        }
        private void Editor_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Methods.ProgramLauncher.ManiaPalConnector != null) Methods.ProgramLauncher.ManiaPalConnector.Kill();

            try
            {
                Methods.GameHandler.GameRunning = false;
                var mySettings = Properties.Settings.Default;
                ManiacEditor.Core.Settings.MySettings.IsMaximized = WindowState == System.Windows.WindowState.Maximized;
                Classes.Editor.Constants.SaveAllSettings();
            }
            catch (Exception ex)
            {
                Debug.Write("Failed to write settings: " + ex);
            }

            if (ManiaHost._process != null)
            {
                ManiaHost.ForceKillSonicMania();
            }

            DeviceModel.Dispose();
            //editorView = null;
            DeviceHost.Child.Dispose();
            //host = null;



        }
        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            var e2 = KeyEventExts.ToWinforms(e);
            if (e2 != null)
            {
                EditorControls.GraphicPanel_OnKeyDown(sender, e2);
            }

        }

        private void Editor_KeyUp(object sender, KeyEventArgs e)
        {
            var e2 = KeyEventExts.ToWinforms(e);
            if (e2 != null)
            {
                EditorControls.GraphicPanel_OnKeyUp(sender, e2);
            }

        }

        public void Editor_Resize(object sender, RoutedEventArgs e) { ZoomModel.Resize(sender, e); }
        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            // Create the interop host control.
            DeviceHost = new System.Windows.Forms.Integration.WindowsFormsHost();

            // Create the MaskedTextBox control.

            // Assign the MaskedTextBox control as the host control's child.
            DeviceHost.Child = DeviceModel;

            DeviceHost.Foreground = (SolidColorBrush)FindResource("NormalText");

            // Add the interop host control to the Grid
            // control's collection of child controls.
            this.ViewPanelForm.Children.Add(DeviceHost);

            DeviceModel.GraphicPanel.Init(DeviceModel);
        }
        #endregion

        #region Splitter Events
        private void Spliter_DragDelta(object sender, DragDeltaEventArgs e) { ZoomModel.Resize(sender, e); }
        private void Spliter_SizeChanged(object sender, SizeChangedEventArgs e) { ZoomModel.SetZoomLevel(Classes.Editor.SolutionState.ZoomLevel, new System.Drawing.Point(Classes.Editor.SolutionState.ViewPositionX, Classes.Editor.SolutionState.ViewPositionY), 0.0, false); }
        #endregion


        public void DrawLayers(int drawOrder = 0)
		{

            // Future Implementation

            /*
            List<int> layerDrawingOrder = new List<int> { };
            var allLayers = Classes.Edit.Scene.EditorSolution.Scene.AllLayers;
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

            var _extraViewLayer = Classes.Editor.Solution.CurrentScene.LayerByDrawingOrder.FirstOrDefault(el => el.Layer.DrawingOrder.Equals(drawOrder));
			_extraViewLayer.Draw(DeviceModel.GraphicPanel);
		}
		public void Run()
		{
			Show();
			Focus();
			DeviceModel.Show();
			DeviceModel.GraphicPanel.Run();

		}
        public void UpdateStartScreen(bool visible, bool firstLoad = false)
        {
            if (firstLoad)
            {
                this.OverlayPanel.Children.Add(StartScreen);
                StartScreen.SelectScreen.ReloadRecentsTree();
                this.ViewPanelForm.Visibility = Visibility.Hidden;
                UI.UpdateToolbars(false, false, true);
                RefreshRecentScenes();
                RefreshDataSources();
            }
            if (visible)
            {
                StartScreen.Visibility = Visibility.Visible;
                StartScreen.SelectScreen.ReloadRecentsTree();
                this.ViewPanelForm.Visibility = Visibility.Hidden;
                UI.UpdateToolbars(false, false, true);
                RefreshRecentScenes();
                RefreshDataSources();
            }
            else
            {
                StartScreen.Visibility = Visibility.Hidden;
                StartScreen.SelectScreen.ReloadRecentsTree();
                this.ViewPanelForm.Visibility = Visibility.Visible;
                UI.UpdateToolbars(false, false, false);
            }

        }

        #endregion
        #region Asset Reloading
        public void ReloadSpecificTextures(object sender, RoutedEventArgs e)
		{
			try
			{
				// release all our resources, and force a reload of the tiles
				// Classes.Edit.Scene.EditorSolution.Entities should take care of themselves
				DisposeTextures();

				if (Classes.Editor.SolutionState.UseEncoreColors)
				{
                    if (Classes.Editor.Solution.CurrentTiles != null) Classes.Editor.Solution.CurrentTiles?.Image.Reload(EncorePalette[0]);
				}
				else
				{
                    if (Classes.Editor.Solution.CurrentTiles != null) Classes.Editor.Solution.CurrentTiles?.Image.Reload();
				}

			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show(ex.Message);
			}
		}
		public void DisposeTextures()
		{
            if (Classes.Editor.Solution.CurrentScene != null)
            {
                // Make sure to dispose the textures of the extra layers too
                if (Classes.Editor.Solution.CurrentTiles != null) Classes.Editor.Solution.CurrentTiles?.DisposeTextures();
                if (Classes.Editor.Solution.FGHigh != null) Classes.Editor.Solution.FGHigh.DisposeTextures();
                if (Classes.Editor.Solution.FGLow != null) Classes.Editor.Solution.FGLow.DisposeTextures();
                if (Classes.Editor.Solution.FGHigher != null) Classes.Editor.Solution.FGHigher.DisposeTextures();
                if (Classes.Editor.Solution.FGLower != null) Classes.Editor.Solution.FGLower.DisposeTextures();

                foreach (var el in Classes.Editor.Solution.CurrentScene.OtherLayers)
                {
                    el.DisposeTextures();
                }
            }
		}
		public void RefreshCollisionColours(bool RefreshMasks = false)
		{
			if (Classes.Editor.Solution.CurrentScene != null && Classes.Editor.Solution.CurrentTiles != null)
			{
                switch (Classes.Editor.SolutionState.CollisionPreset)
                {
                    case 2:
                        CollisionAllSolid = Classes.Editor.SolutionState.CollisionSAColour;
						CollisionTopOnlySolid = Classes.Editor.SolutionState.CollisionTOColour;
						CollisionLRDSolid = Classes.Editor.SolutionState.CollisionLRDColour;
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
                    //UI.ReloadSpritesAndTextures();
                }
			}

		}
		#endregion
		#region Get + Set Methods
		public Rectangle GetScreen()
		{
			if (ManiacEditor.Core.Settings.MySettings.EntityFreeCam) return new Rectangle(Classes.Editor.SolutionState.CustomX, Classes.Editor.SolutionState.CustomY, DeviceModel.mainPanel.Width, DeviceModel.mainPanel.Height);
			else return new Rectangle(Classes.Editor.SolutionState.ViewPositionX, Classes.Editor.SolutionState.ViewPositionY, DeviceModel.mainPanel.Width, DeviceModel.mainPanel.Height);
		}
		public double GetZoom()
		{
			return Classes.Editor.SolutionState.Zoom;
		}
		public Scene GetSceneSelection()
		{
			string selectedScene;

			ManiacEditor.Controls.SceneSelect.SceneSelectWindow select = new ManiacEditor.Controls.SceneSelect.SceneSelectWindow(Classes.Editor.Solution.GameConfig, this);
			select.Owner = Window.GetWindow(this);
			select.ShowDialog();
			if (select.SceneSelect.SelectedSceneResult == null)
				return null;
			selectedScene = select.SceneSelect.SelectedSceneResult;

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
        public void OpenSceneEvent(object sender, RoutedEventArgs e) { FileHandler.OpenScene(); }
        public void OpenDataDirectoryEvent(object sender, RoutedEventArgs e) { FileHandler.OpenDataDirectory(); }
        public void SaveSceneEvent(object sender, RoutedEventArgs e) { FileHandler.Save(); }
        public void SaveSceneAsEvent(object sender, RoutedEventArgs e) { FileHandler.SaveAs(); }
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
		public void ReloadToolStripButton_Click(object sender, RoutedEventArgs e) { UI.ReloadSpritesAndTextures(); }
		public void ToggleSlotIDEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.ShowTileID ^= true; }
        public void ToggleFasterNudgeEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.EnableFasterNudge ^= true; }
        public void ShowCollisionAEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.ShowCollisionA ^= true; }
        public void ShowCollisionBEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.ShowCollisionB ^= true; }
        public void ToggleDebugHUDEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.DebugStatsVisibleOnPanel ^= true; }
        public void MenuButtonChangedEvent(string tag) { UIEvents.SetMenuButtonType(tag); }

        #region Grid Events
        public void ToggleGridEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.ShowGrid ^= true; }
        #endregion

        #region Apps
        private void TileManiacEditTileEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.TileManiacIntergration(); }
        #endregion

        #region Settings and Other Menu Events
        public void AboutScreenEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.AboutScreen(); }
        public void ImportObjectsToolStripMenuItem_Click(Window window = null) { Methods.ProgramLauncher.ImportObjectsToolStripMenuItem_Click(window); }
        public void ImportObjectsWithMegaList(Window window = null) { Methods.ProgramLauncher.ImportObjectsWithMegaList(window); }
        public void ImportSoundsEvent(Window window = null) { Methods.ProgramLauncher.ImportSounds(window); }
        public void OptionsMenuEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OptionsMenu(); }
        #endregion

        #region Game Running Events
        private void MoveThePlayerToHere(object sender, RoutedEventArgs e) { Methods.GameHandler.MoveThePlayerToHere(); }
        private void SetPlayerRespawnToHere(object sender, RoutedEventArgs e) { Methods.GameHandler.SetPlayerRespawnToHere(); }
        private void MoveCheckpoint(object sender, RoutedEventArgs e) { Methods.GameHandler.CheckpointSelected = true; }
        private void RemoveCheckpoint(object sender, RoutedEventArgs e) { Methods.GameHandler.UpdateCheckpoint(new Point(0, 0), false); }
        private void AssetReset(object sender, RoutedEventArgs e) { Methods.GameHandler.AssetReset(); }
        private void RestartScene(object sender, RoutedEventArgs e) { Methods.GameHandler.RestartScene(); }
        #endregion

        #endregion
        #region Layer Toolbar Events
		public void SetupLayerButtons()
		{
			TearDownExtraLayerButtons();
			IList<EditLayerToggleButton> _extraLayerEditButtons = new List<EditLayerToggleButton>(); //Used for Extra Layer Edit Buttons
			IList<EditLayerToggleButton> _extraLayerViewButtons = new List<EditLayerToggleButton>(); //Used for Extra Layer View Buttons

			//EDIT BUTTONS
			foreach (Classes.Editor.Scene.Sets.EditorLayer el in Classes.Editor.Solution.CurrentScene.OtherLayers)
			{
				EditLayerToggleButton tsb = new EditLayerToggleButton()
				{
					Text = el.Name,
                    LayerName = "Edit" + el.Name
				};
                EditorToolbar.LayerToolbar.Items.Add(tsb);
                tsb.DualSelect = true;
                tsb.TextForeground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Color.LawnGreen.A, Color.LawnGreen.R, Color.LawnGreen.G, Color.LawnGreen.B));
				tsb.RightClick += AdHocLayerEdit_RightClick;
                tsb.IsLayerOptionsEnabled = true;

                tsb.Click += AdHocLayerEdit_Click;

				_extraLayerEditButtons.Add(tsb);
			}

			//EDIT BUTTONS SEPERATOR
			Separator tss = new Separator();
            EditorToolbar.LayerToolbar.Items.Add(tss);
			ExtraLayerSeperators.Add(tss);

			//VIEW BUTTONS
			foreach (Classes.Editor.Scene.Sets.EditorLayer el in Classes.Editor.Solution.CurrentScene.OtherLayers)
			{
				EditLayerToggleButton tsb = new EditLayerToggleButton()
				{
					Text = el.Name,
                    LayerName = "Show" + el.Name.Replace(" ", "")
				};
                EditorToolbar.LayerToolbar.Items.Insert(EditorToolbar.LayerToolbar.Items.IndexOf(EditorToolbar.extraViewLayersSeperator), tsb);
				tsb.TextForeground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, Color.FromArgb(0x33AD35).R, Color.FromArgb(0x33AD35).G, Color.FromArgb(0x33AD35).B));
                tsb.IsLayerOptionsEnabled = true;


                _extraLayerViewButtons.Add(tsb);
			}

			//EDIT + VIEW BUTTONS LIST
			for (int i = 0; i < _extraLayerViewButtons.Count; i++)
			{
				ExtraLayerEditViewButtons.Add(_extraLayerViewButtons[i], _extraLayerEditButtons[i]);
			}

            UpdateDualButtonsControlsForLayer(Classes.Editor.Solution.FGLow, EditorToolbar.ShowFGLow, EditorToolbar.EditFGLow);
			UpdateDualButtonsControlsForLayer(Classes.Editor.Solution.FGHigh, EditorToolbar.ShowFGHigh, EditorToolbar.EditFGHigh);
			UpdateDualButtonsControlsForLayer(Classes.Editor.Solution.FGLower, EditorToolbar.ShowFGLower, EditorToolbar.EditFGLower);
			UpdateDualButtonsControlsForLayer(Classes.Editor.Solution.FGHigher, EditorToolbar.ShowFGHigher, EditorToolbar.EditFGHigher);
		}
		public void TearDownExtraLayerButtons()
		{
			foreach (var elb in ExtraLayerEditViewButtons)
			{
                EditorToolbar.LayerToolbar.Items.Remove(elb.Key);
                elb.Value.Click -= AdHocLayerEdit_Click;
				elb.Value.RightClick -= AdHocLayerEdit_RightClick;
                EditorToolbar.LayerToolbar.Items.Remove(elb.Value);
			}
			ExtraLayerEditViewButtons.Clear();


			foreach (var els in ExtraLayerSeperators)
			{
                EditorToolbar.LayerToolbar.Items.Remove(els);
			}
			ExtraLayerSeperators.Clear();

		}
		/// <summary>
		/// Given a scene layer, configure the given visibiltiy and edit buttons which will control that layer.
		/// </summary>
		/// <param name="layer">The layer of the scene from which to extract a name.</param>
		/// <param name="visibilityButton">The button which controls the visibility of the layer.</param>
		/// <param name="editButton">The button which controls editing the layer.</param>
		private void UpdateDualButtonsControlsForLayer(Classes.Editor.Scene.Sets.EditorLayer layer, ToggleButton visibilityButton, EditLayerToggleButton editButton)
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
			if (ClickType == MouseButton.Left && !Classes.Editor.SolutionState.MultiLayerEditMode) Normal();
			else if (ClickType == MouseButton.Left && Classes.Editor.SolutionState.MultiLayerEditMode) LayerA();
			else if (ClickType == MouseButton.Right && Classes.Editor.SolutionState.MultiLayerEditMode) LayerB();

			void Normal()
			{
				EditLayerToggleButton tsb = sender as EditLayerToggleButton;
				Deselect(false);
				if (tsb.IsCheckedN.Value)
				{
					if (!ManiacEditor.Core.Settings.MySettings.KeepLayersVisible)
					{
                        EditorToolbar.ShowFGLow.IsChecked = false;
                        EditorToolbar.ShowFGHigh.IsChecked = false;
                        EditorToolbar.ShowFGLower.IsChecked = false;
                        EditorToolbar.ShowFGHigher.IsChecked = false;
					}
                    EditorToolbar.EditFGLow.ClearCheckedItems(3);
                    EditorToolbar.EditFGHigh.ClearCheckedItems(3);
                    EditorToolbar.EditFGLower.ClearCheckedItems(3);
                    EditorToolbar.EditFGHigher.ClearCheckedItems(3);
                    EditorToolbar.EditEntities.ClearCheckedItems(3);

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
					if (!ManiacEditor.Core.Settings.MySettings.KeepLayersVisible)
					{
                        EditorToolbar.ShowFGLow.IsChecked = false;
                        EditorToolbar.ShowFGHigh.IsChecked = false;
                        EditorToolbar.ShowFGLower.IsChecked = false;
                        EditorToolbar.ShowFGHigher.IsChecked = false;
					}
                    EditorToolbar.EditFGLow.ClearCheckedItems(1);
                    EditorToolbar.EditFGHigh.ClearCheckedItems(1);
                    EditorToolbar.EditFGLower.ClearCheckedItems(1);
                    EditorToolbar.EditFGHigher.ClearCheckedItems(1);
                    EditorToolbar.EditEntities.ClearCheckedItems(1);

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
					if (!ManiacEditor.Core.Settings.MySettings.KeepLayersVisible)
					{
                        EditorToolbar.ShowFGLow.IsChecked = false;
                        EditorToolbar.ShowFGHigh.IsChecked = false;
                        EditorToolbar.ShowFGLower.IsChecked = false;
                        EditorToolbar.ShowFGHigher.IsChecked = false;
					}
                    EditorToolbar.EditFGLow.ClearCheckedItems(2);
                    EditorToolbar.EditFGHigh.ClearCheckedItems(2);
                    EditorToolbar.EditFGLower.ClearCheckedItems(2);
                    EditorToolbar.EditFGHigher.ClearCheckedItems(2);
                    EditorToolbar.EditEntities.ClearCheckedItems(2);

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
        #region Recent Data Folder Methods
        public void ResetDataDirectoryToAndResetScene(string newDataDirectory, bool forceBrowse = false, bool forceSceneSelect = false)
        {
            if (FileHandler.AllowSceneUnloading() != true) return;
            Classes.Editor.Solution.UnloadScene();
            Methods.Internal.Settings.UseDefaultPrefrences();
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
        public void UpdateDataFolderLabel(object sender, RoutedEventArgs e)
        {
            string dataFolderTag_Normal = "Data Directory: {0}";

            EditorStatusBar._baseDataDirectoryLabel.Tag = dataFolderTag_Normal;
            UpdateDataFolderLabel();
            Classes.Editor.SolutionState.ShowingDataDirectory = true;
        }
        private void UpdateDataFolderLabel(string dataDirectory = null)
        {
            if (dataDirectory != null) EditorStatusBar._baseDataDirectoryLabel.Content = string.Format(EditorStatusBar._baseDataDirectoryLabel.Tag.ToString(), dataDirectory);
            else EditorStatusBar._baseDataDirectoryLabel.Content = string.Format(EditorStatusBar._baseDataDirectoryLabel.Tag.ToString(), DataDirectory);
        }
        public void AddRecentDataFolder(string dataDirectory)
        {
            try
            {
                var mySettings = Properties.Settings.Default;
                var dataDirectories = ManiacEditor.Core.Settings.MySettings.DataDirectories;

                if (dataDirectories == null)
                {
                    dataDirectories = new StringCollection();
                    ManiacEditor.Core.Settings.MySettings.DataDirectories = dataDirectories;
                }

                if (dataDirectories.Contains(dataDirectory)) dataDirectories.Remove(dataDirectory);

                if (dataDirectories.Count >= 10)
                {
                    for (int i = 9; i < dataDirectories.Count; i++) dataDirectories.RemoveAt(i);
                }

                dataDirectories.Insert(0, dataDirectory);

                ManiacEditor.Core.Settings.MySettings.Save();

                UpdateDataFolderLabel(dataDirectory);


            }
            catch (Exception ex)
            {
                Debug.Write("Failed to add data folder to recent list: " + ex);
            }
        }
        #endregion
        #region Recent Scenes Methods

        public void RefreshRecentScenes()
        {
            if (Methods.Prefrences.SceneHistoryStorage.Collection.List.Count > 0)
            {

                EditorMenuBar.NoRecentScenesItem.Visibility = Visibility.Collapsed;
                StartScreen.NoRecentsLabel1.Visibility = Visibility.Collapsed;
                CleanUpRecentScenesList();

                foreach (var RecentItem in Methods.Prefrences.SceneHistoryStorage.Collection.List)
                {
                    RecentSceneItems.Add(new Tuple<MenuItem, MenuItem>(CreateRecentScenesMenuLink(RecentItem.EntryName), CreateRecentScenesMenuLink(RecentItem.EntryName, true)));
                }

                foreach (var menuItem in RecentSceneItems.Reverse())
                {
                    EditorMenuBar.RecentScenes.Items.Insert(0, menuItem.Item1);
                    StartScreen.RecentScenesList.Children.Insert(0, menuItem.Item2);
                }
            }
            else
            {
                EditorMenuBar.NoRecentScenesItem.Visibility = Visibility.Visible;
                StartScreen.NoRecentsLabel1.Visibility = Visibility.Visible;
            }
        }

        private MenuItem CreateRecentScenesMenuLink(string target, bool startScreenEntry = false)
        {

            MenuItem newItem = new MenuItem();
            TextBlock label = new TextBlock();

            label.Text = target.Replace("/n/n", Environment.NewLine);
            newItem.Tag = target;
            newItem.Header = label;
            newItem.Click += RecentSceneEntryClicked;
            return newItem;
        }
        public void RecentSceneEntryClicked(object sender, RoutedEventArgs e)
        {
            if (FileHandler.AllowSceneUnloading() != true) return;
            Classes.Editor.Solution.UnloadScene();
            var menuItem = sender as MenuItem;
            string entryName = menuItem.Tag.ToString();
            var item = Methods.Prefrences.SceneHistoryStorage.Collection.List.Where(x => x.EntryName == entryName).FirstOrDefault();
            DataDirectory = item.DataDirectory;
            FileHandler.OpenSceneFromSaveState(item.DataDirectory, item.Result, item.LevelID, item.isEncore, item.CurrentName, item.CurrentZone, item.CurrentSceneID, item.Browsed, item.ResourcePacks);
            Methods.Prefrences.SceneHistoryStorage.AddRecentFile(item);
        }
        private void CleanUpRecentScenesList()
        {
            foreach (var menuItem in RecentSceneItems)
            {
                menuItem.Item1.Click -= RecentSceneEntryClicked;
                menuItem.Item2.Click -= RecentSceneEntryClicked;
                EditorMenuBar.RecentScenes.Items.Remove(menuItem.Item1);
                StartScreen.RecentScenesList.Children.Remove(menuItem.Item2);
            }
            RecentSceneItems.Clear();
        }

        #endregion
        #region Recent Data Sources Methods


        public void RefreshDataSources()
        {
            if (ManiacEditor.Methods.Prefrences.DataStateHistoryStorage.Collection.List.Count > 0)
            {

                EditorMenuBar.NoRecentDataSources.Visibility = Visibility.Collapsed;
                StartScreen.NoRecentsLabel2.Visibility = Visibility.Collapsed;

                CleanUpDataSourcesList();

                foreach (var RecentItem in ManiacEditor.Methods.Prefrences.DataStateHistoryStorage.Collection.List)
                {
                    RecentDataSourceItems.Add(new Tuple<MenuItem, MenuItem>(CreateRecentDataSourceMenuLink(RecentItem.EntryName), CreateRecentDataSourceMenuLink(RecentItem.EntryName, true)));
                }

                foreach (var menuItem in RecentDataSourceItems.Reverse())
                {
                    EditorMenuBar.RecentDataSources.Items.Insert(0, menuItem.Item1);
                    StartScreen.RecentDataContextList.Children.Insert(0, menuItem.Item2);
                }
            }
            else
            {
                EditorMenuBar.NoRecentDataSources.Visibility = Visibility.Visible;
                StartScreen.NoRecentsLabel2.Visibility = Visibility.Visible;
            }
        }

        private MenuItem CreateRecentDataSourceMenuLink(string target, bool wrapText = false)
        {
            MenuItem newItem = new MenuItem();
            TextBlock label = new TextBlock();
            label.Text = target.Replace("/n/n", Environment.NewLine);
            if (wrapText) label.TextWrapping = TextWrapping.Wrap;
            newItem.Header = label;
            newItem.Tag = target;
            newItem.Click += RecentDataSourceEntryClicked;
            return newItem;
        }
        public void RecentDataSourceEntryClicked(object sender, RoutedEventArgs e)
        {
            if (FileHandler.AllowSceneUnloading() != true) return;
            Classes.Editor.Solution.UnloadScene();
            var menuItem = sender as MenuItem;
            string entryName = menuItem.Tag.ToString();
            var item = ManiacEditor.Methods.Prefrences.DataStateHistoryStorage.Collection.List.Where(x => x.EntryName == entryName).FirstOrDefault();
            DataDirectory = item.DataDirectory;
            FileHandler.OpenSceneSelectFromPreviousConfiguration(item);
            ManiacEditor.Methods.Prefrences.DataStateHistoryStorage.AddRecentFile(item);
        }
        private void CleanUpDataSourcesList()
        {
            foreach (var menuItem in RecentDataSourceItems)
            {
                menuItem.Item1.Click -= RecentDataSourceEntryClicked;
                menuItem.Item2.Click -= RecentDataSourceEntryClicked;
                EditorMenuBar.RecentDataSources.Items.Remove(menuItem.Item1);
                StartScreen.RecentDataContextList.Children.Remove(menuItem.Item2);
            }
            RecentDataSourceItems.Clear();

        }


        #endregion

        private void LeftToolbarToolbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UI != null && ZoomModel != null && StartScreen.Visibility != Visibility.Visible)
            {
                if (LeftToolbarToolbox.SelectedIndex == 0)
                {
                    UI.UpdateToolbars(false, false);
                }
                else
                {
                    UI.UpdateToolbars(false, true);
                }
                ManiacEditor.Controls.Base.MainEditor.Instance.Editor_Resize(null, null);
            }

        }


        #endregion
    }
}
