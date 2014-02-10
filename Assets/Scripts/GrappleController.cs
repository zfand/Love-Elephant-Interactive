using UnityEngine;
using System.Collections;

public class GrappleController : MonoBehaviour
{
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
  /// Whether the player is swinging
  /// </summary>
  private bool isSwinging;
  private bool isYanking;
  private Vector3 startPos;
  private float startGrappleTime;
  private float ropeLen = 0f;
  public float maxDashSpeed = 10f;
  private float startDashSpeed;
  private float dashSpeed;
  public GameObject grapplePivot;
  private float lastPercentDone;
  public float yankLength;
  public float swingForce;
  private Vector3 swingPos;
  private Transform ropePos;
  private HingeJoint hinge;
  private HingeJoint pivotHinge;

  void Start()
  {
    startDashSpeed = maxDashSpeed/5;
    lr = this.GetComponent<LineRenderer> ();
    isSwinging = false;
    isYanking = false;
  }

  void OnDrawGizmos()
  {
    if (hitPos != Vector3.zero) {
      Gizmos.color = Color.white;
      Gizmos.DrawLine (transform.position, hitPos);
      Gizmos.DrawSphere (grapplePivot.transform.position, .3f);
      Gizmos.DrawSphere (transform.position, .3f);
    } else {
      if (lr == null) {
        lr = this.GetComponent<LineRenderer> ();
      }
      lr.SetPosition(0,transform.position);
      lr.SetPosition(1,transform.position);

    }
  }

  private void Update()
  {
    YankingUpdate ();
    SwingingUpdate ();

    //Draw Rope
    if (isYanking || isSwinging) {
      lr.SetPosition (0, ropePos.position);
      lr.SetPosition (1, hitPos);
      lr.enabled = true;
      anim.SetBool("Swing", true);
      //rigidbody.useGravity = false;

    } else {
      lr.enabled = false;
      anim.SetBool("Swing", false);
    }
  }

  /// <summary>
  /// Update function for checking/updating the yanking
  ///  Returns True if yanking
  /// </summary>
  private void YankingUpdate()
  {
    // if Button pressed and in Yanking state
    if (Input.GetButton ("Fire1") && isYanking) {
      dashSpeed = Mathf.Lerp (startDashSpeed, maxDashSpeed, lastPercentDone);
      float distanceTraveled = (Time.time - startGrappleTime) * dashSpeed;
      float percentDone = distanceTraveled / (ropeLen * yankLength);
      lastPercentDone = percentDone;
      transform.position = Vector3.Lerp (startPos, swingPos, percentDone);
      if (percentDone >= 1f) {
        isYanking = false;
        isSwinging = true;
        StartSwing();
      }
      //if let go of the button
    } else if (!Input.GetButtonDown ("Fire1") && isYanking) {     
      isYanking = false;
      rigidbody.velocity = Vector3.zero;
      rigidbody.angularVelocity = Vector3.zero;
      Vector3 aftershock = hitPos - transform.position;
      rigidbody.AddForce (aftershock * (30 * dashSpeed));
      dashSpeed = startDashSpeed;
    }
  }

  private void SwingingUpdate()
  {
    if (Input.GetButtonDown ("Fire1") && !isYanking) {
      Vector3 clickedPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
      clickedPosition.z = 0;
      LayerMask layermask = ~(1 << LayerMask.NameToLayer ("Player"));
      RaycastHit hit;
      Physics.Raycast (transform.position, clickedPosition - transform.position, out hit, 1000, layermask);
      if (hit.collider) {
        if (hit.collider.gameObject.tag == "Untagged") {
          return;
        }
        
        startGrappleTime = Time.time;
        hitPos = new Vector3 (hit.point.x, hit.point.y, 0);
        grapplePivot.transform.position = hitPos;
        grapplePivot.transform.rotation = Quaternion.identity;
        isYanking = true;
        startPos = transform.position;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        ropeLen = Vector3.Distance (startPos, grapplePivot.transform.position);
        swingPos = Vector3.Lerp (startPos, grapplePivot.transform.position, .6f);
      }
    } else if (!Input.GetButton ("Fire1") && isSwinging) {
      isSwinging = false;
      dashSpeed = startDashSpeed;
      transform.parent = null;
      Destroy(hinge);
      Destroy(pivotHinge);
    }

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
  }

  private void StartSwing()
  {
    hinge = gameObject.AddComponent<HingeJoint>();
    hinge.axis = Vector3.back;
    hinge.anchor = Vector3.zero;
    hinge.connectedBody = grapplePivot.rigidbody;
    pivotHinge  = grapplePivot.AddComponent<HingeJoint>();
    pivotHinge.axis = Vector3.back;
    pivotHinge.anchor = Vector3.zero;

    Vector3 dir = (grapplePivot.transform.position - transform.position).normalized;
    Vector3 perp = Vector3.Cross(transform.forward, dir);
    bool swingingRight = Vector3.Dot(perp,Vector3.up) > 0;

    if (swingingRight) {
      rigidbody.AddForce(Vector3.right*swingForce);
    } else {
      rigidbody.AddForce(Vector3.right*-swingForce);
    }
  }
}

