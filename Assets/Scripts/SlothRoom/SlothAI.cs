using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  public class SlothAI : MonoBehaviour
  {

    public GameObject player;
    private Animator anim;
    private Color origColor;
    public float Health = 100;
    private bool dying;
    private bool faceLeft;
    private bool turning = false;
    private Material mat;
    public GameObject keyDrop;

    // Use this for initialization
    void Start()
    {
	  anim = GetComponent<Animator> ();
      faceLeft = true;
      player = GameObject.FindGameObjectWithTag ("Player");
      /*
    mat = GetComponent<MeshRenderer>().material;
    origColor = mat.color;
    flashColor = Color.red;
    */
    }
  
    // Update is called once per frame
    void Update()
    {
      /*if (!dying) {
        if (!anim.IsPlaying ("Sloth_Charge"))
          facePlayer ();

        if (sameLevel () && false) {
          if (anim.IsPlaying ("Sloth_BeginCharge")) {
            anim.PlayQueued ("Sloth_Charge", QueueMode.CompleteOthers);
          } else if (anim.IsPlaying ("Sloth_Charge")) {
            charge (faceLeft);
          } else if (anim.IsPlaying ("Sloth_EndCharge")) {
            anim.PlayQueued ("Sloth_Idle", QueueMode.CompleteOthers);
          } else if (anim.IsPlaying ("Sloth_Idle")) {
            StartCoroutine (WaitForSecs (2f));
            anim.PlayQueued ("Sloth_BeginCharge", QueueMode.PlayNow);
          }
        } else {
          if (!anim.IsPlaying ("Sloth_Idle") && !anim.IsPlaying ("Sloth_Charge")) {
            anim.Stop ();
            anim.PlayQueued ("Sloth_Idle", QueueMode.PlayNow);
          }
        }
      }
      */
    }

    IEnumerator WaitForSecs(float secs)
    {
      yield return new WaitForSeconds (secs);
    }

    void charge(bool lookL)
    {
      float xPos = transform.position.x;
      float yPos = transform.position.y;

      if (faceLeft) {
        transform.position = new Vector3 (xPos - 0.1f, yPos, 0f);
      } else {
        transform.position = new Vector3 (xPos + 0.1f, yPos, 0f);
      }
    }

    void OnCollisionEnter(Collision hit)
    {
      if (hit.gameObject.tag == "Wall") {
        facePlayer ();
        //anim.PlayQueued ("Sloth_EndCharge", QueueMode.PlayNow);
      }
    }
  


    //Face the player
    void facePlayer()
    {

      float currentX = this.transform.eulerAngles.x;
      float currentY = this.transform.eulerAngles.y;
      if (!turning) {
        if (player.transform.position.x < this.transform.position.x && !faceLeft) {
          Debug.Log ("turnleft");
          faceLeft = true;
          StartCoroutine (TurnSloth (new Vector3 (currentX, -currentY, 0f)));
        } else if (player.transform.position.x > this.transform.position.x && faceLeft) {
          Debug.Log ("turnright");
          faceLeft = false;
          StartCoroutine (TurnSloth (new Vector3 (currentX, -currentY, 0f)));
        }
      }
    }

    IEnumerator TurnSloth(Vector3 dest)
    {
      float time = 10f;
      float currtime = 0f;
      float interval = 0.1f;
      Vector3 startangle = this.transform.eulerAngles;
      turning = true;
      while (currtime <= time) {
        this.transform.eulerAngles = Vector3.Lerp (startangle, dest, currtime / time);
        currtime += interval;
        yield return 0;
      }
      turning = false;
      this.transform.eulerAngles = dest;
    }
    //If this is at the same level as the player
    bool sameLevel()
    {

      float heightDiff = Mathf.Abs (player.transform.position.y - this.transform.position.y);

      if (heightDiff < 4.5) {
        return true;
      }

      return false;
    }

    public void Dying()
    {
      dying = true;
      Vector3 keyPos = new Vector3 (transform.position.x, transform.position.y + 2f, 0f);
      Instantiate (keyDrop, keyPos, Quaternion.identity);
    }
  }
}