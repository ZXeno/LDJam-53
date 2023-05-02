namespace DefaultNamespace
{
    using System.Collections.Generic;

    public class GameState
    {
        public static GameStateEnum CurrentGameState = GameStateEnum.Loading;

        public static Dictionary<string, object> GameStateData = new Dictionary<string, object>();
    }
}