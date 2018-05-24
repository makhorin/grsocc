using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts
{
    public abstract class Generator : MonoBehaviour
    {
        protected Quaternion _rotation = Quaternion.identity;
        public abstract int MinLevel { get; }

        public abstract IEnumerable<Player> Generate(Player pattern, int level);

        protected Player Generate(Player pattern, Vector2 pos)
        {
            return Instantiate(pattern, pos, _rotation);
        }
    }
}
