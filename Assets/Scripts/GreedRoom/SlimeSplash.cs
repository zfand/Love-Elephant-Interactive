using UnityEngine;
using System.Collections;
namespace Boss 
{
	public class SlimeSplash : MonoBehaviour {

		GameObject splash;
		// Use this for initialization
		void Start () {
			splash = (GameObject)Resources.Load ("Particles/Splash");
			splash.SetActive(false);
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		void OnParticleCollision(GameObject g) {
			if(g.CompareTag("Pipe")){
				Pipe p = g.GetComponent<Pipe>();
				p.ResetCounter();
				if(!p.IsSplashing()){
					p.Splash (new Vector3(g.transform.position.x, 
					                      this.transform.position.y + this.renderer.bounds.size.y/2, 
					                      g.transform.position.z), 
					          true);
				}
			}
		}

	}
}
