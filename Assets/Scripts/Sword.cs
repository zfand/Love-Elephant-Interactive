using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour
{
	public GameObject sword;				// Prefab of the rocket.
	public float speed = 20f;			// The speed the rocket will fire at.


	private PlayerController playerCtrl;		// Reference to the PlayerControl script.
	//private Animator anim;					// Reference to the Animator component.

	private Vector3 lastHit;

	private Vector3 hitPos;
	private Vector3 dirPos;
	private bool isSwinging;

	
	void Start()
	{
		isSwinging = false;
		sword.SetActive(false);
	}

	void Awake()
	{
		// Setting up the references.
//		anim = transform.root.gameObject.GetComponent<Animator>();
		playerCtrl = transform.root.gameObject.GetComponent<PlayerController>();
	}

	void OnDrawGizmos() {
		if (hitPos != null && hitPos != Vector3.zero) {
			//Gizmos.DrawSphere(hitPos, 1);
			//Gizmos.DrawRay(transform.position, (hitPos - transform.position)*10);
			Gizmos.color = Color.white;
			Gizmos.DrawLine(transform.position, hitPos);
		}
		if (dirPos != null && dirPos != Vector3.zero) {
			Gizmos.color = Color.green;
			Gizmos.DrawRay(transform.position, dirPos*5);
			Gizmos.color = Color.black;
			Gizmos.DrawRay(transform.position, Vector3.up*5);
			Gizmos.DrawRay(transform.position, Vector3.down*5);
			Gizmos.DrawRay(transform.position, Vector3.right*5);
			Gizmos.DrawRay(transform.position, Vector3.left*5);
		}
	}

	void Update ()
	{
		// If the fire button is pressed...
		//if(Input.GetButtonDown("Fire1") && !isSwinging)
		if(Input.GetButtonDown("Fire2") && !isSwinging)
		{
			// ... set the animator Shoot trigger parameter and play the audioclip.
//			anim.SetTrigger("Shoot");
//			audio.Play();

			Vector3 clickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			hitPos = clickedPosition;
			hitPos.z = 0;
			hitPos.Normalize ();
			//Debug.Log(hitPos);
			 	
			LayerMask layermask = ~(1 << LayerMask.NameToLayer("Player"));
			Quaternion q = Quaternion.LookRotation(hitPos);
			Quaternion start = Quaternion.Euler(new Vector3(0,0,60));
			Quaternion end = Quaternion.Euler(new Vector3(0,0,-60));
			this.StartCoroutine(SwingSword(start, end));
		}
	}

	IEnumerator SwingSword(Quaternion _start, Quaternion _end)
	{
		isSwinging = true;
		sword.SetActive(true);
		float swingTime = 0;
		float totalSwingTime = 10;
		Vector3 diff = this.transform.root.position - sword.transform.position;
		Quaternion start = _start;
		Quaternion end = _end;
		while(swingTime < totalSwingTime){
			if(!playerCtrl.facingRight)
			{
				end = _start;
				start = _end;
			} else {
				end = _end;
				start = _start;
			}
			swingTime++;
			//sword.transform.position = sword.transform.position - diff;
			sword.transform.rotation = Quaternion.Slerp(start, end, swingTime/totalSwingTime);
			//sword.transform.position = sword.transform.position + diff;
			yield return 0; 

		}
		isSwinging = false;
		sword.transform.rotation = Quaternion.Euler(0, 0, 0);
		//sword.SetActive(false);
	}
}
