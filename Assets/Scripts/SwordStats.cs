using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  public class SwordStats : MonoBehaviour
  {

    public float Damage = 0f;
    public float SwingAngle = 0f;
    // Use this for initialization
    void Start()
    {
      if (Damage == 0f) {
        Debug.LogError ("Damage not assigned to " + this.name);
      }
      if (SwingAngle == 0f) {
        Debug.LogError ("SwingAngle not assigned to " + this.name);
      }
    }
  
    // Update is called once per frame
    void Update()
    {
  
    }
  }
}
