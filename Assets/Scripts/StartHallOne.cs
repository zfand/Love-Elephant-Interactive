using UnityEngine;
using System.Collections;
using Preloader;

namespace LoveElephant
{
  public class StartHallOne : MonoBehaviour
  {

    public GameObject roomKey;

    // Use this for initialization
    void Start()
    {

      if (GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager> ().currentLevelState != LevelState.Complete) {
        Instantiate (roomKey, new Vector3 (-3f, 3f, 0f), Quaternion.identity);
      }
    }
  
    // Update is called once per frame
    void Update()
    {
  
    }
  }
}