using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Core;
using Aftermath.Map;
using Aftermath.Creatures;
using Aftermath.UI;
using Aftermath.State;
using Aftermath.Utils;
using Aftermath.Weapons;
using Aftermath.Lighting;

namespace Aftermath.Scenarios
{
    class Tutorial1 : Scenario
    {
        Zombie _zombie;
        MessageDialog _dialog;
        Player _player;
        World _world;
        public override void Initialize(out World world, out Player player)
        {
            CityBuilder builder = new CityBuilder();
            world = builder.FromBitmap(@"Content\Maps\tutorial.bmp");
            _world = world;
            player = new Player();
            _player = player;
            player.WeildGun(new Pistol9mm());
            builder.PlayerStart.PlaceCreature(player);
            player.Flashlight = new Items.Flashlight();
            world.TimeOfDay = 3 * 60;
            _zombie = new Zombie();
            world.GetTile(9, 4).PlaceCreature(_zombie);

            Engine.Instance.TurnSystem.OnTurnAdvanced += TurnSystem_OnTurnAdvanced;

            _dialog = new MessageDialog(new RectangleF(10, 100, 200, 200));

            string text = "Welcome to the tutorial. You start in a small lit room. Use the cursor keys to move around";
            _dialog.SetText(text, false);
            _dialog.Show();
        }

        void _dialog_OnOk()
        {
            //string text = "Slowly approach the zombie. Use the cursor keys to move.";
            //_dialog.SetText(text, false);
        }

        enum Stage
        {
            Movement,
            DoorOpen,
            Walk,
            Flashlight,
            Continue
        }

        Stage _stage = Stage.Movement;

        Tile _lastPoint;
        bool hasAlerted = false;
        void TurnSystem_OnTurnAdvanced()
        {
            switch (_stage)
            {
                case Stage.Movement:
                    _lastPoint = _player.Location;
                    if (Engine.Instance.TurnSystem.TurnNumber == 5)
                    {
                        _stage = Stage.DoorOpen;
                        _dialog.SetText("Good. Now to open that door. To interact with objects move next to them and " + 
                            "press I. Move next to the door and press I to open it.");
                        _dialog.Show();
                    }
                    break;
                case Stage.DoorOpen:
                    Door door = _player.Location.GetNeighbour(CompassDirection.North).Structure as Door;
                    if (door != null && door.IsOpen)
                    {
                        _lastPoint = _player.Location;
                        _dialog.SetText("Good work. If you press I again you can close the door. Proceed up the " + 
                            "corridor to the next room");
                        _dialog.Show();
                        _stage = Stage.Walk;
                    }
                    break;
                case Stage.Walk:
                    if (_player.Location.GetChebyshevDistanceFrom(_lastPoint) > 8)
                    {
                        foreach (PointLight light in _world.Lights)
                        {
                            light.On = false;
                            light.RecalculateLightfield();
                        }
                        _dialog.SetText("Uh oh the lights have gone out. In the dark you cannot see as far. " + 
                            "Press L to turn on your flashlight.");
                        _dialog.Show();
                        _stage = Stage.Flashlight;
                    }
                    break;
                case Stage.Flashlight:
                    if (_player.Flashlight.On)
                    {
                        _dialog.SetText("Good work. You can turn your flashlight on and off by pressing L. Lets find why " +
                        "the power went out. Continue to the next room and find the generator. ");
                        _dialog.Show();
                        _stage = Stage.Continue;
                    }
                    break;
            }

            //if (_zombie.State == ZombieState.Alert && !hasAlerted)
            //{
            //    hasAlerted = true;
            //    string text = "The yellow warning icon above the zombie indicates it has spotted you. " +
            //                  "You can back out of the zombie's view before it realizes you are alive. Press Enter " +
            //                  "to continue the exercise.";
            //    _dialog.SetText(text, true);
            //}
            //if (_zombie.State == ZombieState.Enraged)
            //{
            //    string text = "The red warning icon above the zombie indicates it has become enraged. " +
            //                   "Enraged zombies can see much further and will chase you";
            //    _dialog.SetText(text, true);
            //}
        }
    }

    public abstract class Scenario
    {
        public abstract void Initialize(out World world, out Player player);
    }
}
