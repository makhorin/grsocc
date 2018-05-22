using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generators
{
    class FirstGenerator : Generator
    {
        
        public override int MinLevel
        {
            get
            {
                return 0;
            }
        }

        

        protected override List<List<Vector2>> FillPositions(float[] xs, float[] ys)
        {
            var positions = new List<List<Vector2>>();
            var position = new List<Vector2>();
            position.Add(new Vector2(xs[0], ys[0]));
            position.Add(new Vector2(xs[0], ys[2]));
            position.Add(new Vector2(xs[1], ys[1]));
            position.Add(new Vector2(xs[2], ys[0]));
            position.Add(new Vector2(xs[2], ys[2]));
            positions.Add(position);

            position = new List<Vector2>();
            position.Add(new Vector2((xs[1] + xs[2]) / 2f, ys[0]));
            position.Add(new Vector2(xs[0], ys[1]));
            position.Add(new Vector2(xs[1], ys[1]));
            position.Add(new Vector2((xs[0] + xs[1]) / 2f, ys[2]));
            position.Add(new Vector2(xs[2], ys[1]));
            positions.Add(position);

            return positions;
        }
    }
}
