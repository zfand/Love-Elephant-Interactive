using UnityEngine;
using System.Collections;

public class CameraScroll : MonoBehaviour
{

  public GameObject cam;
  public bool onlyVertical = false;
  public float verticalLimt;
  public float vMoveTime = 0.1f;
  private GameObject player;
  private float playerStartY;
  private bool following;

  // Use this for initialization
  void Start()
  {
    following = false;
    player = GameObject.FindGameObjectWithTag ("Player");
    playerStartY = player.transform.position.y;
    cam.transform.position = new Vector3 (0, cam.transform.position.y, cam.transform.position.z);
    StartCoroutine (HMoveCam (player.transform.position));
  }
  
  // Update is called once per frame
  void Update()
  {
    if (following) {
      Vector3 pos = cam.transform.position;
      float scrollPos;
      float playerScrollPos;
      Ray minRay;
      Ray maxRay;
      if (onlyVertical) {
        scrollPos = pos.y;
        playerScrollPos = player.transform.position.y;
        maxRay = cam.camera.ScreenPointToRay (new Vector3 (cam.camera.pixelWidth / 2, cam.camera.pixelHeight));
        minRay = cam.camera.ScreenPointToRay (new Vector3 (cam.camera.pixelWidth / 2, 0f));

      } else {
        scrollPos = pos.x;
        playerScrollPos = player.transform.position.x;
        minRay = cam.camera.ScreenPointToRay (new Vector3 (0f, 0f));
        maxRay = cam.camera.ScreenPointToRay (new Vector3 (cam.camera.pixelWidth, 0f));
      }
      float dir = scrollPos - playerScrollPos;

      //move vertically
      if ((dir > 0 && Physics.Raycast (minRay)) || (dir < 0 && Physics.Raycast (maxRay))) {
        scrollPos = playerScrollPos;
      } 

      if (onlyVertical) {
        pos.y = scrollPos;
      } else {
        pos.x = scrollPos;
      }

      cam.transform.position = pos;

      //move horizonally
      if (verticalLimt > 0 && !onlyVertical) {
        if (playerStartY + verticalLimt < player.transform.position.y) {
          pos.y += verticalLimt;
          playerStartY = player.transform.position.y;
          StopCoroutine ("HMoveCam");
          StartCoroutine ("HMoveCam", pos);
        }
        if (playerStartY - verticalLimt > player.transform.position.y) {
          pos.y -= verticalLimt;
          playerStartY = player.transform.position.y;
          StopCoroutine ("HMoveCam");
          StartCoroutine ("HMoveCam", pos);
        }
      }
    }

  }

  private IEnumerator VMoveCam(Vector3 pos)
  {
    float deltaTime = 0;
    float startY = cam.transform.position.y;
    float dir = cam.transform.position.y - pos.y;
    Vector3 newPos;
    Ray ray;

    while (deltaTime <= vMoveTime) {
      if (dir <= 0) {
        ray = cam.camera.ScreenPointToRay (new Vector3 (cam.camera.pixelWidth / 2, cam.camera.pixelHeight));
      } else {
        ray = cam.camera.ScreenPointToRay (new Vector3 (cam.camera.pixelWidth / 2, 0f));
      }

      if (Physics.Raycast (ray)) {
        newPos = cam.transform.position;
        newPos.y = Mathf.Lerp (startY, pos.y, deltaTime / vMoveTime);
        cam.transform.position = newPos;
        deltaTime += Time.deltaTime;
      } else {
        yield break;
      }
      yield return 0;
    }
  }

  private IEnumerator HMoveCam(Vector3 pos)
  {
    float dir = cam.transform.position.x - pos.x;
    float deltaX = Mathf.Sign (dir) * -1f;
    Ray r;
    Vector3 newPos;

    while (Mathf.Abs(cam.transform.position.x) < Mathf.Abs(pos.x)) {
      if (Mathf.Abs (deltaX) > 0.1f) {
        deltaX *= 0.96f;
      }

      if (dir >= 0) {
        r = cam.camera.ScreenPointToRay (new Vector3 (0f, 0f));
      } else {
        r = cam.camera.ScreenPointToRay (new Vector3 (cam.camera.pixelWidth, 0f));
      }


      if (Physics.Raycast (r)) {
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
