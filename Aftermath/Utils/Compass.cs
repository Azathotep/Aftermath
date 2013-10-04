using Aftermath.Input;

namespace Aftermath.Utils
{
    /// <summary>
    /// Provides operations for compass directions
    /// </summary>
    class Compass
    {
        public static CardinalDirection[] CardinalDirections = {CardinalDirection.North, CardinalDirection.East,
                                                                CardinalDirection.South, CardinalDirection.West};

        public static CompassDirection[] CompassDirections = {CompassDirection.North, CompassDirection.East,
                                                              CompassDirection.South, CompassDirection.West,
                                                              CompassDirection.NorthEast, CompassDirection.NorthWest,
                                                              CompassDirection.SouthEast, CompassDirection.SouthWest};

        public static CardinalDirection GetRandomCardinalDirection()
        {
            return CardinalDirections[Dice.Next(4)];
        }

        public static CompassDirection GetRandomCompassDirection()
        {
            return CompassDirections[Dice.Next(8)];
        }

        /// <summary>
        /// Converts a direction into a one unit grid direction 
        /// </summary>
        public static Vector2I DirectionToVector(CardinalDirection direction)
        {
            switch (direction)
            {
                case CardinalDirection.North:
                    return new Vector2I(0, -1);
                case CardinalDirection.East:
                    return new Vector2I(1, 0);
                case CardinalDirection.South:
                    return new Vector2I(0, 1);
                case CardinalDirection.West:
                    return new Vector2I(-1, 0);
            }
            return new Vector2I(1, 0);
        }

        internal static CardinalDirection GetOppositeDirection(CardinalDirection direction)
        {
            switch (direction)
            {
                case CardinalDirection.North:
                    return CardinalDirection.South;
                case CardinalDirection.East:
                    return CardinalDirection.West;
                case CardinalDirection.South:
                    return CardinalDirection.North;
                case CardinalDirection.West:
                    return CardinalDirection.East;
            }
            return CardinalDirection.North;
        }

        internal static Vector2I DirectionToVector(CompassDirection direction)
        {
            switch (direction)
            {
                case CompassDirection.North:
                    return new Vector2I(0, -1);
                case CompassDirection.East:
                    return new Vector2I(1, 0);
                case CompassDirection.South:
                    return new Vector2I(0, 1);
                case CompassDirection.West:
                    return new Vector2I(-1, 0);
                case CompassDirection.NorthEast:
                    return new Vector2I(1, -1);
                case CompassDirection.NorthWest:
                    return new Vector2I(-1, -1);
                case CompassDirection.SouthEast:
                    return new Vector2I(1, 1);
                case CompassDirection.SouthWest:
                    return new Vector2I(-1, 1);
            }
            return new Vector2I(0, 0);
        }

        /// <summary>
        /// Returns the direction mapped by a key or null if the key is not mapped to a direction
        /// </summary>
        public static CompassDirection? DirectionFromKey(InputKey key)
        {
            switch (key)
            {
                case InputKey.Up:
                    return CompassDirection.North;
                case InputKey.Right:
                    return CompassDirection.East;
                case InputKey.Left:
                    return CompassDirection.West;
                case InputKey.Down:
                    return CompassDirection.South;
            }
            return null;
        }
    }

    public enum CardinalDirection
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }

    public enum CompassDirection
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3,
        NorthEast = 4,
        SouthEast = 5,
        SouthWest = 6,
        NorthWest = 7
    }
}
