﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Structures
{
	public class SceneState
	{
		public string DataDirectory
		{
			get
			{
				if (this.ExtraDataDirectories != null && this.ExtraDataDirectories.Count >= 1) return this.ExtraDataDirectories[0];
				else return string.Empty;
			}
			set
			{
				if (this.ExtraDataDirectories == null) this.ExtraDataDirectories = new List<string>();
				else this.ExtraDataDirectories.Clear();
				this.ExtraDataDirectories.Add(value);
			}
		}
		public IList<string> ExtraDataDirectories { get; set; } = new List<string>();
		public bool Is_IZStage { get; set; } = false;
		public string IZ_StageKey { get; set; } = "";
		public string IZ_SceneKey { get; set; } = "";
		public string FilePath { get; set; } = "";
		public int LevelID { get; set; } = -1;
		public bool IsEncoreMode { get; set; } = false;
		public string SceneDirectory { get; set; } = "";
		public string Zone { get; set; } = "";
		public string Name { get; set; } = "";
		public string SceneID { get; set; } = "";
		public string MasterDataDirectory { get; set; } = "";
		public LoadMethod LoadType { get; set; } = SceneState.LoadMethod.Unspecified;
		public bool IsFullPath
		{
			get
			{
				if (LoadType == LoadMethod.RelativePath) return false;
				else return true;
			}
		}
		public bool WasSelfLoaded
		{
			get
			{
				if (LoadType == LoadMethod.SelfLoaded) return true;
				else return false;
			}
		}
		public enum LoadMethod
		{
			RelativePath, FullPath, SelfLoaded, Unspecified
		}
		public SceneState(string filePath = "", int levelID = -1, bool isEncore = false, string sceneDirectory = "", string zone = "", string name = "", string sceneID = "", LoadMethod loadType = SceneState.LoadMethod.Unspecified, IList<string> exDDList = null, string dataDirectory = "")
		{
			FilePath = filePath;
			LevelID = levelID;
			IsEncoreMode = isEncore;
			SceneDirectory = sceneDirectory;
			Zone = zone;
			Name = name;
			SceneID = sceneID;
			LoadType = loadType;
			MasterDataDirectory = dataDirectory;
			if (exDDList != null) ExtraDataDirectories = exDDList;
		}
		public void Clear()
		{
			FilePath = "";
			LevelID = -1;
			IsEncoreMode = false;
			SceneDirectory = "";
			Zone = "";
			Name = "";
			SceneID = "";
			MasterDataDirectory = "";
			LoadType = LoadMethod.Unspecified;
			ExtraDataDirectories = new List<string>();
			IZ_StageKey = "";
			IZ_SceneKey = "";
			Is_IZStage = false;
		}
		public SceneState()
		{

		}

	}
}
