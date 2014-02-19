using UnityEngine;
using System.Collections;

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
      GameObject.FindGameObjectWithTag ("SceneManager").SendMessage ("SMSaveState", LevelState.Complete);
      GameObject.FindGameObjectWithTag ("SceneManager").SendMessage ("SMLoadLevel", "StartHallOne");
    }
  }
}