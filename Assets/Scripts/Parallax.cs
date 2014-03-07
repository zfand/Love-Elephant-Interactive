using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour
{
  
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
  public float width;
  public float height;
  public GameObject cam;

  // Use this for initialization
  void Start()
  {
    //Get player's Z to dynamically place backgrounds
    float startDepth = GameObject.FindGameObjectWithTag ("Player").transform.position.z;
  
    //Create a shader based on the given type
    backShader = new Shader ();
    backShader = Shader.Find (shaderType);
    
    //Create background materials
    firstMaterial = new Material (backShader);
    secondMaterial = new Material (backShader);
    thirdMaterial = new Material (backShader);
    
    //Apply background textures
    firstMaterial.mainTexture = firstTexture;
    secondMaterial.mainTexture = secondTexture;
    thirdMaterial.mainTexture = thirdTexture;

    int intHeight = (int)Mathf.Ceil (height);
    int intWidth = (int)Mathf.Ceil (width) + 5;

    //Instantiate background 1
    firstBackground = GameObject.CreatePrimitive (PrimitiveType.Cube);
    firstBackground.renderer.material = firstMaterial;
    firstBackground.transform.position = new Vector3 (0, 0, startDepth - 5);
    firstBackground.transform.localScale = new Vector3 (intWidth, intHeight, 1);
    
    //Instantiate background 2
    secondBackground = GameObject.CreatePrimitive (PrimitiveType.Cube);
    secondBackground.renderer.material = secondMaterial;
    secondBackground.transform.position = new Vector3 (0, 0, startDepth + 5);
    secondBackground.transform.localScale = new Vector3 (intWidth, intHeight, 1);
    
    //Instantiate background 3
    thirdBackground = GameObject.CreatePrimitive (PrimitiveType.Cube);
    thirdBackground.renderer.material = thirdMaterial;
    thirdBackground.transform.position = new Vector3 (0, 0, startDepth + 10);
    thirdBackground.transform.localScale = new Vector3 (intWidth, intHeight, 1);
    
    //Remove background colliders
    firstBackground.collider.isTrigger = true;
    secondBackground.collider.isTrigger = true;
    thirdBackground.collider.isTrigger = true;
    
  }
  
  // Update is called once per frame
  void Update()
  {
    if (enabled) {
      float camX = cam.transform.position.x;
    
      firstBackground.transform.position = new Vector3 (0, 0, firstBackground.transform.position.z);
      secondBackground.transform.position = new Vector3 (0, 0, secondBackground.transform.position.z);
      thirdBackground.transform.position = new Vector3 (0, 0, thirdBackground.transform.position.z);
    
      Vector3 newFirstPos = new Vector3 (firstBackground.transform.position.x - camX / 4,
                                  firstBackground.transform.position.y,
                                        firstBackground.transform.position.z);
    
      Vector3 newSecondPos = new Vector3 (secondBackground.transform.position.x - camX / 6,
                                   secondBackground.transform.position.y,
                                         secondBackground.transform.position.z);
    
      Vector3 newThirdPos = new Vector3 (thirdBackground.transform.position.x - camX / 8,
                                  thirdBackground.transform.position.y,
                                        thirdBackground.transform.position.z);
    
      firstBackground.transform.position = newFirstPos;
      secondBackground.transform.position = newSecondPos;
      thirdBackground.transform.position = newThirdPos;
    }
  }
}