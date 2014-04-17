using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  public class PreloadPlayer : MonoBehaviour
  {

	public Texture splashImage;

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
	
	  GUI.DrawTexture (new Rect (0f, 0f, Screen.width, Screen.height), splashImage);
	  if(GUI.Button(new Rect(Screen.width * .6f,Screen.height * .7f,150,50), "Start")) {
		GameObject.FindGameObjectWithTag("Player").GetComponent<AudioListener>().enabled = true;
		GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager>().SMSaveState(LevelState.Complete);
		GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager>().SMLoadLevel(door);
	  }
	}

  }
}
