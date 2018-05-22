using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerMovement : MonoBehaviour
    {
        private Vector3 _originalPos;
        private Vector3 _pointToMove;

        private Vector3 _target;

        public void Init(Vector3 pointToMove)
        {
            _pointToMove = pointToMove;
            _target = _pointToMove;
        }

        private void Awake()
        {
            _originalPos = transform.position;
        }

        private void Update()
        {
            float step = 1f * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _target, step);
            if (Vector3.Distance(transform.position, _target) < 0.01f)
            {
                if (_target == _pointToMove)
                    _target = _originalPos;
                else
                    _target = _pointToMove;
            }
        }
    }
}
