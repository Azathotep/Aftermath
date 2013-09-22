namespace Aftermath.Utils
{
    struct RectangleF
    {
        float _x;
        float _y;
        float _width;
        float _height;

        public RectangleF(float x, float y, float width, float height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public bool Contains(Vector2F p)
        {
            if ((p.X > _x) && (p.X < _x + _width) && (p.Y > _y) && (p.Y < _y + _height))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        public float Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        public float Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }
    }  
}
