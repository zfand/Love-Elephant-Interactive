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

    public float damage = 0f;
    public float swingAngle = 0f;

    /// <summary>
    /// Gets the damage that's being dealt to an enemy
    /// </summary>
    public float getDamage(float defense)
    {
      //TODO make this interesting
      return damage / defense;
    }
  }
}
