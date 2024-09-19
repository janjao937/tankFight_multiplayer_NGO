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
  public GameInfo userGamePreferences;

}

[Serializable]
public class GameInfo
{
    public Map map;
    public GameMode gameMode;
    public GameQueue gameQueue;

    public string ToMultiplayQueue()
    {
        return "";
    }
}

