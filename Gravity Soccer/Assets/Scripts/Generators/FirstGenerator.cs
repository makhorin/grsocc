using System;
using System.Collections.Generic;
using UnityEngine;

using Random = System.Random;
namespace Assets.Scripts.Generators
{
    public class FirstGenerator : Generator
    {
        public override int MinLevel
        {
            get
            {
                return Level;
            }
        }

        public int Level;
        public int MaxPlayers;
        public float XDimension;
        public float YDimesion;

        protected Random _rnd = new Random();

        protected override List<Vector2> GetNext()
        {
            var result = new List<Vector2>();

            if (MaxPlayers < 4)
            {
                GenerateOnX(result);
            }
            else
            {
                switch (_rnd.Next(0, 1))
                {
                    case 0:
                        GenerateOnX(result);
                        break;
                    case 1:
                        GenerateOnY(result);
                        break;
                }
            }

            return result;
        }

        void GenerateOnX(List<Vector2> result)
        {
            var half = MaxPlayers / 2f;
            var xl = XDimension;
            var yl = YDimesion * 2;
            var s = xl * yl;
            var ps = s / half;
            var xOffset = ps / yl;

            for (var i = 0; i < half; i++)
            {
                var startX = -XDimension + (xOffset * i);
                var xPos = startX + _rnd.NextDouble() * (xOffset * 0.8);

                var yPos = -YDimesion + _rnd.NextDouble() * yl;

                var pos = new Vector2((float)xPos, (float)yPos);

                if (result.Count > 0)
                {
                    var len = Math.Sqrt(Math.Pow(pos.x - result[i - 1].x, 2) + Math.Pow(pos.y - result[i - 1].y, 2)) - 0.6f;
                    if (len < 0f)
                        pos = new Vector2(pos.x, pos.y + 0.6f);
                }

                result.Add(pos);
                result.Add(new Vector2(-pos.x, pos.y));
            }
        }

        void GenerateOnY(List<Vector2> result)
        {
            var half = MaxPlayers / 2f;
            var xl = XDimension * 2;
            var yl = YDimesion;
            var s = xl * yl;
            var ps = s / half;
            var yOffset = ps / xl;

            for (var i = 0; i < half; i++)
            {
                var startY = -YDimesion + (yOffset * i);
                var yPos = startY + _rnd.NextDouble() * (yOffset * 0.8);

                var xPos = -XDimension + _rnd.NextDouble() * xl;

                var pos = new Vector2((float)xPos, (float)yPos);

                if (result.Count > 0)
                {
                    var len = Math.Sqrt(Math.Pow(pos.x - result[i - 1].x, 2) + Math.Pow(pos.y - result[i - 1].y, 2)) - 0.6f;
                    if (len < 0f)
                        pos = new Vector2(pos.x + 0.3f, pos.y);
                }

                result.Add(pos);
                result.Add(new Vector2(pos.x, -pos.y));
            }
        }
    }
}
