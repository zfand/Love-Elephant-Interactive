using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  public class HurtBox : MonoBehaviour
  {
    public BossStats stats;

    public float GetDamage(){
      return stats.GetDamage();
    }
  }
}