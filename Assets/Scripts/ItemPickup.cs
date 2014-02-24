using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  /// <summary>
  /// Item pickup.
  ///  Script for GameObjects that can be picked up and placed in the inventory
  /// </summary>
  public class ItemPickup : MonoBehaviour
  {
    /// <summary>
    /// The name of the prefab of the Item that will be stored
    /// </summary>
    public string prefabName;
    /// <summary>
    /// A Milliseconds delay before picking up the Item
    /// </summary>
    public float pickupDelay = 0f;

    void OnCollisionEnter(Collision c)
    {
      if (c.gameObject.CompareTag("Player")) {
        if (pickupDelay <= 0f) {
          OnPickup(c.gameObject);
        }
        StartCoroutine(DelayedPickup(c.gameObject));
      }
    }

    private IEnumerator DelayedPickup(GameObject player)
    {
      float deltaTime = 0f;
      while (deltaTime <= pickupDelay) {
        deltaTime += Time.deltaTime;
        yield return 0;
      }

    }
    private void OnPickup(GameObject player) {
      player.GetComponent<Inventory>().AddItem(prefabName);
      //TODO REMOVE THIS TESTING CODE
      player.GetComponent<Equipment>().Equip(player.GetComponent<Inventory>().TakeItem(prefabName));
      Destroy(this.gameObject);
    }
  }
}