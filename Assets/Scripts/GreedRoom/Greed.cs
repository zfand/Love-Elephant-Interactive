using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LoveElephant
{
  public class Greed : MonoBehaviour
  {
  

    public GameObject greed_ctrl;
    public GameObject greed_mesh;
    public BossStats stats;
	public List<GameObject> Fires;
    private bool flaming = false;
    private bool almostDead = false;
    private bool exploding = false;

    // Use this for initialization
    void Start()
    {
      	if(stats == null){
			stats = this.GetComponent<BossStats> ();
		}
    }
  
    // Update is called once per frame
    void Update()
    {
      float hpleft = stats.healthPercent;
      if (!flaming && hpleft < 0.5f) {
		foreach(GameObject g in Fires){
			g.particleSystem.Play ();
		}
        flaming = true;
		} else if (!almostDead && hpleft < 0.2f) {		
			foreach(GameObject g in Fires){
					g.particleSystem.emissionRate *= 8;
			}
			almostDead = true;
      } else if (!exploding && hpleft <= 0f) {
        exploding = true;
        Die(true);
      }
    }

    public void Die(bool drop)
    {
      greed_ctrl.SetActive (false);
      BlowUpGreed (drop);
    }

    private void BlowUpGreed(bool drop)
    {
      //yield return new WaitForSeconds (0.5f);
      Transform boom = greed_ctrl.transform.Find ("GreedExplosion");
	  boom.transform.parent = null;
      if (drop) {
        GameObject upgrade = (GameObject)Instantiate (stats.drop);
        upgrade.transform.position = boom.transform.position;
        upgrade.transform.position = new Vector3 (upgrade.transform.position.x - 1f, upgrade.transform.position.y + 1f, 0f);
        upgrade.rigidbody.AddExplosionForce (500, boom.transform.position, 10);

        GameObject shrapnel = (GameObject)Instantiate (Resources.Load ("Shrapnel"));
        shrapnel.transform.position = boom.transform.position;
        foreach (Transform t in shrapnel.transform) {
          t.gameObject.rigidbody.AddExplosionForce (500, boom.transform.position, 10);
        }
      }
      boom.position = greed_ctrl.transform.position;
      boom.particleSystem.Play ();
      //greed_mesh.GetComponent<MeshExploder> ().Explode ();

	  greed_mesh.gameObject.SetActive (false);
	  Destroy(greed_mesh);

      this.gameObject.SetActive (false);
      Destroy(this);
    }
  }
}
