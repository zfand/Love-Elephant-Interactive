using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  public class HookshotStats : MonoBehaviour
  {

    public float FireSpeed = 0f;
    public float ReelSpeed = 0f;
    public float Damage = 0f;
    public float MaxLength = 0f;
    // Use this for initialization
    void Start()
    {
      if (FireSpeed == 0f) {
        Debug.LogError ("FireSpeed not assigned to " + this.name);
      }
      if (ReelSpeed == 0f) {
        Debug.LogError ("ReelSpeed not assigned to " + this.name);
      }
      if (Damage == 0f) {
        Debug.LogError ("Damage not assigned to " + this.name);
      }
      if (MaxLength == 0f) {
        Debug.LogError ("MaxLength not assigned to " + this.name);
      }
    }
  }
}
