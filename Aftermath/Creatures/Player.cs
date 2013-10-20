using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;
using Aftermath.Utils;
using Aftermath.Lighting;
using Aftermath.Items;
using Aftermath.Map;

namespace Aftermath.Creatures
{
    public class Player : Human
    {
        public Player()
        {
            _health = 50;
        }

        public override bool IsPlayerControlled
        {
            get
            {
                return true;
            }
        }

        public override CreatureType Type
        {
            get { return CreatureType.Player; }
        }

        public override GameTexture Texture
        {
            get 
            {
                if (IsAlive)
                    return new GameTexture("character", new RectangleI(0, 0, 64, 64));
                return new GameTexture("zombie_dead", new RectangleI(0, 0, 64, 64));
            }
        }

        public override void PostTurn()
        {
            //update the flashlight to the player's new position
            if (Flashlight != null)
                Flashlight.SetPosition(Location);

            Location.DropScent(500);

            //if (Dice.Next(50) == 0)
            //    Map.Sound.Emit(Location, 100);

            base.PostTurn();
        }

        Flashlight _flashlight;
        public Flashlight Flashlight
        {
            get
            {
                return _flashlight;
            }
            set
            {
                _flashlight = value;
            }
        }

        internal void ToggleFlashlight()
        {
            if (Flashlight == null)
                return;
            Flashlight.On = !Flashlight.On;
            EndTurn();
        }

        public override void Serialize(System.IO.BinaryWriter bw)
        {
            base.Serialize(bw);
            bool hasFlashlight = Flashlight != null;
            bw.Write((bool)hasFlashlight);
            if (hasFlashlight)
            {
                bw.Write(Flashlight.On);
            }

            //inventory
            bw.Write((ushort)Inventory.Count);
            foreach (Item item in Inventory)
            {
                bw.Write((ushort)item.Type);
                item.Serialize(bw);
            }
        }

        public override void Deserialize(System.IO.BinaryReader br, World world)
        {
            base.Deserialize(br, world);
            bool hasFlashlight = br.ReadBoolean();
            if (hasFlashlight)
            {
                Flashlight = new Flashlight();
                world.RegisterPointLight(Flashlight.Light);
                Flashlight.On = br.ReadBoolean();
            }
            //inventory
            int numItems = br.ReadUInt16();
            for (int i=0;i<numItems;i++)
            {
                Item item = ChunkSerializer.DeserializeItem(br);
                AddItemToInventory(item);
            }
        }
    }
}
