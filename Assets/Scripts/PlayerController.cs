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
	private bool lastGrounded;
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
    /// <summary>
    /// Reference to the equip of the player
    /// </summary>
    private Equipment equip;
    /// <summary>
    /// The last input from the user
    /// </summary>
    private float lastInput;
	

	private Vector3 mouse;
	

	public Texture2D Reticle;

	public AudioSource[] audioSources;
	public AudioClip player_run;
	public AudioClip player_land;
      
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
	  lastGrounded = true;
      lastInput = 1000f;
    }
  
    // Update is called once per frame
    private void Update()
    {
	  mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (transform.position - Camera.main.transform.position).magnitude));

      
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
		
	  if (!lastGrounded && grounded && !jump) {
		audioSources[1].clip = player_land;
		audioSources[1].Play ();
	  }

	  lastGrounded = grounded;
    }

    private void FixedUpdate()
	{  
	  
		
	  //Flip Facing Direction
	  if (!FacingMouse(mouse)) {
	  	FlipFacing ();
	  }

      if (inputEnabled) {
        float h = Input.GetAxis ("Horizontal");
        if (Mathf.Abs (h) < Mathf.Abs (lastInput)) {
          lastInput = h;
          h = 0;
          anim.SetFloat ("Speed", 0);
        } else {
          lastInput = h;
          anim.SetFloat ("Speed", Mathf.Abs (h));
        }

		if (h != 0 && !audioSources[2].isPlaying && grounded) {
			audioSources[2].loop = true;
			audioSources[2].clip = player_run;
			audioSources[2].Play ();
		} else if (h == 0 || !grounded) {
	 		audioSources[2].Stop ();
			audioSources[2].loop = false;
		}
        //Make sure you're not grinding up against a wall
        if (grounded || !isColliding) {
          if (grounded && h == 0) {
            //rigidbody.velocity = Vector3.zero;
            //rigidbody.angularVelocity = Vector3.zero;
          } else if (h * rigidbody.velocity.x < mStats.maxRunSpeed) {
            if (grounded) {
              rigidbody.AddForce (Vector3.right * h * mStats.moveForce);
            } else if (h != 0) {
              rigidbody.AddForce ((Vector3.right * h) * (mStats.moveForce/2));
            }
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

	private bool FacingMouse(Vector3 mouse) {

		return (mouse.x >= this.transform.position.x && facingRight) ||
				(mouse.x < this.transform.position.x && !facingRight);

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

	void OnGUI() {
			float scale = 2;
			Rect position = new Rect( Input.mousePosition.x - ((Reticle.width*scale) / 2), (Screen.height - Input.mousePosition.y) - ((Reticle.height*scale) / 2), Reticle.width*2 , Reticle.height*2 );
			GUI.DrawTexture(position, Reticle, ScaleMode.StretchToFill);
	}
  }
}
