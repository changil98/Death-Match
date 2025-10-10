using ExitGames.Client.Photon;
using Photon.Realtime;

public static class CustomPropertyExtensions
{
    private const string GameStateKey = "GameState";
    private const string CurrentRoundKey = "CurrentRound";

    public static void SetGameState(this Room room, GameState state)
    {
        room.SetCustomProperties(new Hashtable { { GameStateKey, (int)state } });
    }

    public static GameState GetGameState(this Room room)
    {
        return (GameState)room.CustomProperties[GameStateKey];
    }

    public static void SetCurrentRound(this Room room, int round)
    {
        room.SetCustomProperties(new Hashtable { { CurrentRoundKey, round } });
    }

    public static int GetCurrentRound(this Room room)
    {
        return (int)room.CustomProperties[CurrentRoundKey];
    }
}