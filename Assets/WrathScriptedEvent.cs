using UnityEngine;
using System.Collections;

namespace LoveElephant
{
	public class WrathScriptedEvent : MonoBehaviour {

		public Texture blackTexture;

		/// <summary>
		/// The alpha fade value for fading.
		/// </summary>
		float alphaFadeValue = 1;
		
		/// <summary>
		/// Whether the scene is fading in.
		/// </summary>
		bool fadingIn;
		
		/// <summary>
		/// whether the scene is fading out.
		/// </summary>
		bool fadingOut;
		
		/// <summary>
		/// The length of the fade
		/// </summary>
		public float fadeLength = 5;


		public GameObject GreedGluttony;
		public GameObject Background;
		private TrackPlayer tracking;
		private bool active = false;
		// Use this for initialization
		void Start () {
			tracking = transform.GetComponent<TrackPlayer>();
			alphaFadeValue = 0;
		}
		
		// Update is called once per frame
		void Update () {
			if(GreedGluttony == null && !active){
				Activate ();
			}
			if(alphaFadeValue == 1){
				Application.LoadLevel ("Credits");
			}
		
		}

		void Activate(){
			active = true;
			StartCoroutine(Move());
		}

		IEnumerator Move(){
			bool exploded = false;
			while (this.transform.position.y < 0){

				if(this.transform.position.y > -2 && !exploded){
					exploded = true;
					Background.GetComponent<MeshExploder>().Explode ();
					GameObject.Destroy(Background);
				}
				this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.05f, this.transform.position.z);
				yield return 0;
			}
			tracking.Activate();
			FadeOut();
		}

		/// <summary>
		/// Raises the GU event.
		/// </summary>
		private void OnGUI(){
			float maxtime = 3;
			float deltatime = 0;
			if(fadingIn)
			{
				alphaFadeValue -= Time.deltaTime / fadeLength;
				if(alphaFadeValue < 0)
				{
					fadingIn = false;
					alphaFadeValue = 0;
				}
			}
			if(fadingOut)
			{
				alphaFadeValue += Time.deltaTime / fadeLength;
				if(alphaFadeValue > 1)
				{
					fadingOut = false;
					alphaFadeValue = 1;
				}
			}
			GUI.color = new Color(0, 0, 0, alphaFadeValue);
			GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height), blackTexture);
		}
		
		/// <summary>
		/// Sets fadeout values for OnGUI
		/// </summary>
		public void FadeOut()
		{
			fadingIn = false;
			fadingOut = true;
			alphaFadeValue = 0;
		}
		
		/// <summary>
		/// Tells if the app is fading
		/// </summary>
		public bool IsFading()
		{
			return fadingIn || fadingOut;
		}
	}
}
