using UnityEngine;
using System.Collections;

public class IgnorePlayerCollision : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    GameObject player = GameObject.FindGameObjectWithTag ("Player");
    if (player != null && collider != null) {
      Physics.IgnoreCollision (player.collider, collider);
    }
  }
}

