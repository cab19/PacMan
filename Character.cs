using System;
using SplashKitSDK;



public class Character
{
    protected const int CELLSIZE = 24; // constant for animations cell width and height
    protected Gameboard _Gameboard;
    protected Bitmap _CharacterBitmap; // bitmap image of Character
    protected Bitmap _WaitingBitmap; // bitmap image of Character waiting
    protected Window _GameWindow; // reference to window object
    protected AnimationScript _MoveScript; // animation script
    protected Animation _Animation; // animation 
    protected DrawingOptions _DrawOpt; // drawing options for animation
    public char _Direction; // input handler to set direction of character
    protected Point2D _Position; // current position of character
    protected Point2D _CurrPivot; // the pivot character has just touched
    protected Point2D _StartPosition; // the start point for each character //********************************************************** */
    public int[] _PermissableDirection; // the permissable directions the character can move, set by pivot points



    public int WidthHeight
    {
        get { return CELLSIZE; } // characters height is same as cell size
    }

    public Point2D PointRtn(double X, double Y)
    {
        Point2D temp;
        temp.X = X;
        temp.Y = Y;
        return temp;
    }

    public Point2D Position
    {
        get { return _Position; }
    }

    public Point2D CharacterPivotPosition
    {
        get { return PointRtn(_Position.X + (CELLSIZE / 4), _Position.Y + (CELLSIZE / 4)); }
    }


    public Character(Window gameWindow, Gameboard gameBoard, string charName, string charScript, int startX, int startY, int[] animSize)
    {
        _GameWindow = gameWindow; // reference to window
        _Gameboard = gameBoard;
        _CharacterBitmap = SplashKit.LoadBitmap(charName, charName + ".png"); // create bitmap 
        _CharacterBitmap.SetCellDetails(CELLSIZE, CELLSIZE, animSize[0], animSize[1], animSize[2]); // cell width, height, cols, rows, count for animation
        _MoveScript = SplashKit.LoadAnimationScript("MoveScript", charScript + ".txt"); // Load the animation script
        _Animation = _MoveScript.CreateAnimation("MoveLeft"); // Create the animation
        _DrawOpt = SplashKit.OptionWithAnimation(_Animation); // Create a drawing option
        _PermissableDirection = new int[] { 0, 0, 1, 1, 0 }; // Up, Down, Left, Right, Stop - all set to false on instantiation
        _CurrPivot = PointRtn(0, 0); // intialise curr pivot to empty, as character hasn't passed a pivot yet
        _Position.X = startX; // starting X position
        _Position.Y = startY; // starting Y position
        _StartPosition.X = startX; // save box position X & Y for reset
        _StartPosition.Y = startY;
    }

    // put character to their respective start position
    public virtual void Move2StartPosi()
    {
        _Position.X = _StartPosition.X;
        _Position.Y = _StartPosition.Y;
    }

    public void Draw() // draw the animated sprite
    {
        _GameWindow.DrawBitmap(_CharacterBitmap, _Position.X, _Position.Y, _DrawOpt);
        _Animation.Update();
    }

    // used to determine if two points are intersecting
    public bool pointsIntersected(Point2D p1, Point2D p2)
    {
        if (p1.X == p2.X)
            if (p1.Y == p2.Y)
                return true;

        return false;
    }

    // keep character in maze
    public virtual void KeepInMaze()
    {
        foreach (Pivot piv in _Gameboard.Pivots)
        {
            Point2D pivot = piv.Position; // get the position of the current pivot

            if (pointsIntersected(pivot, CharacterPivotPosition) && !pointsIntersected(_CurrPivot, CharacterPivotPosition))
            {

                _PermissableDirection = (int[])piv.PermissableDir.Clone(); // update permissable directions
                _CurrPivot = pivot; // keep record of pivot

                // Update animation based on the characters current direction (reflected by animation name)
                // and the stopflag, ie if character moving up hits pivot with up stop then pause
                // animation by assigning still cell/image.
                switch (_PermissableDirection[4])
                {
                    case 1:
                        if (_Animation.Name == "moveup")
                            _Direction = 'X';
                        break;
                    case 2:
                        if (_Animation.Name == "movedown")
                            _Direction = 'X';
                        break;
                    case 3:
                        if (_Animation.Name == "moveleft")
                            _Direction = 'X';
                        break;
                    case 4:
                        if (_Animation.Name == "moveright")
                            _Direction = 'X';
                        break;
                    case 5:
                        _Direction = 'X';
                        break;
                }
                break; // pivot hit exit
            }
        }
    }


    // Handle tunnel, character has left maze, put them to other side
    public void StayOnWindow()
    {
        const int PADDING = 10;

        if (_Position.X > _GameWindow.Width + PADDING)
            _Position.X = -PADDING;

        if (_Position.X < -PADDING)
            _Position.X = _GameWindow.Width + PADDING;
    }
}