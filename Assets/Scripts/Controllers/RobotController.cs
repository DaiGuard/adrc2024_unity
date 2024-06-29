using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

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

    [SerializeField]
    private string cmd_vel_name = "/cmd_vel";
    [SerializeField]
    private string current_vel_name = "/current_vel";

    private ROSConnection ros;
    private TwistMsg cmd_vel;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<TwistMsg>(current_vel_name);
        ros.Subscribe<TwistMsg>(cmd_vel_name, CallbackCmdVel);
    }

    // Update is called once per frame
    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null) {
            if(gamepad.triangleButton.ReadValue() < 1.0)
            {
                targetSteer = gamepad.rightStick.x.ReadValue() * steerRatio;
                targetVelocity = gamepad.leftStick.y.ReadValue() * velocityRatio;
            }
            else 
            {
                targetSteer = (float)cmd_vel.angular.z * steerRatio;
                targetVelocity = (float)cmd_vel.linear.x * velocityRatio;
            }
        }
        else {
            targetSteer = 0.0f;
            targetVelocity = 0.0f;
        }

        rightSteer.SetDriveTarget(ArticulationDriveAxis.X, targetSteer);
        leftSteer.SetDriveTarget(ArticulationDriveAxis.X, targetSteer);

        rightDriveWheel.SetDriveTargetVelocity(ArticulationDriveAxis.X, targetVelocity);
        leftDriveWheel.SetDriveTargetVelocity(ArticulationDriveAxis.X, targetVelocity);

        var msg = new TwistMsg();
        msg.linear.x = targetVelocity / velocityRatio;
        msg.angular.z = targetSteer / steerRatio;

        ros.Publish(current_vel_name, msg);
    }

    void CallbackCmdVel(TwistMsg msg) 
    {
        cmd_vel = msg;
    }
}
