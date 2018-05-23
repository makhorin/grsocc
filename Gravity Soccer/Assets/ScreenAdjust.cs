using UnityEngine;

public class ScreenAdjust : MonoBehaviour {

    private float _orthographicSize = 2;
    private float _aspect = 0.5625f;
    void Start()
    {
        Camera.main.projectionMatrix = Matrix4x4.Ortho(
                -_orthographicSize * _aspect, _orthographicSize * _aspect,
                -_orthographicSize, _orthographicSize,
                Camera.main.nearClipPlane, Camera.main.farClipPlane);
    }

}
