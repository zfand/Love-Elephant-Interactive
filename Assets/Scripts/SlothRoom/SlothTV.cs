using UnityEngine;
using System.Collections;
using Item;

namespace LoveElephant
{
  public class SlothTV : MonoBehaviour
  {
  

    public Transform deathPos;

    private bool faceLeft;
    private bool exploding = false;

    // Use this for initialization
    void Start()
    {
    }
  
    // Update is called once per frame
    void Update()
    {
    }
  
    void OnTriggerEnter(Collider other)
    {/*
      if (other.gameObject.tag == "Weapon") {
      
        float starthealth = health / OriginalHealth;
        health -= other.gameObject.GetComponent<WeaponStats>().getDamage(defense);

        float endhealth = health / OriginalHealth;

        if (starthealth >= 0.50 && endhealth <= 0.50) {
          this.transform.Find ("TVFire").particleSystem.Play ();
        } else if (starthealth >= 0.20 && endhealth <= 0.20) {
          this.transform.Find ("TVFire").particleSystem.emissionRate *= 2;
        }
        StartCoroutine (FlashRed ());
        if (health <= 0 && !exploding) {
          exploding = true;
          Transform expl = transform.parent.Find ("TVExplosion");
          expl.particleSystem.Play ();
          GameObject upgrade = (GameObject)Instantiate (deathDrop);
          upgrade.transform.position = expl.transform.position;
        
          upgrade.transform.position = new Vector3 (upgrade.transform.position.x - 1f, upgrade.transform.position.y + 1f, 0f);
          upgrade.rigidbody.AddExplosionForce (500, expl.transform.position, 10);

      this.gameObject.GetComponent<MeshExploder>().Explode();

          GameObject shrapnel = (GameObject)Instantiate (Resources.Load ("Shrapnel"));
          shrapnel.transform.position = expl.transform.position;
          foreach (Transform t in shrapnel.transform) {
          
            t.gameObject.rigidbody.AddExplosionForce (500, expl.transform.position, 10);
          }

          this.gameObject.SetActive (false);
        }
      }
*/
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

    public void Explode()
    {
      StartCoroutine (BlowUpTV ());
    }

    IEnumerator BlowUpTV()
    {
      Transform boom = this.transform.parent.Find ("TVExplosion");
      float totaltime = 0.5f;
      float currtime = 0f;
      while (totaltime >= currtime) {
        currtime += Time.deltaTime;
        yield return 0;
      }
      boom.position = deathPos.position;
      boom.particleSystem.Play ();
      this.gameObject.SetActive (false);
    }
  }
}
