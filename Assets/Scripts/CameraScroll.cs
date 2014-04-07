using UnityEngine;
using System.Collections;

public class CameraScroll : MonoBehaviour
{

  public GameObject cam;
  public float horizontalLimit;
  private GameObject player;
  private float playerStartY;

  // Use this for initialization
  void Start()
  {
    player = GameObject.FindGameObjectWithTag ("Player");
    playerStartY = player.transform.position.y;
  }
  
  // Update is called once per frame
  void Update()
  {
    Vector3 pos = cam.transform.position;

    Ray leftRay = cam.camera.ScreenPointToRay (new Vector3 (0f, 0f));
    Ray rightRay = cam.camera.ScreenPointToRay (new Vector3 (cam.camera.pixelWidth, 0f));
    Ray upRay = cam.camera.ScreenPointToRay (new Vector3 (cam.camera.pixelWidth/2, cam.camera.pixelHeight));
    Ray downRay = cam.camera.ScreenPointToRay (new Vector3 (cam.camera.pixelWidth/2, 0f));

    float xdir = cam.transform.position.x - player.transform.position.x;
    float ydir = cam.transform.position.y - player.transform.position.y;

    Debug.Log(xdir);

    if (xdir > 0 && Physics.Raycast (leftRay)) {
      Debug.Log("L");
      pos.x = player.transform.position.x;
    } 

    if (xdir < 0 && Physics.Raycast (rightRay)) {
      Debug.Log("R");
      pos.x = player.transform.position.x;
    }

    /*
    if (horizontalLimit > 0 && Mathf.Abs(playerStartY - player.transform.position.y) > horizontalLimit) {
      Debug.Log("U");
      pos.y = player.transform.position.y;
      playerStartY = pos.y;
    }
    */

    cam.transform.position = pos;
  }
}
