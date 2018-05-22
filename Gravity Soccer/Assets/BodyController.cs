using UnityEngine;

public class BodyController : MonoBehaviour {

    public Renderer Body;
    private float _timeForRed;
    private bool _isRed;
    private Color _baseColor;

    void Start () {
        Body = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (_timeForRed > 0f)
        {
            _timeForRed -= Time.deltaTime;
            if (_isRed)
                return;
            _baseColor = Body.material.color;
            var matPropBlock = new MaterialPropertyBlock();
            Body.GetPropertyBlock(matPropBlock);
            matPropBlock.SetColor("_Color", Color.red);
            Body.SetPropertyBlock(matPropBlock);
            _isRed = true;
        }
        else
        {
            if (!_isRed)
                return;
            var matPropBlock = new MaterialPropertyBlock();
            Body.GetPropertyBlock(matPropBlock);
            matPropBlock.SetColor("_Color", _baseColor);
            Body.SetPropertyBlock(matPropBlock);
            _isRed = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ball":
                _timeForRed = 0.2f;
                break;
        }
    }
}
