using UnityEngine;
using System.Collections;

public class StrechArmStrongFix : MonoBehaviour
{
  private Vector3 startPos;
  // Use this for initialization
  void Start()
  {
    startPos = transform.localPosition;
  }
	
  // Update is called once per frame
  void LateUpdate()
  {
    transform.localPosition = startPos;
  }
}

