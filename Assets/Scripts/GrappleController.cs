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
	/// Reference to the SpringJoint
	/// </summary>
	private HingeJoint hinge;
	/// <summary>
	/// Whether the player is swinging
	/// </summary>
	private bool isSwinging;
	private bool isYanking;

	private Vector3 startPos;
	private float startGrappleTime;
	private float ropeLen = 0f;
	public float dashSpeed = 0f;
	private float startDashSpeed;
	public GameObject grapplePivot;
	private float lastPercentDone;
	public float yankLength;

	private Vector3 swingPos;

	private Transform ropePos;

	void Start ()
	{
		startDashSpeed = dashSpeed;
		lr = this.GetComponent<LineRenderer>();
		isSwinging = false;
		isYanking = false;
	}

	void OnDrawGizmos() {
		if (hitPos != null && hitPos != Vector3.zero) {
			Gizmos.color = Color.white;
			Gizmos.DrawLine(transform.position, hitPos);
		}
	}

	private void Update() {
		if (Input.GetButton ("Fire1") && isYanking) {
			dashSpeed = Mathf.Lerp (startDashSpeed, 10f, lastPercentDone);
			float distanceTraveled = (Time.time - startGrappleTime) * dashSpeed;
			float percentDone = distanceTraveled/ropeLen;
			lastPercentDone = percentDone;
			transform.position = Vector3.Lerp(startPos,swingPos,percentDone);
			Debug.Log (percentDone + ", " + yankLength);
			if (percentDone >= yankLength) {
				isYanking = false;
				isSwinging = true;
				rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			}
		} else if (!Input.GetButtonDown ("Fire1") && isYanking) {
			isYanking = false;
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
			Vector3 aftershock = hitPos - transform.position;
			rigidbody.AddForce(aftershock * (5 * dashSpeed));
			dashSpeed = startDashSpeed;

		} else if (Input.GetButtonDown ("Fire1") && !isYanking) {
			Vector3 clickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			clickedPosition.z = 0;
			LayerMask layermask = ~(1 << LayerMask.NameToLayer("Player"));
			RaycastHit hit;
			Physics.Raycast(transform.position, clickedPosition - transform.position, out hit, 1000, layermask);
			if (hit.collider) {
				startGrappleTime = Time.time;
				hitPos = new Vector3(hit.point.x,hit.point.y,0);
				grapplePivot.transform.position = hitPos;
				isYanking = true;
				startPos = transform.position;
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
				ropeLen = Vector3.Distance (startPos, grapplePivot.transform.position);
				swingPos = Vector3.Lerp (startPos, grapplePivot.transform.position, .6f);
			}
		} else if (Input.GetButton ("Fire1") && isSwinging) {

			//All this should probably be in its own function

			//GrapplePivot on ceiling
			if (grapplePivot.transform.position.y > transform.position.y) {
				if (Input.GetKey ("a")) {
					transform.RotateAround(grapplePivot.transform.position, Vector3.back, 70 * Time.deltaTime);
				} else if (Input.GetKey ("d")) {
					transform.RotateAround(grapplePivot.transform.position, Vector3.back, -70 * Time.deltaTime);
				} else {
					if (Mathf.Abs(transform.position.x - grapplePivot.transform.position.x) > .1) {
						if (transform.position.x < grapplePivot.transform.position.x) {
							transform.RotateAround(grapplePivot.transform.position, Vector3.back, -40 * Time.deltaTime);
						} else {
							transform.RotateAround(grapplePivot.transform.position, Vector3.back, 40 * Time.deltaTime);
						}
					}
				}
			} else if (grapplePivot.transform.position.y < transform.position.y) {
				//GrapplePivot on floor
			} else {
				//GrapplePivot on wall
			}

			//TODO: Get vector3 of player arc tangent to add to the player after releasing from swing

			transform.rotation = Quaternion.Euler(Vector3.zero);

		} else if (!Input.GetButton ("Fire1") && isSwinging) {
			isSwinging = false;
			Destroy (hinge);
			dashSpeed = startDashSpeed;
			rigidbody.useGravity = true;
			transform.parent = null;
			rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
		}

		if (isYanking || isSwinging) {
			lr.SetPosition(0,transform.position);
			lr.SetPosition(1,hitPos);
			lr.enabled = true;
		} else {
			lr.enabled = false;
		}
	}

	private void Awake() 
	{
		anim = transform.root.gameObject.GetComponentInChildren<Animator>();
		if (anim == null)
		{
			Debug.LogError("The Player's Animator is NULL!");
		}
		ropePos = transform.Find("RopePos");
		if (ropePos == null) {
			Debug.LogError("There is no RopePos transform on the Player");
		}
	}

}

