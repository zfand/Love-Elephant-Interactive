using UnityEngine;
using System.Collections;

namespace Boss
{
  public class SlothAI : MonoBehaviour
  {

    public GameObject player;
    public float chargeForce;
    public float chargeDistance;
    public float idleTime;
    public float idleMax;
    public float idleMin;
    public BossStats[] stats;
    private Animator anim;
    private Color origColor;
    private bool dying;
    private int facing;
    private bool turning = false;
    private bool startHit = false;
    private bool charging;
    private bool startdying = false;
    AnimatorStateInfo info;
    Transform stunSwirl;
    // Use this for initialization
    void Start()
    {
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
      rigidbody.AddForce(-Vector3.up * 10f);

      info = anim.GetCurrentAnimatorStateInfo (0);
      if (!dying) {
        if (info.IsName ("IdleState") && !anim.IsInTransition(0)) {
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

            if (roll >= 65) {
              anim.SetTrigger("Spin");
            } else if (roll >= 80) {
              anim.SetTrigger("Shock");
              StartCoroutine ( DirtyShock());
            } else {
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
          if (info.IsName ("Dying")) {
            startdying = true;
          }
        }
      }
    }

    IEnumerator WaitForSecs(float secs)
    {
      yield return new WaitForSeconds (secs);
    }

    IEnumerator Charge()
    {
      Vector3 startPos = transform.position;
      float deltaTime = 0;

      rigidbody.AddForce(Vector3.left * facing * chargeForce, ForceMode.Impulse);

      while (charging && Vector3.Distance(startPos, transform.position) < chargeDistance) {
        rigidbody.AddForce(Vector3.left * facing * chargeForce);
        yield return 0;
      }
      rigidbody.velocity = Vector3.zero;
      rigidbody.angularVelocity = Vector3.zero;

      while (deltaTime < 1f) {
        deltaTime += Time.deltaTime;
        yield return 0;
      }

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

    IEnumerator DirtyShock()
    {
      yield return 0;
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