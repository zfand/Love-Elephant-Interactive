using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  public class PreloadPlayer : MonoBehaviour
  {

    // Use this for initialization
    void Start()
    {
	  GameObject.FindGameObjectWithTag("Player").GetComponent<AudioListener>().enabled = false;
    }
  
    // Update is called once per frame
    void Update()
    {
    }

	void OnGUI () {
	
	  DoorConfig door = new DoorConfig();
	  door.connectedRoom = "RW_WrathRoom";
	  door.playerSpawnPos = "SpawnerDoorLeft";

	  GUI.Box(new Rect(Screen.width/2 - 75,Screen.height/2 - 30,150,50), "The Humans Are Dead");
	
	  if(GUI.Button(new Rect(Screen.width/2-40,Screen.height/2,80,40), "Start")) {
		GameObject.FindGameObjectWithTag("Player").GetComponent<AudioListener>().enabled = true;
		GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager>().SMSaveState(LevelState.Complete);
		GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager>().SMLoadLevel(door);
	  }
	}

  }
}
