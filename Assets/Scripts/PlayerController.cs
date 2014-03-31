using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  public class PlayerController : MonoBehaviour
  {
    [HideInInspector]
    /** Determines if the player is currently facing Right**/
    public bool
      facingRight = false;
    [HideInInspector]
    /// <summary>
    /// replace this with a list of inputs to be done
    /// </summary>
  public bool
      jump = false;
    [HideInInspector]
    /// <summary>
    /// Determines if the input is enabled or disabled
    /// </summary>
    public bool

      inputEnabled = true;
    [HideInInspector]
    /// <summary>
  /// Determines if the player is currently touching the ground
  /// </summary>
  public bool
      grounded = false;
    private MovementStats mStats;

    public MovementStats movementStats {
      set { this.mStats = value;}
    }
    /// <summary>
    /// The gravity only affecting the player
    /// </summary>
    public float gravity;
    /// <summary>
    /// Flag for when the player is colliding with anything
    /// </summary>
    private bool isColliding;
    /// <summary>
    /// Reference to the player's Animator
    /// </summary>
    private Animator anim;
    /// <summary>
    /// A point at the bottom of the player
    /// </summary>
    public Transform groundCheck;
    private Equipment equip;
      
    private void Awake()
    {
      anim = this.GetComponentInChildren<Animator> ();
      equip = this.GetComponent<Equipment> ();

      if (anim == null) {
        Debug.LogError ("The Player's Animator is NULL!");
      }

      if (equip == null) {
        Debug.LogError ("There is no Equipment component on the Player");
      }
    }
  

    // Use this for initialization
    private void Start()
    {
      mStats = equip.boot.GetComponent<Boot> ().stats;
    }
  
    // Update is called once per frame
    private void Update()
    {
      // The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
      grounded |= Physics.Linecast (transform.position, groundCheck.position, ~(1 << LayerMask.NameToLayer ("Player")));

      anim.SetBool ("Grounded", grounded);
      
      // If the jump button is pressed and the player is grounded then the player should jump.
      if (Input.GetButtonDown ("Jump") && grounded) {
        jump = true;
        grounded = false;
      }

      if (rigidbody.velocity.magnitude > 50) {
        rigidbody.velocity = rigidbody.velocity.normalized * 50;
      }
    }

    private void FixedUpdate()
    {
      if (inputEnabled) {
        float h = Input.GetAxis ("Horizontal");

        anim.SetFloat ("Speed", Mathf.Abs (h));

        //Flip Facing Direction
        if (h > 0 && !facingRight || h < 0 && facingRight) {
          FlipFacing ();
        }

        //Make sure you're not grinding up against a wall
        if (grounded || !isColliding) {
          if (grounded && h == 0) {
            //rigidbody.velocity = Vector3.zero;
            //rigidbody.angularVelocity = Vector3.zero;
          } else if (h * rigidbody.velocity.x < mStats.maxRunSpeed) {
            rigidbody.AddForce (Vector3.right * h * mStats.moveForce);
          }
          if (Mathf.Abs (rigidbody.velocity.x) > mStats.maxRunSpeed && grounded) {
            rigidbody.velocity = new Vector3 (Mathf.Sign (rigidbody.velocity.x) * mStats.maxRunSpeed, rigidbody.velocity.y, 0);
          }
        }
      
        // If the player should jump...
        if (jump) {
          // Set the Jump animator trigger parameter.
          anim.SetTrigger ("Jump");

          // Add a vertical force to the player.
          rigidbody.AddForce (new Vector3 (0f, mStats.jumpForce, 0f), ForceMode.VelocityChange);
      
          jump = false;
        }

        // fake gravity
        rigidbody.AddForce (-Vector3.up * gravity, ForceMode.Acceleration);  
      }
    }

    private void OnTriggerEnter(Collider c)
    {
      grounded = true;
    }

    private void OnTriggerExit(Collider c)
    {
      grounded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
      isColliding = true;
    }

    private void OnCollisionExit(Collision collision)
    {
      isColliding = false;
    }

    /// <summary>
    ///  Flips the direction the player is facing
    /// </summary>
    private void FlipFacing()
    {
      facingRight = !facingRight;
      Vector3 scale = transform.localScale;
      transform.Rotate (new Vector3 (0, 180, 0));
      scale.x *= -1;
      //transform.localScale = scale;
    }
  }
}
