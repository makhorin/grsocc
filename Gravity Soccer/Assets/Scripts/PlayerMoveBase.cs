using UnityEngine;

namespace Assets.Scripts
{
    public abstract class PlayerMoveBase : MonoBehaviour
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
                transform.position = Vector3.MoveTowards(transform.position, MovePos(_ball.transform.position), Time.deltaTime * (_speed * SpeedCoef));
            _ballY = _ball.transform.position.y;
        }

        protected abstract Vector3 MovePos(Vector3 ballPos);

        protected abstract float SpeedCoef { get; }

        public void Reset()
        {
            transform.position = _startPosition;
        }
    }
}
