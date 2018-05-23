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
        private Color _baseHeadColor;
        private Color _baseBodyColor;

        private void Start()
        {
            var matPropBlock = new MaterialPropertyBlock();
            Head.GetPropertyBlock(matPropBlock);
            _baseHeadColor = HeadColors[_rnd.Next(0, HeadColors.Length - 1)];
            matPropBlock.SetColor("_Color", _baseHeadColor);
            Head.SetPropertyBlock(matPropBlock);
        }

        public void MakeRed(bool make)
        {
            if (make)
            {
                _baseBodyColor = Body.material.color;
                var matPropBlock = new MaterialPropertyBlock();
                Body.GetPropertyBlock(matPropBlock);
                matPropBlock.SetColor("_Color", Color.red);
                Body.SetPropertyBlock(matPropBlock);

                matPropBlock = new MaterialPropertyBlock();
                Head.GetPropertyBlock(matPropBlock);
                matPropBlock.SetColor("_Color", Color.red);
                Head.SetPropertyBlock(matPropBlock);
            }
            else
            {
                var matPropBlock = new MaterialPropertyBlock();
                Body.GetPropertyBlock(matPropBlock);
                matPropBlock.SetColor("_Color", _baseBodyColor);
                Body.SetPropertyBlock(matPropBlock);

                matPropBlock = new MaterialPropertyBlock();
                Head.GetPropertyBlock(matPropBlock);
                matPropBlock.SetColor("_Color", _baseHeadColor);
                Head.SetPropertyBlock(matPropBlock);
            }
        }
    }
}
