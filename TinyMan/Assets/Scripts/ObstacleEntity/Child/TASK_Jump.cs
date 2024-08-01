using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TASK_Jump : ObstacleEntity
{
    public float JumpForce = 2;
    public override void PerformObstacleAction(PlatformerController controller)
    {
        if (controller == null) return;

        controller.JumpWithForce(JumpForce);
    }
}
