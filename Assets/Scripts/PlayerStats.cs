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

    // Use this for initialization
    void Start()
    {
      maxHealth = health;
      mat = GetComponentInChildren<SpriteRenderer>().material;
    }
  
    // Update is called once per frame
    void Update()
    {
  
    }

    private void OnCollisionEnter(Collision c)
    {
      if (!invincible && c.gameObject.tag == "Boss") {

        Vector3 dir = Vector3.zero;
        foreach (ContactPoint p in c.contacts) {
          dir += p.normal;
        }
        dir = dir.normalized * 25f;
        rigidbody.AddForce (dir, ForceMode.Impulse);
        float dmg = c.gameObject.GetComponent<BossStats> ().attackDmg;

        health -= dmg/ armor;

        if (health >= 0f) {
          StartCoroutine(Invincible());
        } else {
          GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager>().SMLoadPerviousLevel();
        }
      }
    }

    private IEnumerator Invincible()
    {
      invincible = true;
      Color originalColor = mat.color;
      Color fadeColor = originalColor;
      fadeColor.a = 0.5f;

      for (int i = 1; i <= 5; i++) {
        mat.color = fadeColor;
        yield return new WaitForSeconds(invincibleTime/5);
        mat.color = originalColor;
        yield return new WaitForSeconds(invincibleTime/10);
      }

      invincible = false;
    }
  }
}
