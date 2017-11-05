using System;
using System.Drawing;

namespace DeepQLearning.DRLAgent
{
    // item is circle thing on the floor that agent can interact with (see or eat, etc)

    // A single agent

    [Serializable]
    public class QAgent
    {
        private readonly Environment _environment;
        [NonSerialized] private Pen _blackPen = new Pen(Color.Black);

        [NonSerialized] private Pen _greenPen = new Pen(Color.LightGreen, 2);

        [NonSerialized] private Pen _greenPen2 = new Pen(Color.LightGreen, 1);

        [NonSerialized] private Pen _redPen = new Pen(Color.Red, 2);

        [NonSerialized] private Pen _redPen2 = new Pen(Color.Red, 1);

        public int SimulationSpeed { get; set; } = 1;

        public QAgent(Environment environment) =>
            _environment = environment;

        public void Reinitialize()
        {
            _greenPen = new Pen(Color.LightGreen, 2);
            _redPen = new Pen(Color.Red, 2);
            _greenPen2 = new Pen(Color.LightGreen, 1);
            _redPen2 = new Pen(Color.Red, 1);
            _blackPen = new Pen(Color.Black);

            SimulationSpeed = 1;
            _environment.Agents[0].Brain.Learning = false;
            _environment.Agents[0].Brain.EpsilonTestTime = 0.01;

            var previousPosition = _environment.Agents[0].PreviousPosition;
            previousPosition.X = 500;
            previousPosition.Y = 500;
        }

        public void Tick()
        {
            _environment.Tick();
        }

        // Draw everything and return stats
        public string DrawWorld(Graphics graphics)
        {
            var agents = _environment.Agents;

            // draw walls in environment
            for (int i = 0, n = _environment.Walls.Count; i < n; i++)
            {
                var q = _environment.Walls[i];
                DrawLine(graphics, q.FirstPoint, q.SecondPoint, _blackPen);
            }

            // draw agents
            for (int i = 0, n = agents.Count; i < n; i++)
            {
                // draw agent's body
                var a = agents[i];
                DrawArc(graphics, a.PreviousPosition, (int) a.Radius, 0, (float) (Math.PI * 2), _blackPen);

                // draw agent's sight
                for (int ei = 0, ne = a.Eyes.Count; ei < ne; ei++)
                {
                    var e = a.Eyes[ei];
                    var sr = e.SensedProximity;
                    Pen pen;

                    switch (e.ObjectType)
                    {
                        case ObjectType.Food:
                            pen = _redPen2; // apples
                            break;
                        case ObjectType.Poison:
                            pen = _greenPen2; // poison
                            break;
                        case ObjectType.Nothing:
                            pen = _blackPen; // wall
                            break;
                        default:
                            pen = _blackPen; // wall
                            break;
                    }

                    var newX = a.PreviousPosition.X + sr * Math.Sin(a.PreviousAngle + e.Angle);
                    var newY = a.PreviousPosition.Y + sr * Math.Cos(a.PreviousAngle + e.Angle);
                    var b = new Vector(newX, newY);

                    DrawLine(graphics, a.PreviousPosition, b, pen);
                }
            }

            // draw items
            for (int i = 0, n = _environment.Items.Count; i < n; i++)
            {
                var pen = _blackPen;
                var it = _environment.Items[i];
                switch (it.ObjectType)
                {
                    case ObjectType.Food:
                        pen = _redPen;
                        break;
                    case ObjectType.Poison:
                        pen = _greenPen;
                        break;
                    case ObjectType.Nothing:
                        break;
                    case ObjectType.Wall:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                DrawArc(graphics, it.Position, (int) it.Radius, 0, (float) (Math.PI * 2), pen);
            }

            return _environment.Agents[0].Brain.VisSelf();
        }

        public void StartLearning()
        {
            _environment.Agents[0].Brain.Learning = true;
        }

        public void StopLearning()
        {
            _environment.Agents[0].Brain.Learning = false;
        }

        private static void DrawArc(Graphics graphics, Vector center, int radius, float startAngle, float sweepAngle, Pen pen)
        {
            var rect = new Rectangle((int) center.X - radius, (int) center.Y - radius, radius * 2, radius * 2);
            var startAngleDegree = RadiusToDegree(startAngle);
            var sweepAngleDegree = RadiusToDegree(sweepAngle);
            graphics.DrawArc(pen, rect, startAngleDegree, sweepAngleDegree);
        }

        private static void DrawLine(Graphics graphics, Vector a, Vector b, Pen pen)
        {
            Point[] points =
            {
                new Point((int) a.X, (int) a.Y),
                new Point((int) b.X, (int) b.Y)
            };

            graphics.DrawLines(pen, points);
        }

        private static float RadiusToDegree(float radius) => (float) (radius * 180 / Math.PI);
    }
}