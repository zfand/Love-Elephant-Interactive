using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
  [HideInInspector]
  /** Determines if the player is currently facing Right**/
    public bool
    facingRight = false;
  [HideInInspector]
  //replace this with a list of inputs to be done
  public bool
    jump = false;
  
  /// <summary>
  /// Amount of Force added to move the player left or right.
  /// </summary>
  public float moveForce = 365f;
  /// <summary>
  /// The fastest the player can travel in the x axis.
  /// </summary>
  public float maxSpeed = 5f;

  /// <summary>
  /// Amount of force added when the player jumps.
  /// </summary>
  public float jumpForce = 1000f;

  /// <summary>
  /// Reference to the player's Animator
  /// </summary>
  private Animator anim;

  /// <summary>
  /// A point at the bottom of the player
  /// </summary>
  private Transform groundCheck;
  
  private void Awake()
  {
    anim = this.GetComponentInChildren<Animator> ();
    groundCheck = transform.Find ("GroundCheck");

    if (anim == null) {
      Debug.LogError ("The Player's Animator is NULL!");
    }

    if (groundCheck == null) {
      Debug.LogError ("There is no GroundCheck transform on the Player");
    }
  }
  

  // Use this for initialization
  private void Start()
  {
  
  }
  
  // Update is called once per frame
  private void Update()
  {
  
    // The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
    bool grounded = Physics.Linecast (transform.position, groundCheck.position);
    //.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));  

    anim.SetBool ("Grounded", grounded);
    
    // If the jump button is pressed and the player is grounded then the player should jump.
    if (Input.GetButtonDown ("Jump") && grounded)
      jump = true;

    if (rigidbody.velocity.magnitude > 50) {
      rigidbody.velocity /= 2;
    }

  }

  private void FixedUpdate()
  {
    float h = Input.GetAxis ("Horizontal");

    anim.SetFloat ("Speed", Mathf.Abs (h));

    //Flip Facing Direction
    if (h > 0 && !facingRight || h < 0 && facingRight) {
      FlipFacing ();
    }

    //Add force
    if (h * rigidbody.velocity.x < maxSpeed) {
      rigidbody.AddForce (Vector2.right * h * moveForce);
    }
    if (Mathf.Abs (rigidbody.velocity.x) > maxSpeed) {
      rigidbody.velocity = new Vector3 (Mathf.Sign (rigidbody.velocity.x) * maxSpeed, rigidbody.velocity.y, 0);
    }

    // If the player should jump...
    if (jump) {
      // Set the Jump animator trigger parameter.
      anim.SetTrigger ("Jump");

      // Add a vertical force to the player.
      rigidbody.AddForce (new Vector3 (0f, jumpForce, 0));
      
      jump = false;
    }

  }

  /// <summary>
  ///  Flips the direction the player is facing
  /// </summary>
  private void FlipFacing()
  {
    facingRight = !facingRight;
    Vector3 scale = transform.localScale;
    scale.x *= -1;
    transform.localScale = scale;
  }
}

