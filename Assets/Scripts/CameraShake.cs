using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {

	private Vector3 OriginalPos;
	private Quaternion OriginalRot;

	private float ShakeIntensity;
	private float ShakeDecay;
	private bool Shaking;
	void Start()
	{
		Shaking = false;   
	}
	
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	public void Shake()
	{
		if(!Shaking){
			StartCoroutine(DoShake());
		}
	}

 	IEnumerator DoShake() {
		
		OriginalPos = transform.position;
		OriginalRot = transform.rotation;
		
		ShakeIntensity = 0.3f;
		ShakeDecay = 0.02f;
		Shaking = true;

		while(ShakeIntensity > 0)
		{
			transform.position = OriginalPos + Random.insideUnitSphere * ShakeIntensity;
			transform.rotation = new Quaternion(OriginalRot.x + Random.Range(-ShakeIntensity, ShakeIntensity)*.2f,
			                                    OriginalRot.y + Random.Range(-ShakeIntensity, ShakeIntensity)*.2f,
			                                    OriginalRot.z + Random.Range(-ShakeIntensity, ShakeIntensity)*.2f,
			                                    OriginalRot.w + Random.Range(-ShakeIntensity, ShakeIntensity)*.2f);
			
			ShakeIntensity -= ShakeDecay;
			yield return 0;

		}

		Shaking = false;  
	}
}
