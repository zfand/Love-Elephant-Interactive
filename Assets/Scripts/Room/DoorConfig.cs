using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  /// <summary>
  /// Door config.
  ///  Data for each Door in a room
  /// </summary>
  [System.Serializable]
  public class DoorConfig
  {
    /// <summary>
    /// Reference to the Door Object
    /// </summary>
    public GameObject door;
    /// <summary>
    /// The Room(Scene) that the Door connects to
    /// </summary>
    public string connectedRoom;
    /// <summary>
    /// The position the player will move to when going through the room
    /// </summary>
    public string playerSpawnPos;
  }
}

