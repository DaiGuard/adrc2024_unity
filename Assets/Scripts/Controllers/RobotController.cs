using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [SerializeField]
    private float targetSteer;
    [SerializeField]
    private float targetVelocity;

    [SerializeField]
    private ArticulationBody rightSteer;
    [SerializeField]
    private ArticulationBody leftSteer;
    [SerializeField]
    private ArticulationBody rightDriveWheel;
    [SerializeField]
    private ArticulationBody leftDriveWheel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rightSteer.SetDriveTarget(ArticulationDriveAxis.X, targetSteer);
        leftSteer.SetDriveTarget(ArticulationDriveAxis.X, targetSteer);

        rightDriveWheel.SetDriveTargetVelocity(ArticulationDriveAxis.X, targetVelocity);
        leftDriveWheel.SetDriveTargetVelocity(ArticulationDriveAxis.X, targetVelocity);
    }
}
