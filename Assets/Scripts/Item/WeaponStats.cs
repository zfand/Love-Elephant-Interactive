using UnityEngine;
using System.Collections;

namespace Item
{
  public class WeaponStats : MonoBehaviour
  {

    public float damage = 0f;
    public float swingAngle = 0f;

    public float getDamage(float defense)
    {
      return damage / defense;
    }
  }
}
