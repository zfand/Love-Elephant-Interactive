using UnityEngine;
using System.Collections;
using System.Linq;
using Item;

namespace LoveElephant
{
  /// <summary>
  ///   Equipment.
  ///    Items currently being used by the player.
  /// </summary>
  public class Equipment : MonoBehaviour
  {

    /// <summary>
    /// Reference to the HookShot GameObject on the Player
    /// </summary>
    public GameObject hookShot;
    /// <summary>
    /// Reference to the Weapon GameObject on the Player
    /// </summary>
    public GameObject weapon;
    /// <summary>
    /// Reference to the Boot GameObject on the Player
    /// </summary>
    public GameObject boot;
    /// <summary>
    /// Reference to the Player's Inventory
    /// </summary>
    private Inventory box;
    /// <summary>
    /// Reference to the PlayerController
    /// </summary>
    private PlayerController playerCrtl;

    private void Awake()
    {
      hookShot = null;
      weapon = null;
      boot = null;

      playerCrtl = this.GetComponent<PlayerController> ();
      if (playerCrtl == null) {
        Debug.LogError ("Could not find PlayerController on the Player!");
      }
      box = this.GetComponent<Inventory> ();
      if (box == null) {
        Debug.LogError ("Could not find Inventory on the Player!");
      }

      //Try to equip all items on the player
      this.transform.Cast<Transform> ().ToList ().ForEach (child => Equip (child.gameObject));
    }

	public string GetCurrentEquip(string equipType){
		switch(equipType){
			case "Weapon":
				if(weapon != null){
					return weapon.name;
				} else return "";
			case "Hookshot":
				if(hookShot != null){
					return hookShot.name;
				} else return "";
			case "Boot":
				if(boot != null){
					return boot.name;
				} else return "";
			default:
				return "";
			}

			
	}
    /// <summary>
    /// Equips Item onto the Player, Unequiping existing Items
    /// </summary>
    public void Equip(GameObject item)
    {
      if (item == null) {
        return;
      }
      if (item.tag == "HookShot") {
        if (hookShot != null) {
          Unequip (hookShot);
        }
        hookShot = item;
        hookShot.transform.parent = this.transform;
      } else if (item.tag == "Weapon") {
        if (weapon != null) {
          item.transform.position = weapon.transform.position;
          item.transform.rotation = weapon.transform.rotation;
          item.transform.localScale = weapon.transform.localScale;
          Unequip (weapon);
        }
        weapon = item;
        weapon.transform.parent = this.transform;
      } else if (item.tag == "Boot") {
        if (boot != null) {
          Unequip (boot);
        }
        boot = item;
        boot.transform.parent = this.transform;
        playerCrtl.movementStats = boot.GetComponent<Boot> ().stats;
      }
    }

    /// <summary>
    /// Unequips the given item then adds it to the inventory
    /// </summary>
    private void Unequip(GameObject item)
    {
      item.transform.parent = null;
      box.AddItem (item);
    }
  }	
}
