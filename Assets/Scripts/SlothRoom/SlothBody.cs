using UnityEngine;
using System.Collections;
using Item;

namespace LoveElephant
{
public class SlothBody : MonoBehaviour
{
  public float health = 100f;
  public float defense = 1f;

  private Color origColor;
  private Color flashColor;
  private bool dying;
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
    fullHealth = health;
    dying = false;

  }
  
  // Update is called once per frame
  void Update()
  {
  }
	
  void OnTriggerEnter(Collider other)
  {
    if (!dying) {
      if (other.gameObject.tag == "Weapon") {
        float startperc = health / fullHealth;
        
        health -= other.gameObject.GetComponent<WeaponStats>().getDamage(defense);
        
        float endperc = health / fullHealth; 
        if (startperc >= 0.50 && endperc <= 0.50) {
          this.transform.Find ("BodyFire").particleSystem.Play ();
        }
        StartCoroutine (FlashRed ());
        if (health <= 0 && !exploding) {
          StartCoroutine (SlothDying ());
        }
      }
    }
  }
  
  void OnCollisionEnter(Collision hit)
  {
	if (hit.gameObject.tag == "Wall") {
		transform.parent.GetComponent<SlothAI>().HitWall();
		//facePlayer ();
		//anim.PlayQueued ("Sloth_EndCharge", QueueMode.PlayNow);
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
 //   Animator anim = transform.parent.animator;
 //   anim.Play("Dying");
    this.Dying ();
	SlothAI ai = this.transform.parent.GetComponent<SlothAI> ();
   	while (!ai.IsDead()) {
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
