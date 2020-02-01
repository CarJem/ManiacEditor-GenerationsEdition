﻿using System.Windows;

namespace ManiacEditor.Interfaces
{
    /// <summary>
    /// Interaction logic for SceneSelectWindow.xaml
    /// </summary>
    public partial class SceneSelectWindow : Window
	{
		public SceneSelect SceneSelect;
		public SceneSelectWindow(RSDKv5.Gameconfig config = null, Editor instance = null)
		{
			InitializeComponent();
			SceneSelect = new SceneSelect(config, instance, this);
			FrameHost.Children.Add(SceneSelect);
		}
	}
}