using UnityEngine;
using System.Collections;

public class HurtBox : MonoBehaviour
{
  /// <summary>
  /// The damage the Boss does on hit
  /// </summary>
  public float attackDmg;
  /// <summary>
  /// reference to hurt collider
  /// </summary>
  public Collider collider;

  public void enabled(bool toggle) {
    if (collider != null) {
      collider.enabled = toggle;
    }
  }
}

