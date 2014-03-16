using UnityEngine;
using System.Collections;

public class HurtBox : MonoBehaviour
{
  /// <summary>
  /// The damage the Boss does on hit
  /// </summary>
  public float attackDmg;

  public void Hurtenabled(bool toggle) {
    this.enabled = toggle;
    if (collider != null) {
      collider.enabled = toggle;
    }
  }
}

