﻿using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  public class Crate : MonoBehaviour
  {

    // Use this for initialization
    void Start()
    {
  
    }
  
    // Update is called once per frame
    void Update()
    {
  
    }

    void OnTriggerEnter(Collider hit)
    {
      if (hit.gameObject.tag == "Weapon" || hit.gameObject.tag == "Boss") {
        ((MeshExploder)this.GetComponent ("MeshExploder")).Explode (); 
        this.gameObject.SetActive (false);
      }
    }

    void OnCollisionEnter(Collision hit)
    {
      if (hit.gameObject.tag == "Weapon" || hit.gameObject.tag == "Boss") {
		GetComponent<AudioSource>().Play ();
        ((MeshExploder)this.GetComponent ("MeshExploder")).Explode (); 
		this.gameObject.renderer.enabled = false;
		StartCoroutine (breakCrate());
      }
    }

	IEnumerator breakCrate() {
  	  yield return new WaitForSeconds(1f);
	  Destroy (this.gameObject);
	}
  }
}