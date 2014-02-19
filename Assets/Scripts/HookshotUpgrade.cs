using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  public class HookshotUpgrade : MonoBehaviour
  {
    public string Upgrade;
    public float PickupTimer = 15f;
    private bool can_pickup = false;
    // Use this for initialization
    void Start()
    {
  
    }
  
    // Update is called once per frame
    void Update()
    {
      //this is so that the player doesnt accidentally pick
      //the upgrade up without noticing
      if (!can_pickup) {
        PickupTimer--;
        if (PickupTimer <= 0) {
          can_pickup = true;
        }
      }
    }

    void OnCollisionStay(Collision c)
    {
      if (c.gameObject.tag == "Player" && can_pickup) {
        PlayerController player = c.gameObject.GetComponent<PlayerController> ();
        player.EquipHookshot (Upgrade);
        Destroy (this.gameObject);
      }
    }
  }
}
