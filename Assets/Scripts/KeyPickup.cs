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
    /// <summary>
    /// The key to be added to the Inventory
    /// </summary>
    public KeyType key;

    void OnTriggerEnter(Collider c)
    {
      if (c.gameObject.CompareTag ("Player")) {
        c.gameObject.GetComponent<Inventory> ().AddKey (key.ToString());
        Destroy (this.gameObject);
      }
    }
  }
}

