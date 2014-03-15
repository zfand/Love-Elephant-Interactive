using UnityEngine;
using System.Collections;

namespace Boss
{
  public class SlothAI : MonoBehaviour
  {

    public GameObject player;
    private Animator anim;
    private Color origColor;
    public float ChargeSpeed;
    private bool dying;
    private bool faceLeft;
    private bool turning = false;
    private bool startHit = false;
    private Material mat;
    private bool hitwall;
    private bool charging;
    public float IdleTime;
    public float IdleMax;
    public float IdleMin;
    private bool startdying = false;
    AnimatorStateInfo info;
    Transform stunSwirl;
    // Use this for initialization
    void Start()
    {
      anim = GetComponent<Animator> ();
      faceLeft = true;
      charging = false;
      player = GameObject.FindGameObjectWithTag ("Player");
      stunSwirl = transform.FindChild ("Stun");
      stunSwirl.gameObject.SetActive (false);
      /*
    mat = GetComponent<MeshRenderer>().material;
    origColor = mat.color;
    flashColor = Color.red;
    */
    }
  
    // Update is called once per frame
    void Update()
    {
      info = anim.GetCurrentAnimatorStateInfo (0);
      if (!dying) {

        if (info.IsName ("IdleState")) {
          stunSwirl.gameObject.SetActive (false); 
          facePlayer ();
          IdleTime--;
          if (IdleTime <= 0) {
            anim.SetTrigger ("BeginCharge");
            IdleTime = Mathf.Ceil (Random.Range (IdleMin, IdleMax));
          }

        } else if (!charging && info.IsName ("Charge") && !startHit) {
          charging = true;
          StartCoroutine (Charge ());
        } else if (info.IsName ("EndCharge")) {
          stunSwirl.gameObject.SetActive (true);  
          startHit = false;
        }
      } else {
        if (startdying) {
          if (info.IsName ("Dead")) {
            startdying = false;
            this.GetComponentInChildren<SlothBody>().Die();
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
      float newspeed = ChargeSpeed;
      if (faceLeft) {
        newspeed *= -1;
      }
      while (!hitwall) {
        transform.position = new Vector3 (transform.position.x + newspeed, transform.position.y, transform.position.z);
        yield return 0;
      }
      charging = false;
      hitwall = false;
      startHit = true;
      anim.SetTrigger ("HitWall");
    }

    public void HitWall()
    {
      if (charging) {
        hitwall = true;
      }
    }
  
    //Face the player
    void facePlayer()
    {
      float currentX = this.transform.eulerAngles.x;
      float currentY = this.transform.eulerAngles.y;
      if (!turning) {
        if (player.transform.position.x < this.transform.position.x && !faceLeft) {
          faceLeft = true;
          StartCoroutine (TurnSloth (new Vector3 (currentX, -currentY, 0f)));
        } else if (player.transform.position.x > this.transform.position.x && faceLeft) {
          faceLeft = false;
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