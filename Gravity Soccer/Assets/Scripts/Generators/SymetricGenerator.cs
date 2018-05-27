using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Generators
{
    class SymetricGenerator : Generator
    {
        public override int MinLevel
        {
            get
            {
                return 0;
            }
        }

        public float Dimension = 0f;
        private float _yDimension = 1f;
        protected System.Random _rnd = new System.Random();

        public override IEnumerable<Player> Generate(Player pattern, int level, float dimensioCoef)
        {
            var movingX = Interpolate(level, _levels, _movingX);
            var movingXY = Interpolate(level, _levels, _movingXY);
            var movingBF = Interpolate(level, _levels, _movingBackForth);
            var speed = Interpolate(level, _levels, _speed);
            var maxPlayers = Interpolate(level, _levels, _players);

            var xl = Dimension;
            var yl = dimensioCoef;

            maxPlayers = (int)Math.Ceiling(maxPlayers * yl);
            var yOffset = yl / maxPlayers;

            var list = new List<Player>();
            for (var i = 0; i < maxPlayers; i++)
            {
                var startX = -Dimension;
                var xPos = UnityEngine.Random.Range(startX, startX + (xl * 0.9f));

                var startY = -_yDimension + (yOffset * i);
                var yPos = UnityEngine.Random.Range(startY, startY + (yOffset * 0.8f));

                var pos = new Vector2(xPos, yPos);

                list.Add(Generate(pattern,pos));
                list.Add(Generate(pattern, new Vector2(-pos.x, pos.y)));
            }

            var indexes = new List<int>();
            var k = 0;
            indexes.AddRange(list.Select(l => k++));
            while (movingX > 0 && indexes.Count > 0)
            {
                var playerIndex = indexes[_rnd.Next(0, indexes.Count)];

                var player = list[playerIndex];
                player.gameObject.AddComponent<PlayerMoveToX>().Init(speed);
                indexes.Remove(playerIndex);
                movingX--;
            }

            while (movingXY > 0 && indexes.Count > 0)
            {
                var playerIndex = indexes[_rnd.Next(0, indexes.Count)];
                var player = list[playerIndex];
                player.gameObject.AddComponent<PlayerMoveXY>().Init(speed);
                indexes.Remove(playerIndex);
                movingXY--;
            }

            while (movingBF > 0 && indexes.Count > 0)
            {
                var playerIndex = indexes[_rnd.Next(0, indexes.Count)];
                var player = list[playerIndex];
                var origPos = new Vector2(player.transform.position.x - 0.2f, player.transform.position.y);
                var targetPos = new Vector2(player.transform.position.x + 0.2f, player.transform.position.y);
                player.gameObject.AddComponent<PlayerMovement>().Init(origPos, targetPos,speed);
                indexes.Remove(playerIndex);
                movingBF--;
            }

            return list;
        }

        private int[] _levels = new[] { 1, 2, 5, 10, 50 };
        private int[] _players = new[] { 1, 1, 1, 2, 3 };
        private int[] _movingX = new[] { 0, 1, 1, 3, 4 };
        private int[] _movingXY = new[] { 0, 0, 1, 2, 4 };
        private int[] _movingBackForth = new[] { 2, 2, 2, 4, 6 };
        private float[] _speed = new[] { 1f, 1f, 1f, 1.2f, 2f };

        private int Interpolate(int x, int[] xd, int[] yd)
        {
            int i;
            for (i = 1; i < _levels.Length; i++)
            {
                if (_levels[i] >= x)
                {
                    if (_levels[i] == x)
                        return yd[i];
                    return (int)Math.Floor(Interpolate(xd[i - 1], yd[i - 1], xd[i], yd[i], x));
                }   
            }

            return yd[i - 1];
        }

        private float Interpolate(int x, int[] xd, float[] yd)
        {
            int i;
            for (i = 1; i < _levels.Length; i++)
            {
                if (_levels[i] >= x)
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
