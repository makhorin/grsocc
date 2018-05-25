using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerMoveToX : MonoBehaviour
    {
        BallController _ball;
        private void Start()
        {
                        
        }

        public void Init(BallController ball)
        {
            _ball = ball;
        }

        private void Update()
        {
            if (_ball == null)
                return;
            if (_ball.transform.position.y > transform.position.y)
                return;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(_ball.transform.position.x, transform.position.y), Time.deltaTime);
        }
    }
}
