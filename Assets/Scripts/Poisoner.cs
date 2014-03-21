using UnityEngine;
using System.Collections;

namespace LoveElephant {
	public class Poisoner : MonoBehaviour {
		/// <summary>
		/// Number of times that poison will damage the player
		/// </summary>
		public float DamageTimes;
		/// <summary>
		/// The damage per damage iteration
		/// </summary>
		public float DamagePer;

		/// <summary>
		/// The ticks between damage.
		/// </summary>
		public float TicksBetweenDamage;

		private bool poisoning = false;
		private float currentDamageTime = 0;
		private float currentTicks = 0;

		private GameObject player;
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			if(poisoning){
				currentTicks++;
				if(currentTicks >= TicksBetweenDamage){
					currentTicks = 0;
					currentDamageTime++;
					player.GetComponent<PlayerStats>().PoisonPlayer(DamagePer);
					if(currentDamageTime >= DamageTimes){
						Destroy (this.gameObject);
					}
				}
			}
		
		}

		public void BeginPoison(){
			player = GameObject.FindGameObjectWithTag("Player");
			if(player == null){
				Debug.LogError ("poisoner cannot find player");
			}
			poisoning = true;
			currentTicks = TicksBetweenDamage;

		}

		public void Reset() {
			currentDamageTime = 0;
		}

	}
}