using UnityEngine;
using System.Collections;
namespace LoveElephant
{
	public class GuiItemInfo : MonoBehaviour {
		
		Transform lockicon;
		Transform equipIcon;
		Color originalColor;
		Color lockColor;
		bool locked;
		bool equipped;
		// Use this for initialization
		void Start () {
			lockicon = transform.FindChild ("LockIcon");
			if(lockicon == null){
				Debug.LogError("No Lock Icon");
			}
			originalColor = new Color(1f, 1f, 1f);
			lockColor = new Color(0.4f, 0.4f, 0.4f);

		}
		
		// Update is called once per frame
		void Update () {
		}

		public void Initialize(){

			originalColor = new Color(1f,1f,1f);
			lockColor = new Color(0.4f, 0.4f, 0.4f);
			lockicon = transform.FindChild ("LockIcon");
			equipIcon = transform.FindChild ("EIcon");
			if(lockicon == null){
				Debug.LogError("No Lock Icon");
			}
			if(equipIcon == null){
				Debug.LogError("No Equip Icon");
			}
			gameObject.SetActive(false);
		}


		public void Lock(){
			lockicon.renderer.enabled = true;
			locked = true;
			GetComponent<SpriteRenderer>().color = lockColor;
		}

		
		public void UnLock(){
			lockicon.renderer.enabled = false;
			locked = false;
			GetComponent<SpriteRenderer>().color = originalColor;
		}

		public bool IsLocked(){
			return locked;
		}

		public void Equip(){			
			equipIcon.renderer.enabled = true;
			equipped = true;
		}

		public void UnEquip(){
			equipIcon.renderer.enabled = false;
			equipped = false;
		}

		public bool IsEquipped(){
			return equipped;
		}
	}
}
