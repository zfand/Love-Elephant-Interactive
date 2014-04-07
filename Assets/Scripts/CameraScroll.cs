using UnityEngine;
using System.Collections;

public class CameraScroll : MonoBehaviour
{

  public GameObject cam;
  public float horizontalLimit;
  public float hMoveTime = 0.1f;
  private GameObject player;
  private float playerStartY;
  private bool following;

  // Use this for initialization
  void Start()
  {
    following = false;
    player = GameObject.FindGameObjectWithTag ("Player");
    playerStartY = player.transform.position.y;
    cam.transform.position = new Vector3(0, cam.transform.position.y, cam.transform.position.z);
    StartCoroutine(VMoveCam(player.transform.position));
  }
  
  // Update is called once per frame
  void Update()
  {
    if (following) {
    Vector3 pos = cam.transform.position;

    Ray leftRay = cam.camera.ScreenPointToRay (new Vector3 (0f, 0f));
    Ray rightRay = cam.camera.ScreenPointToRay (new Vector3 (cam.camera.pixelWidth, 0f));

    float xdir = cam.transform.position.x - player.transform.position.x;

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

  }

  private IEnumerator HMoveCam(Vector3 pos) {
    float deltaTime = 0;
    float startY = cam.transform.position.y;
    float dir = cam.transform.position.y - pos.y;
    Vector3 newPos;
    Ray ray;

    while (deltaTime <= hMoveTime) {
      if (dir <= 0) {
        ray = cam.camera.ScreenPointToRay (new Vector3 (cam.camera.pixelWidth/2, cam.camera.pixelHeight));
      } else {
        ray = cam.camera.ScreenPointToRay (new Vector3 (cam.camera.pixelWidth/2, 0f));
      }

      if (Physics.Raycast (ray)) {
        newPos = cam.transform.position;
        newPos.y = Mathf.Lerp(startY, pos.y, deltaTime / hMoveTime);
        cam.transform.position = newPos;
        deltaTime += Time.deltaTime;
      } else {
        yield break;
      }
      yield return 0;
    }
  }

  private IEnumerator VMoveCam(Vector3 pos) {
    float dir = cam.transform.position.x - pos.x;
    float deltaX = Mathf.Sign(dir)*-1f;
    Ray r;
    Vector3 newPos;

    while (Mathf.Abs(cam.transform.position.x) < Mathf.Abs(pos.x)) {
      if (Mathf.Abs(deltaX) > 0.1f) {
        deltaX *= 0.96f;
      }

      if (dir >= 0) {
        r = cam.camera.ScreenPointToRay (new Vector3 (0f, 0f));
      } else {
        r = cam.camera.ScreenPointToRay (new Vector3 (cam.camera.pixelWidth, 0f));
      }


      if (Physics.Raycast(r)) {
        newPos = cam.transform.position;
        newPos.x += deltaX;
        cam.transform.position = newPos;
      } else {
        following = true;
        yield break;
      }
      yield return 0;
    }
    following = true;
  }
}
