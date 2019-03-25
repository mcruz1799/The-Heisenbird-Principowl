using System;

[Serializable]

//Track User's Progress through the current level system.
//TODO: Trim or Increase stats tracked.
public class LevelProgress
{
  public int level;
  public bool completed; //Other stats will be null unless this is true.
  public int score;
}