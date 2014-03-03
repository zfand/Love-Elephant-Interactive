using UnityEngine;
using System.Collections;
using LoveElephant.Room;

namespace LoveElephant
{
  public class Door : MonoBehaviour
  {
    /// <summary>
    /// The required key to open the door
    /// </summary>
    public KeyType requiredKey;

    void OnTriggerEnter(Collider c)
    {
      if (c.gameObject.CompareTag ("Player")) {
        if (c.gameObject.GetComponent<Inventory>().CheckKey(requiredKey.ToString())) {
          GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>().SwitchRooms(this.transform.parent.gameObject);
        }
      }
    }
  }
}
