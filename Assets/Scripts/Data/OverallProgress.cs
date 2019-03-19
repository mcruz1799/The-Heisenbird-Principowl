using System;

[Serializable]

//TODO: Decide whether to have multiple saves possible, or just a single one.
public class OverallProgress
{
  public LevelProgress[] levels;
  public String saveName;
}
