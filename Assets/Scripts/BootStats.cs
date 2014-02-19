using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  public class BootStats : MonoBehaviour
  {

    public float SpeedMod = 0f;
    public float JumpMod = 0f;
    // Use this for initialization
    void Start()
    {
      if (SpeedMod == 0f) {
        Debug.LogError ("Damage not assigned to " + this.name);
      }
      if (JumpMod == 0f) {
        Debug.LogError ("SwingAngle not assigned to " + this.name);
      }
    }
  }
}
