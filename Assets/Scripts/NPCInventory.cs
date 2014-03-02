using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LoveElephant 
{
	public class NPCInventory : MonoBehaviour {

		public List<string> Items;
		public float RenderGap;
		bool isOpen;
		List<GameObject> ItemObjects;
		int ItemCount;
		string npctype;
		List<string> PlayerInv;


		// Use this for initialization
		void Start () {
			isOpen = false;
			ItemObjects = new List<GameObject>();
			ItemCount = Items.Count;
			npctype = this.transform.parent.GetComponent<TalkToNPC>().NPCType;
			GameObject item;
			string itempath;
			float totalwidth = 0;
			foreach(string s in Items){
				itempath = "Gui/" + npctype + "/" + s;
				GameObject uninstantiatedItem = Resources.Load<GameObject> (itempath);
				item =  Instantiate (uninstantiatedItem) as GameObject;

				if(item == null){
					Debug.LogError(itempath + " not valid");
					return;
				}
				item.name = uninstantiatedItem.name;
				totalwidth += item.renderer.bounds.size.x;
				item.transform.parent = this.transform;
				ItemObjects.Add (item);
			}
			GameObject g; 
			float xcoord = this.transform.position.x - totalwidth/2;
			Vector3 newpos;
			for(int i = 0; i < ItemObjects.Count; i++){
				g = ItemObjects[i];
				newpos = new Vector3(xcoord, this.transform.position.y + 0.2f, this.transform.position.z);
				g.transform.position = newpos;
				xcoord += g.gameObject.renderer.bounds.size.x + RenderGap;

			}

		}
		
		// Update is called once per frame
		void Update () {
			
		}


		public void SetPlayerInfo(List<string> inv){
			PlayerInv = inv;
			GameObject lockicon = Resources.Load<GameObject> ("Gui/LockIcons/LockIcon");
			GameObject newlock;
			bool foundmatch = false;
			
			foreach(GameObject g in ItemObjects){
				//if player does not have an item unlocked, add lock icon
				foreach(string item in inv){
					if(item == g.name){
						foundmatch = true;
					}
				}
				if(!foundmatch){
					g.GetComponent<GuiLock>().Lock();
				} else {
					g.GetComponent<GuiLock>().UnLock ();
				}
				foundmatch = false;
			}
		}

		public void Open(){			
			isOpen = true;
			foreach(GameObject g in ItemObjects){
				g.SetActive(true);
			}
		}		

		public void Close(){
			isOpen = false;
			foreach(GameObject g in ItemObjects){
				g.SetActive(false);
			}
		}

		public bool IsOpen(){
			return isOpen;
		}
	}
}
