using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  public class Door : MonoBehaviour
  {
    /// <summary>
    /// The required key to open the door
    /// </summary>
    public KeyType requiredKey;
    /// <summary>
    /// The room that this door brings you to
    /// </summary>
    public string room;

    void OnTriggerEnter(Collider c)
    {
      if (c.gameObject.CompareTag ("Player")) {
        if (c.gameObject.GetComponent<Inventory>().CheckKey(requiredKey.ToString())) {
          GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>().SwitchRooms(room);;
        }
      }
    }
  }
}
