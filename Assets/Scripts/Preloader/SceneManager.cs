using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LoveElephant.Room;

namespace Preloader
{
/// <summary>
/// Scene manager.
///   Manages the loading of scenes (levels) in the world and retains information about them.
///    
///    Mantains
///   -Player data
///   -Level Completion
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
    /// The image displayed while loading
    /// </summary>
    public Texture loadingImage;

    /// <summary>
    /// The State of all the Levels in the game
    /// </summary>
    private Dictionary<string, LevelState> allLevelStates;
    /// <summary>
    /// The data from the door used for setting up a scene
    /// </summary>
    private DoorConfig doorData;
  
    private void OnDrawGizmos()
    {
      #if UNITY_EDITOR
    Gizmos.DrawIcon(transform.position,"SceneManager.png",true);
      #endif
    }

    private void Awake()
    {
      DontDestroyOnLoad (transform.gameObject);

      player = GameObject.FindWithTag ("Player");
      if (player != null) {
        DontDestroyOnLoad (player);
      }
    }

    private void OnGUI()
    {
      if (Application.isLoadingLevel) {
        GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), loadingImage);
      }
    }

    /// <summary>
    /// Sets the state of the current scene
    ///  USE gameObject.SendMessage("SMSaveState", LevelState.None);
    /// </summary>
    public void SMSaveState(LevelState state)
    {
      if (allLevelStates == null) {
        allLevelStates = new Dictionary<string, LevelState> ();
      } 
      allLevelStates [currentScene] = state;
    }

    /// <summary>
    /// Loads a new level of the given name
    ///  USE gameObject.SendMessage("SMLoadLevel", LevelState.None);
    /// </summary>
    public void SMLoadLevel(string name, DoorConfig data)
    {
      if (!allLevelStates.ContainsKey (name)) {
        allLevelStates.Add (name, LevelState.None);
      }
      currentScene = name;
      currentLevelState = allLevelStates [name];

      this.doorData = data;
      Application.LoadLevel (name);
    }

    void OnLevelWasLoaded(int level)
    {
      if (this.doorData != null) {
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");

        GameObject spawn = spawners.Where(spawner => spawner.name == doorData.playerSpawnPos).First<GameObject>();
        if (spawn == null) {
          Debug.Log ("Couldn't find spawn location " + doorData.playerSpawnPos);
          spawn = spawners[0];
        }
        player.transform.position = spawn.transform.position;

        this.doorData = null;
      }
    }
  }
}

