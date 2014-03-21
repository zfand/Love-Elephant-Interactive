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
					Close();
				}
			}
		}

		public bool IsSplashing(){
			return splashing;
		}
		public void Splash(Vector3 position, bool _puddle){
			splashing = true;
			splash = Instantiate(Resources.Load("Particles/Splash")) as GameObject;
            splash.transform.position = position;
			splash.SetActive(true);
			if(_puddle){
				SpawnPuddle(position);
			}
		}
		public void SpawnPuddle(Vector3 position){
			puddle = Instantiate(Resources.Load ("Items/StatusEffects/PoisonPuddle")) as GameObject;
			puddle.transform.position = position;
		}

		public void Close() {
			open = false;
			openTimer = 0;
			splashing = false;
			puddle.GetComponent<PoisonPuddle>().FadeOut();
			Destroy (splash);

			particles.Stop ();

		}
	}
}

