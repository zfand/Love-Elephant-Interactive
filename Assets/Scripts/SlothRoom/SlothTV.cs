using UnityEngine;
using System.Collections;
using Item;

namespace Boss
{
  public class SlothTV : MonoBehaviour
  {
  

    public Transform deathPos;
    public GameObject tv_ctrl;
    private BossStats stats;
    private bool faceLeft;
    private bool flaming = false;
    private bool almostDead = false;
    private bool exploding = false;

    // Use this for initialization
    void Start()
    {
      stats = this.GetComponent<BossStats> ();
    }
  
    // Update is called once per frame
    void Update()
    {
      float hpleft = stats.healthPercent;
      if (!flaming && hpleft < 0.5f) {
        this.transform.Find ("TVFire").particleSystem.Play ();
        flaming = true;
      } else if (!almostDead && hpleft < 0.2f) {
        this.transform.Find ("TVFire").particleSystem.emissionRate *= 2;
        almostDead = true;
      } else if (!exploding && hpleft <= 0f) {
        exploding = true;
        Transform expl = transform.parent.Find ("TVExplosion");
        expl.particleSystem.Play ();
        GameObject upgrade = (GameObject)Instantiate (stats.drop);
        upgrade.transform.position = expl.transform.position;
        upgrade.transform.position = new Vector3 (upgrade.transform.position.x - 1f, upgrade.transform.position.y + 1f, 0f);
        upgrade.rigidbody.AddExplosionForce (500, expl.transform.position, 10);
        
        this.gameObject.GetComponent<MeshExploder> ().Explode ();
        
        GameObject shrapnel = (GameObject)Instantiate (Resources.Load ("Shrapnel"));
        shrapnel.transform.position = expl.transform.position;
        foreach (Transform t in shrapnel.transform) {
          t.gameObject.rigidbody.AddExplosionForce (500, expl.transform.position, 10);
        }
        this.gameObject.SetActive (false);

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
      if (tv_ctrl != null) {
        tv_ctrl.SetActive (false);
      }
      StartCoroutine (BlowUpTV ());
    }

    private IEnumerator BlowUpTV()
    {
      Transform boom = this.transform.parent.Find ("TVExplosion");
      yield return new WaitForSeconds (0.5f);
      boom.position = deathPos.position;
      boom.particleSystem.Play ();
      this.gameObject.SetActive (false);
    }
  }
}
