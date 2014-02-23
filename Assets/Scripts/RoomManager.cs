using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  public class RoomManager : MonoBehaviour
  {

    public GameObject leftExit;
    public GameObject rightExit;
    public GameObject midExitB;
    public GameObject midExitT;
    public string neededKeyRight;
    public string neededKeyLeft;
    public string neededKeyMidB;
    public string neededKeyMidT;
    public string leftRoom;
    public string rightRoom;
    public string midRoomB;
    public string midRoomT;
    public GameObject player;

    // Use this for initialization
    void Start()
    {
      player = GameObject.FindGameObjectWithTag ("Player");
    }
  
    // Update is called once per frame
    void Update()
    {
    
    }

    public bool checkLeft()
    {
      Debug.Log ("checking left");
      return (neededKeyLeft == "none" || player.GetComponent<Inventory> ().CheckKey (neededKeyLeft));
    }

    public bool checkRight()
    {
      return (neededKeyRight == "none" || player.GetComponent<Inventory> ().CheckKey (neededKeyRight));
    }

    public bool checkMidB()
    {
      return (neededKeyMidB == "none" || player.GetComponent<Inventory> ().CheckKey (neededKeyMidB));
    }

    public bool checkMidT()
    {
      return (neededKeyMidT == "none" || player.GetComponent<Inventory> ().CheckKey (neededKeyMidT));
    }
  }
}