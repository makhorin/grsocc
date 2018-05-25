using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Generators
{
    public class AdjustGenerator : Generator
    {
        public override int MinLevel
        {
            get
            {
                return 0;
            }
        }

        public float Dimension;
        protected Random _rnd = new Random();

        public override IEnumerable<Player> Generate(Player pattern, int level)
        {
            var moving = Interpolate(level, _levels, _moving);
            var speed = Interpolate(level, _levels, _speed);
            var maxPlayers = Interpolate(level, _levels, _players);
            var movingOffset = Interpolate(level, _levels, _offset);
            var minOffset = 0.1f;
            var xl = Dimension * 2;

            var sc = maxPlayers + (maxPlayers % 2);

            var half = sc / 2f;
            var a = xl / half;

            var centers = new List<Vector2>();

            for (var i = 0; i < half; i++)
                for (var j = 0; j < half; j++)
                    centers.Add(new Vector2(a * i + a / 2f, a * j + a / 2f));

            for (var i = 0; i < maxPlayers; i++)
            {

                var j = _rnd.Next(0, centers.Count);
                var center = centers[j];
                centers.RemoveAt(j);
                var yPos = -Dimension + center.y - a / 4f;
                var xPos = -Dimension + center.x - a / 4f;
                xPos += (float)_rnd.NextDouble() * a / 2f;
                yPos += (float)_rnd.NextDouble() * a / 2f;

                var pos = new Vector2(xPos, yPos);

                var player = Generate(pattern, pos);
                if (moving > 0)
                {
                    moving--;
                    var xOffset = (float)_rnd.NextDouble() * movingOffset + minOffset;
                    var start = new Vector2(pos.x + xOffset, pos.y );
                    var finish = new Vector2(pos.x - xOffset, pos.y);
                    var movement = player.gameObject.AddComponent<PlayerMovement>();
                    movement.Init(start, finish, speed);
                }

                yield return player;
            }
        }

        private int[] _levels = new[] { 0, 2, 4, 10, 50 };
        private int[] _players = new[] { 2, 4, 6, 8, 10 };
        private int[] _moving = new[] { 0, 2, 2, 4, 6 };
        private float[] _speed = new[] { 0f, 0.5f, 1f, 0.5f, 1f };
        private float[] _offset = new[] { 0f, 0.2f, 0.3f, 0.5f, 0.5f };

        private int Interpolate(int x, int[] xd, int[] yd)
        {
            int i;
            for (i = 1; i < _levels.Length; i++)
            {
                if (_levels[i] > x)
                    return (int)Math.Ceiling(Interpolate(xd[i - 1], yd[i - 1], xd[i], yd[i], x));
            }

            return yd[i - 1];
        }

        private float Interpolate(int x, int[] xd, float[] yd)
        {
            int i;
            for (i = 1; i < _levels.Length; i++)
            {
                if (_levels[i] > x)
                    return (float)Interpolate(xd[i - 1], yd[i - 1], xd[i], yd[i], x);
            }

            return yd[i - 1];
        }


        static double Interpolate(double x0, double y0, double x1, double y1, double x)
        {
            return y0 * (x - x1) / (x0 - x1) + y1 * (x - x0) / (x1 - x0);
        }
    }
}
