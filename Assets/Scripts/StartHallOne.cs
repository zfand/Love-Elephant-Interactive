using UnityEngine;
using System.Collections;
using Preloader;

namespace LoveElephant
{
  public class StartHallOne : MonoBehaviour
  {

    public GameObject roomKey;
	public Vector3 keyPos;

    // Use this for initialization
    void Start()
    {

      if (GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager> ().currentLevelState != LevelState.Complete) {
        Instantiate (roomKey, keyPos, Quaternion.identity);
      }
    }
  }
}
