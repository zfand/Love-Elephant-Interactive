using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  [System.Serializable]
  public class BossStats : MonoBehaviour
  {
    /// <summary>
    /// The current hit points of the Boss
    /// </summary>
    public float health;
    /// <summary>
    /// The damage the Boss does on hit
    /// </summary>
    public float attackDmg;
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
    /// <summary>
    /// Determines if this script should drop it's loot
    /// </summary>
    public bool dropLoot = true;
    [HideInInspector]
    /// <summary>
    /// Flag for when the Boss is attacking
    /// </summary>
    public bool attacking;
    public SkinnedMeshRenderer mesh;
    public SkinnedMeshRenderer[] extraMeshs;
    /// <summary>
    /// The max health of the Boss
    /// </summary>
    private float maxHealth;
    /// <summary>
    /// The material of the Boss
    /// </summary>
    private Material mat;
    private Material[] extraMat;
    /// <summary>
    /// The original color of the mat
    /// </summary>
    private Color originalColor;
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
      get { return health > 0f; }
    }

    // Use this for initialization
    private void Start()
    {
      maxHealth = health;
      if (mesh != null) {
        mat = mesh.material;
      } else {
        mat = GetComponent<SkinnedMeshRenderer> ().material;
      }
      extraMat = new Material[extraMeshs.Length];
      for (int i = 0; i < extraMeshs.Length; i++) {
        extraMat[i] = extraMeshs[i].material;
      }
      originalColor = mat.color;
    }
  
    private void OnTriggerEnter(Collider other)
    {
      if (other.gameObject.tag == "Weapon") {
        this.TakeDamage (other.gameObject.GetComponent<WeaponStats> ().GetDamage ());
        StartCoroutine ("Flash");
      }
    }

    public float GetDamage()
    {
      if (attacking) {
        return attackDmg;
      } 
      return 0;
    }

    /// <summary>
    /// Boss takes the damage and returns how much damage it took after it's own modifers
    /// </summary>
    public float TakeDamage(float dmg)
    {
	  if (this.transform.GetComponent<AudioSource>() != null)
		  GetComponent<AudioSource>().Play();
      dmg /= armor;
      health -= dmg;

      if (!alive && dropLoot) {
        Vector3 keydrop = new Vector3 (this.transform.position.x, this.transform.position.y + 2, 0f);
        GameObject newdrop = Instantiate (drop, keydrop, Quaternion.identity) as GameObject;
				newdrop.rigidbody.AddExplosionForce(300, keydrop, 2);
        dropLoot = false;
      }
      return dmg;
    }

    /// <summary>
    /// Flashes the Boss red
    /// </summary>
    private IEnumerator Flash()
    {
      float deltaTime = 0f;

      mat.color = Color.red;
      foreach (Material m in extraMat) {
        m.color = Color.red;
      }

      while (deltaTime < flashTime) {
        deltaTime += Time.deltaTime;
        yield return 0;
      }
      mat.color = originalColor;
      foreach (Material m in extraMat) {
        m.color = originalColor;
      }
    }
  }
}
