using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  public class GrappleMechanic : MonoBehaviour
  {
    /// <summary>
    /// Represents the point where the player swings from
    /// </summary>
    public GameObject anchorPrefab;
    private GameObject anchor;
    /// <summary>
    /// The max length of the Rope
    /// </summary>
    public float maxRopeLength = 1f;
    /// <summary>
    /// The minimum length of the Rope
    /// </summary>
    public float minRopeLength = 1f;
    /// <summary>
    /// The extend time it takes for the rope to extend
    /// </summary>
    public float extendTime = 1f;
    /// <summary>
    /// The length of the yank.
    /// </summary>
    public float yankLen;
    /// <summary>
    /// The time it takes to reel the rope.
    /// </summary>
    public float reelTime;
    /// <summary>
    /// The yank force.
    /// </summary>
    public float yankForce;
    /// <summary>
    /// The swing force.
    /// </summary>
    public float swingForce;
  
    /// <summary>
    /// The sprite covering the end of the line 
    /// </summary>
    public GameObject grappleSpike;
    public AudioClip grapple_shoot;
    public AudioClip grapple_hit;
    public AudioClip grapple_extend;
    public AudioSource[] audioSources;

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
    /// If the player is currently Ground Reeling
    /// </summary>
    private bool groundReeling;
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
      groundReeling = false;
      lr = this.GetComponent<LineRenderer> ();
      if (lr == null) {
        Debug.LogError ("The Hookshot does not have a LineRenderer!");
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
        if (Vector3.Distance (hitPos, transform.parent.position) > maxRopeLength) {
          StopSwing (true);
        }
      }

      //Let go while swinging
      if (!Input.GetButton ("Fire1") && (state == GrappleState.Swinging || state == GrappleState.Attached)) {
        StopSwing (true);
      }

      //Reel Up!
      if (Input.GetButton ("Up") && state == GrappleState.Swinging) {
        float ropLen = Vector3.Distance (transform.parent.position, anchor.transform.position);
        if (ropLen >= minRopeLength) {
          StopCoroutine ("GroundReel");
          StartCoroutine ("Reel", Vector3.up);
        }
        audioSources [2].loop = false;
      }

      //let out the line
      else if (Input.GetButton ("Down") && state == GrappleState.Swinging) {
        float ropLen = Vector3.Distance (transform.position, anchor.transform.position);
        if (ropLen <= maxRopeLength) {
          StartCoroutine ("Reel", Vector3.down);
        }
        audioSources [2].loop = false;
      }

      //Reel up from ground!
      if (Input.GetButton ("Up") && state == GrappleState.Attached && !groundReeling) {
        StartCoroutine ("GroundReel");
      }

      //Yank!
      if (Input.GetButtonDown ("Jump") && (state == GrappleState.Attached || state == GrappleState.Swinging)) {
        float force = yankForce;
        if (state == GrappleState.Swinging) {
          force *= 2;
        }
        transform.parent.rigidbody.AddForce ((hitPos - transform.parent.position).normalized * force, ForceMode.VelocityChange);
        StopSwing (true);

      }

      //walk off a ledge while attached
      if (state == GrappleState.Attached && !pController.grounded) {
        StartSwing ();
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
        if (state == GrappleState.Swinging) {
          anim.SetBool ("Swing", true);
        }
        grappleSpike.transform.position = hitPos;
      } else if (state == GrappleState.Off) {
        grappleSpike.SetActive (false);
        lr.enabled = false;
        anim.SetBool ("Swing", false);
      }

      if (state != GrappleState.Off) {
        grappleSpike.SetActive (true);
      }
  
    }

    private void FixedUpdate()
    {
      //if swinging
      if (state == GrappleState.Swinging) {
        float h = Input.GetAxis ("Horizontal");
        transform.parent.rigidbody.AddForce (Vector2.right * h * swingForce, ForceMode.Acceleration);
        transform.parent.rigidbody.AddForce (-Vector3.up * pController.gravity, ForceMode.Acceleration);  
      }
      //delete joints 
      else if (joint != null) {
        StopSwing (false);
      }
    }

    /// <summary>
    /// Called when shoot button is clicked
    /// </summary>
    private void Shoot()
    {
      Vector3 clickedPosition = Input.mousePosition;
      //Find the distance the camera is from the play plane
      clickedPosition.z = Camera.main.transform.position.z * -1;
      clickedPosition = Camera.main.ScreenToWorldPoint (clickedPosition);
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

        audioSources [1].clip = grapple_shoot;
        audioSources [1].Play ();
        
        //If the angle is too low
        float dot = Vector3.Dot (Vector3.up, (clickedPosition - transform.parent.position).normalized);
        if (dot < 0f) { // 0 is 90 degrees to the left or right
          return;
        }

        //Ok we're good create the point
        hitPos = new Vector3 (hit.point.x, hit.point.y, 0);
        SetAnchorPos (hitPos);
    
        //if the grapple point isn't too far away
        if (distance <= maxRopeLength) {
          state = GrappleState.Extending;
          //if we're in the air start swinging
          if (!pController.grounded) {
            StopCoroutine ("ExtendRop");
            StartCoroutine ("ExtendRope", GrappleState.Swinging);
          } else {
            StopCoroutine ("ExtendRop");
            StartCoroutine ("ExtendRope", GrappleState.Attached);
          }
          //the grapple point is too far away
        } else {
          state = GrappleState.Failed;
          StopCoroutine ("ExtendRop");
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
        anchor = Instantiate (anchorPrefab) as GameObject;
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
        hitPos = transform.parent.position + (hitPos - transform.parent.position).normalized * maxRopeLength;
      }

      while (deltaTime < extendTime) {
        lr.SetPosition (0, ropePos.position);
        lr.SetPosition (1, Vector3.Lerp (startPos, hitPos, deltaTime / extendTime));
        grappleSpike.transform.position = Vector3.Lerp (startPos, hitPos, deltaTime / extendTime);
        deltaTime += Time.deltaTime;
        yield return 0;
      }
      if (state == GrappleState.Failed) {
        StopSwing (true);
      } else {
        audioSources [0].clip = grapple_hit;
        audioSources [0].Play ();
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
      if (Vector3.Distance (transform.parent.position, hitPos) > maxRopeLength) {
        hitPos = transform.parent.position + (hitPos - transform.parent.position).normalized * maxRopeLength;
      }
      while (deltaTime < retractTime) {
        lr.SetPosition (0, ropePos.position);
        lr.SetPosition (1, Vector3.Lerp (hitPos, ropePos.position, deltaTime / retractTime));
        grappleSpike.transform.position = Vector3.Lerp (hitPos, ropePos.position, deltaTime / retractTime);
        deltaTime += Time.deltaTime;
        yield return 0;
      }
      lr.enabled = false;
      state = GrappleState.Off;
    }

    /// <summary>
    /// Reel the rope in or out depending on the direction
    /// </summary>
    private IEnumerator Reel(Vector3 dir)
    {
      joint.autoConfigureConnectedAnchor = false;
      Vector3 startPos = joint.connectedAnchor;
      Vector3 endPos = startPos + dir * yankLen;
      float deltaTime = 0f;

      while (deltaTime < reelTime) {

        if (!audioSources [2].isPlaying) {
          audioSources [2].loop = true;
          audioSources [2].clip = grapple_extend;
          audioSources [2].Play ();
        }

        // check to see if we're still swinging
        if (state != GrappleState.Swinging) {
          break;
        }
        joint.connectedAnchor = Vector3.Lerp (startPos, endPos, deltaTime / reelTime);
        deltaTime += Time.deltaTime;
        yield return 0;
      }
    }

    /// <summary>
    /// Reels the rope up from the ground and starts swinging!
    /// </summary>
    private IEnumerator GroundReel()
    {
      groundReeling = true;
      Vector3 startPos = transform.parent.position;
      Vector3 yankPos = transform.parent.position + (hitPos - transform.parent.position).normalized * yankLen;
      float detlaTime = 0f;

      while (transform.parent.position != yankPos) {
        transform.parent.position = Vector3.Lerp (startPos, yankPos, detlaTime / reelTime);
        lr.SetPosition (0, ropePos.position);
        detlaTime += Time.deltaTime;
        yield return 0;
      }
      if (Input.GetButton ("Fire1")) {
        StartSwing ();
      }
      groundReeling = false;
    }
 
    /// <summary>
    /// Starts the swinging process (creates Joints)
    /// </summary>
    private void StartSwing()
    {
      state = GrappleState.Swinging;
      pController.inputEnabled = false;

      //Create Joints
      joint = transform.parent.gameObject.AddComponent<HingeJoint> ();
      joint.axis = Vector3.back;
      joint.anchor = Vector3.zero;
      joint.connectedBody = anchor.rigidbody;
      anchorJoint = anchor.AddComponent<HingeJoint> ();
      anchorJoint.axis = Vector3.back;
      anchorJoint.anchor = Vector3.zero;
    }

    /// <summary>
    /// Resets the player from swinging (deletes Joints)
    /// </summary>
    public void StopSwing(bool retract)
    {
      pController.inputEnabled = true;
      
      if (retract) {
        state = GrappleState.Failed;
        StopCoroutine ("RetractRope");
        StartCoroutine ("RetractRope", extendTime * 0.8f);
      }

      DestroyImmediate (joint, true);
      DestroyImmediate (anchorJoint, true);
    }
  }
}
