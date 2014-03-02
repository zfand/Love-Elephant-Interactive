using UnityEngine;
using System.Collections;
namespace LoveElephant
{
	public class GuiLock : MonoBehaviour {
		
		Transform lockicon;
		Color originalColor;
		Color lockColor;
		// Use this for initialization
		void Start () {
			originalColor = GetComponent<SpriteRenderer>().color;
			lockColor = new Color(0.4f, 0.4f, 0.4f);
			lockicon = transform.FindChild ("LockIcon");
			if(lockicon == null){
				Debug.LogError("No Lock Icon");
			}
			gameObject.SetActive(false);
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void Lock(){
			lockicon.gameObject.SetActive(true);
			GetComponent<SpriteRenderer>().color = lockColor;
		}

		
		public void UnLock(){
			lockicon.gameObject.SetActive(false);
			GetComponent<SpriteRenderer>().color = originalColor;
		}
	}
}
