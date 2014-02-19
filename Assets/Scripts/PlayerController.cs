using UnityEngine;
using System.Collections;
using Item;

namespace LoveElephant
{
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
    [HideInInspector]
    /// <summary>
  /// Determines if the player is currently touching the ground
  /// </summary>
  public bool
      grounded = false;
    private MovementStats mStats;
    public MovementStats movementStats {
      set { this.mStats =  value;}
    }
    
    /// <summary>
    /// Reference to the player's Animator
    /// </summary>
    private Animator anim;
    /// <summary>
    /// A point at the bottom of the player
    /// </summary>
    private Transform groundCheck;

    private Equipment equip;
  
    private void Awake()
    {
      anim = this.GetComponentInChildren<Animator> ();
      groundCheck = transform.Find ("GroundCheck");
      equip = this.GetComponent<Equipment>();

      if (anim == null) {
        Debug.LogError ("The Player's Animator is NULL!");
      }

      if (groundCheck == null) {
        Debug.LogError ("There is no GroundCheck transform on the Player");
      }

      if (equip == null) {
        Debug.LogError ("There is no Equipment component on the Player");
      }
    }
  

    // Use this for initialization
    private void Start()
    {
      mStats = equip.boot.GetComponent<Boot>().stats;
    }
  
    // Update is called once per frame
    private void Update()
    {
  
      // The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
      grounded = Physics.Linecast (transform.position, groundCheck.position);
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
      if (h * rigidbody.velocity.x < mStats.maxSpeed) {
        rigidbody.AddForce (Vector2.right * h * mStats.moveForce);
      }
      if (Mathf.Abs (rigidbody.velocity.x) > mStats.maxSpeed) {
        rigidbody.velocity = new Vector3 (Mathf.Sign (rigidbody.velocity.x) * mStats.maxSpeed, rigidbody.velocity.y, 0);
      }

      // If the player should jump...
      if (jump) {
        // Set the Jump animator trigger parameter.
        anim.SetTrigger ("Jump");

        // Add a vertical force to the player.
        rigidbody.AddForce (new Vector3 (0f, mStats.jumpForce, 0));
      
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
}
