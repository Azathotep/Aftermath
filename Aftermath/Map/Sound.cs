using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.AI;
using Aftermath.Core;

namespace Aftermath.Map
{
    /// <summary>
    /// Represents an emitted sound on the map that creatures within range can hear
    /// </summary>
    public class Sound
    {
        HomingField _field;
        Tile _source;
        int _volume;

        /// <summary>
        /// Construct a new sound
        /// </summary>
        /// <param name="source">source of the sound</param>
        /// <param name="volume">volume of the sound, determines how far the sound travels</param>
        private Sound(Tile source, int volume)
        {
            _source = source;
            _volume = volume;
        }

        int _turnEmitted;
        /// <summary>
        /// Returns the turn number this sound was emitted on
        /// </summary>
        public int TurnEmitted
        {
            get
            {
                return _turnEmitted;
            }
        }

        public static Sound Emit(Tile source, int volume)
        {
            Sound sound = new Sound(source, volume);
            sound._turnEmitted = Engine.Instance.TurnSystem.TurnNumber;
            sound._field = new HomingField();
            sound._field.AddHomingTarget(source);
            sound._field.Generate(volume, sound.field_OnTileReached);
            return sound;
        }
        
        void field_OnTileReached(Tile tile, int distance)
        {
            if (tile.Creature == null)
                return;
            tile.Creature.Hear(this);
        }

        /// <summary>
        /// Returns the next tile towards the source of the sound
        /// </summary>
        public Tile GetNextTileTowardsSource(Tile start)
        {
            return _field.GetNextTileTowardsTarget(start);
        }

        public int GetDistanceFromSource(Tile tile)
        {
            return _field.GetDistanceFromTarget(tile);
        }
    }
}
