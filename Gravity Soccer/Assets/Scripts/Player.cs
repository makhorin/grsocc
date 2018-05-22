using UnityEngine;
using Random = System.Random;
namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public Renderer Head;
        public Renderer Body;
        public Color[] HeadColors;
        private static Random _rnd = new Random();
        
        private void Start()
        {
            var matPropBlock = new MaterialPropertyBlock();
            Head.GetPropertyBlock(matPropBlock);
            matPropBlock.SetColor("_Color", HeadColors[_rnd.Next(0, HeadColors.Length - 1)]);
            Head.SetPropertyBlock(matPropBlock);
        }
    }
}
