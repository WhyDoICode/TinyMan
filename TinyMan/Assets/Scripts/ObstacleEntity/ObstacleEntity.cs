using System.Collections;
using UnityEngine;

public class ObstacleEntity : MonoBehaviour
{
    // Virtual method that child classes can override
    public virtual void PerformObstacleAction(PlatformerController controller)
    {
        if (controller != null)
        {
            Debug.Log("Performing default obstacle action on: " + controller.gameObject.name);
            controller.JumpWithForce(5); // Default jump force
        }
    }
}
