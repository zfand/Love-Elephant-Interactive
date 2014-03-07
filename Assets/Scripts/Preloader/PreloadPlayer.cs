using UnityEngine;
using System.Collections;
using LoveElephant.Room;

namespace Preloader
{
  public class PreloadPlayer : MonoBehaviour
  {

    // Use this for initialization
    void Start()
    {
    }
  
    // Update is called once per frame
    void Update()
    {
    }

    void Awake()
    {
      DoorConfig door = new DoorConfig();
      door.playerSpawnPos = "SpawnerDoorStart";
      GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager>().SMSaveState(LevelState.Complete);
      GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager>().SMLoadLevel("StartHallOne", door);
    }
  }
}