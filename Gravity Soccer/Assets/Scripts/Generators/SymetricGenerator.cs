﻿using System;
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
                var yPos = UnityEngine.Random.Range(startY, startY + (yOffset * 0.3f));

                var pos = new Vector2(xPos, yPos);

                list.Add(Generate(pattern,pos));
                list.Add(Generate(pattern, new Vector2(-pos.x, pos.y)));
            }

            var indexes = new List<int>();
            var k = 0;
            indexes.AddRange(list.Select(l => k++));
            var mx = Math.Floor(list.Count * movingX);
            while (mx > 0 && indexes.Count > 0)
            {
                var playerIndex = indexes[_rnd.Next(0, indexes.Count)];

                var player = list[playerIndex];
                player.gameObject.AddComponent<PlayerMoveToX>().Init(speed);
                indexes.Remove(playerIndex);
                mx--;
            }

            var mxy = Math.Floor(list.Count * movingXY);
            while (mxy > 0 && indexes.Count > 0)
            {
                var playerIndex = indexes[_rnd.Next(0, indexes.Count)];
                var player = list[playerIndex];
                player.gameObject.AddComponent<PlayerMoveXY>().Init(speed);
                indexes.Remove(playerIndex);
                mxy--;
            }

            var mbf = Math.Floor(list.Count * movingBF);
            while (mbf > 0 && indexes.Count > 0)
            {
                var playerIndex = indexes[_rnd.Next(0, indexes.Count)];
                var player = list[playerIndex];
                var origPos = new Vector2(player.transform.position.x - 0.2f, player.transform.position.y);
                var targetPos = new Vector2(player.transform.position.x + 0.2f, player.transform.position.y);
                player.gameObject.AddComponent<PlayerMovement>().Init(origPos, targetPos,speed);
                indexes.Remove(playerIndex);
                mbf--;
            }

            return list;
        }

        private int[] _levels = new[] { 1, 2, 5, 8, 11, 20, 50 };
        private int[] _players = new[] { 1, 1, 1, 1, 1, 1, 2 };
        private float[] _movingX = new[] { 0, 0.2f, 0.2f, 0.3f, 0.1f, 0.3f, 0.3f };
        private float[] _movingXY = new[] { 0, 0, 0.1f, 0.1f, 0f, 0.2f, 0.3f };
        private float[] _movingBackForth = new[] { 0, 0f, 0.2f, 0.2f, 0.1f, 0.2f, 0.3f };
        private float[] _speed = new[] { 0.3f, 0.5f, 0.4f, 0.5f, 0.4f, 0.7f, 1f };

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
