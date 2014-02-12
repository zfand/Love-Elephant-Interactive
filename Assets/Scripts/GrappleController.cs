using UnityEngine;
using System.Collections;

public class GrappleController : MonoBehaviour
{
  /// <summary>
  /// Represents the point where the player swings from
  /// </summary>
  public GameObject anchor;
  /// <summary>
  /// The max length of Rope
  /// </summary>
  public float maxLength = 1f;

  public float extendTime = 1f;

  public float maxDashSpeed = 10f;
  public float yankLen;
  public float yankTime;
  public float swingForce;

  /////////////////////////////////////////////////////////////////////////
  ///                     Private                                       ///
  /////////////////////////////////////////////////////////////////////////


  /// <summary>
  ///  Reference to the Animator component.
  /// </summary>
  private Animator anim;
  /// <summary>
  /// Draws the line for the grappling rope
  /// </summary>
  private LineRenderer lr;
  /// <summary>
  /// The position the Grappling hook hit
  /// </summary>
  private Vector3 hitPos;
  /// <summary>
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
  private enum GrappleState {
    Off,
    Extending,
    Attached,
    Swinging
  }

  private void Awake()
  {
    anim = transform.root.gameObject.GetComponentInChildren<Animator> ();
    if (anim == null) {
      Debug.LogError ("The Player's Animator is NULL!");
    }
    ropePos = transform.Find ("RopePos");
    if (ropePos == null) {
      Debug.LogError ("There is no RopePos transform on the Player");
    }
    pController = this.GetComponent<PlayerController>();
    if (pController == null) {
      Debug.LogError ("The Player's PlayerController is NULL!");
    }
  }

  void Start()
  {
    state = GrappleState.Off;
    lr = this.GetComponent<LineRenderer> ();
  }

  void OnDrawGizmos()
  {
    if (hitPos != Vector3.zero) {
      Gizmos.color = Color.white;
      Gizmos.DrawLine (transform.position, hitPos);
      Gizmos.DrawWireSphere (anchor.transform.position, .3f);
      Gizmos.DrawWireSphere (transform.position, .3f);
    } else {
      //fix the center of the Object
      if (lr == null) {
        lr = this.GetComponent<LineRenderer> ();
      }
      lr.SetPosition(0,transform.position);
      lr.SetPosition(1,transform.position);

    }
  }

  private void Update()
  {
    if (Input.GetButtonDown("Fire1") && state == GrappleState.Off) {
      Shoot();
    }

    if (Input.GetButtonUp("Fire1") && state != GrappleState.Off) {
      StopSwing();
    }

    if (Input.GetButtonDown("Up") && state != GrappleState.Off) {
      StartCoroutine("Yank");
    }

    if (state == GrappleState.Swinging) {
      if (pController.grounded) {
        StopSwing();
      }
    }

    //Draw Rope
    if (state == GrappleState.Swinging || state == GrappleState.Attached) {
      lr.SetPosition (0, ropePos.position);
      lr.SetPosition (1, hitPos);
      lr.enabled = true;
      anim.SetBool("Swing", true);
    } else if (state == GrappleState.Off) {
      lr.enabled = false;
      anim.SetBool("Swing", false);
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
    Physics.Raycast (transform.position, clickedPosition - transform.position, out hit, maxLength, layermask);
    if (hit.collider && !(hit.collider.gameObject.tag == "Untagged")) {
      hitPos = new Vector3 (hit.point.x, hit.point.y, 0);
      anchor.transform.position = hitPos;
      anchor.transform.rotation = Quaternion.identity;
      state = GrappleState.Extending;
      if (!pController.grounded) {
        StartCoroutine("ExtendRope",GrappleState.Swinging);
      } else {
        StartCoroutine("ExtendRope",GrappleState.Attached);
      }
    }
  }

  /// <summary>
  /// Extends the rope out of the player
  /// </summary>
  /// <returns>The rope.</returns>
  private IEnumerator ExtendRope(GrappleState completeState) 
  {
    float deltaTime = 0f;

    lr.SetPosition (0, ropePos.position);
    lr.SetPosition (1, ropePos.position);
    lr.enabled = true;

    while (deltaTime < extendTime) {
      lr.SetPosition (0, ropePos.position);
      lr.SetPosition(1, Vector3.Lerp(ropePos.position,hitPos, deltaTime/extendTime));
      deltaTime += Time.deltaTime;
      yield return 0;
    }
    state = completeState;
    if (state == GrappleState.Swinging) {
      StartSwing();
    }
  }

  /// <summary>
  /// Update function for checking/updating the yanking
  ///  Returns True if yanking
  /// </summary>
  private IEnumerator Yank()
  {
    Vector3 startPos = transform.position;
    Vector3 yankPos = transform.position + (hitPos - transform.position).normalized * yankLen;
    float detlaTime = 0f;

    while (transform.position != yankPos)
    {
      transform.position = Vector3.Lerp(startPos, yankPos, detlaTime/yankTime);
      lr.SetPosition(0,ropePos.position);
      detlaTime += Time.deltaTime;
      yield return 0;
    }
    if (state == GrappleState.Swinging) {
      StopSwing();
    }
    StartSwing();
    rigidbody.velocity = Vector3.zero;
    rigidbody.angularVelocity = Vector3.zero;
  }
 
  /// <summary>
  /// Starts the swinging process (creates Joints)
  /// </summary>
  private void StartSwing()
  {
    state = GrappleState.Swinging;
    //Create Joints
    joint = gameObject.AddComponent<HingeJoint>();
    joint.axis = Vector3.back;
    joint.anchor = Vector3.zero;
    joint.connectedBody = anchor.rigidbody;
    anchorJoint  = anchor.AddComponent<HingeJoint>();
    anchorJoint.axis = Vector3.back;
    anchorJoint.anchor = Vector3.zero;

    //Add Force
    /*
    Vector3 dir = (anchor.transform.position - transform.position).normalized;
    Vector3 perp = Vector3.Cross(transform.forward, dir);
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
  public void StopSwing()
  {
    state = GrappleState.Off;
    DestroyImmediate(joint, true);
    DestroyImmediate(anchorJoint, true);
  }
}

