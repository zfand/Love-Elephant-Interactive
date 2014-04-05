using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  public class Gluttony : MonoBehaviour
  {

    public float knockBackForce;
    private bool dying = false;
    private bool faceLeft;
    private bool flaming = false;
	private bool almostdead = false;
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
      } else if(!almostdead && hpLeft < 0.2f){
			this.transform.Find ("BodyFire").particleSystem.emissionRate = 30;
			almostdead = true;
	  } else if (!dying && hpLeft <= 0f) {
        this.transform.parent.GetComponent<GreedGluttonyAI> ().Dying ();
        dying = true;
      }
    }

    void OnTriggerEnter(Collider c) {
      if (c.tag == "Weapon") {
        rigidbody.AddForce((transform.position - c.transform.position).normalized *  knockBackForce, ForceMode.Impulse);
      }
    }


    public void Die()
    {
			StartCoroutine(Explode());
    }

	public IEnumerator Explode(){
		yield return new WaitForSeconds(1);
		Transform expl = this.transform.parent.Find ("GluttonyExplosion");
		expl.particleSystem.Play ();
		expl.parent = null;
		
		//this.gameObject.GetComponent<MeshExploder> ().Explode ();
		Destroy(transform.parent.gameObject);
	}
  }
}
