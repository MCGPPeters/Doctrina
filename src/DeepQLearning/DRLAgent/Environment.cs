using System;
using System.Collections.Generic;
using DeepQLearning.ConvnetSharp;

namespace DeepQLearning.DRLAgent
{
    [Serializable]
    public class Environment
    {
        private int Height { get; }
        private readonly Util _util;

        private int Width { get; }
        public List<Agent> Agents { get; }
        private int Clock { get; set; }

        public List<Item> Items { get; private set; }

        public List<Wall> Walls { get; }

        public Environment(Brain brain, int canvasWidth, int canvasHeight)
        {
            Agents = new List<Agent>();
            Width = canvasWidth;
            Height = canvasHeight;

            _util = new Util();
            Clock = 0;

            // set up walls in the world
            Walls = new List<Wall>();
            const int pad = 10;

            AddBox(Walls, pad, pad, Width - pad * 2, Height - pad * 2);
            AddBox(Walls, 100, 100, 200, 300); // inner walls

            Walls.RemoveAt(Walls.Count - 1);
            AddBox(Walls, 400, 100, 200, 300);
            Walls.RemoveAt(Walls.Count - 1);

            // set up food and poison
            Items = new List<Item>();
            for (var k = 0; k < 30; k++)
            {
                var x = _util.RandomDouble(20, Width - 20);
                var y = _util.RandomDouble(20, Height - 20);
                var t = _util.Random.Next(1, 3); // food or poison (1 and 2)
                var it = new Item(x, y, (ObjectType)t);
                Items.Add(it);
            }

            Agents = new List<Agent>
            {
                new Agent(brain)
            };
        }

        private static void AddBox(ICollection<Wall> walls, double x, double y, double w, double h)
        {
            walls.Add(new Wall(new Vector(x, y), new Vector(x + w, y)));
            walls.Add(new Wall(new Vector(x + w, y), new Vector(x + w, y + h)));
            walls.Add(new Wall(new Vector(x + w, y + h), new Vector(x, y + h)));
            walls.Add(new Wall(new Vector(x, y + h), new Vector(x, y)));
        }

        // helper function to get closest colliding walls/items
        private Intersection StuffCollide(Vector p1, Vector p2, bool checkWalls, bool checkItems)
        {
            var minres = new Intersection {Intersect = false};

            // collide with walls
            if (checkWalls)
                for (int i = 0, n = Walls.Count; i < n; i++)
                {
                    var wall = Walls[i];
                    var res = LineIntersection(p1, p2, wall.FirstPoint, wall.SecondPoint);
                    if (res.Intersect)
                    {
                        res.ObjectType = ObjectType.Wall;
                        if (!minres.Intersect)
                        {
                            minres = res;
                        }
                        else
                        {
                            // check if its closer
                            if (res.Ua < minres.Ua)
                                minres = res;
                        }
                    }
                }

            // collide with items
            if (checkItems)
                for (int i = 0, n = Items.Count; i < n; i++)
                {
                    var it = Items[i];
                    var res = LinePointIntersection(p1, p2, it.Position, it.Radius);
                    if (res.Intersect)
                    {
                        res.ObjectType = it.ObjectType; // store type of item
                        if (!minres.Intersect)
                        {
                            minres = res;
                        }
                        else
                        {
                            // check if its closer
                            if (res.Ua < minres.Ua)
                                minres = res;
                        }
                    }
                }

            return minres;
        }

        // line intersection helper function: does line segment (firstPoint,secondPoint) intersect segment (p3,p4) ?
        private static Intersection LineIntersection(Vector p1, Vector p2, Vector p3, Vector p4)
        {
            var result = new Intersection {Intersect = false};

            var denom = (p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y);
            if (denom == 0.0) result.Intersect = false;

            var ua = ((p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X)) / denom;
            var ub = ((p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X - p3.X)) / denom;
            if (ua > 0.0 && ua < 1.0 && ub > 0.0 && ub < 1.0)
            {
                var up = new Vector(p1.X + ua * (p2.X - p1.X), p1.Y + ua * (p2.Y - p1.Y));
                return new Intersection {Ua = ua, Ub = ub, Up = up, Intersect = true}; // up is intersection point
            }
            return result;
        }

        private static Intersection LinePointIntersection(Vector A, Vector B, Vector C, double rad)
        {
            var result = new Intersection {Intersect = false};

            var v = new Vector(B.Y - A.Y, -(B.X - A.X)); // perpendicular vector
            var d = Math.Abs((B.X - A.X) * (A.Y - C.Y) - (A.X - C.X) * (B.Y - A.Y));
            d = d / v.Length();
            if (d > rad) return result;

            v = v.Normalize();
            v = v.Scale(d);
            var ua = 0.0;
            var up = C.Add(v);
            if (Math.Abs(B.X - A.X) > Math.Abs(B.Y - A.Y)) ua = (up.X - A.X) / (B.X - A.X);
            else ua = (up.Y - A.Y) / (B.Y - A.Y);
            if (ua > 0.0 && ua < 1.0) result = new Intersection {Ua = ua, Up = up, Intersect = true};
            return result;
        }

        public void Tick()
        {
            // tick the environment
            Clock++;

            // fix input to all agents based on environment process eyes
            for (int i = 0, n = Agents.Count; i < n; i++)
            {
                var agent = Agents[i];
                for (int ei = 0, ne = agent.Eyes.Count; ei < ne; ei++)
                {
                    var eye = agent.Eyes[ei];
                    // we have a line from p to p->eyep
                    var positionOfEye = new Vector(agent.CurrentPosition.X + eye.MaximumRange * Math.Sin(agent.Angle + eye.Angle),
                        agent.CurrentPosition.Y + eye.MaximumRange * Math.Cos(agent.Angle + eye.Angle));
                    var res = StuffCollide(agent.CurrentPosition, positionOfEye, true, true);

                    if (res.Intersect)
                    {
                        // eye collided with wall
                        eye.SensedProximity = res.Up.DistanceFrom(agent.CurrentPosition);
                        eye.ObjectType = res.ObjectType;
                    }
                    else
                    {
                        eye.SensedProximity = eye.MaximumRange;
                        eye.ObjectType = ObjectType.Nothing;
                    }
                }
            }

            // let the agents behave in the world based on their input
            for (int i = 0, n = Agents.Count; i < n; i++)
                Agents[i].Act();

            // apply outputs of agents on evironment
            for (int i = 0, n = Agents.Count; i < n; i++)
            {
                var agent = Agents[i];
                var previousPositionOfAgent = agent.CurrentPosition;
                agent.PreviousPosition = previousPositionOfAgent; // back up old position
                agent.PreviousAngle = agent.Angle; // and angle

                // steer the agent according to outputs of wheel velocities
                var v = new Vector(0, agent.Radius / 2.0);
                v = v.Rotate(agent.Angle + Math.PI / 2);
                var w1P = previousPositionOfAgent.Add(v); // positions of wheel 1 and 2
                var w2P = previousPositionOfAgent.Subtract(v);
                var vv = previousPositionOfAgent.Subtract(w2P);
                vv = vv.Rotate(-agent.RotationSpeedOfFirstWheel);
                var vv2 = previousPositionOfAgent.Subtract(w1P);
                vv2 = vv2.Rotate(agent.RotationSpeedOfSecondWheel);
                var np = w2P.Add(vv);
                np = np.Scale(0.5);
                var np2 = w1P.Add(vv2);
                np2 = np2.Scale(0.5);
                agent.CurrentPosition = np.Add(np2);

                agent.Angle -= agent.RotationSpeedOfFirstWheel;
                if (agent.Angle < 0) agent.Angle += 2 * Math.PI;
                agent.Angle += agent.RotationSpeedOfSecondWheel;
                if (agent.Angle > 2 * Math.PI) agent.Angle -= 2 * Math.PI;

                // agent is trying to move from p to op. Check walls
                var res = StuffCollide(agent.PreviousPosition, agent.CurrentPosition, true, false);
                if (res.Intersect)
                    agent.CurrentPosition = agent.PreviousPosition;

                // handle boundary conditions
                var agentCurrentPosition = agent.CurrentPosition;
                if (agent.CurrentPosition.X < 0)
                {
                    agentCurrentPosition.X = 0;
                }
                if (agent.CurrentPosition.X > Width)
                {
                    agentCurrentPosition.X = Width;
                }
                if (agent.CurrentPosition.Y < 0)
                {
                    agentCurrentPosition.Y = 0;
                }
                if (agent.CurrentPosition.Y > Height)
                {
                    agentCurrentPosition.Y = Height;
                }
            }

            // tick all items
            var updateItems = false;
            for (int i = 0, n = Items.Count; i < n; i++)
            {
                var it = Items[i];
                it.Age += 1;

                // see if some agent gets lunch
                for (int j = 0, m = Agents.Count; j < m; j++)
                {
                    var a = Agents[j];
                    var d = a.CurrentPosition.DistanceFrom(it.Position);
                    if (d < it.Radius + a.Radius)
                    {
                        // wait lets just make sure that this isn't through a wall
                        var rescheck = StuffCollide(a.CurrentPosition, it.Position, true, false);
                        if (!rescheck.Intersect)
                        {
                            // ding! nom nom nom
                            switch (it.ObjectType)
                            {
                                case ObjectType.Food:
                                    a.DigestionSignal += 5.0; // mmm
                                    break;
                                case ObjectType.Poison:
                                    a.DigestionSignal += -6.0; // ewww poison
                                    break;
                                case ObjectType.Nothing:
                                    break;
                                case ObjectType.Wall:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            it.Cleanup = true;
                            updateItems = true;
                            break; // break out of loop, item was consumed
                        }
                    }
                }

                if (it.Age > 5000 && Clock % 100 == 0 && _util.RandomDouble(0, 1) < 0.1)
                {
                    it.Cleanup = true; // replace this one, has been around too long
                    updateItems = true;
                }
            }
            if (updateItems)
            {
                var nt = new List<Item>();
                for (int i = 0, n = Items.Count; i < n; i++)
                {
                    var it = Items[i];
                    if (!it.Cleanup) nt.Add(it);
                }
                Items = nt; // swap
            }
            if (Items.Count < 30 && Clock % 10 == 0 && _util.RandomDouble(0, 1) < 0.25)
            {
                var newitx = _util.RandomDouble(20, Width - 20);
                var newity = _util.RandomDouble(20, Height - 20);
                var newitt = _util.Random.Next(1, 3); // food or poison (1 and 2)
                var newit = new Item(newitx, newity, (ObjectType)newitt);
                Items.Add(newit);
            }

            // agents are given the opportunity to learn based on feedback of their action on environment
            for (int i = 0, n = Agents.Count; i < n; i++)
                Agents[i].EvaluateAction();
        }
    }
}