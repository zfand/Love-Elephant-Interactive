using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  /// <summary>
  /// Key pickup.
  ///  Script for Keys that can be picked up and placed in the inventory
  /// </summary>
  public class KeyPickup : MonoBehaviour
  {
    public enum Keys
    {
      Green = 1,
      White = 2
    }
    public Keys key;

    void OnCollisionEnter(Collision c)
    {
      if (c.gameObject.CompareTag ("Player")) {
        c.gameObject.GetComponent<Inventory> ().AddKey (key.ToString());
        Destroy (this.gameObject);
      }
    }
  }
}

