using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generators
{
    public class MovingGenerator : FirstGenerator
    {
        public Vector2 GoalKeeper;
        public float Distance;
        public float MinSpeed;
        public float MaxSpeed;
        public int MinMoving;
        public int MaxMoving;
        public float GoalKeeperChance;

        public override int MinLevel
        {
            get
            {
                return Level;
            }
        }

        public override IEnumerable<Player> Generate(Player pattern)
        {
            var result = new List<Vector2>();
            var half = MaxPlayers / 2f;

            var xl = XDimension;
            var yl = YDimesion * 2;
            var s = xl * yl;
            var ps = s / half;
            var xOffset = ps / yl;

            var moving = _rnd.Next(MinMoving, MaxMoving);
            var speed = MinSpeed + ((MaxSpeed - MinSpeed) * _rnd.NextDouble());

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

                if ((MaxPlayers - (i * 2)) <= moving)
                {
                    var pos1 = result[result.Count - 1];
                    var pos2 = result[result.Count - 2];

                    var player = Generate(pattern, pos1);
                    var pToMove = startX;
                    if ((pos1.x - startX) < xOffset / 2f)
                        pToMove = startX + xOffset;
                    player.gameObject.AddComponent<PlayerMovement>().Init(new Vector2(pToMove, pos1.y), (float)speed);
                    yield return player;

                    player = Generate(pattern, pos2);
                    player.gameObject.AddComponent<PlayerMovement>().Init(new Vector2(-pToMove, pos1.y), (float)speed);
                    yield return player;
                }
                else
                {
                    yield return Generate(pattern, result[result.Count - 1]);
                    yield return Generate(pattern, result[result.Count - 2]);
                }
            }


            if (GoalKeeperChance < _rnd.NextDouble())
                yield break;
            speed = MinSpeed + ((MaxSpeed - MinSpeed) * _rnd.NextDouble());
            var goalKeeper = Generate(pattern, GoalKeeper);
            goalKeeper.gameObject.AddComponent<PlayerMovement>().Init(new Vector2(GoalKeeper.x - Distance / 2f, GoalKeeper.y), new Vector2(GoalKeeper.x + Distance / 2f, GoalKeeper.y), (float)speed);
            yield return goalKeeper;
        }
    }
}
