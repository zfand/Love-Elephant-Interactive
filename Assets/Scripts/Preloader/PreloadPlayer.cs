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
	  door.connectedRoom = "WW_TutOne";
	  door.playerSpawnPos = "SpawnerDoorLeft";

	  GUI.Box(new Rect(10,10,100,90), "The Humans Are Dead");
	
		if(GUI.Button(new Rect(20,40,80,20), "Start")) {
		GameObject.FindGameObjectWithTag("Player").GetComponent<AudioListener>().enabled = true;
		GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager>().SMSaveState(LevelState.Complete);
		GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager>().SMLoadLevel(door);
	  }
	}

  }
}
