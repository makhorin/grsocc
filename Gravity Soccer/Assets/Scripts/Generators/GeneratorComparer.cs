using System.Collections.Generic;

namespace Assets.Scripts.Generators
{
    public class GeneratorComparer : IComparer<Generator>
    {
        public int Compare(Generator x, Generator y)
        {
            return x.MinLevel.CompareTo(y.MinLevel);
        }
    }
}
