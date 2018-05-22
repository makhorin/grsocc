using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generators
{
    public class FirstMoving : Generator
    {
        public override int MinLevel
        {
            get
            {
                return 4;
            }
        }

        public override IEnumerable<Player> Generate(Player pattern, float[] x, float[] y)
        {
            var generated = base.Generate(pattern, x, y);

            if (_positions == null)
                _positions = FillPositions(x, y);

            var positions = _positions[_pos];

            for (var i = 0; i < _positions[_pos].Count; i++)
            {
                var p = Instantiate(pattern, _positions[_pos][i], _rotation);
                if (_pos == 0 && p.transform.position.y == y[1])
                {
                    var mvmnt = p.gameObject.AddComponent<PlayerMovement>();
                    mvmnt.Init(new Vector2(x[2], y[1]));
                }

                if (_pos == 1 && (p.transform.position.x == x[0] || p.transform.position.x == x[2]))
                {
                    var mvmnt = p.gameObject.AddComponent<PlayerMovement>();
                    mvmnt.Init(new Vector2(x[1], y[1]));
                }

                yield return p;
            }
                
            _pos++;
            if (_pos >= _positions.Count)
                _pos = 0;
        }

        protected override List<List<Vector2>> FillPositions(float[] xs, float[] ys)
        {
            var positions = new List<List<Vector2>>();
            var position = new List<Vector2>();
            position.Add(new Vector2((xs[0] + xs[1]) / 2f, ys[0]));
            position.Add(new Vector2((xs[1] + xs[2]) / 2f, ys[0]));

            position.Add(new Vector2(xs[0], ys[2]));
            position.Add(new Vector2(xs[1], ys[2]));
            position.Add(new Vector2(xs[2], ys[2]));

            position.Add(new Vector2(xs[0], ys[1]));
            position.Add(new Vector2(xs[1], ys[1]));
            positions.Add(position);

            position = new List<Vector2>();
            position.Add(new Vector2(xs[0], ys[2]));
            position.Add(new Vector2(xs[1], ys[2]));
            position.Add(new Vector2(xs[2], ys[2]));

            position.Add(new Vector2(xs[0], ys[0]));
            position.Add(new Vector2(xs[1], ys[0]));
            position.Add(new Vector2(xs[2], ys[0]));

            positions.Add(position);

            return positions;
        }
    }
}
