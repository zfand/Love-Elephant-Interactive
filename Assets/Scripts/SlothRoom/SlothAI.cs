using UnityEngine;
using System.Collections;

namespace Boss
{
  public class SlothAI : MonoBehaviour
  {

    public GameObject player;
    public float chargeSpeed;
    public float chargeDistance;
    public float idleTime;
    public float idleMax;
    public float idleMin;
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
    void Update()
    {
      info = anim.GetCurrentAnimatorStateInfo (0);
      if (!dying) {

        if (info.IsName ("IdleState")) {
          stunSwirl.gameObject.SetActive (false); 
          facePlayer ();
          idleTime -= Time.deltaTime;
          if (idleTime <= 0) {
            idleTime = Mathf.Ceil (Random.Range (idleMin, idleMax));

            float roll = Random.Range (0,100);
            float dist = Vector3.Distance(player.transform.position, transform.position);

            //if close
            roll = (dist < 5f) ? roll + 35 : roll;
            roll = (dist < 15f) ? roll + 25 : roll;
            //if too far
            roll = (dist > 30f) ? roll - 65 : roll;

            if (roll >= 65) {
              anim.SetTrigger("Spin");
              StartCoroutine ( DirtySpin());
            } else if (roll >= 80) {
              anim.SetTrigger("Shock");
              StartCoroutine ( DirtyShock());
            } else {
              anim.SetTrigger ("BeginCharge");
            }
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
      float newspeed = chargeSpeed * facing;
      Vector3 startPos = transform.position;

      while (charging && Vector3.Distance(startPos, transform.position) < chargeDistance) {
        transform.position = new Vector3 (transform.position.x - newspeed, transform.position.y, transform.position.z);
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

    IEnumerator DirtySpin()
    {
      float interval = 15f;
      float totalrot = 0;
      Vector3 startRotation = this.transform.eulerAngles;

      bool rotating = true;
      turning = true;

      while (rotating) { 
        totalrot += interval;
        if (totalrot >= 720) {
          rotating = false;
          yield return 0;
        }
        transform.Rotate (new Vector3 (0f, interval, 0f));
        yield return 0;
      }
      turning = false;
      
      this.transform.eulerAngles = startRotation;
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

    public void Dying()
    {
      dying = true;
      anim.SetTrigger ("Dying");
    }
  }
}