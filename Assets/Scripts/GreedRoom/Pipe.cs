using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Boss
{
	public class Pipe : MonoBehaviour
	{
		public float OpenTime;
		private bool open;
		private float openTimer;

		private ParticleEmitter particles;

		public void Start() {
			particles = this.transform.FindChild("Particles").particleEmitter;
		}

		public void Open(){
			open = true;
			openTimer = 0;
			particles.particleSystem.Play ();
		}

		public void Update() {
			if(open){
				openTimer++;
				if(openTimer > OpenTime){
					Close();
				}
			}
		}

		public void Close() {
			open = false;
			openTimer = 0;
			particles.particleSystem.Stop ();

		}
	}
}

