using UnityEngine;
using System.Collections;
using Item;

namespace Boss
{
  public class BossStats : MonoBehaviour
  {
    /// <summary>
    /// The current hit points of the Boss
    /// </summary>
    public float health;
    /// <summary>
    /// The toughness of the Boss
    /// </summary>
    public float armor;
    /// <summary>
    /// The life steal of the Boss
    /// </summary>
    [Range(0f,1f)]
    public float
      lifeSteal = 0f;
    /// <summary>
    /// The amount of time the Boss flashes red
    /// </summary>
    public float flashTime;
    /// <summary>
    /// Item that is dropped on death
    /// </summary>
    public GameObject drop;
    private bool dropped;

    /// <summary>
    /// The max health of the Boss
    /// </summary>
    private float maxHealth;
    /// <summary>
    /// The material of the Boss
    /// </summary>
    private Material mat;

    /// <summary>
    /// Gets the percentage of health left in the Boss
    /// </summary>
    public float healthPercent {
      get { return this.health / this.maxHealth; }
    }

    /// <summary>
    /// Determines whether the Boss is Alive
    /// </summary>
    public bool alive {
      get { return health >= 0f; }
    }


    // Use this for initialization
    private void Start()
    {
      maxHealth = health;
      mat = GetComponent<SkinnedMeshRenderer> ().material;
    }
  
    private void OnTriggerEnter(Collider other)
    {
      if (other.gameObject.tag == "Weapon") {
        this.TakeDamage (other.gameObject.GetComponent<WeaponStats> ().getDamage ());
        StartCoroutine ("Flash");
      }
    }

    /// <summary>
    /// Boss takes the damage and returns how much damage it took after it's own modifers
    /// </summary>
    public float TakeDamage(float dmg)
    {
      dmg /= armor;
      health -= dmg;
	  if (!alive && !dropped) {
		Vector3 keydrop = new Vector3(this.transform.position.x, this.transform.position.y + 2, 0f);
		Instantiate(drop, keydrop, Quaternion.identity);
	    dropped = true;
      }
      return dmg;
    }

    /// <summary>
    /// Flashes the Boss red
    /// </summary>
    private IEnumerator Flash()
    {
      Color originalColor = mat.color;
      mat.color = Color.red;
      yield return new WaitForSeconds (flashTime);
      mat.color = originalColor;
    }
  }
}
