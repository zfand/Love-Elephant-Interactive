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
	private ConfigurableJoint spring;
	/// <summary>
	/// Whether the player is swinging
	/// </summary>
	private bool isSwinging;


	public float ropeLen = 0f;
	public float dashSpeed = 0f;
	private Transform ropePos;


	void Start ()
	{
		lr = this.GetComponent<LineRenderer>();
		isSwinging = false;
	}

	void OnDrawGizmos() {
		if (hitPos != null && hitPos != Vector3.zero) {
			Gizmos.color = Color.white;
			Gizmos.DrawLine(transform.position, hitPos);
		}
	}

	private void Update ()
	{
		if(Input.GetButtonDown("Fire1") && !isSwinging)
		{
			Vector3 clickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			clickedPosition.z = 0;
			hitPos = clickedPosition;
			LayerMask layermask = ~(1 << LayerMask.NameToLayer("Player"));
			RaycastHit hit;
			Physics.Raycast(transform.position, clickedPosition - transform.position, out hit, 1000, layermask);
			if (hit.collider) {
				hitPos = new Vector3(hit.point.x,hit.point.y,0);
				Swing();
			} else {
				Debug.Log("MISS");
			}
		}

		if (!Input.GetButton("Fire1") && isSwinging) {
			isSwinging = false;
			lr.enabled = false;
			StopCoroutine("Swing");
			anim.SetBool("Swing",false);
			Destroy(spring);

		}
		
		if (isSwinging) {
			lr.SetPosition(0,ropePos.position);
			lr.SetPosition(1,hitPos);
			lr.enabled = true;
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

	void Swing()
	{
		anim.SetBool("Swing",true);
		isSwinging = true;
		ropeLen = Vector3.Distance(transform.position, hitPos)/3;

		spring = gameObject.AddComponent<ConfigurableJoint>();
		//spring.anchor = new Vector3(0,0.5f,0);
		spring.connectedAnchor = hitPos;
		spring.autoConfigureConnectedAnchor =false;
		spring.xMotion = ConfigurableJointMotion.Limited;
		spring.yMotion = ConfigurableJointMotion.Limited;
		spring.zMotion = ConfigurableJointMotion.Locked;
		spring.angularZMotion = ConfigurableJointMotion.Locked;
		SoftJointLimit limits = new SoftJointLimit();
		limits.limit = ropeLen;
		limits.damper = 0.2f;
		spring.linearLimit = limits;
		Debug.Break();
	}
}

