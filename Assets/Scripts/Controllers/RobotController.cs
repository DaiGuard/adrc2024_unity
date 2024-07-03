using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using UnityEngine.UI;
using UnityEngine.Splines;

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

    [SerializeField]
    private GameObject targetObject;

    private bool isAutoDrive;

    private Vector3 startPos;
    private Quaternion startRot;
    private GameObject robotObject;

    // Start is called before the first frame update
    void Start()
    {
        robotObject = this.gameObject;
        startPos = robotObject.transform.position;
        startRot = robotObject.transform.rotation;

        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<TwistMsg>(current_vel_name);
        ros.Subscribe<TwistMsg>(cmd_vel_name, CallbackCmdVel);
    }

    // Update is called once per frame
    void Update()
    {
        var gamepad = Gamepad.current;
        if(isAutoDrive)
        {
            var trans = targetObject.transform.position - robotObject.transform.position;
            trans = robotObject.transform.worldToLocalMatrix * trans;

            var diff_d = trans.z;
            var diff_a = 0.0f;
            if(trans.magnitude > 0.1) {
                diff_a = Mathf.Atan2(trans.x, trans.z);
            }

            // targetVelocity += diff_d * 50.0f;
            targetVelocity = 3100;
            targetSteer = diff_a * Mathf.Rad2Deg;

            // Debug.Log(diff_d + "," + diff_a);
        }
        else {
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

    public void AutoDriveChanged(Toggle sw)
    {
        var animate = targetObject.GetComponent<SplineAnimate>();
        if(sw.isOn)
        {
            var ab = robotObject.GetComponent<ArticulationBody>();
            ab.TeleportRoot(startPos, startRot);

            isAutoDrive = true;
            animate.Restart(false);
            animate.Play();
        }
        else {
            isAutoDrive = false;
            animate.Pause();
        }
    }
}
