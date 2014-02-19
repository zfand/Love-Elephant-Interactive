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
    //replace this with a list of inputs to be done
  public bool
      jump = false;
    [HideInInspector]
    /// <summary>
  /// Determines if the player is currently touching the ground
  /// </summary>
  public bool
      grounded = false;
  
    /// <summary>
    /// Amount of Force added to move the player left or right.
    /// </summary>
    public float moveForce = 365f;
    /// <summary>
    /// The fastest the player can travel in the x axis.
    /// </summary>
    public float maxSpeed = 5f;  
    //TODO remove
    private float originalspeed;
    /// <summary>
    /// Amount of force added when the player jumps.
    /// </summary>
    public float jumpForce = 1000f;
    //TODO remove
    private float originalJumpForce;

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
      originalspeed = maxSpeed;
      originalJumpForce = jumpForce;
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

    public void EquipWeapon(string weapon)
    {
      Transform weapon_parent = this.transform.FindChild ("Weapon");
      Transform oldsword = weapon_parent.GetChild (0);
      GameObject newsword = (GameObject)Instantiate (Resources.Load (weapon), oldsword.position, oldsword.rotation);
      Vector3 oldpos = oldsword.transform.position;
      float diff = Mathf.Abs (oldsword.renderer.bounds.size.x - newsword.renderer.bounds.size.x);
      DestroyObject (oldsword.gameObject);
      newsword.transform.position = oldpos; 
      //newsword.transform.Translate(-diff/2, diff/2, 0);
      newsword.transform.parent = weapon_parent;
    
      Quaternion old_rotation = weapon_parent.rotation;
      weapon_parent.rotation = Quaternion.identity;
      newsword.transform.position = new Vector3 (newsword.transform.position.x - diff,
                                              newsword.transform.position.y, 
                                              newsword.transform.position.z);
      weapon_parent.rotation = old_rotation;
      //Debug.Break();


    }
  
    public void EquipBoots(string boot)
    {
      Transform boot_parent = this.transform.FindChild ("Boots");
      if (boot_parent.childCount > 0) {
        Transform oldboots = boot_parent.GetChild (0);

        if (oldboots != null) {
          DestroyObject (oldboots.gameObject);
        }
      }
      GameObject newBoots = (GameObject)Instantiate (Resources.Load ("Items/"+boot), boot_parent.position, boot_parent.rotation);

      newBoots.transform.parent = boot_parent;

      BootStats stats = newBoots.GetComponent<BootStats> ();
      maxSpeed = originalspeed;
      maxSpeed += stats.SpeedMod;
      jumpForce = originalJumpForce;
      jumpForce *= stats.JumpMod;
    }
  
    public void EquipHookshot(string hookshot)
    {

    }
  }
}
