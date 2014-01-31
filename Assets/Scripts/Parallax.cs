using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {

	private GameObject firstBackground;
	private GameObject secondBackground;
	private GameObject thirdBackground;

	private Material firstMaterial;
	private Material secondMaterial;
	private Material thirdMaterial;

	private Shader backShader;

	public Texture firstTexture;
	public Texture secondTexture;
	public Texture thirdTexture;

	public string shaderType;

	public int width;
	public int height;

	public bool hideWalls;

	// Use this for initialization
	void Start () {

		//Get player's Z to dynamically place backgrounds
		float startDepth = GameObject.FindGameObjectWithTag("Player").transform.position.z;

		if (hideWalls) {
			//Find existing walls and make them invisible if desired
			GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
			foreach (GameObject w in walls) {
				w.renderer.enabled = false;
			}
		}

		//Create a shader based on the given type
		backShader = new Shader();
		backShader = Shader.Find (shaderType);

		//Create background materials
		firstMaterial = new Material(backShader);
		secondMaterial = new Material(backShader);
		thirdMaterial = new Material(backShader);

		//Apply background textures
		firstMaterial.mainTexture = firstTexture;
		secondMaterial.mainTexture = secondTexture;
		thirdMaterial.mainTexture = thirdTexture;

		//Instantiate background 1
		firstBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
		firstBackground.renderer.material = firstMaterial;
		firstBackground.transform.position = new Vector3(0,0,startDepth+1);
		firstBackground.transform.localScale = new Vector3(100,50,1);

		//Instantiate background 2
		secondBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
		secondBackground.renderer.material = secondMaterial;
		secondBackground.transform.position = new Vector3(0,0,startDepth+2);
		secondBackground.transform.localScale = new Vector3(100,50,1);

		//Instantiate background 3
		thirdBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
		thirdBackground.renderer.material = thirdMaterial;
		thirdBackground.transform.position = new Vector3(0,0,startDepth+3);
		thirdBackground.transform.localScale = new Vector3(100,50,1);

		//Remove background colliders
		firstBackground.collider.isTrigger = true;
		secondBackground.collider.isTrigger = true;
		thirdBackground.collider.isTrigger = true;

	}
	
	// Update is called once per frame
	void Update () {
		float playerX = GameObject.FindGameObjectWithTag("Player").transform.position.x;
		float playerY = GameObject.FindGameObjectWithTag("Player").transform.position.y;

		firstBackground.transform.position = new Vector3(0,0,firstBackground.transform.position.z);
		secondBackground.transform.position = new Vector3(0,0,secondBackground.transform.position.z);
		thirdBackground.transform.position = new Vector3(0,0,thirdBackground.transform.position.z);

		Vector3 newFirstPos = new Vector3(firstBackground.transform.position.x - playerX/5,
		                                  firstBackground.transform.position.y - playerY/10,
		                                  firstBackground.transform.position.z);
		
		Vector3 newSecondPos = new Vector3(secondBackground.transform.position.x - playerX/8,
		                                   secondBackground.transform.position.y - playerY/12,
		                                   secondBackground.transform.position.z);
		
		Vector3 newThirdPos = new Vector3(thirdBackground.transform.position.x - playerX/10,
		                                  thirdBackground.transform.position.y - playerY/15,
		                                  thirdBackground.transform.position.z);

		firstBackground.transform.position = newFirstPos;
		secondBackground.transform.position = newSecondPos;
		thirdBackground.transform.position = newThirdPos;

	}
}
