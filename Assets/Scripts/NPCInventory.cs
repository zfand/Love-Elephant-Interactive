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
		Inventory PlayerInv;
		Equipment playerequip;

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
				
				g.GetComponent<GuiItemInfo>().Initialize();

			}

		}
		
		// Update is called once per frame
		void Update () {
			if(isOpen){
				if(Input.GetButtonDown("Fire2")){
					Vector3 clickedPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
					LayerMask layermask = ~(1 << LayerMask.NameToLayer ("NPC"));      
					RaycastHit hit;
					Physics.Raycast (clickedPosition, Vector3.forward, out hit, 1000, layermask);
					if (hit.collider) {
						//If the Object is a UI Item
						if ((hit.collider.gameObject.tag == "GuiItem")) {
							GuiItemInfo info = hit.collider.gameObject.GetComponent<GuiItemInfo>();
							if(!info.IsLocked() && !info.IsEquipped()){
								UnequipAll();
								Equip(hit.collider.gameObject.name);
								playerequip.Equip (PlayerInv.TakeItem(hit.collider.gameObject.name));
							}
						}
					}
				}
			}

		}
			
			
		public void SetPlayerInfo(List<string> inv, Equipment pequip, Inventory p_inv){
			string equip = pequip.GetCurrentEquip(npctype);
			playerequip = pequip;
			PlayerInv = p_inv;
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
					g.GetComponent<GuiItemInfo>().Lock();
					g.GetComponent<GuiItemInfo>().UnEquip();
				} else {
					g.GetComponent<GuiItemInfo>().UnLock ();
					if(equip == g.name){
						g.GetComponent<GuiItemInfo>().Equip();
					} else {
						g.GetComponent<GuiItemInfo>().UnEquip();
					}

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

		void UnequipAll(){
			foreach(GameObject g in ItemObjects){
				g.GetComponent<GuiItemInfo>().UnEquip();
			}
		}


		void Equip(string objName){			
			foreach(GameObject g in ItemObjects){
				if(g.name == objName){
					g.GetComponent<GuiItemInfo>().Equip();
					break;
				}
			}
		}
	}
}
