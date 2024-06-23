
using System.Numerics;

namespace BoalsProject
{
    internal class Bounds
    {
        public static Bounds Empty => new(new Vector2(0, 0), 1, 1);

        private Vector2 _position;
        private float _height;
        private float _width;

        public float MinX => _position.X;
        public float MaxX => _position.X + _width;
        public float MinY => _position.Y;
        public float MaxY => _position.Y + _height;
        public Vector2 center => new(_position.X + _width / 2, _position.Y + _height / 2);
        
        public Bounds(Vector2 position, float height, float width)
        {
            _position = position;
            _height = height;
            _width = width;
        }

        public void UpdatePosition(Vector2 position)
        {
            _position = position;
        }

        public void UpdateSize(float width, float height)
        {
            _height = height;
            _width = width;
        }
    }
}
