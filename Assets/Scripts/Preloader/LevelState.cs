/// <summary>
/// Level state. 
///  Used to determine the state of the Level On Awake
/// </summary>
using System;
namespace Preloader
{
  public enum LevelState
  {
    None,
    SkipIntro = 1,
    OnFire = 2,
    DuncanBrokeIt = 3,
    Complete = 4
  }
}
