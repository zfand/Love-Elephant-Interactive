using UnityEngine;
using System.Collections;
using Boss;
using Preloader;

namespace LoveElephant
{
  public class PlayerStats : MonoBehaviour
  {
    public float health;
    public float armor;
    public float invincibleTime;
    private float maxHealth;
    private Material mat;
    private bool invincible = false;
    public GameObject healthBar;

    // Use this for initialization
    void Start()
    {
      maxHealth = health;
      mat = GetComponentInChildren<SpriteRenderer> ().material;
    }
  
    // Update is called once per frame
    void Update()
    {
  
    }

    private void OnCollisionEnter(Collision c)
    {
      if (!invincible && c.collider.tag == "HurtBox") {
        float dmg = c.collider.GetComponent<HurtBox> ().GetDamage ();
        if (dmg > 0) {
          Vector3 dir = Vector3.zero;
          foreach (ContactPoint p in c.contacts) {
            dir += p.normal;
          }
          dir = dir.normalized * 25f;
          rigidbody.AddForce (dir, ForceMode.Impulse);
          TakeDamage(dmg);
        }
      }
    }

    private void TakeDamage(float dmg)
    {
      health -= dmg / armor;
      
      if (health >= 0f) {
        StartCoroutine (Invincible ());
      } else {
        healthBar.renderer.enabled = false;
        health = maxHealth;
        rigidbody.velocity = Vector3.zero;
        GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager> ().SMLoadPerviousLevel ();
      }
    }
    
    private IEnumerator Invincible()
    {
      healthBar.renderer.enabled = true;
      healthBar.renderer.material.color = new Color (1f, 0f, 0f);
      Vector3 newScale = new Vector3 ((0.6f * (health / maxHealth)), .1f, .1f);
      healthBar.transform.localScale = newScale;
      invincible = true;
      Color originalColor = mat.color;
      Color fadeColor = originalColor;
      fadeColor.a = 0.5f;

      for (int i = 1; i <= 5; i++) {
        mat.color = fadeColor;
        yield return new WaitForSeconds (invincibleTime / 5);
        mat.color = originalColor;
        yield return new WaitForSeconds (invincibleTime / 10);
      }

      invincible = false;
      healthBar.renderer.enabled = false;
    }
  }
}
