using UnityEngine;
using System.Collections;

namespace Item
{
  /// <summary>
  /// Weapon stats.
  ///  All stats related to the weapon
  /// </summary>
  public class WeaponStats : MonoBehaviour
  {
    /// <summary>
    /// The damage that life steal does to the enemy
    /// </summary>
    public float damage = 0f;
    /// <summary>
    /// Determines if this weapon does life steal
    /// </summary>
    public bool lifeSteal = false;
    /// <summary>
    /// The percent of the damage that is given to the player
    /// </summary>
    [Range(0f,1f)]
    public float stealPercent;
    /// <summary>
    /// Reference to the Player
    /// </summary>
    public GameObject player;

    /// <summary>
    /// Gets the damage that's being dealt to an enemy
    /// </summary>
    public float getDamage(float defense)
    {
      if (lifeSteal) {
        if (player == null) {
          Debug.LogError("This weapon needs a reference to the player for lifesteal!");
        }
        // do life steal player.heal(damage*stealPercent);
      }
      //TODO make this interesting
      return damage / defense;
    }
  }
}
