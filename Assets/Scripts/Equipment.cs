using UnityEngine;
using System.Collections;
using System.Linq;
using Item;

namespace LoveElephant
{
  public class Equipment : MonoBehaviour
  {

    public GameObject hookShot;
    public GameObject weapon;
    public GameObject boot;

    private Inventory box;
    private MovementStats bootStats;
    private PlayerController playerCrtl;

    private void Awake() {
      hookShot = null;
      weapon = null;
      boot = null;

      playerCrtl = this.GetComponent<PlayerController>();

      this.transform.Cast<Transform>().ToList().ForEach(child => Equip(child.gameObject));
    }

    public void Equip(GameObject item) {
      if (item.tag == "HookShot") {
        if (hookShot != null) {
          Unequip(hookShot);
        }
        hookShot = item;
        hookShot.transform.parent = this.transform;
      }
      else if (item.tag == "Weapon") {
        if (weapon != null) {
          Unequip(weapon);
        }
        weapon = item;
        weapon.transform.parent = this.transform;
      }
      else if (item.tag == "Boot") {
        if (boot != null) {
          Unequip(boot);
        }
        boot = item;
        boot.transform.parent = this.transform;
        playerCrtl.movementStats = boot.GetComponent<Boot>().stats;
      }
    }

    private void Unequip(GameObject item) {
      item.transform.parent = null;
      //For now
      Destroy(item);
    }

    /*
    public void EquipWeapon(string weapon)
    {
      Transform weapon_parent = this.transform.FindChild ("Weapon");
      Transform oldsword = weapon_parent.GetChild (0);
      GameObject newsword = (GameObject)Instantiate (Resources.Load (weapon), oldsword.position, oldsword.rotation);
      Vector3 oldpos = oldsword.transform.position;
      float diff = Mathf.Abs (oldsword.renderer.bounds.size.x - newsword.renderer.bounds.size.x);
      DestroyObject (oldsword.gameObject);
      newsword.transform.position = oldpos; 
      //newsword.transform.Translate(-diff/2, diff/2, 0);
      newsword.transform.parent = weapon_parent;
      
      Quaternion old_rotation = weapon_parent.rotation;
      weapon_parent.rotation = Quaternion.identity;
      newsword.transform.position = new Vector3 (newsword.transform.position.x - diff,
                                                 newsword.transform.position.y, 
                                                 newsword.transform.position.z);
      weapon_parent.rotation = old_rotation;
      //Debug.Break();
      
      
    }
    
    public void EquipBoots(string boot)
    {
      Transform boot_parent = this.transform.FindChild ("Boots");
      if (boot_parent.childCount > 0) {
        Transform oldboots = boot_parent.GetChild (0);
        
        if (oldboots != null) {
          DestroyObject (oldboots.gameObject);
        }
      }
      GameObject newBoots = (GameObject)Instantiate (Resources.Load ("Items/"+boot), boot_parent.position, boot_parent.rotation);
      
      newBoots.transform.parent = boot_parent;
      
      BootStats stats = newBoots.GetComponent<BootStats> ();
      maxSpeed = originalspeed;
      maxSpeed += stats.SpeedMod;
      jumpForce = originalJumpForce;
      jumpForce *= stats.JumpMod;
    }
    **/
  }
}
