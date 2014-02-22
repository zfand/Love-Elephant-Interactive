using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Item;

namespace LoveElephant
{
/// <summary>
/// Inventory.
///   It contains all the gameobjects that the player is currently holding (not Equip).
/// </summary>
  public class Inventory : MonoBehaviour
  {
    public string[] itemInfo;
    /// <summary>
    /// All the Items in the Inventory stored as names of prefabs
    /// </summary>
    private Dictionary<string, List<string>> items;
    /// <summary>
    /// The accepted item tags to be put in Inventory
    /// </summary>
    private string[] acceptedItemTags = {"Weapon", "Boot", "HookShot","Item"};

    public void Start()
    {
      items = new Dictionary<string, List<string>> ();
      foreach (string key in acceptedItemTags) {
        items.Add (key, new List<string> ());
      }
    }

    public void OnDrawGizmos()
    {
      if (items != null) {
        int length = 0;
        foreach (string key in items.Keys) {
          length += items [key].Count;
        }
        itemInfo = new string[length];
        int count = 0;
        foreach (string key in items.Keys) {
          foreach (string item in items[key]) {
            itemInfo [count++] = item;
          }
        }
      }
    }

    /// <summary>
    /// Adds the name of the Item to the inventory
    /// </summary>
    private void Add(GameObject item)
    {
      items [item.tag].Add (item.name);
      Destroy (item);
    }

    /// <summary>
    /// Returns a new instance of the Item
    /// </summary>
    private GameObject Create(string name)
    {
      GameObject item = Instantiate (Resources.Load<GameObject> ("Items/" + name)) as GameObject;
      item.transform.parent = this.transform;
      item.name = name;
      return item;
    }

    /// <summary>
    /// Adds the Given item to the Inventory. Returns whether if the Item was Acceptable
    /// </summary>
    public bool AddItem(GameObject item)
    {
      if (Array.Exists (acceptedItemTags, element => element == item.tag)) {
        this.Add (item);
        return true;
      }
      return false;
    }

    public void AddItem(string itemName)
    {
      GameObject item = Create(itemName);
      if (item != null) {
        AddItem(item);
      }
    }

    /// <summary>
    /// Takes the item of the given name out of the Inventory
    /// </summary>
    public GameObject TakeItem(string name)
    {
      foreach (string key in items.Keys) {
        foreach (string item in items[key].Where(item => item == name)) {
          items [key].Remove (item);
          return Create (item);
        }
      }
      return null;
    }

    /// <summary>
    /// Returns a list of all the items with the given tag
    /// </summary>
    public List<string> GetItemsByTag(string tag)
    {
      return items [tag];
    }

    /// <summary>
    /// Swaps the two items in and out of the 
    /// </summary>
    public GameObject SwapItems(GameObject itemIn, string itemOut)
    {
      AddItem (itemIn);
      return TakeItem (itemOut);
    }

  }
}

