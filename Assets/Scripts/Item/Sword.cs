using UnityEngine;
using System.Collections;
using LoveElephant;

namespace Item
{
  public class Sword : MonoBehaviour
  {
    public GameObject sword;
    public float speed = 20f;     // The speed the rocket will fire at.

  
    public float totaldegrees = 80;
    public float degreesperframe = 5;
    private PlayerController playerCtrl;    // Reference to the PlayerControl script.
    //private Animator anim;          // Reference to the Animator component.

    private Vector3 lastHit;
    private Vector3 hitPos;
    private bool isSwinging;
    private Vector3 OriginalSwordPosition;
    private Quaternion OriginalSwordRotation;
  
    void Start()
    {
      isSwinging = false;
      OriginalSwordPosition = sword.transform.localPosition;
      OriginalSwordRotation = sword.transform.localRotation;

      // Setting up the references.
      playerCtrl = transform.parent.gameObject.GetComponent<PlayerController> ();

      sword.SetActive (false);
    }

    void OnDrawGizmos()
    {
      if (hitPos != Vector3.zero) {
        //Gizmos.DrawSphere(hitPos, 1);
        //Gizmos.DrawRay(transform.position, (hitPos - transform.position)*10);
        Gizmos.color = Color.white;
        Gizmos.DrawLine (transform.position, hitPos);
      }
    }

    void Update()
    {
      // If the fire button is pressed...
      //if(Input.GetButtonDown("Fire1") && !isSwinging)
      if (Input.GetButtonDown ("Fire2") && !isSwinging) {
        // ... set the animator Shoot trigger parameter and play the audioclip.
//      anim.SetTrigger("Shoot");
        //      audio.Play();

        Vector3 clickedPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        hitPos = clickedPosition;
        hitPos.z = 0;
        hitPos.Normalize ();

        //LayerMask layermask = ~(1 << LayerMask.NameToLayer ("Player"));
        //Quaternion q = Quaternion.LookRotation (hitPos);
        Quaternion start = Quaternion.Euler (new Vector3 (0, 0, 60));
        Quaternion end = Quaternion.Euler (new Vector3 (0, 0, -60));
        this.StartCoroutine (SwingSword (start, end));
      }
    }

    IEnumerator SwingSword(Quaternion _start, Quaternion _end)
    {
      isSwinging = true;
      sword.SetActive (true);
      /**
    float swingTime = 0;
    float totalSwingTime = 10;
    Vector3 diff = this.transform.root.position - sword.transform.position;
    Quaternion start = _start;
    Quaternion end = _end;
    while(swingTime < totalSwingTime){
      if(!playerCtrl.facingRight)
      {
        end = _start;
        start = _end;
      } else {
        end = _end;
        start = _start;
      }
      swingTime++;
      sword.transform.RotateAround(
      //sword.transform.position = sword.transform.position - diff;
      sword.transform.rotation = Quaternion.Slerp(start, end, swingTime/totalSwingTime);
      //sword.transform.position = sword.transform.position + diff;
      yield return 0; 
    }
    */
      float degrees = 0;
      while (degrees < totaldegrees) {
        degrees += degreesperframe;
        if (!playerCtrl.facingRight) {
          sword.transform.RotateAround (playerCtrl.transform.position, new Vector3 (0, 0, 1), degreesperframe);
        } else {
          sword.transform.RotateAround (playerCtrl.transform.position, new Vector3 (0, 0, 1), -degreesperframe);
        }
        yield return 0;
      }
      isSwinging = false;
      sword.transform.localPosition = OriginalSwordPosition;
      sword.transform.localRotation = OriginalSwordRotation;
      sword.SetActive (false);
    }
  }
}
