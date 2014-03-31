using UnityEngine;
using System.Collections;

namespace LoveElephant
{
  /// <summary>
  /// Movement stats.
  ///  All stats related to player movement
  /// </summary>
  [System.Serializable]
  public class MovementStats
  {

    /// <summary>
    /// Amount of Force added to move the player left or right.
    /// </summary>
    public float moveForce = 365f;
    /// <summary>
    /// The fastest the player can travel in the x axis.
    /// </summary>
    public float maxRunSpeed = 5f;  
    
    /// <summary>
    /// Amount of force added when the player jumps.
    /// </summary>
    public float jumpForce = 1000f;
  }
}
