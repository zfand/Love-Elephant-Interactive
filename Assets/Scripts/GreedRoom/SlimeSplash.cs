using UnityEngine;
using System.Collections;
namespace LoveElephant 
{
	public class SlimeSplash : MonoBehaviour {

		GameObject splash;
		public bool SpawnPuddle = true;
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
					p.Splash (this.transform.position, 
					          SpawnPuddle);
				}
			}
		}

	}
}
