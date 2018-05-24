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
            var moving = 2;
            var speed = 1;
            var maxPlayers = 10;

            var result = new List<Vector2>();
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
                    var start = new Vector2(pos.x, pos.y);
                    var finish = new Vector2(pos.x, pos.y);
                    var movement = player.gameObject.AddComponent<PlayerMovement>();
                    movement.Init(start, finish, speed);
                }

                yield return player;
            }
        }
    }
}
