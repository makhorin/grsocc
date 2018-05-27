using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerMoveToX : PlayerMoveBase
    {
        protected override float SpeedCoef
        {
            get
            {
                return 1.8f;
            }
        }

        protected override Vector3 MovePos(Vector3 ballPos)
        {
            return new Vector3(ballPos.x, transform.position.y);
        }
    }
}
