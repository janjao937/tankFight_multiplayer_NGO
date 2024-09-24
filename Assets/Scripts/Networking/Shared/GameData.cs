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
  public GameInfo UserGamePreferences = new GameInfo();

}

[Serializable]
public class GameInfo
{
    public Map Map;
    public GameMode GameMode;
    public GameQueue GameQueue;

    public string ToMultiplayQueue()
    {
        return GameQueue switch{
          GameQueue.Solo=>"solo-queue",
          GameQueue.Team=>"team-queue",
          _=>"solo-queue"
        };
    }
}

