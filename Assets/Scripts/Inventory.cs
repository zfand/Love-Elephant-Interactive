using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Inventory.
///   It contains all the gameobjects that the player is currently holding (not Equip).
/// </summary>
public class Inventory : MonoBehaviour
{
  //public string itemInfo;
  /// <summary>
  /// All the Items in the Inventory
  /// </summary>
  private List<GameObject> items;
  /// <summary>
  /// The accepted item tags to be put in Inventory
  /// </summary>
  private string[] acceptedItemTags  = {"Weapon", "Boot", "HookShot","Item"};

  public void Start() {
    items = new List<GameObject>();
  }
  /// <summary>
  /// Adds the Given item to the Inventory. Returns whether if the Item was Acceptable
  /// </summary>
  public bool AddItem(GameObject item) {
    if (Array.Exists(acceptedItemTags, element => element == item.tag)) {
      items.Add(item);
      return true;
    }
    return false;
  }

  /// <summary>
  /// Takes the item of the given name out of the Inventory
  /// </summary>
  public GameObject TakeItem(string name) {
    foreach (GameObject item in items.Where(item => item.name == name)) {
      items.Remove(item);
      return item;
    }
    return null;
  }

  /// <summary>
  /// Returns a list of all the items with the given tag
  /// </summary>
  public List<GameObject> GetItemsByTag(string tag) {
    List<GameObject> list = new List<GameObject>();
    foreach (GameObject item in items.Where(item => item.tag == tag)) {
      list.Add(item);
    }
    return list;
  }

  /// <summary>
  /// Swaps the two items in and out of the 
  /// </summary>
  public GameObject SwapItems(GameObject itemIn, GameObject itemOut) {
    AddItem(itemIn);
    return TakeItem(itemOut.name);
  }


}

