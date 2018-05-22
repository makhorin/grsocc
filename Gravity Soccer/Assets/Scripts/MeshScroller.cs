using UnityEngine;

public class MeshScroller : MonoBehaviour
{
    private Renderer _renderer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }
    void Update()
    {
        _renderer.material.mainTextureOffset = new Vector2(0, _renderer.material.mainTextureOffset.y - 1f);
    }
}
