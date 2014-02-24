using UnityEngine;
using System.Collections;
using LoveElephant;

namespace Item
{
  public class GrappleMechanic : MonoBehaviour
  {
    /// <summary>
    /// Represents the point where the player swings from
    /// </summary>
    public GameObject anchorPrefab;
    private GameObject anchor;
    /// <summary>
    /// The max length of Rope
    /// </summary>
    public float maxLength = 1f;
    public float extendTime = 1f;
    public float maxDashSpeed = 10f;
    public float yankLen;
    public float yankTime;
    public float yankForce;
    public float swingForce;

    /////////////////////////////////////////////////////////////////////////
    ///                     Private                                       ///
    /////////////////////////////////////////////////////////////////////////


    /// <summary>
    ///  Reference to the Animator component.
    /// </summary>
    private Animator anim;
    /// <summary>
    /// Reference to the Equipment Component on the Player
    /// </summary>
    private Equipment equip;
    /// <summary>
    /// Draws the line for the grappling rope
    /// </summary>
    private LineRenderer lr;
    /// <summary>
    /// The position the Grappling hook hit
    /// </summary>
    private Vector3 hitPos;
    /// The Current state of Grappling
    /// </summary>
    private GrappleState state;
    /// <summary>
    /// Reference to the player's Controller
    /// </summary>
    private PlayerController pController;
    /// <summary>
    /// Reference to the Player's HingeJoint
    /// </summary>
    private HingeJoint joint;
    /// <summary>
    /// Reference to the Anchor's HingeJoint
    /// </summary>
    private HingeJoint anchorJoint;
    /// <summary>
    /// The position the rope is displayed from
    /// </summary>
    private Transform ropePos;
    /// <summary>
    /// The Different States of Grappling
    /// </summary>
    private enum GrappleState
    {
      Off,
      Failed,
      Extending,
      Attached,
      Swinging
    }

    private void Start()
    {    
      state = GrappleState.Off;
      lr = this.GetComponent<LineRenderer> ();
      if (lr == null) {
        Debug.LogError("The Hookshot does not have a LineRenderer!");
      }
      anim = transform.parent.GetComponentInChildren<Animator> ();
      if (anim == null) {
        Debug.LogError ("The Parent does have an Animator!");
      }
      ropePos = transform.parent.Find ("RopePos");
      if (ropePos == null) {
        Debug.LogError ("There is no RopePos Transform on the Parent!");
      }
      pController = transform.parent.GetComponent<PlayerController> ();
      if (pController == null) {
        Debug.LogError ("There is no PlayerController Componenetnet on the Parent!");
      }
    }

    private void OnDrawGizmos()
    {
      if (hitPos != Vector3.zero) {
        Gizmos.color = Color.white;
        if (anchor != null) {
          Gizmos.DrawWireSphere (anchor.transform.position, .3f);
        }
      }
    }

    private void Update()
    {
      //Shoot out rope
      if (Input.GetButtonDown ("Fire1") && state == GrappleState.Off) {
        Shoot ();
      }

      //Attached
      if (state == GrappleState.Attached) {
        if (Vector3.Distance (hitPos, transform.parent.position) > maxLength) {
          StopSwing (true);
        }
      }

      //Let go while swinging
      if (!Input.GetButton ("Fire1") && state == GrappleState.Swinging) {
        StopSwing (true);
      }

      //Reel Up!
      if (Input.GetButton("Up") && state == GrappleState.Swinging) {
        StartCoroutine ("Reel", Vector3.up);
      }

      //Reel Up!
      if (Input.GetButton("Down") && state == GrappleState.Swinging) {
        StartCoroutine ("Reel", Vector3.down);
      }

      //Yank!
      if (Input.GetButtonDown ("Up") && state == GrappleState.Attached) {
        StartCoroutine ("Yank");
      }

      //Hit the Ground
      if (state == GrappleState.Swinging) {
        if (pController.grounded) {
          StopSwing (true);
        }
      }

      //Draw Rope
      if (state == GrappleState.Swinging || state == GrappleState.Attached) {
        lr.SetPosition (0, ropePos.position);
        lr.SetPosition (1, hitPos);
        lr.enabled = true;
        anim.SetBool ("Swing", true);
      } else if (state == GrappleState.Off) {
        lr.enabled = false;
        anim.SetBool ("Swing", false);
      }
    }

    /// <summary>
    /// Called when shoot button is clicked
    /// </summary>
    private void Shoot()
    {
      Vector3 clickedPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
      clickedPosition.z = 0;
      LayerMask layermask = ~(1 << LayerMask.NameToLayer ("Player"));      
      RaycastHit hit;
      Physics.Raycast (transform.parent.position, clickedPosition - transform.parent.position, out hit, 1000, layermask);
      var distance = Vector3.Distance (hit.point, transform.parent.position);
      //Hit!
      if (hit.collider) {
        //If the Object isn't grapplable
        if ((hit.collider.gameObject.tag == "Untagged")) {
          return;
        }
        //If the angle is too low
        float dot = Vector3.Dot (Vector3.up, (clickedPosition - transform.parent.position).normalized);
        if(dot < 0f) { // 0 is 90 degrees to the left or right
          return;
        }


        //Ok we're good create the point
        hitPos = new Vector3 (hit.point.x, hit.point.y, 0);
        SetAnchorPos(hitPos);
        //if the grapple point isn't too far away
        if (distance <= maxLength) {
          state = GrappleState.Extending;
          //if we're in the air start swinging
          if (!pController.grounded) {
            StartCoroutine ("ExtendRope", GrappleState.Swinging);
          } else {
            StartCoroutine ("ExtendRope", GrappleState.Attached);
          }
          //the grapple point is too far away
        } else {
          state = GrappleState.Failed;
          StartCoroutine ("ExtendRope", GrappleState.Off);
        } 
      }
    }

    /// <summary>
    /// Sets the anchor position and creates it if needed
    /// </summary>
    private void SetAnchorPos(Vector3 pos) 
    {
      if (anchor == null) {
        anchor = Instantiate(anchorPrefab) as GameObject;
      }
      anchor.transform.position = hitPos;
      anchor.transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// Extends the rope out of the player
    /// </summary>
    private IEnumerator ExtendRope(GrappleState completeState)
    {
      float deltaTime = 0f;
      Vector3 startPos = ropePos.position;

      lr.SetPosition (0, ropePos.position);
      lr.SetPosition (1, ropePos.position);
      lr.enabled = true;

      //shorten rope
      if (state == GrappleState.Failed) {
        hitPos = transform.parent.position + (hitPos - transform.parent.position).normalized * maxLength;
      }

      while (deltaTime < extendTime) {
        lr.SetPosition (0, ropePos.position);
        lr.SetPosition (1, Vector3.Lerp (startPos, hitPos, deltaTime / extendTime));
        deltaTime += Time.deltaTime;
        yield return 0;
      }
      if (state == GrappleState.Failed) {
        StopSwing (true);
      } else {
        state = completeState;     
        if (Input.GetButton ("Fire1") && state == GrappleState.Swinging) {
          StartSwing ();
        }
      }
    }

    /// <summary>
    /// Retracts the rope to the player
    /// </summary>
    private IEnumerator RetractRope(float retractTime)
    {
      float deltaTime = 0f;
      lr.enabled = true;
      if (Vector3.Distance (transform.parent.position, hitPos) > maxLength) {
        hitPos = transform.parent.position + (hitPos - transform.parent.position).normalized * maxLength;
      }
      while (deltaTime < retractTime) {
        lr.SetPosition (0, ropePos.position);
        lr.SetPosition (1, Vector3.Lerp (hitPos, ropePos.position, deltaTime / retractTime));
        deltaTime += Time.deltaTime;
        yield return 0;
      }
      lr.enabled = false;
      state = GrappleState.Off;
    }

    private IEnumerator Reel(Vector3 dir) 
    {
      joint.autoConfigureConnectedAnchor = false;
      Vector3 startPos = joint.connectedAnchor;
      Vector3 endPos = startPos+dir*yankLen;
      float deltaTime = 0f;

      while (deltaTime < yankTime) {
        joint.connectedAnchor = Vector3.Lerp(startPos,endPos, deltaTime/yankTime);
        deltaTime += Time.deltaTime;
        yield return 0;
      }
    }

    /// <summary>
    /// Update function for checking/updating the yanking
    ///  Returns True if yanking
    /// </summary>
    private IEnumerator Yank()
    {
      Vector3 startPos = transform.parent.position;
      Vector3 yankPos = transform.parent.position + (hitPos - transform.parent.position).normalized * yankLen;
      float detlaTime = 0f;

      while (transform.parent.position != yankPos) {
        transform.parent.position = Vector3.Lerp (startPos, yankPos, detlaTime / yankTime);
        lr.SetPosition (0, ropePos.position);
        detlaTime += Time.deltaTime;
        yield return 0;
      }
      if (Input.GetButton ("Fire1")) {
        StartSwing ();
      }
    }
 
    /// <summary>
    /// Starts the swinging process (creates Joints)
    /// </summary>
    private void StartSwing()
    {
      state = GrappleState.Swinging;

      //Create Joints
      joint = transform.parent.gameObject.AddComponent<HingeJoint> ();
      joint.axis = Vector3.back;
      joint.anchor = Vector3.zero;
      joint.connectedBody = anchor.rigidbody;
      anchorJoint = anchor.AddComponent<HingeJoint> ();
      anchorJoint.axis = Vector3.back;
      anchorJoint.anchor = Vector3.zero;

      //Add Force
      /*
    Vector3 dir = (anchor.transform.parent.position - transform.parent.position).normalized;
    Vector3 perp = Vector3.Cross(transform.parent.forward, dir);
    bool swingingRight = Vector3.Dot(perp,Vector3.up) > 0;

    if (swingingRight) {
      rigidbody.AddForce(Vector3.right*swingForce);
    } else {
      rigidbody.AddForce(Vector3.right*-swingForce);
    }
    */
    }

    /// <summary>
    /// Resets the player from swinging (deletes Joints)
    /// </summary>
    public void StopSwing(bool retract)
    {
      if (retract) {
        state = GrappleState.Failed;
        StartCoroutine ("RetractRope", extendTime * 0.8f);
      }

      DestroyImmediate (joint, true);
      DestroyImmediate (anchorJoint, true);
    }
  }
}
