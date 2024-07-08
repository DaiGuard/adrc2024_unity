using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using UnityEngine.UI;
using UnityEngine.Splines;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;


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

    private bool IsAutoDrive = false;

    private Vector3 startPos;
    private Quaternion startRot;
    private GameObject robotObject;

    [SerializeField]
    private float emagencyStopDistance;

    [SerializeField]
    private float nearDistance;

    [SerializeField]
    private float startDistance;

    private bool startFlag;

    // Start is called before the first frame update
    void Start()
    {
        robotObject = this.gameObject;
        startPos = robotObject.transform.position;
        startRot = robotObject.transform.rotation;

        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<TwistStampedMsg>(current_vel_name);
        ros.Subscribe<TwistMsg>(cmd_vel_name, CallbackCmdVel);
    }

    // Update is called once per frame
    void Update()
    {
        var gamepad = Gamepad.current;
        if(IsAutoDrive)
        {
            var trans = targetObject.transform.position - robotObject.transform.position;
            trans = robotObject.transform.worldToLocalMatrix * trans;

            var diff_a = 0.0f;
            if(startFlag)
            {
                targetVelocity = 0.0f;
                targetSteer = 0.0f;

                if(trans.magnitude > startDistance)
                {
                    startFlag = false;
                }
            }
            else 
            {
                if(trans.magnitude > emagencyStopDistance)
                {
                    IsAutoDrive = false;

                    targetVelocity = 0.0f;
                    targetSteer = 0.0f;
                }
                else if(trans.magnitude > nearDistance) {
                    diff_a = Mathf.Atan2(trans.x, trans.z);

                    targetVelocity = 2000;
                    targetSteer = diff_a * Mathf.Rad2Deg;

                }
                else
                {
                    targetVelocity = 2000;
                    targetSteer = 0.0f;
                }
            }
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

        var msg = new TwistStampedMsg();
        var time = Time.time;
        msg.header.stamp.sec = (int)System.Math.Truncate(time);
        msg.header.stamp.nanosec = (uint)((time - msg.header.stamp.sec) * 1e+9);
        msg.twist.linear.x = targetVelocity / velocityRatio;
        msg.twist.angular.z = targetSteer / steerRatio;

        ros.Publish(current_vel_name, msg);
    }

    void CallbackCmdVel(TwistMsg msg) 
    {
        cmd_vel = msg;
    }

    public bool GetAutoDrive()
    {
        return IsAutoDrive;
    }
    public void SetAutoDrive(bool enable)
    {
        if(!IsAutoDrive && enable)
        {
            startFlag = true;
        }

        IsAutoDrive = enable;
    }

    public void ResetPos()
    {
        var ab = robotObject.GetComponent<ArticulationBody>();
        ab.velocity = Vector3.zero;
        ab.angularVelocity = Vector3.zero;
        ab.TeleportRoot(startPos, startRot);
    }
}
