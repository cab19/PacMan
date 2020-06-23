using SplashKitSDK;
using System;
using System.Collections.Generic;

public class PacmanGame
{
    private const int LIVES = 3;
    private int _StartDelay = 3; // delay for start of game
    private Pacman _Pacman;
    private List<Ghost> _Ghosts = new List<Ghost>();
    private List<string> _GhostsEaten = new List<string>();
    private Ghost _Blinky;
    private Ghost _Inky;
    private Ghost _Pinky;
    private Ghost _Clyde;
    private Window _GameWindow;
    private Gameboard _Gameboard;
    private Timer _StartTimer; // timer for starting game
    private bool _GameStopped;
    private bool _GameStarting; // flag for the delay from spacebar to start.
    private bool _GameOver;

    public PacmanGame(Window gameWindow)
    {
        SplashKit.LoadMusic("siren", "siren.mp3");
        SplashKit.LoadSoundEffect("theme", "theme.mp3");
        SplashKit.LoadSoundEffect("dead", "dead.mp3");
        SplashKit.LoadSoundEffect("eating", "eating.mp3");
        SplashKit.LoadSoundEffect("eaten", "eaten.mp3");
        SplashKit.LoadSoundEffect("vulnerable", "vulnerable.mp3");        

        _StartTimer = SplashKit.CreateTimer("startTimer");
        _GameWindow = gameWindow; // reference to the passed in window
        _Gameboard = new Gameboard(_GameWindow); // instantiate gameboard object
        _GameStopped = true;
        _GameOver = false;
        SplashKit.LoadFont("emulogic", "emulogic.ttf");
        _Pacman = new Pacman(_GameWindow, _Gameboard, "pacman", "pacman", 162, 336, new int[] { 3, 10, 30 }); // instantiate player and save to field
        _Pacman._Lives = LIVES; // set lives to begin game
        // instantiate ghosts
        _Blinky = new Ghost(_GameWindow, _Gameboard, "blinky", "character", 162, 234, new int[] { 3, 6, 18 }); // instantiate blinky
        _Inky = new Ghost(_GameWindow, _Gameboard, "inky", "character", 138, 234, new int[] { 3, 6, 18 }); // instantiate inky
        _Pinky = new Ghost(_GameWindow, _Gameboard, "pinky", "character", 162, 234, new int[] { 3, 6, 18 }); // instantiate pinky
        _Clyde = new Ghost(_GameWindow, _Gameboard, "clyde", "character", 186, 234, new int[] { 3, 6, 18 }); // instantiate clyde
        _Ghosts.Add(_Blinky);
        _Ghosts.Add(_Pinky);
        _Ghosts.Add(_Inky);
        _Ghosts.Add(_Clyde);
    }

    public void ResetLevel()
    {
        _GameStopped = false;
        _Pacman._PermissableDirection = new int[] { 0, 0, 1, 1, 0 };
        _Pacman.Move2StartPosi();
        _Pacman.Start();
        _Pacman._Alive = true; // pacman is alive
        
        foreach (Ghost g in _Ghosts) // move characters to start posi's
        {
            g.Move2StartPosi();
        }

        _Pacman._Direction = 'L'; // set pacman off and running...
        _Blinky._Direction = (Convert.ToBoolean(SplashKit.Rnd(0,1)))? 'L': 'R'; // set pacman off and running...            
        _Pinky.StartGhost(5);
        _Inky.StartGhost(5);
        _Clyde.StartGhost(5);
        SplashKit.ResumeMusic(); // resume siren :)
    }

    public void HandleInput()
    {
        if (_GameStopped)
        {
            if (SplashKit.KeyTyped(KeyCode.SpaceKey))
                StartGame(true);
        }
    }

    // used when game first starts, or when new round starts (ie pacman ate all pellets)
    public void StartGame(bool resetScore)
    {
        _GameStarting = true;
        _Gameboard.CreatePellets(); // refresh pellets
        _Pacman.Start(); // pacman to starting posi
        _Pacman._Lives = LIVES; // reset lives.
        if(resetScore)
        {
            _Pacman._Score = 0;
            SplashKit.PlaySoundEffect("theme");
        }
        foreach (Ghost g in _Ghosts) // move characters to start posi's
        {
            g.Move2StartPosi();
        }
    }

    public void Draw()
    {
        _GameWindow.Clear(Color.Black);  // clear window with black
        _Gameboard.DrawMaze(); // draws maze jpeg to screen
        _Gameboard.DrawBoard(); // creates maze and pivot visual representations for testing

        _Pacman.Draw(); // draw pacman
        foreach (Ghost g in _Ghosts)
        {
            g.Draw();
        }
        UpdateHUD();
        _GameWindow.Refresh(60); // refresh screen
    }

    public void UpdateHUD()
    {
        if (_GameStarting)
            _GameWindow.DrawText("READY!", Color.Yellow, "emulogic", 15, 130, 265);

        if (_GameOver)
            _GameWindow.DrawText("Game Over", Color.Yellow, "emulogic", 12, 118, 268);

        _Pacman.UpdateHUD();
    }

    // update game state
    public void Update()
    {
        if (_GameStarting)
        {
            if (!_StartTimer.IsStarted)
            {
                _GameOver = false;
                _StartTimer.Start();
            }
            else
            {
                if (_StartTimer.Ticks / 1000 >= _StartDelay)
                {
                    _GameStarting = false;
                    _StartTimer.Stop(); // stop timer
                    _StartTimer.Reset(); // reset timer
                    ResetLevel();
                }
            }
        }


        if (!_GameStopped)
        {
            
            if(!SplashKit.MusicPlaying()) // play siren if not already
                SplashKit.PlayMusic("siren", -1);
            _Pacman.KeepInMaze(); // check collision with pivots, keeps pacman in the maze...
            _Pacman.HandleInput();
            foreach (Ghost g in _Ghosts)
            {
                g.KeepInMaze(); // check collision with 'ghost pivots' (reduced set, no cluster), keeps ghost in the maze and determines direction...
            }
            PelletCollision(); // check if pacman has eaten/collided with pellet
            CheckCollision(); // check if pacman and ghosts collide
            _Pacman.Update(); // update player
            foreach (Ghost g in _Ghosts)
            {
                g.Update(); // update ghost
            }
        }


        if (_GhostsEaten.Count > 0) // clears list of eaten ghosts each time vulnerability ends
        {
            if (!Ghost._Vulnerable && !Ghost._VulnerableTransition)
                _GhostsEaten.Clear();
        }


    }

    // check collision between pacman and ghosts.
    private void CheckCollision()
    {
        Rectangle pacman = SplashKit.InsetRectangle(SplashKit.RectangleFrom(_Pacman.Position, _Pacman.WidthHeight, _Pacman.WidthHeight), 3);
        Rectangle ghost;

        foreach (Ghost g in _Ghosts)
        {
            ghost = SplashKit.RectangleFrom(g.Position, g.WidthHeight, g.WidthHeight); // create collision box for current ghost
            if (SplashKit.RectanglesIntersect(pacman, ghost))
            {
                if (Ghost._Vulnerable || Ghost._VulnerableTransition) // ghost can be 'eaten'
                {
                    if(!SplashKit.SoundEffectPlaying("eaten"))
                        SplashKit.PlaySoundEffect("eaten");
                    g._CurrTarget = _Gameboard.GhostPivots[65]; // set target to pivot in ghost box (return ghost to centre box)
                    g._Harmless = true; // "eyes" can't kill pacman
                    bool ghostEaten = false; // check if ghost has already been eaten
                    foreach (string eg in _GhostsEaten) // used for scoring, first ghost eaten = 200, second = 400, etc
                    {
                        if (g.Name == eg)
                            ghostEaten = true; // ghost has already been eaten

                    }
                    if (!ghostEaten) // first time ghost has been eaten, add to eaten list
                    {
                        _GhostsEaten.Add(g.Name);
                        _Pacman._Score += 200 * _GhostsEaten.Count;
                    }
                }
                else if (!g._Harmless) // ghost kills pacman :(
                {
                    SplashKit.PauseMusic();
                    SplashKit.PlaySoundEffect("dead");
                    _GameStopped = true;
                    _Pacman._Lives -= 1;  // negate a life

                    if (_Pacman._Lives > 0)
                    {
                        _Pacman._Alive = false;
                        _GameStarting = true;
                    }
                    else                    
                        _GameOver = true;
                    
                }
            }
        }
    }

    // pacman pellet collision
    private void PelletCollision()
    {
        // TODO: scoring for pellets, super pellet

        List<Pellet> pellets2Remove = new List<Pellet>(); // list of pellet objects to remove

        if (_Gameboard._Pellets.Count > 0)
        {

            Rectangle pacman = SplashKit.InsetRectangle(SplashKit.RectangleFrom(_Pacman.Position, _Pacman.WidthHeight, _Pacman.WidthHeight), 3);

            foreach (Pellet p in _Gameboard._Pellets)
            {
                Rectangle pellet = SplashKit.RectangleFrom(p.Position, p.WidthHeight, p.WidthHeight);
                if (SplashKit.RectanglesIntersect(pacman, pellet))
                {
                    
                    pellets2Remove.Add(p);
                    // add to player score here and super pellet activation

                    if (p.SuperPellet)
                    {
                        Ghost._Vulnerable = true; // set ghosts to vulnerable
                        _Pacman._Score += 50;
                        SplashKit.PlaySoundEffect("vulnerable");
                    }
                    else
                    {
                        _Pacman._Score += 10;
                        if(!SplashKit.SoundEffectPlaying("vulnerable"))
                            SplashKit.PlaySoundEffect("eating");
                    }



                }
            }
            // remove pellets
            foreach (Pellet p in pellets2Remove)
            {
                _Gameboard._Pellets.Remove(p);
            }
        }
        else
            StartGame(false); // no pellets = new round.
    }
}