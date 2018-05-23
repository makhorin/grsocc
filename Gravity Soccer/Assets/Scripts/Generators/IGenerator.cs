using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts
{
    public abstract class Generator : MonoBehaviour
    {
        protected Quaternion _rotation = Quaternion.identity;
        public abstract int MinLevel { get; }
        
        public virtual IEnumerable<Player> Generate(Player pattern)
        {                

            foreach (var pos in GetNext())
                yield return Generate(pattern, pos);
        }

        protected Player Generate(Player pattern, Vector2 pos)
        {
            return Instantiate(pattern, pos, _rotation);
        }

        protected abstract List<Vector2> GetNext();
    }
}
