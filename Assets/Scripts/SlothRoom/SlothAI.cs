using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  public class SlothAI : MonoBehaviour
  {
    
	public AudioSource[] audioSources;
	public AudioClip sloth_charge_zap;
	public AudioClip sloth_zap;
	public AudioClip sloth_charge;
	public AudioClip sloth_damage;
	public AudioClip sloth_melee;
	public AudioClip sloth_idle;
	public AudioClip sloth_die;


    public GameObject player;
    public float chargeForce;
    public float chargeDistance;
    public float idleTime;
    public float idleMax;
    public float idleMin;
    public BossStats[] stats;
    public SlothTV tv;
    public GameObject shockBuild;
		public GameObject swingBuild;
    public GameObject shock;
    public float shockDamage;
    public float shockDelayTime;
    private Animator anim;
    private Color origColor;
    private bool dying;
    private int facing;
    private bool turning = false;
    private bool startHit = false;
    private bool charging;
    private bool startdying = false;
		private bool spinning = false;
    AnimatorStateInfo info;
    Transform stunSwirl;
    // Use this for initialization
    void Start()
    {
	  audioSources[0].loop = true;
	  audioSources[0].clip = sloth_idle;
	  audioSources[0].Play ();
      anim = GetComponent<Animator> ();
      facing = 1;
      charging = false;
      player = GameObject.FindGameObjectWithTag ("Player");
      stunSwirl = transform.FindChild ("Stun");
      stunSwirl.gameObject.SetActive (false);

    }
  
    // Update is called once per frame
    void FixedUpdate()
    {
      info = anim.GetCurrentAnimatorStateInfo (0);
      if (!dying) {
        if (info.IsName ("IdleState") && !anim.IsInTransition(0) && !spinning) {
          SetAttacking(false);
          stunSwirl.gameObject.SetActive (false);
          facePlayer ();
          idleTime -= Time.deltaTime;
          if (idleTime <= 0) {
            idleTime = Mathf.Ceil (Random.Range (idleMin, idleMax));

            float roll = Random.Range (0,100);
            float dist = Vector3.Distance(player.transform.position, transform.position);

            //if close
            roll = (dist < 5f) ? roll + 35 : roll;
            roll = (dist < 10f) ? roll + 25 : roll;
            //if too far
            roll = (dist > 30f) ? roll - 65 : roll;

            if (roll >= 80) {
			  audioSources[1].clip = sloth_charge_zap;
			  audioSources[1].Play ();
              StartCoroutine("Shock");
			} else if (roll >= 65 && tv != null) {
			  audioSources[1].clip = sloth_melee;
			  audioSources[1].Play ();
			  StartCoroutine(Spin());
            } else {
              rigidbody.constraints = RigidbodyConstraints.FreezeAll;
              anim.SetTrigger ("BeginCharge");
            }
            SetAttacking(true);
          }

        } else if (!charging && info.IsName ("Charge") && !startHit) {
          charging = true;
          startHit = true;
          StartCoroutine (Charge ());
        } else if (info.IsName ("EndCharge")) {
          startHit = false;
        } else if (info.IsName ("Stunned")) {
          stunSwirl.gameObject.SetActive (true);  
        }
      } else {
        if (startdying) {
          if (info.IsName ("Dead")) {
            startdying = false;
            this.GetComponentInChildren<SlothBody> ().Die ();
          }
		} else {
		  audioSources[0].loop = false;
		  audioSources[0].clip = sloth_die;
		  audioSources[0].Play ();
          if (info.IsName ("Dying")) {
            startdying = true;
          }
        }
      }
    }

    IEnumerator Charge()
    {
      rigidbody.constraints = ~RigidbodyConstraints.FreezePositionX;
      Vector3 startPos = transform.position;

      rigidbody.AddForce(Vector3.left * facing * chargeForce, ForceMode.Acceleration);

      while (charging && Vector3.Distance(startPos, transform.position) < chargeDistance) {
        //rigidbody.AddForce(Vector3.left * facing * chargeForce);
        yield return 0;
      }
      rigidbody.velocity = Vector3.zero;
      rigidbody.angularVelocity = Vector3.zero;

      charging = false;
      anim.SetTrigger ("EndCharge");
    }

    public void HitWall()
    {
      if (charging) {
        charging = false;
        anim.SetTrigger ("Stunned");   
      }
    }
  
    //Face the player
    void facePlayer()
    {
      float currentX = this.transform.eulerAngles.x;
      float currentY = this.transform.eulerAngles.y;
      if (!turning) {
        if (player.transform.position.x < this.transform.position.x && facing != 1) {
          facing = 1;
          StartCoroutine (TurnSloth (new Vector3 (currentX, -currentY, 0f)));
        } else if (player.transform.position.x > this.transform.position.x && facing != -1) {
          facing = -1;
          StartCoroutine (TurnSloth (new Vector3 (currentX, -currentY, 0f)));
        }
      }
    }

	IEnumerator Spin(){
		swingBuild.particleSystem.Play ();
		spinning = true;
		while(swingBuild.particleSystem.isPlaying){
			yield return 0;
		}
		
		anim.SetTrigger("Spin");
		spinning = false;
	}


    IEnumerator TurnSloth(Vector3 dest)
    {
      float interval = 5f;
      float totalrot = 0;
      bool rotating = true;
      turning = true;

      while (rotating) { 
        totalrot += interval;
        if (totalrot >= 180) {
          rotating = false;
          yield return 0;
        }
        transform.Rotate (new Vector3 (0f, interval, 0f));
        yield return 0;
      }
      turning = false;
    
      this.transform.eulerAngles = dest;
    }

    private IEnumerator Shock()
    {
      rigidbody.constraints = RigidbodyConstraints.FreezeAll;
      anim.SetTrigger("Shock");
      shockBuild.particleSystem.Play();
      yield return new WaitForSeconds(shockDelayTime);
      shockBuild.particleSystem.Stop();

	  audioSources[1].clip = sloth_zap;
	  audioSources[1].Play ();
	
      shock.particleSystem.Play();
      if (Vector3.Distance(shock.transform.position,player.transform.position) <= 4f) {
        player.GetComponent<PlayerStats>().TakeDamage(shockDamage);
      }
      anim.SetTrigger("Shock");
      rigidbody.constraints = ~RigidbodyConstraints.FreezePositionX;

    }

    //If this is at the same level as the player
    bool sameLevel()
    {
      return Mathf.Abs (player.transform.position.y - this.transform.position.y) < 4.5;
    }

    private void SetAttacking(bool toggle) {
      foreach (BossStats s in stats) {
        s.attacking = toggle;
      }
    }

    public void Dying()
    {
      dying = true;
      anim.SetTrigger ("Dying");
    }
  }
}