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

namespace Aftermath.Scenarios
{
    class Tutorial1 : Scenario
    {
        MessageDialog _dialog = new MessageDialog();

        Zombie _zombie;
        public override void Initialize(out World world, out Player player)
        {
            CityBuilder builder = new CityBuilder();
            world = builder.FromBitmap(@"Content\Maps\tutorial.bmp");
            player = new Player();
            player.WeildGun(new Creature.Gun());
            world.GetTile(9,14).PlaceCreature(player);
            world.TimeOfDay = 12 * 60;
            _zombie = new Zombie();
            world.GetTile(9, 4).PlaceCreature(_zombie);

            Engine.Instance.TurnSystem.OnTurnAdvanced += TurnSystem_OnTurnAdvanced;

            _dialog.OnOk += _dialog_OnOk;
            _dialog.Show();

            _dialog.Position = new RectangleF(100, 100, 200, 200);

            string text = "Text";
            
            _dialog.SetText(text, true);
        }

        void _dialog_OnOk()
        {
            string text = "Slowly approach the zombie. Use the cursor keys to move.";
            _dialog.SetText(text, false);
        }

        bool hasAlerted = false;
        void TurnSystem_OnTurnAdvanced()
        {
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
