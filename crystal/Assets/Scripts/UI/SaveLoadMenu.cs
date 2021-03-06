﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class SaveLoadMenu : MonoBehaviour {

	public Text menuLabel, actionButtonLabel;

	public InputField nameInput;

	public RectTransform listContent;

	public SaveLoadItem itemPrefab;

	public HexGrid hexGrid;

	bool saveMode;

	public void Open (bool saveMode) {
		this.saveMode = saveMode;
		if (saveMode) {
			menuLabel.text = "Save Map";
			actionButtonLabel.text = "Save";
		}
		else {
			menuLabel.text = "Load Map";
			actionButtonLabel.text = "Load";
		}
		string firstFile = FillList();
		gameObject.SetActive(true);
		HexMapCamera.Locked = true;
		SelectItem(firstFile);
	}

	public void Close () {
		gameObject.SetActive(false);
		HexMapCamera.Locked = false;
	}

	public void Action () {
		string path = GetSelectedPath();
		if (path == null) {
			return;
		}
		if (saveMode) {
			Save(path);
		}
		else {
			Load(path);
		}
		Close();
	}

	public void SelectItem (string name) {
		nameInput.text = name;
	}

	public void Delete () {
		string path = GetSelectedPath();
		if (path == null) {
			return;
		}
		if (File.Exists(path)) {
			File.Delete(path);
		}
		nameInput.text = "";
		FillList();
	}

	string FillList () {
		for (int i = 0; i < listContent.childCount; i++) {
			Destroy(listContent.GetChild(i).gameObject);
		}
#if UNITY_IOS || UNITY_ANDROID
        string[] paths =
        	Directory.GetFiles(Application.persistentDataPath, "*.map");
#else
        string[] paths =
        Directory.GetFiles(Path.Combine(Application.dataPath,"maps"), "*.map");
#endif
        Array.Sort(paths);
		string firstFile = "";
		for (int i = 0; i < paths.Length; i++) {
			SaveLoadItem item = Instantiate(itemPrefab);
			item.menu = this;
			item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
			if(string.IsNullOrEmpty(firstFile))
			 	firstFile = item.MapName;
			item.transform.SetParent(listContent, false);
		}
		return firstFile;
	}

	string GetSelectedPath () {
		string mapName = nameInput.text;
		if (mapName.Length == 0) {
			return null;
		}
#if UNITY_IOS || UNITY_ANDROID
        return Path.Combine(Application.persistentDataPath, mapName + ".map");
#else
        return Path.Combine(Path.Combine(Application.dataPath, "maps"), mapName + ".map");
#endif
    }

	void Save (string path) {
		using (
			BinaryWriter writer =
			new BinaryWriter(File.Open(path, FileMode.Create))
		) {
			writer.Write(2);
			hexGrid.Save(writer);
		}
	}

	void Load (string path) {
		if (!File.Exists(path)) {
			Debug.LogError("File does not exist " + path);
			return;
		}
		using (BinaryReader reader = new BinaryReader(File.OpenRead(path))) {
			int header = reader.ReadInt32();
			if (header <= 2) {
				hexGrid.Load(reader, header);
				HexMapCamera.ValidatePosition();
			}
			else {
				Debug.LogWarning("Unknown map format " + header);
			}
		}
	}
}