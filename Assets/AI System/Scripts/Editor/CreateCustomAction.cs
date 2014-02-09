using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public static class CreateCustomAction {
	[MenuItem("Assets/Create/Action")]
	public static void CreateActionAsset()
	{
		UnityEngine.Object[] selectedAsset = Selection.GetFiltered (typeof(UnityEngine.Object), SelectionMode.DeepAssets);

		string mPath = EditorUtility.SaveFilePanelInProject (
			"Create Asset of type " + selectedAsset[0].name.ToString (),
			"New " + selectedAsset[0].name + ".asset",
			"asset", "",EditorPrefs.GetString("ActionPath",AssetDatabase.GetAssetPath(selectedAsset[0])));

		if (!string.IsNullOrEmpty (mPath)) {
			EditorPrefs.SetString("ActionPath",mPath);
			UnityEngine.Object asset = ScriptableObject.CreateInstance (selectedAsset [0].name);
			AssetDatabase.CreateAsset (asset, mPath);
			AssetDatabase.SaveAssets ();
		}
	}

	[MenuItem ("Assets/Create/Action", true)]
	static bool ValidateCreateAction () {
		UnityEngine.Object[] selectedAsset = Selection.GetFiltered (typeof(UnityEngine.Object), SelectionMode.DeepAssets);
		if (selectedAsset.Length > 0 ) {

			return (AppDomain.CurrentDomain.GetAssemblies()
			        .SelectMany(assembly => assembly.GetTypes())
			        .Where(type => type.IsSubclassOf(typeof(CustomAction)))
			        .Where(type => type.Name == selectedAsset[0].name)
			        .Select( type => selectedAsset[0].name).ToArray().Length>0);
		}
		return false;
	}
}
