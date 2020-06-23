using System;
using SplashKitSDK;


public class Ghost : Character
{
    private const int DEFAULT_DELAY = 5;
    private Point2D _CurrGhostPivot; // the target the ghost just hit
    public Pivot _CurrTarget; // the target the ghost should head
    private Timer _Timer; // timer for starting ghosts
    private static Timer _VulnTimer; // timer for ghost vulnerability
    public int _Delay; // amount of delay before ghost starts.
    private string _Name; // name of character
    private bool _Waiting; // ghost is waiting in the centre pen
    private bool _WaitDirUp; // keep track of direction of waiting ghost
    public static bool _Vulnerable; // ghost is vulnerable to pacman
    public static bool _VulnerableTransition; // ghost transitioning back to normal mode
    public bool _Harmless; // ghost is harmless to pacman

    public Point2D CurrTarget
    {
        get { return _CurrTarget.Position; }
    }

    public string Name
    {
        get { return _Name; }
    }

    public Ghost(Window gameWindow, Gameboard gameBoard, string charName, string charScript, int startX, int startY, int[] animSize) : base(gameWindow, gameBoard, charName, charScript, startX, startY, animSize)
    {
        _Animation.Assign("MoveUp"); // start anim frame
        _Name = charName; // character name
        _WaitDirUp = false; // flag to pause ghost
        _Waiting = false; // centre box waiting
        _Timer = SplashKit.CreateTimer("timer"); // timer
        _VulnTimer = SplashKit.CreateTimer("vulntimer"); // vulerability timer
        _Vulnerable = false; // flag for vulnerability
        _CurrGhostPivot = PointRtn(0, 0); // intialise curr pivot to empty, as character hasn't passed a pivot yet
        _CurrTarget = RandomPivot(); // target from ghost pivot list
        if (_Name == "blinky") // blinky is annoying..
            _Position.Y = _StartPosition.Y - 42;
    }


    // put character to their respective start position
    public override void Move2StartPosi()
    {
        _Animation.Assign("MoveUp");
        _Direction = 'X';
        _Position.X = _StartPosition.X;
        if (_Name == "blinky") // blinky is annoying..
            _Position.Y = _StartPosition.Y - 42;
        else
            _Position.Y = _StartPosition.Y;
    }

    // ghost is ready to start exploring
    public void StartGhost(int delay)
    {
        _Delay = delay;
        _Waiting = true;
    }

    // animate the ghost up and down
    public void GhostWaiting()
    {
        // start timer, if not already 
        if (!_Timer.IsStarted)
            _Timer.Start();
        else
        {
            // start counting until release
            // - put ghost on pivot, set direction up
            if (_Timer.Ticks / 1000 >= _Delay)
            {
                _Waiting = false;
                _Position.X = 162;
                _Position.Y = 234;
                _Direction = 'U';
                _Timer.Stop(); // stop timer
                _Timer.Reset(); // reset timer
            }
        }

        // animation logic, move ghosts up and down
        if (_WaitDirUp)
        {
            _Position.Y -= .5; // move ghost up
            if (_Position.Y <= _StartPosition.Y - 10)
                _WaitDirUp = false;
        }
        else
        {
            _Position.Y += .5; // move ghost down
            if (_Position.Y >= _StartPosition.Y)
                _WaitDirUp = true;
        }
    }



    // returns a random pivot from list (negating added control pivots)
    private Pivot RandomPivot()
    {
        return _Gameboard.GhostPivots[SplashKit.Rnd(10, _Gameboard.GhostPivots.Count - 10)];
    }


    // make sure ghosts stay in the maze!!!
    public override void KeepInMaze()
    {
        foreach (Pivot piv in _Gameboard.GhostPivots)
        {
            Point2D pivot = piv.Position; // get the position of the current pivot

            if (!_Waiting)
            {
                if (pointsIntersected(pivot, CharacterPivotPosition) && !pointsIntersected(_CurrGhostPivot, CharacterPivotPosition))
                {
                    _PermissableDirection = (int[])piv.PermissableDir.Clone(); // update permissable directions
                    _CurrGhostPivot = pivot; // keep record of pivot
                    MoveGhostPivot(); // moves ghost to selected pivot, "normal mode" 
                    break; // pivot hit exit
                }
            }
        }
    }

    // moves ghost in random fashion (ghost movement version 1)
    public void MoveGhost()
    {
        char[] charDirs = new char[] { 'U', 'D', 'L', 'R' };
        int[] tmpPermDir = (int[])_PermissableDirection.Clone();

        // remove current dir as an option, no doubling back...
        switch (_Direction)
        {
            case 'U':
                tmpPermDir[1] = 0; // moving up, remove down option
                break;
            case 'D':
                tmpPermDir[0] = 0; // moving down, remove up option
                break;
            case 'L':
                tmpPermDir[3] = 0; // moving left, remove right option
                break;
            case 'R':
                tmpPermDir[2] = 0; // moving right, remove left option
                break;
        }

        string perDirs = ""; // create list of permissible directions
        for (int i = 0; i < 4; i++)
        {
            if (Convert.ToBoolean(tmpPermDir[i]))
                perDirs += charDirs[i] + "|";
        }

        string[] choice = perDirs.Split('|'); // split list to an array
        char tmpDir;

        if(choice[0]=="")
            tmpDir = 'U';
        else    
            tmpDir = Convert.ToChar(choice[SplashKit.Rnd(0, choice.Length - 1)]);

        _Direction = tmpDir;
    }

    //moves the ghost to the _CurrTarget pivot (movement version 2)
    private void MoveGhostPivot()
    {
        int targetX = (int)_CurrTarget.Position.X; // targets x position
        int targetY = (int)_CurrTarget.Position.Y; // targets y position
        int currX = (int)CharacterPivotPosition.X;
        int currY = (int)CharacterPivotPosition.Y;
        bool hit65 = false;
        Point2D curr = PointRtn(currX, currY);
        if (pointsIntersected(_Gameboard.GhostPivots[65].Position, curr))
            hit65 = true; // ghost box

        if (currX == targetX && currY == targetY)
        {            
            if (hit65) // hit centre pivot, stop ghost set to waiting.
            {                
                if (!_Waiting)
                {
                    _Direction = 'X'; // stop ghost
                    _Waiting = true; // waiting true
                    _Harmless = false;
                    _Delay = DEFAULT_DELAY;
                }
            }
            _CurrTarget = RandomPivot();
        }

        char[] charDirs = new char[] { 'U', 'D', 'L', 'R' };
        string pref = ""; // direction preference

        // below target
        if (currY > targetY)
            pref = "0|";
        else if (currY < targetY)
            pref = "1|";

        if (currX > targetX)
            pref += "2|";
        else if (currX < targetX)
            pref += "3|";

        if (currY == targetY)
            pref += "0|";

        if (currX == targetX)
            pref += "2|";

        string[] choice = pref.Split('|'); // split list to an array

        // pad out the missing directions
        if (Array.IndexOf(choice, "0") < 0) // doesn't contain up?
            pref += "0|";

        if (Array.IndexOf(choice, "1") < 0) // doesn't contain down?
            pref += "1|";

        if (Array.IndexOf(choice, "2") < 0) // doesn't contain left?
            pref += "2|";

        if (Array.IndexOf(choice, "3") < 0) // doesn't contain right?
            pref += "3|";

        string[] finalOrder = pref.Split('|'); // split list to an array


        // clone permissible dir, set double back to 0
        int[] tmpPermDir = (int[])_PermissableDirection.Clone();
        if(!hit65) // if at centre pivot don't stop ghosts doubling back, they get stuck otherwise.
        {
            switch (_Direction)
            {
                case 'U':
                    // set permissible down to false
                    tmpPermDir[1] = 0;
                    break;
                case 'D':
                    // set permissible up to false
                    tmpPermDir[0] = 0;
                    //move up to last
                    break;
                case 'L':
                    // set permissible right to false
                    tmpPermDir[3] = 0;
                    // move right to last
                    break;
                case 'R':
                    // set permissible left to false
                    tmpPermDir[2] = 0;
                    // move left to last
                    break;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (Convert.ToBoolean(tmpPermDir[Convert.ToInt16(finalOrder[i])]))
            {
                _Direction = charDirs[Convert.ToInt16(finalOrder[i])];
                break;
            }
        }
    }

    // Move the character based on set direction
    public void Update()
    {        
        if (_Waiting)
            GhostWaiting();

        StayOnWindow(); // make sure character is on window (handles tunnel)

        if (_Vulnerable && !_Harmless || _VulnerableTransition && !_Harmless)
        {                            
            if (_Vulnerable && _Animation.Name != "vulnerable" )
                _Animation.Assign("vulnerable");
            if (_VulnerableTransition && _Animation.Name != "vulntransistion")
                _Animation.Assign("vulntransistion");
            if (!_VulnTimer.IsStarted)
            {
                _VulnTimer.Start();
            }
            else
            {
                if (_VulnTimer.Ticks / 1000 >= 4) // blue for 4 seconds
                {
                    _Vulnerable = false;
                    _VulnerableTransition = true;

                    if (_VulnTimer.Ticks / 1000 >= 8) // alternate for 3
                    {
                        _VulnerableTransition = false;
                        _VulnTimer.Stop(); // stop timer
                        _VulnTimer.Reset(); // reset timer
                    }
                }
            }
        }
        else
        {
            // movement animations
            string anim;
            switch (_Direction)
            {
                case 'U':
                    if (_Animation.Name != "moveup")
                    {
                        anim = (_Harmless)? "EyesUp" : "MoveUp";
                        _Animation.Assign(anim);
                    }
                    break;
                case 'D':
                    if (_Animation.Name != "movedown")
                    {
                        anim = (_Harmless)? "EyesDown" : "MoveDown";
                        _Animation.Assign(anim);
                    }
                    break;
                case 'L':
                    if (_Animation.Name != "moveleft")
                    {
                        anim = (_Harmless)? "EyesLeft" : "MoveLeft";
                        _Animation.Assign(anim);
                    }
                    break;
                case 'R':
                    if (_Animation.Name != "moveright")
                    {
                        anim = (_Harmless)? "EyesRight" : "MoveRight";
                        _Animation.Assign(anim);
                    }
                    break;
            }
        }


        // update position based on direction char.
        switch (_Direction)
        {
            case 'U':
                _Position.Y -= .5;
                break;
            case 'D':
                _Position.Y += .5;
                break;
            case 'L':
                _Position.X -= .5;
                break;
            case 'R':
                _Position.X += .5;
                break;
        }
    }
}