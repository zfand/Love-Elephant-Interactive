using UnityEngine;
using System.Collections;

public class CameraScroll : MonoBehaviour
{

  public GameObject cam;
  public float horizontalLimit;
  public float hMoveTime = 0.1f;
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

    float xdir = cam.transform.position.x - player.transform.position.x;
    float ydir = cam.transform.position.y - player.transform.position.y;

    //move vertically
    if (xdir > 0 && Physics.Raycast (leftRay)) {
      pos.x = player.transform.position.x;
    } 

    if (xdir < 0 && Physics.Raycast (rightRay)) {
      pos.x = player.transform.position.x;
    }

    cam.transform.position = pos;

    //move horizonally
    if (horizontalLimit > 0) {
      if(playerStartY + horizontalLimit < player.transform.position.y) {
        pos.y += horizontalLimit;
        playerStartY = player.transform.position.y;
        StopCoroutine("HMoveCam");
        StartCoroutine("HMoveCam", pos);
      }
      if(playerStartY - horizontalLimit > player.transform.position.y) {
        pos.y -= horizontalLimit;
        playerStartY = player.transform.position.y;
        StopCoroutine("HMoveCam");
        StartCoroutine("HMoveCam", pos);
      }
    }

  }

  private IEnumerator HMoveCam(Vector3 pos) {
    float deltaTime = 0;
    float startY = cam.transform.position.y;
    Ray ray;
    if (cam.transform.position.y - pos.y <= 0) {
      ray = cam.camera.ScreenPointToRay (new Vector3 (cam.camera.pixelWidth/2, cam.camera.pixelHeight));
    } else {
      ray = cam.camera.ScreenPointToRay (new Vector3 (cam.camera.pixelWidth/2, 0f));
    }


    while (deltaTime <= hMoveTime) {
      if (Physics.Raycast (ray)) {
      Vector3 newPos = cam.transform.position;
        newPos.y = Mathf.Lerp(startY, pos.y, deltaTime / hMoveTime);
        cam.transform.position = newPos;
        deltaTime += Time.deltaTime;
      }
      yield return 0;
    }
  }
}
