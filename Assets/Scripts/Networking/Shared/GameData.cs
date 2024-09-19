using System;

public enum Map
{
  Default
}

public enum GameMode
{
  Default
}

public enum GameQueue
{
  Solo,
  Team
}

[System.Serializable]
public class UserData
{
  public string UserName;
  public string UserAuthId;
  public GameInfo UserGamePreferences;

}

[Serializable]
public class GameInfo
{
    public Map Map;
    public GameMode GameMode;
    public GameQueue GameQueue;

    public string ToMultiplayQueue()
    {
        return "";
    }
}

