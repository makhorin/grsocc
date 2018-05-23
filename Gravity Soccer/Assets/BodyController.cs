using Assets.Scripts;
using UnityEngine;

public class BodyController : MonoBehaviour {

    private float _timeForRed;
    private bool _isRed;
    public Player Player;
    
    void Update ()
    {
        if (_timeForRed > 0f)
        {
            _timeForRed -= Time.deltaTime;
            if (_isRed)
                return;
            Player.MakeRed(true);
            _isRed = true;
        }
        else
        {
            if (!_isRed)
                return;
            Player.MakeRed(false);
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
