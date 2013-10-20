using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using System.IO;
using Aftermath.Creatures;
using Aftermath.Items;
using Aftermath.Weapons;
using Aftermath.Lighting;

namespace Aftermath.Map
{
    /// <summary>
    /// Manages saving and loading the map to disk
    /// </summary>
    class ChunkSerializer
    {
        //version of the serializer. Only saved maps that match this version can be loaded. Older version maps
        //must be converted. A converter would have to be written. I am leaving that as a task independent of the game.
        const int currentVersion = 1;

        public static Item DeserializeItem(BinaryReader br)
        {
            ItemType type = (ItemType)br.ReadUInt16();
            Item item = null;
            switch (type)
            {
                case ItemType.Pistol9mm:
                    item = new Pistol9mm();
                    break;
                default:
                    throw new Exception("Invalid item type");
            }
            item.Deserialize(br);
            return item;
        }

        public World Deserialize(string filePath)
        {
            FileStream fs = File.OpenRead(filePath);
            BinaryReader br = new BinaryReader(fs);
            ushort version = br.ReadUInt16();
            if (version != currentVersion)
            {
                //chunk file in incorrect version, may require upgrade if older
                return null;
            }
            short width = br.ReadInt16();
            short height = br.ReadInt16();

            World world = new World(width, height);
            for (int y = 0; y < world.Height; y++)
                for (int x = 0; x < world.Width; x++)
                {
                    Tile tile = world.GetTile(x, y);
                    tile.Deserialize(br);
                }

            //deserialize corpses
            int numCorpses = br.ReadInt32();
            for (int i = 0; i < numCorpses; i++)
            {
                ushort x = br.ReadUInt16();
                ushort y = br.ReadUInt16();
                Tile tile = world.GetTile(x, y);
                CreatureType type = (CreatureType)br.ReadByte();
                switch (type)
                {
                    case CreatureType.Player:
                        tile.Corpse = new Player();
                        break;
                    case CreatureType.Zombie:
                        tile.Corpse = new Zombie();
                        break;
                }
            }

            //deserialize creatures
            int numCreatures = br.ReadInt32();
            for (int i = 0; i < numCreatures; i++)
            {
                ushort x = br.ReadUInt16();
                ushort y = br.ReadUInt16();
                Tile tile = world.GetTile(x, y);

                Creature creature=null;
                CreatureType type = (CreatureType)br.ReadByte();
                switch (type)
                {
                    case CreatureType.Player:
                        creature = new Player();
                        break;
                    case CreatureType.Zombie:
                        creature = new Zombie();
                        break;
                    default:
                        throw new Exception("Unknown creature type");
                }
                creature.Deserialize(br, world);
                tile.PlaceCreature(creature);
            }

            //deserialize items
            int numItems = br.ReadInt32();
            for (int i = 0; i < numItems; i++)
            {
                ushort x = br.ReadUInt16();
                ushort y = br.ReadUInt16();
                Tile tile = world.GetTile(x, y);
                Item item = DeserializeItem(br);
                tile.PlaceItem(item);
            }

            //deserialize structures
            int numStructures = br.ReadInt32();
            for (int i = 0; i < numStructures; i++)
            {
                ushort x = br.ReadUInt16();
                ushort y = br.ReadUInt16();
                Tile tile = world.GetTile(x, y);
                StructureType type = (StructureType)br.ReadByte();
                Structure structure = null;
                switch (type)
                {
                    case StructureType.Door:
                        structure = new Door();
                        break;
                    default:
                        throw new Exception("Invalid structure type");
                }
                structure.Deserialize(br);
                tile.PlaceStructure(structure);
            }
            br.Close();
            return world;
        }

        public void Serialize(World world, string filePath)
        {
            FileStream fs = File.OpenWrite(filePath);
            BinaryWriter sw = new BinaryWriter(fs);

            sw.Write((ushort)currentVersion);
            sw.Write((short)world.Width);
            sw.Write((short)world.Height);

            List<Creature> creatures = new List<Creature>();
            List<Creature> corpses = new List<Creature>();
            List<Item> items = new List<Item>();
            List<Structure> structures = new List<Structure>();

            //initial parse and serialize tiles
            for (int y=0;y<world.Height;y++)
                for (int x=0;x<world.Width;x++)
                {
                    Tile tile = world.GetTile(x,y);
                    tile.Serialize(sw);

                    if (tile.Creature != null)
                        creatures.Add(tile.Creature);
                    if (tile.Corpse != null)
                        creatures.Add(tile.Corpse);
                    if (tile.Item != null)
                        items.Add(tile.Item);
                    if (tile.Structure != null)
                        structures.Add(tile.Structure);
                }

            //serialize corpses
            sw.Write((int)corpses.Count);
            foreach (Creature corpse in corpses)
            {
                sw.Write((ushort)corpse.Location.X);
                sw.Write((ushort)corpse.Location.Y);
                sw.Write((byte)corpse.Type);
            }

            //serialize creatures
            sw.Write((int)creatures.Count);
            foreach (Creature creature in creatures)
            {
                sw.Write((ushort)creature.Location.X);
                sw.Write((ushort)creature.Location.Y);
                sw.Write((byte)creature.Type);
                creature.Serialize(sw);
                //TODO heardsound
            }

            //serialize items
            sw.Write((int)items.Count);
            foreach (Item item in items)
            {
                sw.Write((ushort)item.Location.X);
                sw.Write((ushort)item.Location.Y);
                sw.Write((ushort)item.Type);
                item.Serialize(sw);
            }

            //serialize structures
            sw.Write((int)structures.Count);
            foreach (Structure structure in structures)
            {
                sw.Write((ushort)structure.Location.X);
                sw.Write((ushort)structure.Location.Y);
                sw.Write((byte)structure.Type);
                structure.Serialize(sw);
            }

            sw.Close();
        }
    }
}
