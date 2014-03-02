using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Item;
namespace LoveElephant 
{
	public class TalkToNPC : MonoBehaviour {

		public string NPCType;
		Transform EIcon;
		Transform Items;
		bool shopOpened;
		bool openshop;
		bool shopready;
		NPCInventory npcshop;
		List<string> playerInventory;
		// Use this for initialization
		void Start () {
			
			EIcon = transform.FindChild("EIcon");
			EIcon.renderer.enabled = false;
			Items = transform.FindChild("ItemShop");
			npcshop = Items.GetComponent<NPCInventory>();
			shopOpened = false;
			shopready = false;
		}
		
		// Update is called once per frame
		void Update () {
			if(shopready){
				if(Input.GetKeyDown(KeyCode.E)){
					if(shopOpened){
						Items.gameObject.SetActive(false);
						EIcon.renderer.enabled = true;
						CloseShop();
					} else {				
						Items.gameObject.SetActive(true);
						EIcon.renderer.enabled = false;
						OpenShop ();
					}
				}
			}
		}



		void OnTriggerEnter(Collider other){
			//show press e to talk icon
			if(other.gameObject.CompareTag ("Player")) {
				EIcon.renderer.enabled = true;
				shopready = true;
				playerInventory = other.gameObject.GetComponent<Inventory>().GetItemsByTag(NPCType);
				npcshop.SetPlayerInfo(playerInventory);
			}
		}


		void OnTriggerExit(Collider other){
			if(other.gameObject.CompareTag ("Player")) {
				EIcon.renderer.enabled = false;
				shopready = false;
				playerInventory = null;
				CloseShop();
			}
		}

		void CloseShop(){
			npcshop.Close();
			shopOpened = false;
		}

		void OpenShop(){
			npcshop.Open();
			shopOpened = true;
		}

	}
}
