﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SharpDX;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections.Specialized;
using System.Windows.Forms.PropertyGridInternal;
using Cyotek.Windows.Forms;
using ManiacEditor.Interfaces;
using Keys = System.Windows.Forms.Keys;
using KeysConverter = System.Windows.Forms.KeysConverter;
using MessageBox = RSDKrU.MessageBox;

namespace ManiacEditor.Interfaces
{
	/// <summary>
	/// Interaction logic for OptionsMenu.xaml
	/// </summary>
	public partial class OptionsMenu : Window
	{
		bool collisionColorsRadioGroupCheckChangeAllowed = true;
		public Editor EditorInstance;
		System.Windows.Forms.Timer CheckGraphicalSettingTimer;
		public OptionsMenu(Editor instance)
		{
			InitializeComponent();
			EditorInstance = instance;

			CheckGraphicalSettingTimer = new System.Windows.Forms.Timer();
			CheckGraphicalSettingTimer.Interval = 10;
			CheckGraphicalSettingTimer.Tick += CheckGraphicalPresetModeState;

			if (Settings.MyDefaults.ScrollLockDirectionDefault == true) radioButtonX.IsChecked = true;
			else radioButtonY.IsChecked = true;

            if (Settings.MyDefaults.SceneSelectFilesViewDefault) SceneSelectRadio2.IsChecked = true;
            else SceneSelectRadio1.IsChecked = true;

            collisionColorsRadioGroupUpdate(Settings.MyDefaults.DefaultCollisionColors);
			collisionColorsRadioGroupCheckChangeAllowed = true;
			if (Settings.MyDefaults.DefaultGridSizeOption == 0) uncheckOtherGridDefaults(1);
			if (Settings.MyDefaults.DefaultGridSizeOption == 1) uncheckOtherGridDefaults(2);
			if (Settings.MyDefaults.DefaultGridSizeOption == 2) uncheckOtherGridDefaults(3);
			if (Settings.MyDefaults.DefaultGridSizeOption == 3) uncheckOtherGridDefaults(4);

			foreach (RadioButton rdo in Extensions.FindVisualChildren<RadioButton>(MenuLangGroup))
			{
				if (rdo.Tag.ToString() == Settings.MyDefaults.MenuLanguageDefault)
				{
					rdo.IsChecked = true;
				}
			}

			foreach (RadioButton rdo in Extensions.FindVisualChildren<RadioButton>(ButtonLayoutGroup))
			{
				if (rdo.Tag.ToString() == Settings.MyDefaults.MenuButtonLayoutDefault)
				{
					rdo.IsChecked = true;
				}
			}



			if (Settings.MySettings.NightMode)
			{
				DarkModeCheckBox.IsChecked = true;
			}

			CheckGraphicalPresetModeState(null, null);

			SetKeybindTextboxes();
			UpdateCustomColors();

		}

		private void UpdateCustomColors()
		{
			CSAC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Settings.MyDefaults.CollisionSAColour.A, Settings.MyDefaults.CollisionSAColour.R, Settings.MyDefaults.CollisionSAColour.G, Settings.MyDefaults.CollisionSAColour.B));
			SSTOC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Settings.MyDefaults.CollisionTOColour.A, Settings.MyDefaults.CollisionTOColour.R, Settings.MyDefaults.CollisionTOColour.G, Settings.MyDefaults.CollisionTOColour.B));
			CSLRDC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Settings.MyDefaults.CollisionLRDColour.A, Settings.MyDefaults.CollisionLRDColour.R, Settings.MyDefaults.CollisionLRDColour.G, Settings.MyDefaults.CollisionLRDColour.B));
			WLC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Settings.MyDefaults.WaterEntityColorDefault.A, Settings.MyDefaults.WaterEntityColorDefault.R, Settings.MyDefaults.WaterEntityColorDefault.G, Settings.MyDefaults.WaterEntityColorDefault.B));
			GDC.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(Settings.MyDefaults.DefaultGridColor.A, Settings.MyDefaults.DefaultGridColor.R, Settings.MyDefaults.DefaultGridColor.G, Settings.MyDefaults.DefaultGridColor.B));
		}

		private void CheckGraphicalPresetModeState(object sender, EventArgs e)
		{
			minimalRadioButton2.IsChecked = false;
			basicRadioButton2.IsChecked = false;
			superRadioButton2.IsChecked = false;
			hyperRadioButton2.IsChecked = false;
			customRadioButton2.IsChecked = false;


			if (EditorSettings.isMinimalPreset())
			{
				minimalRadioButton2.IsChecked = true;
			}
			else if (EditorSettings.isBasicPreset())
			{
				basicRadioButton2.IsChecked = true;
			}
			else if (EditorSettings.isSuperPreset())
			{
				superRadioButton2.IsChecked = true;
			}
			else if (EditorSettings.isHyperPreset())
			{
				hyperRadioButton2.IsChecked = true;
			}
			else
			{
				customRadioButton2.IsChecked = true;
			}
		}

		private void radioButton1_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (SceneSelectRadio2 != null)
			{
				if (SceneSelectRadio2.IsChecked == true)
				{
					Settings.MyDefaults.SceneSelectFilesViewDefault = true;
				}
				else
				{
					Settings.MyDefaults.SceneSelectFilesViewDefault = false;

				}
			}
		}

		private void radioButton2_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (SceneSelectRadio2 != null)
			{
				if (SceneSelectRadio2.IsChecked == true)
				{
					Settings.MyDefaults.SceneSelectFilesViewDefault = true;
				}
				else
				{
					Settings.MyDefaults.SceneSelectFilesViewDefault = false;

				}
			}

		}

		private void radioButtonY_CheckedChanged_1(object sender, RoutedEventArgs e)
		{
			if (radioButtonY != null)
			{
				if (radioButtonY.IsChecked == true)
				{
					Settings.MyDefaults.ScrollLockDirectionDefault = true;
				}
				else
				{
					Settings.MyDefaults.ScrollLockDirectionDefault = false;

				}
			}
		}

		private void radioButtonX_CheckedChanged_1(object sender, RoutedEventArgs e)
		{
			if (radioButtonX != null)
			{
				if (radioButtonX.IsChecked == true)
				{
					Settings.MyDefaults.ScrollLockDirectionDefault = false;
				}
				else
				{
					Settings.MyDefaults.ScrollLockDirectionDefault = true;

				}
			}

		}

        private void ResetToDefault(object sender, RoutedEventArgs e)
        {
            if (MainSettings.IsSelected)
            {
                ResetSettingsToDefault(sender, e);
            }
            else if (Controls.IsSelected)
            {
                ResetControlsToDefault(sender, e);
            }
        }

        private void ResetSettingsToDefault(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to wipe your settings?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				Settings.MySettings.Reset();
			}
			else
			{

			}
		}

		private void ResetControlsToDefault(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to reset your control configuration?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				Properties.KeyBinds.Default.Reset();
			}
			else
			{

			}
		}

		private void RPCCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (Settings.MySettings.ShowDiscordRPC == false)
			{
				RPCCheckBox.IsChecked = true;
				Settings.MySettings.ShowDiscordRPC = true;
				EditorInstance.Discord.UpdateDiscord(EditorInstance.Discord.ScenePath);
			}
			else
			{
				RPCCheckBox.IsChecked = false;
				Settings.MySettings.ShowDiscordRPC = false;
				EditorInstance.Discord.UpdateDiscord();
			}
		}

		private void button5_Click(object sender, RoutedEventArgs e)
		{
			EditorSettings.exportSettings();
		}

		private void importOptionsButton_Click(object sender, RoutedEventArgs e)
		{
			EditorSettings.importSettings();
		}

		private void button11_Click(object sender, RoutedEventArgs e)
		{
			String title = "Save Settings";
			String details = "Are you sure you want to save your settings, if the editor breaks because of one of these settings, you will have to redownload or manually reset you editor's config file! It's best you use the OK button to 'test' out the features before you save them.";
			if (MessageBox.Show(details, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				Settings.MySettings.Save();
			}
			else
			{
				return;
			}
		}

        private void ModLoader_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Title = "Select Mania Mod Manager.exe";
            ofd.Filter = "Windows PE Executable|*.exe";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Settings.MyDefaults.ModLoaderPath = ofd.FileName;
        }

        private void SonicMania_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Title = "Select Sonic Mania.exe";
            ofd.Filter = "Windows PE Executable|*.exe";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Settings.MyDefaults.SonicManiaPath = ofd.FileName;
        }

        private void button15_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new System.Windows.Forms.OpenFileDialog();
			ofd.Title = "Select RSDK Animation Editor.exe";
			ofd.Filter = "Windows PE Executable|*.exe";
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				Settings.MyDefaults.AnimationEditorPath = ofd.FileName;
		}

		private void uncheckOtherGridDefaults(int i)
		{
			switch (i)
			{
				case 1:
                    Settings.MyDefaults.DefaultGridSizeOption = 0;
					x16checkbox.IsChecked = true;
					x128checkbox.IsChecked = false;
					x256checkbox.IsChecked = false;
					customGridCheckbox.IsChecked = false;
					break;
				case 2:
                    Settings.MyDefaults.DefaultGridSizeOption = 1;
                    x16checkbox.IsChecked = false;
					x128checkbox.IsChecked = true;
					x256checkbox.IsChecked = false;
					customGridCheckbox.IsChecked = false;
					break;
				case 3:
                    Settings.MyDefaults.DefaultGridSizeOption = 2;
                    x16checkbox.IsChecked = false;
					x128checkbox.IsChecked = false;
					x256checkbox.IsChecked = true;
					customGridCheckbox.IsChecked = false;
					break;
				case 4:
                    Settings.MyDefaults.DefaultGridSizeOption = 3;
                    x16checkbox.IsChecked = false;
					x128checkbox.IsChecked = false;
					x256checkbox.IsChecked = false;
					customGridCheckbox.IsChecked = true;
					break;
					/*default:
						//x16checkbox.Checked = true; //Default
						x128checkbox.Checked = false;
						x256checkbox.Checked = false;
						customGridCheckbox.Checked = false;
						break;*/
			}


		}

		private void x16checkbox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (x16checkbox.IsChecked == true)
			{
				uncheckOtherGridDefaults(1);
			}
		}

		private void X128checkbox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (x128checkbox.IsChecked == true)
			{
				uncheckOtherGridDefaults(2);
			}
		}

		private void customGridCheckbox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (customGridCheckbox.IsChecked == true)
			{
				uncheckOtherGridDefaults(4);
			}
		}

		private void x256checkbox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (x256checkbox.IsChecked == true)
			{
				uncheckOtherGridDefaults(3);
			}
		}

		private void collisionColorsRadioGroupUpdate(int type)
		{
			bool[] groups = new[] { false, false, false };
			for (int i = 0; i < 3; i++) if (type == i) groups[i] = true;
			if (collisionColorsRadioGroupCheckChangeAllowed == true)
			{
				collisionColorsRadioGroupCheckChangeAllowed = false;
				radioButton4.IsChecked = false || groups[0];
				radioButton3.IsChecked = false || groups[1];
				radioButton1.IsChecked = false || groups[2];
			}

		}

		private void comboBox8_DropDown(object sender, RoutedEventArgs e)
		{
			//Grid Default Color
			ColorPickerDialog colorSelect = new ColorPickerDialog();
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Settings.MyDefaults.DefaultGridColor = colorSelect.Color;
			}
		}

		private void comboBox7_DropDown(object sender, RoutedEventArgs e)
		{
			//Water Color
			ColorPickerDialog colorSelect = new ColorPickerDialog();
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Settings.MyDefaults.WaterEntityColorDefault = colorSelect.Color;
				EditorInstance.UIModes.waterColor = colorSelect.Color;
			}
		}

		private void comboBox6_DropDown(object sender, RoutedEventArgs e)
		{
			//Collision Solid(Top Only) Color
			ColorPickerDialog colorSelect = new ColorPickerDialog();
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Settings.MyDefaults.CollisionTOColour = colorSelect.Color;
			}
		}

		private void comboBox5_DropDown(object sender, RoutedEventArgs e)
		{
			//Collision Solid(LRD) Color
			ColorPickerDialog colorSelect = new ColorPickerDialog();
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Settings.MyDefaults.CollisionLRDColour = colorSelect.Color;
			}
		}

		private void comboBox4_DropDown(object sender, RoutedEventArgs e)
		{
			//Collision Solid(All) Color
			ColorPickerDialog colorSelect = new ColorPickerDialog();
			System.Windows.Forms.DialogResult result = colorSelect.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Settings.MyDefaults.CollisionSAColour = colorSelect.Color;
			}
		}

		private void OptionBox_FormClosing(object sender, CancelEventArgs e)
		{
			/*
            if (checkBox15.Checked && !Settings.mySettings.NightMode)
            {
                DialogResult result = MessageBox.Show("To apply this setting correctly, you will have to restart the editor, would you like to that now?", "Restart to Apply", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    Settings.mySettings.NightMode = true;
                    Settings.mySettings.Save();
                    Application.Restart();
                    Environment.Exit(0);
                }
            }
            else if (!checkBox15.Checked && Settings.mySettings.NightMode)
            {
                DialogResult result = MessageBox.Show("To apply this setting correctly, you will have to restart the editor, would you like to that now?", "Restart to Apply", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    Settings.mySettings.NightMode = false;
                    Settings.mySettings.Save();
                    Application.Restart();
                    Environment.Exit(0);
                }
            }
            */
			if (DarkModeCheckBox.IsChecked == true && !Settings.MySettings.NightMode)
			{
				Settings.MySettings.NightMode = true;
				Settings.MySettings.Save();
				App.ChangeSkin(Skin.Dark);
				App.SkinChanged = true;

			}
			else if (!DarkModeCheckBox.IsChecked == true && Settings.MySettings.NightMode)
			{
				Settings.MySettings.NightMode = false;
				Settings.MySettings.Save();
				App.ChangeSkin(Skin.Light);
				App.SkinChanged = true;

			}
		}



		public object this[string propertyName]
		{
			get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
			set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
		}

		private void radioButton12_Click(object sender, RoutedEventArgs e)
		{
			RadioButton button = sender as RadioButton;
			if (sender != null) Settings.MyDefaults.MenuLanguageDefault = button.Tag.ToString();
		}

		private void SetButtonLayoutDefault(object sender, RoutedEventArgs e)
		{
			RadioButton button = sender as RadioButton;
			if (sender != null) Settings.MyDefaults.MenuButtonLayoutDefault = button.Tag.ToString();
		}


		private void SetGraphicalPresetSetting(object sender, RoutedEventArgs e)
		{
			RadioButton button = sender as RadioButton;
			if (sender != null) EditorSettings.ApplyPreset(button.Tag.ToString());
			CheckGraphicalPresetModeState(null, null);
		}

		private void EditKeyCombo(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button)) return;
			Button KeyBind = sender as Button;
			bool state = true;
			if (state)
			{
				string keybindName = KeyBind.Tag.ToString();

				StringCollection keyBindList = Settings.MyKeyBinds[keybindName] as StringCollection;

				KeyBindConfigurator keybinder = new KeyBindConfigurator(keybindName);
				keybinder.ShowDialog();
				if (keybinder.DialogResult == true)
				{
					KeysConverter kc = new KeysConverter();
					System.Windows.Forms.Keys keyBindtoSet = keybinder.CurrentBindingKey;
					int keyIndex = keybinder.ListIndex;

					var keybindDict = Settings.MyKeyBinds[keybindName] as StringCollection;
					String KeyString = kc.ConvertToString(keyBindtoSet);
					keybindDict.RemoveAt(keyIndex);
					keybindDict.Add(KeyString);
					Settings.MyKeyBinds[keybindName] = keybindDict;
				}
			}
			SetKeybindTextboxes();

		}
		List<string> AllKeyBinds = new List<string>();
		List<string> KnownKeybinds = new List<string>();

		private void RefreshKeybindList()
		{
			AllKeyBinds.Clear();
			AllKeyBinds = new List<string>();
			KnownKeybinds.Clear();
			KnownKeybinds = new List<string>();
			foreach (SettingsProperty currentProperty in Settings.MyKeyBinds.Properties)
			{
				KnownKeybinds.Add(currentProperty.Name);
			}
			foreach (string keybind in KnownKeybinds)
			{
				if (!Extensions.KeyBindsSettingExists(keybind)) continue;
				var keybindDict = Settings.MyKeyBinds[keybind] as StringCollection;
				if (keybindDict != null && keybindDict.Count != 0)
				{
					foreach (string item in keybindDict)
					{
						AllKeyBinds.Add(item);
					}
				}

			}
		}
        private void SetKeybindTextboxes()
        {
            RefreshKeybindList();
            foreach (StackPanel stack in Extensions.FindVisualChildren<StackPanel>(ControlsPage))
            {
                foreach (Button t in Extensions.FindVisualChildren<Button>(stack))
                {
                    ProcessKeybindingButtons(t);
                }

            }
		}

		private void ProcessKeybindingButtons(Button t)
		{
            if (t.Tag != null && t.Tag.ToString() == "LOCK") return;
			t.Foreground = (SolidColorBrush)FindResource("NormalText");
			if (t.Tag != null)
			{
				System.Tuple<string,string> tuple = KeyBindPraser(t.Tag.ToString());
				t.Content = tuple.Item1;
				if (tuple.Item2 != null)
				{
					if (HasSingleMultipleOccurances(t.Tag.ToString())) t.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));
					else t.Foreground = (SolidColorBrush)FindResource("NormalText");
					ToolTipService.SetShowOnDisabled(t, true);
					t.ToolTip = tuple.Item2;
				}
				else
				{
					if (HasMultipleOccurances(tuple.Item1)) t.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));
					else t.Foreground = (SolidColorBrush)FindResource("NormalText");
				}
			}
		}

		public bool HasMultipleOccurances(String targetBinding)
		{
			if (targetBinding == "None" || targetBinding == "N/A") return false;
			int occurances = 0;
			foreach (string currentBinding in AllKeyBinds)
			{
				if (targetBinding == currentBinding) occurances++;
			}
			if (occurances < 2) return false;
			else return true;
		}

		public bool HasSingleMultipleOccurances(String keyRefrence)
		{		
			List<string> keyBindList = new List<string>();
			List<string> keyBindDuplicatesList = new List<string>();

			var keybindDict = Settings.MyKeyBinds[keyRefrence] as StringCollection;
			if (keybindDict != null) keyBindList = keybindDict.Cast<string>().ToList();
			if (keyBindList != null)
			{
				foreach (string keybind in keyBindList)
				{
					if (HasMultipleOccurances(keybind)) keyBindDuplicatesList.Add(keybind);
				}
			}
			if (keyBindDuplicatesList.Count < 2) return false;
			else return true;
			

		}

		private Tuple<string,string> KeyBindPraser(string keyRefrence)
		{
			if (keyRefrence == "NULL") return new Tuple<string, string>("N/A", null);

			List<string> keyBindList = new List<string>();
			List<string> keyBindModList = new List<string>();

			if (!Extensions.KeyBindsSettingExists(keyRefrence)) return new Tuple<string, string>("N/A", null);

			var keybindDict = Settings.MyKeyBinds[keyRefrence] as StringCollection;
			if (keybindDict != null)
			{
				keyBindList = keybindDict.Cast<string>().ToList();
			}
			else
			{
				return new Tuple<string, string>("N/A",null);
			}

			if (keyBindList == null)
			{
				return new Tuple<string, string>("N/A", null);
			}

			if (keyBindList.Count > 1)
			{
				string tooltip = "Possible Combos for this Keybind:";
				foreach (string keyBind in keyBindList)
				{
					tooltip += Environment.NewLine + keyBind;
				}
				return new Tuple<string, string>(string.Format("{0} Keybinds", keyBindList.Count), tooltip);
			}
			else if ((keyBindList.Count == 1))
			{
				return new Tuple<string, string>(keyBindList[0], null);
			}
			else
			{
				return new Tuple<string, string>("N/A", null);
			}

		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CheatCodeManager cheatCodeManager = new CheatCodeManager();
            cheatCodeManager.Owner = EditorInstance;
            cheatCodeManager.ShowDialog();
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


    }
}