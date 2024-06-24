using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RobotController : MonoBehaviour
{
    [SerializeField]
    private float steerRatio;
    [SerializeField]
    private float velocityRatio;

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
        var gamepad = Gamepad.current;
        if (gamepad != null) {
            targetSteer = gamepad.rightStick.x.ReadValue() * steerRatio;
            targetVelocity = gamepad.leftStick.y.ReadValue() * velocityRatio;
        }
        else {
            targetSteer = 0.0f;
            targetVelocity = 0.0f;
        }

        rightSteer.SetDriveTarget(ArticulationDriveAxis.X, targetSteer);
        leftSteer.SetDriveTarget(ArticulationDriveAxis.X, targetSteer);

        rightDriveWheel.SetDriveTargetVelocity(ArticulationDriveAxis.X, targetVelocity);
        leftDriveWheel.SetDriveTargetVelocity(ArticulationDriveAxis.X, targetVelocity);
    }
}
