using System;
using SplashKitSDK;


public class Pacman : Character
{
    public int _Lives; // players starting lives
    public bool _Alive; // flag for character 
    public int _Score; // counter for score
    private Bitmap _LivesBitmap;

    public Pacman(Window gameWindow, Gameboard gameBoard, string charName, string charScript, int startX, int startY, int[] animSize) : base(gameWindow, gameBoard, charName, charScript, startX, startY, animSize)
    {
        _LivesBitmap = new Bitmap("Life", "pac.png");
        _Animation.Assign("Stopped");
        _Alive = true;
        _Score = 0;
    }

    public void Start()
    {
        _Direction = 'X'; // make sure character stopped
        Move2StartPosi(); // move to starting position
        _Animation.Assign("Stopped"); // apply stopped bitmap
    }

    // update hud
    public void UpdateHUD()
    {
        for (int i = 0; i < _Lives; i++)
        {
            _GameWindow.DrawBitmap(_LivesBitmap, 20 + ((_LivesBitmap.Width + 5) * i), _GameWindow.Height - (_LivesBitmap.Height + 5)); // display lives 
        }

        _GameWindow.DrawText($"SCORE: {_Score}", Color.White, "emulogic", 15, 40, 20); // display score
    }


    // Handles the key inputs from the player.

    public void HandleInput()
    {
        if (SplashKit.KeyDown(KeyCode.UpKey) || SplashKit.KeyDown(KeyCode.WKey))
        {
            if (Convert.ToBoolean(_PermissableDirection[0]))
            {
                _Direction = 'U';
            }
        }
        else if (SplashKit.KeyDown(KeyCode.DownKey) || SplashKit.KeyDown(KeyCode.SKey))
        {
            if (Convert.ToBoolean(_PermissableDirection[1]))
            {
                _Direction = 'D';
            }
        }
        else if (SplashKit.KeyDown(KeyCode.LeftKey) || SplashKit.KeyDown(KeyCode.AKey))
        {
            if (Convert.ToBoolean(_PermissableDirection[2]))
            {
                _Direction = 'L';
            }
        }
        else if (SplashKit.KeyDown(KeyCode.RightKey) || SplashKit.KeyDown(KeyCode.DKey))
        {
            if (Convert.ToBoolean(_PermissableDirection[3]))
            {
                _Direction = 'R';
            }
        }
    }

    // updates pacmans position and the associated animation depending on current direction.
    public virtual void Update()
    {
        StayOnWindow(); // make sure character is on window, handles tunnel

        if (_Alive) // whilst pacman is alive move him
        {
            switch (_Direction)
            {
                case 'U':
                    _Position.Y -= 1;
                    if (_Animation.Name != "moveup")
                        _Animation.Assign("MoveUp");
                    break;
                case 'D':
                    _Position.Y += 1;
                    if (_Animation.Name != "movedown")
                        _Animation.Assign("MoveDown");
                    break;
                case 'L':
                    _Position.X -= 1;
                    if (_Animation.Name != "moveleft")
                        _Animation.Assign("MoveLeft");
                    break;
                case 'R':
                    _Position.X += 1;
                    if (_Animation.Name != "moveright")
                        _Animation.Assign("MoveRight");
                    break;
                default:
                    if (_Animation.Name == "moveup")
                        _Animation.Assign("StopUp");
                    if (_Animation.Name == "movedown")
                        _Animation.Assign("StopDown");
                    if (_Animation.Name == "moveleft")
                        _Animation.Assign("StopLeft");
                    if (_Animation.Name == "moveright")
                        _Animation.Assign("StopRight");
                    break;
            }
        }
        else
        {
            if (_Animation.Name != "dead")
                _Animation.Assign("Dead");
        }
    }

}