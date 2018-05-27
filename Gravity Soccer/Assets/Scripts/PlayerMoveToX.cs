﻿using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerMoveToX : PlayerMoveBase
    {
        protected override float SpeedCoef
        {
            get
            {
                return 2f;
            }
        }

        protected override Vector3 MovePos(Vector3 ballPos)
        {
            return new Vector3(ballPos.x, transform.position.y);
        }
    }
}
