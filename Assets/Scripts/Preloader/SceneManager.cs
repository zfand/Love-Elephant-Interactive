using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Scene manager.
///   Manages the loading of scenes (levels) in the world and retains information about them.
///    
///    Mantains
/// 	-Player data
/// 	-Level Completion
///	
/// </summary>
public class SceneManager : MonoBehaviour
{
	public GameObject player;

	/// <summary>
	/// The state of the current level.
	/// </summary>
	public LevelState currentLevelState;

	/// <summary>
	/// The current scene.
	/// </summary>
	public string currentScene;

	/// <summary>
	/// The State of all the Levels in the game
	/// </summary>
	private Dictionary<string, LevelState> allLevelStates;

	
	private void OnDrawGizmos()
	{
		#if UNITY_EDITOR
		Gizmos.DrawIcon(transform.position,"SceneManager.png",true);
		#endif
	}

	private void Awake() 
	{
		DontDestroyOnLoad(transform.gameObject);

		player = GameObject.FindWithTag("Player");
		if (player != null) 
		{
			DontDestroyOnLoad(player);
		}
	}

	/// <summary>
	/// Sets the state of the current scene
	///  USE gameObject.SendMessage("SMSaveState", LevelState.None);
	/// </summary>
	public void SMSaveState(LevelState state) 
	{
		if (allLevelStates == null) 
		{
			allLevelStates = new Dictionary<string, LevelState> ();
		}	
		allLevelStates[currentScene] = state;
	}

	/// <summary>
	/// Loads a new level of the given name
	///  USE gameObject.SendMessage("SMLoadLevel", LevelState.None);
	/// </summary>
	public void SMLoadLevel(string name) 
	{
		if (!allLevelStates.ContainsKey(name)) 
		{
			allLevelStates.Add(name, LevelState.None);
		}
		currentScene = name;
		currentLevelState = allLevelStates[name];
		Application.LoadLevel(name);
	}
}

