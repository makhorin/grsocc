using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts
{
    public abstract class Generator : MonoBehaviour
    {
        protected Random _rnd = new Random();
        protected Quaternion _rotation = Quaternion.identity;
        public abstract int MinLevel { get; }
        protected int _pos = 0;
        protected List<List<Vector2>> _positions;
        public virtual IEnumerable<Player> Generate(Player pattern, float[] x, float[] y)
        {
            if (_positions == null)
            {
                _positions = FillPositions(x, y);
               // _positions = _positions.OrderBy(p => _rnd.Next()).ToList();
            }
                

            for (var i = 0; i < _positions[_pos].Count; i++)
                yield return Instantiate(pattern, _positions[_pos][i], _rotation);

            _pos++;
            if (_pos >= _positions.Count)
            {
               // _positions = _positions.OrderBy(p => _rnd.Next()).ToList();
                _pos = 0;
            }
        }

        protected abstract List<List<Vector2>> FillPositions(float[] xs, float[] ys);
    }
}
