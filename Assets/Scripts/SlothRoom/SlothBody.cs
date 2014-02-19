using UnityEngine;
using System.Collections;

namespace LoveElephant
{
public class SlothBody : MonoBehaviour
{
  
  public GameObject player;
  private Color origColor;
  private Color flashColor;
  private bool dying;
  public float Health = 100;
  private float fullHealth;
  private bool faceLeft;
  private Material mat;
  private bool exploding = false;
  // Use this for initialization
  void Start()
  {
    mat = GetComponent<SkinnedMeshRenderer> ().material;
    origColor = mat.color;
    flashColor = Color.red;
    fullHealth = Health;
    dying = false;

  }
  
  // Update is called once per frame
  void Update()
  {
  }
	
  void OnTriggerEnter(Collider other)
  {
    if (!dying) {
      if (other.gameObject.tag == "Sword") {
        //Debug.Break();
        float startperc = Health / fullHealth;
        Sword sword = player.GetComponent<Sword> ();
        Health -= sword.Damage;
        sword.Hit ();
        
        float endperc = Health / fullHealth; 
        if (startperc >= 0.50 && endperc <= 0.50) {
          this.transform.Find ("BodyFire").particleSystem.Play ();
        }
        StartCoroutine (FlashRed ());
        if (Health <= 0 && !exploding) {
          StartCoroutine (SlothDying ());
        }
      }
    }
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

  IEnumerator SlothDying()
  {
    Animation anim = transform.parent.animation;
    anim.PlayQueued ("Sloth_Death", QueueMode.PlayNow);
    this.Dying ();
    while (anim.isPlaying) {
      yield return 0;
    }
	
	exploding = true;
    this.transform.parent.Find ("SlothExplosion").particleSystem.Play ();
    
    Transform tv = this.transform.parent.Find ("tv");
    if (tv.gameObject.activeInHierarchy) {
      SlothTV script = tv.GetComponent <SlothTV> ();
      script.Explode ();
    }
  
    this.gameObject.SetActive (false);

  }

  void Dying()
  {
      
    this.transform.parent.GetComponent<SlothAI> ().Dying ();
    dying = true;
  }
}
}
