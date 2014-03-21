using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LoveElephant;

namespace Boss
{
	public class Pipe : MonoBehaviour
	{
		public float OpenTime;
		private bool open;
		private float openTimer;
		private bool splashing;
		private ParticleSystem particles;
		private GameObject splash;
		private GameObject puddle;
		private float colliding_counter;
		private float colliding_counter_max = 5;
		public void Start() {
			particles = this.transform.GetComponent<ParticleSystem>();
			splashing = false;
			open = false;

		}

		public void Open(){
			open = true;
			openTimer = 0;
			particles.Play ();
		}

		public void Update() {
			if(open){
				openTimer++;
				if(openTimer > OpenTime){
					StartCoroutine(Close());
				}
			}

			colliding_counter--;
		}

		public bool IsSplashing(){
			return splashing;
		}
		public void Splash(Vector3 position, bool _puddle){
			colliding_counter = colliding_counter_max;

			splashing = true;
			splash = Instantiate(Resources.Load("Particles/Splash")) as GameObject;
            splash.transform.position = position;
			splash.SetActive(true);
			if(!open){
				splash.particleSystem.loop = false;
			}
			if(_puddle){
				SpawnPuddle(position);
			}
		}
		public void SpawnPuddle(Vector3 position){
			if(puddle == null){
				puddle = Instantiate(Resources.Load ("Items/StatusEffects/PoisonPuddle")) as GameObject;
				puddle.transform.position = position;
			}
		}

		public IEnumerator Close() {
			particles.Stop ();
			open = false;
			openTimer = 0;
			while(colliding_counter > 0){
				yield return 0;
			}
			if(splash != null){
				Destroy (splash);
			}
			if(puddle != null){
				puddle.GetComponent<PoisonPuddle>().FadeOut();
			}
			splashing = false;
			
		}
	}
}

