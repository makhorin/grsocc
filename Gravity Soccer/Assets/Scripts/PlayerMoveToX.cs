using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerMoveToX : MonoBehaviour
    {
        BallController _ball;
        float _speed;
        Vector3 _startPosition;

        private void Start()
        {
            _startPosition = transform.position;
        }

        public void Init(float speed)
        {
            _speed = speed;
        }

        public void Init(BallController ball)
        {
            _ball = ball;
            _ballY = _ball.transform.position.y;
        }

        private float _ballY;
        private void Update()
        {
            if (_ball == null)
                return;
            if (_ball.transform.position.y < transform.position.y &&
                _ballY < _ball.transform.position.y)
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(_ball.transform.position.x, transform.position.y), Time.deltaTime * _speed);
            else
                transform.position = Vector3.MoveTowards(transform.position, _startPosition, Time.deltaTime * _speed);
            _ballY = _ball.transform.position.y;
        }
    }
}
