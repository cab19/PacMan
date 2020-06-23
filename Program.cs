using System;
using SplashKitSDK;

public class Program
{
    public static void Main()
    {
        Window gameWindow = new Window("Pacman", 348, 475); // create game window
        PacmanGame thisGame = new PacmanGame(gameWindow); // create game object
        


        while(!gameWindow.CloseRequested)
        {
            SplashKit.ProcessEvents(); // process events
            thisGame.HandleInput();
            thisGame.Update();
            thisGame.Draw();
        }
    }
}