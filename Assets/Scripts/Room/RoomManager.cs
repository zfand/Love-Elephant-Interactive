using UnityEngine;
using System.Collections;

namespace LoveElephant
{

  /// <summary>
  /// Room manager.
  ///  Manages local instances of the room.
  /// </summary>
  public class RoomManager : MonoBehaviour
  {

    public DoorConfig[] doors;
    /// <summary>
    /// Reference to the player
    /// </summary>
    private GameObject player;

    // Use this for initialization
    private void Start()
    {
      if (player == null) {
        player = GameObject.FindGameObjectWithTag ("Player");
      }
    }

    private DoorConfig findConnection(GameObject door)
    {
      foreach (DoorConfig config in doors) {
        if (config.door == door) {
          return config;
        }
      }
      return null;
    }

    /// <summary>
    /// Switches to the given room if it's a connected Room
    /// </summary>
    public void SwitchRooms(GameObject door)
    {
      DoorConfig config = findConnection(door);

      if (config != null) {
        SceneManager sm = GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager> ();
        sm.SMSaveState (LevelState.Complete);
        sm.SMLoadLevel (config);
      }
    }
  }
}