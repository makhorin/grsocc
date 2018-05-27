using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerMoveXY : PlayerMoveBase
    {
        protected override float SpeedCoef
        {
            get
            {
                return 1f;
            }
        }

        protected override Vector3 MovePos(Vector3 ballPos)
        {
            return new Vector3(ballPos.x, ballPos.y);
        }
    }
}
