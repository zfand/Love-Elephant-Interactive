using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  public class SlothTV : MonoBehaviour
  {
  
    public GameObject player;
    public Transform deathPos;
    private Color origColor;
    private Color flashColor;
    public float Health = 100;
    private float OriginalHealth;
    private bool faceLeft;
    private Material mat;
    private bool exploding = false;

    // Use this for initialization
    void Start()
    {
      mat = GetComponent<SkinnedMeshRenderer> ().material;
      origColor = mat.color;
      flashColor = Color.red;
      OriginalHealth = Health;
    }
  
    // Update is called once per frame
    void Update()
    {
    }
  
    void OnTriggerEnter(Collider other)
    {
      if (other.gameObject.tag == "Sword") {
        //Debug.Break();
      
        float starthealth = Health / OriginalHealth;
        Sword sword = player.GetComponent<Sword> ();
        Health -= sword.Damage;
        sword.Hit ();
        float endhealth = Health / OriginalHealth;

        if (starthealth >= 0.50 && endhealth <= 0.50) {
          this.transform.Find ("TVFire").particleSystem.Play ();
        } else if (starthealth >= 0.20 && endhealth <= 0.20) {
          this.transform.Find ("TVFire").particleSystem.emissionRate *= 2;
        }
        StartCoroutine (FlashRed ());
        if (Health <= 0 && !exploding) {
          exploding = true;
          Transform expl = transform.parent.Find ("TVExplosion");
          expl.particleSystem.Play ();
          GameObject upgrade = (GameObject)Instantiate (Resources.Load ("BFBlowtorchUpgrade"));
          upgrade.transform.position = expl.transform.position;
        
          upgrade.transform.position = new Vector3 (upgrade.transform.position.x - 1f, upgrade.transform.position.y + 1f, 0f);
          upgrade.rigidbody.AddExplosionForce (500, expl.transform.position, 10);

          GameObject shrapnel = (GameObject)Instantiate (Resources.Load ("Shrapnel"));
          shrapnel.transform.position = expl.transform.position;
          foreach (Transform t in shrapnel.transform) {
          
            t.gameObject.rigidbody.AddExplosionForce (500, expl.transform.position, 10);
          }

          this.gameObject.SetActive (false);
        }
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
      //Debug.Break ();
      while (totaltime >= currtime) {
        currtime += Time.deltaTime;
        yield return 0;
      }
      boom.position = deathPos.position;
      boom.particleSystem.Play ();
      this.gameObject.SetActive (false);
    }

    //flash red on hit 
    IEnumerator FlashRed()
    {
      float elapsedTime = 0f;
      float totaltime = 0.25f;
      float intervalTime = .05f;
      int index = 0;
      while (elapsedTime < totaltime) {
        if (index % 2 == 0) {
          mat.color = origColor;
        } else {
          mat.color = flashColor;
        }
      
        elapsedTime += intervalTime;
        index++;
        yield return new WaitForSeconds (intervalTime);
      }
      mat.color = origColor;
    
    }
  }
}
