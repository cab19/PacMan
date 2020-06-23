using System;
using SplashKitSDK;

public class Pellet
{
    private Point2D _Position; // position of pellet
    private int _WidthHeight; // pellet width and height
    private bool _SuperPellet; // standard or super sized
    private Color _Colour; // pellet colour

    public Point2D Position
    {
        get { return _Position; }
    }

    public bool SuperPellet
    {
        get { return _SuperPellet; }
    }

    public Color Colour
    {
        get { return _Colour; }
    }

    public int WidthHeight
    {
        get { return _WidthHeight; }
    }

    public Pellet(Point2D position, bool super)
    {
        _Position = position;
        _SuperPellet = super;
        _WidthHeight = (super) ? 4 : 2; // set pellet height to 4 if super, 2 if normal
        _Colour = Color.RGBColor(245, 185, 171);
    }
}