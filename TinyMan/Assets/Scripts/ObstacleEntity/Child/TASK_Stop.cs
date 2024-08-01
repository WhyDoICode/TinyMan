using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TASK_Stop : ObstacleEntity
{
    public override void PerformObstacleAction(PlatformerController controller)
    {
        if (controller == null) return;

        controller.Stop();
    }
}
