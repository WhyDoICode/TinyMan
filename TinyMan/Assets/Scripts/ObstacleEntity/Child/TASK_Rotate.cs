using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TASK_Rotate : ObstacleEntity
{
    public override void PerformObstacleAction(PlatformerController controller)
    {
        if (controller == null) return;
      
        controller.RotateDirection();
    }
}
