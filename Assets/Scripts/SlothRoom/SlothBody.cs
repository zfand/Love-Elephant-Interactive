using UnityEngine;
using System.Collections;
using Item;

namespace Boss
{
  public class SlothBody : MonoBehaviour
  {


    private bool dying = false;
    private bool faceLeft;
    private bool flaming = false;
    private BossStats stats;

    // Use this for initialization
    void Start()
    {
      stats = this.GetComponent<BossStats> ();
    }
  
    // Update is called once per frame
    void Update()
    {
      float hpLeft = stats.healthPercent;
      if (!flaming && hpLeft < 0.5f) {
        this.transform.Find ("BodyFire").particleSystem.Play ();
        flaming = true;
      } else if (!dying && hpLeft <= 0f) {
        this.transform.parent.GetComponent<SlothAI> ().Dying ();
        dying = true;
      }
    }
  
    void OnCollisionEnter(Collision hit)
    {
      if (hit.gameObject.tag == "Wall") {
        transform.parent.GetComponent<SlothAI> ().HitWall ();
        //facePlayer ();
        //anim.PlayQueued ("Sloth_EndCharge", QueueMode.PlayNow);
      }
    }

    void OnCollisionStay(Collision hit)
    {
      if (hit.gameObject.tag == "Wall") {
        transform.parent.GetComponent<SlothAI> ().HitWall ();
        //facePlayer ();
        //anim.PlayQueued ("Sloth_EndCharge", QueueMode.PlayNow);
      }
    }

    public void Die()
    {
      this.transform.parent.Find ("SlothExplosion").particleSystem.Play ();
    
      SlothTV tv = this.transform.parent.GetComponentInChildren<SlothTV> ();
      if (tv != null) {
        tv.Die ();
      }

      this.gameObject.GetComponent<MeshExploder> ().Explode ();
      this.gameObject.SetActive (false);

    }
  }
}
