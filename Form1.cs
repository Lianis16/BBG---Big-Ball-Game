using System.Numerics;

namespace BoalsProject;

public partial class Form1 : Form
{
    internal static Bounds DrawFieldBounds = BoalsProject.Bounds.Empty;
    internal static List<Ball> Balls {
        get{
            if (ActiveForm == null) return new List<Ball>();
            return ((Form1)ActiveForm)._balls;
        }
    }

    private Bitmap _bitmap;
    private Graphics _graphics;
    private List<Ball> _balls;
    private bool isRunning = true;
        
        
    public Form1()
    {
        InitializeComponent();

        Rectangle rectangle = drawField.Bounds;
        DrawFieldBounds = new(new Vector2(rectangle.X, rectangle.Y), rectangle.Height, rectangle.Width);

        _bitmap = new Bitmap(rectangle.Width, rectangle.Height);
        _graphics = Graphics.FromImage(_bitmap);

        _balls = GenerateRandomBalls(8);

        GlobalActions.UpdateAction += () =>
        {
            _graphics.Clear(drawField.BackColor);

            foreach (var ball in _balls)
            {
                ball.Draw(_graphics);
            }

            drawField.Image = _bitmap;
                
            if (_balls.FindAll(b => b.BallType == BallType.REGULAR).Count == 0)
            {
                isRunning = false;
                MessageBox.Show("Simulation is over!", "Simulation is over", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };

        StartUpdate();
    }

    private List<Ball> GenerateRandomBalls(int count)
    {
        Bounds bounds = DrawFieldBounds;
        Random r = new Random();
        List<Ball> balls = new();

        while (balls.Count != count)
        {
            BallType type = r.NextEnum<BallType>();
            int radius = r.Next(25, 100);
            
            Vector2 position = new (r.Next((int)bounds.MinX + radius, (int)bounds.MaxX - radius), 
                r.Next((int)bounds.MinY + radius, (int)bounds.MaxY - radius));
            
            Vector2 velocity = type != BallType.MONSTER ? new Vector2(r.Next(55, 155), r.Next(55, 155)) : new Vector2(0, 0);

            Ball newBall = new(type, radius, position, velocity);

            if (newBall.GetBallsCollide(balls).Count == 0)
            {
                balls.Add(newBall);
            }
        }

        return balls;
    }
        
    private async void StartUpdate()
    {
        var myTime = new MyTime();
        isRunning = true;
        await foreach (var _ in InvokeUpdate()){}
    }

    private async IAsyncEnumerable<object> InvokeUpdate()
    {
        while (isRunning)
        {
            GlobalActions.Update();
            yield return new object();
            await Task.Delay(1);
        }
    }
}
    
public static class RandomExtensions
{   
    public static T NextEnum<T>(this Random random)
        where T : struct, Enum
    {
        var values = Enum.GetValues<T>();

        return values[random.Next(values.Length)];
    }
}