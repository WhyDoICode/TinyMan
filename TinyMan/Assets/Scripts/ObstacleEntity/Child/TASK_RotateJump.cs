using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateJump : ObstacleEntity
{
    public float JumpHeight = 5;
    public override void PerformObstacleAction(PlatformerController controller)
    {
        if (controller == null) return;
            
        controller.RotateDirectionAndJump(JumpHeight);
    }
}
