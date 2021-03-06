﻿using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerMovement : MonoBehaviour
    {
        private Vector3 _originalPos;
        private Vector3 _pointToMove;

        private Vector3 _target;
        float _speed;

        public void Init(Vector3 originalPos, Vector3 pointToMove, float speed)
        {
            _pointToMove = pointToMove;
            _originalPos = originalPos;
            _target = _pointToMove;
            _speed = speed * 0.5f;
        }

        private void Update()
        {
            float step = _speed * Time.deltaTime;
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
