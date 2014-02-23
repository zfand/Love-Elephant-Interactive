using UnityEngine;
using System.Linq;
using System.Collections;
using Preloader;

namespace LoveElephant
{

  /// <summary>
  /// Room manager.
  ///  Manages local instances of the room.
  /// </summary>
  public class RoomManager : MonoBehaviour
  {

    /// <summary>
    /// Doors in the Room
    /// </summary>
    public GameObject[] doors;
    /// <summary>
    /// The Rooms(Scenes) connected to this one
    /// </summary>
    public string[] connectedRooms;
    /// <summary>
    /// The starting Position of the 
    /// </summary>
    public GameObject playerStartPos;
    /// <summary>
    /// Reference to the player
    /// </summary>
    public GameObject player;

    // Use this for initialization
    private void Start()
    {
      if (player == null) {
        player = GameObject.FindGameObjectWithTag ("Player");
      }
      if (playerStartPos != null) {
        player.transform.position = playerStartPos.transform.position;
      }
      doors = GameObject.FindGameObjectsWithTag ("Door");
    }

    /// <summary>
    /// Switches to the given room if it's a connected Room
    /// </summary>
    public void SwitchRooms(string room)
    {
      if (connectedRooms.Contains (room)) {
        GameObject.FindGameObjectWithTag ("Parallax").GetComponent<Parallax> ().disable ();
        SceneManager sm = GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager>();
        sm.SMSaveState(LevelState.Complete);
        sm.SMLoadLevel(room);
      }
    }
  }
}