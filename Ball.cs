using System.Numerics;

namespace BoalsProject
{
    internal enum BallType
    {
        REGULAR,
        MONSTER,
        REPELENT
    }

    internal class Ball
    {
        private static readonly Random R = new();

        private BallType _type;
        private float _radius;
        private Vector2 _position;
        private Color _color;
        private Brush _brush;
        private Pen _pen;
        private Vector2 _velocity;
        private readonly Bounds _bounds;

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                _brush = new SolidBrush(_color);
            }
        }
        public Vector2 Velocity
        {
            get => _velocity;
            private set => _velocity = value;
        }
        public Vector2 Position
        {
            get => _position;
            private set
            {
                _position = value;
                _bounds.UpdatePosition(_position);
            }
        }
        public float Radius
        {
            get => _radius;
            private set
            {
                _radius = value;
                _bounds.UpdateSize(value, value);
            }
        }
        public BallType BallType => _type;

        public Ball(BallType type, float radius, Vector2 position, Vector2 velocity)
        {
            _type = type;
            _radius = radius;
            _position = position;
            _velocity = velocity;
            _color = GetRandomColor();
            _brush = new SolidBrush(_color);
            _bounds = new Bounds(_position, radius, radius);

            if (_type == BallType.REGULAR) _pen = Pens.Aqua;
            else if (_type == BallType.MONSTER) _pen = Pens.Red;
            else _pen = Pens.Gold;

            GlobalActions.UpdateAction += CheckBallsCollide;
            if (_type != BallType.MONSTER)
            {
                GlobalActions.UpdateAction += Move;
            }
        }

        ~Ball() => Delete();

        private void CheckBallsCollide()
        {
            foreach (Ball other in GetBallsCollide(Form1.Balls))
            {
                if (_type == BallType.REGULAR && other._type == BallType.REGULAR)// 1
                {
                    if (Radius >= other.Radius)
                    {
                        Radius += other.Radius;
                        Color = CombineColors(_color, Radius, other._color, other.Radius);
                        other.Delete();
                    }
                }
                else if (_type == BallType.REGULAR && other._type == BallType.MONSTER)// 2
                {
                    other.Radius += Radius;
                    Delete();
                }
                else if (_type == BallType.REGULAR && other._type == BallType.REPELENT)// 3
                {
                    other.Color = _color;
                    Vector2 direction = _position - other._position;
                    direction = Vector2.Normalize(direction);
    
                    float speed = _velocity.Length();
                    _velocity = direction * speed;
                }
                else if (_type == BallType.REPELENT && other._type == BallType.REPELENT)// 4
                {
                    var tempColor = other._color;
                    other.Color = _color;
                    _color = tempColor;
                }
                else if (_type == BallType.REPELENT && other._type == BallType.MONSTER)// 5
                {
                    Radius /= 2;
                }
            }
        }

        private Color CombineColors(Color color1, float radius1, Color color2, float radius2)
        {
            float totalRadius = radius1 + radius2;
            int combinedR = (int)((color1.R * radius1 + color2.R * radius2) / totalRadius);
            int combinedG = (int)((color1.G * radius1 + color2.G * radius2) / totalRadius);
            int combinedB = (int)((color1.B * radius1 + color2.B * radius2) / totalRadius);

            return Color.FromArgb(combinedR, combinedG, combinedB);
        }

        private void Delete()
        {
            GlobalActions.UpdateAction -= CheckBallsCollide;
            GlobalActions.UpdateAction -= Move;
            Form1.Balls.Remove(this);
            _position = new(99999, 99999);
        }

        private void Move()
        {
            Bounds formBounds = Form1.DrawFieldBounds;

            if (_bounds.MaxX > formBounds.MaxX) _velocity.X = -MathF.Abs(_velocity.X);
            else if (_bounds.MinX < formBounds.MinX) _velocity.X = MathF.Abs(_velocity.X);
            if (_bounds.MaxY > formBounds.MaxY) _velocity.Y = -MathF.Abs(_velocity.Y);
            else if (_bounds.MinY < formBounds.MinY) _velocity.Y = MathF.Abs(_velocity.Y);

            Position += _velocity.Multiply(MyTime.DeltaTime);
        }

        public List<Ball> GetBallsCollide(List<Ball> otherBalls)
        {
            return otherBalls.FindAll(other => this != other && IsColliding(other));
        }

        private bool IsColliding(Ball other)
        {
            return Vector2.Distance(_bounds.center, other._bounds.center) < MathF.Abs((Radius + other.Radius) / 2);
        }

        public void Draw(Graphics g)
        {
            g.FillEllipse(_brush, _position.X, _position.Y, _radius, _radius);
            g.DrawEllipse(_pen, _position.X, _position.Y, _radius, _radius);
        }

        private Color GetRandomColor()
        {
            return Color.FromArgb(R.Next(0, 255), R.Next(0, 255), R.Next(0, 255));
        }
    }
}