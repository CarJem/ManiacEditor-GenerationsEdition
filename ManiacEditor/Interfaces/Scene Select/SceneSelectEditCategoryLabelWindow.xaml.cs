﻿using System.Collections.Generic;
using System.Windows;

namespace ManiacEditor.Interfaces
{
    /// <summary>
    /// Interaction logic for SceneSelectEditCategoryLabelWindow.xaml
    /// </summary>
    public partial class SceneSelectEditCategoryLabelWindow : Window
	{
		public RSDKv5.Gameconfig.Category Category;
		public List<RSDKv5.Gameconfig.SceneInfo> Scenes;

		public SceneSelectEditCategoryLabelWindow()
		{
			Category = new RSDKv5.Gameconfig.Category();
			Scenes = new List<RSDKv5.Gameconfig.SceneInfo>();
			InitializeComponent();
		}

		public SceneSelectEditCategoryLabelWindow(RSDKv5.Gameconfig.Category category, List<RSDKv5.Gameconfig.SceneInfo> scenes)
		{
			if (category == null)
			{
				Category = new RSDKv5.Gameconfig.Category();
				Scenes = new List<RSDKv5.Gameconfig.SceneInfo>();
			}
			else
			{
				Category = category;
				Scenes = scenes;
			}

			InitializeComponent();
		}

		private void EditSceneSelectInfo_Loaded(object sender, RoutedEventArgs e)
		{
			textBox1.Text = Category.Name;
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			Category.Name = textBox1.Text;
			Category.Scenes = Scenes;
			this.DialogResult = true;
			Close();
		}

		private void button2_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			Close();
		}
	}
}
