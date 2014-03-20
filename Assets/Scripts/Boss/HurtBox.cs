using UnityEngine;
using System.Collections;

namespace Boss
{
  public class HurtBox : MonoBehaviour
  {
    public BossStats stats;

    public float GetDamage(){
      return stats.GetDamage();
    }
  }
}