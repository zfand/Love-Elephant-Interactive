using UnityEngine;
using UnityEditor;
using System.Collections;

public class CreateAiController  {
	[MenuItem("Assets/Create/AIController")]
	public static void CreateAIControllerAsset()
	{
		UnityEditorTools.CreateAsset<AIController>();
	}
}
