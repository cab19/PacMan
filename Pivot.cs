using System;
using SplashKitSDK;

public class Pivot
{
    public Point2D _Position;
    private int[] _PermissableDir;

    public Point2D Position
    {
        get { return _Position; }
    }
    
    public int[] PermissableDir
    {
        get { return _PermissableDir; }
    }
    
    public Pivot(Point2D posi, int[] permissableDir)
    {
        _Position = posi;
        _PermissableDir = permissableDir;
    }
}