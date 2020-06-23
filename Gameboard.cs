using System;
using SplashKitSDK;
using System.Collections.Generic;

public class Gameboard
{
    private const int PIXELSIZE = 12; // constant for animations cell width and height
    private  int _X = 0; // animations cell width and height
    private  int _Y = 60; // animations cell width and height
    private Window _GameWindow;
    private Bitmap _Maze;
    private List<Pivot> _Pivots = new List<Pivot>(); // list of pivots, used for wall collision
    private List<Pivot> _GhostPivots = new List<Pivot>(); // list of the central pivots used to direct ghosts
    public List<Pellet> _Pellets = new List<Pellet>(); // list of pellet objects  

    private const int ARRAYSIZEX = 29; // constant for size of array -29
    private const int ARRAYSIZEY = 32; // constant for size of array -32

    // the walls of the maze
    private int[,] tiles = {
        { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        { 1,0,0,1,1,1,0,0,1,1,1,1,0,0,1,0,0,1,1,1,1,0,0,1,1,1,0,0,1},
        { 1,0,0,1,1,1,0,0,1,1,1,1,0,0,1,0,0,1,1,1,1,0,0,1,1,1,0,0,1},
        { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        { 1,0,0,1,1,1,0,0,1,0,0,1,1,1,1,1,1,1,0,0,1,0,0,1,1,1,0,0,1},
        { 1,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,1},
        { 1,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,1},
        { 1,1,1,1,1,1,0,0,1,1,1,1,0,0,1,0,0,1,1,1,1,0,0,1,1,1,1,1,1},
        { 1,1,1,1,1,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,1,1,1,1,1},
        { 1,1,1,1,1,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,1,1,1,1,1},
        { 1,1,1,1,1,1,0,0,1,0,0,1,1,1,1,1,1,1,0,0,1,0,0,1,1,1,1,1,1},
        { 0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0},
        { 1,1,1,1,1,1,0,0,1,0,0,1,1,1,1,1,1,1,0,0,1,0,0,1,1,1,1,1,1},
        { 1,1,1,1,1,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,1,1,1,1,1},
        { 1,1,1,1,1,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,1,1,1,1,1},
        { 1,1,1,1,1,1,0,0,1,0,0,1,1,1,1,1,1,1,0,0,1,0,0,1,1,1,1,1,1},
        { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        { 1,0,0,1,1,1,0,0,1,1,1,1,0,0,1,0,0,1,1,1,1,0,0,1,1,1,0,0,1},
        { 1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1},
        { 1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1},
        { 1,1,1,0,0,1,0,0,1,0,0,1,1,1,1,1,1,1,0,0,1,0,0,1,0,0,1,1,1},
        { 1,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,1},
        { 1,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,1},
        { 1,0,0,1,1,1,1,1,1,1,1,1,0,0,1,0,0,1,1,1,1,1,1,1,1,1,0,0,1},
        { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
    };

    // pivot positions on the maze
    private int[,] pivotPositions = {
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,1,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0,0,0,1,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,1,0,0,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,0,0,1,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,1,0,0,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,0,0,1,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,1,0,0,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,0,0,1,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,1,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,1,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,1,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,1,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,1,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}
    };

    // pellet positions (normal = 1, super = 2)
    private int[,] pelletPositions = {
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0 },
        { 0,0,1,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0,0,0,1,0 },
        { 0,0,2,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0,0,0,2,0 },
        { 0,0,1,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0,0,0,1,0 },
        { 0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0 },
        { 0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,1,0 },
        { 0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,1,0 },
        { 0,0,1,1,1,1,1,1,0,0,1,1,1,1,0,0,1,1,1,1,0,0,1,1,1,1,1,1,0 },
        { 0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0 },
        { 0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0 },
        { 0,0,1,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0,0,0,1,0 },
        { 0,0,1,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0,0,0,1,0 },
        { 0,0,2,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,2,0 },
        { 0,0,0,0,1,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,1,0,0,0 },
        { 0,0,0,0,1,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,1,0,0,0 },
        { 0,0,1,1,1,1,1,1,0,0,1,1,1,1,0,0,1,1,1,1,0,0,1,1,1,1,1,1,0 },
        { 0,0,1,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,1,0 },
        { 0,0,1,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,1,0 },
        { 0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }
    };


    public List<Pivot> Pivots // readonly property
    {
        get { return _Pivots; }
    }

    public List<Pivot> GhostPivots // readonly property
    {
        get { return _GhostPivots; }
    }

    public Gameboard(Window gameWindow)
    {
        _GameWindow = gameWindow; // save window
        _Maze = new Bitmap("Maze", "maze.jpg"); // create bitmap
        CreateBoard(); // create pivot objects which represent pivot points
    }

    // draw the maze image to window
    public void DrawMaze()
    {
        _GameWindow.DrawBitmap(_Maze, 0, 60);
    }



    // Control logic
    //     X        Convoluted but the only way I could think
    //   X X X      to make sure character stays within maze as my attempt
    //     X        to use collision detection didn't work...
    //
    // Using the passed in permissible direction array, ti calculate the above cluster/matrix
    private void createCluster(int arrX, int arrY)
    {
        Point2D tempPointer;
        tempPointer.X = _X + (PIXELSIZE * arrX) + (PIXELSIZE / 2);
        tempPointer.Y = _Y + (PIXELSIZE * arrY) + (PIXELSIZE / 2);
        int origX = (int)tempPointer.X;
        int origY = (int)tempPointer.Y;

        int[] tmpDirArray = new int[5];
        tmpDirArray = CreatePermissibleDirections(arrX, arrY, false); // now I have array of permissible dirs
        int[] origDirArray = (int[])tmpDirArray.Clone();

        // centre control
        Pivot tmpP = new Pivot(tempPointer, tmpDirArray); // point 2d, array ints
        _Pivots.Add(tmpP);
        _GhostPivots.Add(tmpP);


        //if we can go up, create up pivot and associated dir permissions
        if (Convert.ToBoolean(origDirArray[0]))
        {
            tempPointer.X = origX;
            tempPointer.Y = origY - 1; //top control placed 1 pixel above
            tmpDirArray = CreatePermissibleDirections(arrX, arrY - 1, true);
            tmpP = new Pivot(tempPointer, tmpDirArray); // point 2d, array ints
            _Pivots.Add(tmpP);
        }

        //if we can go down, create down pivot and associated dir permissions
        if (Convert.ToBoolean(origDirArray[1]))
        {
            tempPointer.X = origX;
            tempPointer.Y = origY + 1; //bottom control placed 1 pixel below
            tmpDirArray = CreatePermissibleDirections(arrX, arrY + 2, true);
            tmpP = new Pivot(tempPointer, tmpDirArray); // point 2d, array ints
            _Pivots.Add(tmpP);
        }

        //if we can go left, create left pivot and associated dir permissions
        if (Convert.ToBoolean(origDirArray[2]))
        {
            tempPointer.X = origX - 1; //left control placed 1 pixel to left
            tempPointer.Y = origY;
            tmpDirArray = CreatePermissibleDirections(arrX - 1, arrY, true);
            tmpP = new Pivot(tempPointer, tmpDirArray); // point 2d, array ints
            _Pivots.Add(tmpP);
        }

        //if we can go right, create right pivot and associated dir permissions
        if (Convert.ToBoolean(origDirArray[3]))
        {
            tempPointer.X = origX + 1; //right control placed 1 pixel to right
            tempPointer.Y = origY;
            tmpDirArray = CreatePermissibleDirections(arrX + 2, arrY, true);
            tmpP = new Pivot(tempPointer, tmpDirArray); // point 2d, array ints
            _Pivots.Add(tmpP);
        }
    }

    // cross referencing the tiles with the pivot points a set of permissible directions can be created.
    private int[] CreatePermissibleDirections(int arrX, int arrY, bool dontStop)
    {
        int currX;
        int currY;
        int[] tmpPerDir = new int[5];
        int stopFlag = 0; // Change to enum, 0 dont stop, 1 stop for up, 2 stop for down, 3 stop for left, 4 dtop right, 5 stop all dirs

        // check up movement
        currX = arrX;
        currY = arrY - 1;

        if (!Convert.ToBoolean(tiles[currY, currX]))
        {
            tmpPerDir[0] = 1;
        }
        else
        {
            stopFlag = (stopFlag == 0) ? 1 : 5; // check if stopdir has already been assigned, if so change flag to all.
        }

        // check down movement
        currX = arrX;
        currY = arrY + 2; // tile sizes are a quarter of character size, reference is the top left corner, to see if you can go down, check two tiles down.

        if (!Convert.ToBoolean(tiles[currY, currX]))
        {
            tmpPerDir[1] = 1;
        }
        else
        {
            stopFlag = (stopFlag == 0) ? 2 : 5; // check if stopdir has already been assigned, if so change flag to all.
        }

        // check left movement
        currX = arrX - 1;
        currY = arrY;

        if (!Convert.ToBoolean(tiles[currY, currX]))
        {
            tmpPerDir[2] = 1;
        }
        else
        {
            stopFlag = (stopFlag == 0) ? 3 : 5; // check if stopdir has already been assigned, if so change flag to all.
        }

        // check right movement
        currX = arrX + 2;
        currY = arrY;

        if (!Convert.ToBoolean(tiles[currY, currX]))
        {
            tmpPerDir[3] = 1;
        }
        else
        {
            stopFlag = (stopFlag == 0) ? 4 : 5; // check if stopdir has already been assigned, if so change flag to all.
        }

        if (!dontStop)
            tmpPerDir[4] = stopFlag;

        return tmpPerDir;
    }

    // the maze logic for control of characters
    public void CreateBoard()
    {
        for (int i = 0; i < ARRAYSIZEY; i++)
        {
            for (int j = 0; j < ARRAYSIZEX; j++)
            {
                if (Convert.ToBoolean(pivotPositions[i, j])) // check if pivot location
                {
                    createCluster(j, i); // create control cluster for this point of the map.
                }
            }
        }
        CreatePellets(); // create pellets
        FixPivots(); // 8 pivot points out of 232 programatically created are incorrect :( manually update them
    }

    public void CreatePellets()
    {
        // creation of pellet objects
        for (int i = 0; i < ARRAYSIZEY; i++)
        {
            for (int j = 0; j < ARRAYSIZEX; j++)
            {
                if (Convert.ToBoolean(pelletPositions[i, j])) // check if pellet location
                {

                    Point2D tempPointer; // temp Point2D, pixel size * i/j to define x/y
                    tempPointer.X = _X + PIXELSIZE * j - 1;
                    tempPointer.Y = _Y + PIXELSIZE * i - 1;
                    bool superPellet = (pelletPositions[i, j] == 2) ? true : false; // 2 indicates super pellet location
                    Pellet tmpP = new Pellet(tempPointer, superPellet); // instantiate pellet object
                    _Pellets.Add(tmpP); // add to list
                }
            }
        }
    }

    private void FixPivots()
    {
        Pivot tmpP = _Pivots[217];
        tmpP.PermissableDir[0] = 0;
        tmpP = _Pivots[214];
        tmpP.PermissableDir[1] = 0;
        tmpP = _Pivots[188];
        tmpP.PermissableDir[0] = 0;
        tmpP = _Pivots[191];
        tmpP.PermissableDir[1] = 0;
        tmpP = _Pivots[185];
        tmpP.PermissableDir[1] = 0;
        tmpP = _Pivots[182];
        tmpP.PermissableDir[0] = 0;
        tmpP = _Pivots[155];
        tmpP.PermissableDir[0] = 0;
        tmpP = _Pivots[152];
        tmpP.PermissableDir[1] = 0;

        // setup pivot for ghost box, duping cluster, correct it's location.
        createCluster(6, 1); // ghost pivot, above box
        tmpP = _Pivots[232];
        tmpP._Position = setPivotLocation(168 + _X, 138 + _Y);  // _X & _Y are used for offset    
        tmpP = _Pivots[233];                                    // _Y has been increased by 60 to allow for space above board.
        tmpP._Position = setPivotLocation(168 + _X, 139 + _Y);
        tmpP = _Pivots[234];
        tmpP._Position = setPivotLocation(169 + _X, 138 + _Y);
        tmpP = _Pivots[235];
        tmpP._Position = setPivotLocation(167 + _X, 138 + _Y);
        int[] tmpDirArray = new int[] { 1, 0, 0, 0, 5 };
        tmpP = new Pivot(setPivotLocation(168 + _X, 178 + _Y), tmpDirArray); // point 2d, array ints
        _Pivots.Add(tmpP);
        _GhostPivots.Add(tmpP);
    }

    // helper method to create point2D
    private Point2D setPivotLocation(int x, int y)
    {
        Point2D tmpPosi;
        tmpPosi.X = x;
        tmpPosi.Y = y;
        return tmpPosi;
    }

    // Draw pellets (the maze & pivots for testing/visualisation)
    public void DrawBoard()
    {
        foreach (Pellet p in _Pellets)
        {
            if(p.SuperPellet)
                _GameWindow.FillCircle(p.Colour, p.Position.X, p.Position.Y, p.WidthHeight); //visual representation
            else
                _GameWindow.FillRectangle(p.Colour, p.Position.X, p.Position.Y, p.WidthHeight, p.WidthHeight); //visual representation
        }
        
        //testing section, shows maze walls
        // for (int i = 0; i < ARRAYSIZEY; i++)
        // {
        //     for (int j = 0; j < ARRAYSIZEX; j++)
        //     {
        //         if (Convert.ToBoolean(tiles[i, j])) // check if tile
        //         {
        //             // draw square
        //             _GameWindow.FillRectangle(Color.White, _X + PIXELSIZE * j, _Y + PIXELSIZE * i, PIXELSIZE, PIXELSIZE); //visual representation                
        //         }
        //     }
        // }

        // testing section, shows pivot points
        // foreach (Pivot p in _Pivots)
        // {
        //     _GameWindow.FillRectangle(Color.Gold, p.Position.X, p.Position.Y, 1, 1); //visual representation
        // }

        // testing section, shows pivot points
        // foreach (Pivot p in _Pivots)
        // {
        //     _GameWindow.FillRectangle(Color.RandomRGB(200), p.Position.X, p.Position.Y, 1, 1); //visual representation
        // }

        //testing section, shows ghost pivot points
        // foreach (Pivot p in _GhostPivots)
        // {
        //     _GameWindow.FillRectangle(Color.Gold, p.Position.X, p.Position.Y, 1, 1); //visual representation
        // }
        

        // testing, highlights single pivot
        // Pivot tmpP = _GhostPivots[0]; // testing to find erroneous pivots, delete when finished.
        // _GameWindow.FillRectangle(Color.RandomRGB(200), tmpP.Position.X, tmpP.Position.Y, 5, 5); //visual representation
    }
}