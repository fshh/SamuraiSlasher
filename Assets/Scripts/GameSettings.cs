using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Colors
{
    string name;
    Color light;
    Color dark;

    public Colors(string name, Color light, Color dark)
    {
        this.name = name;
        this.light = light;
        this.dark = dark;
    }
}

public static class GameSettings
{
    public static readonly int MAX_PLAYERS = 4;

    public static int NumberOfPlayers { private set; get; } = 1;
    /// <summary>
    /// A list the of NumberOfPlayers size that
    /// </summary>
    public static Color[] playerColors = null;

    /// <summary>
    /// Sets the number of players in the game to the given number.
    /// </summary>
    /// <param name="num">The number of players</param>
    public static void SetNumberOfPlayers(int num)
    {
        if (num > MAX_PLAYERS || num <= 0)
        {
            throw new System.ArgumentException("Given invalid number of players: " + num);
        }
        NumberOfPlayers = num;
    }
}
