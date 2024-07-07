using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Palmmedia.ReportGenerator.Core.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class AutoDriveController : MonoBehaviour
{
    [SerializeField]
    private CourseSwitcher courseSwitch;

    [SerializeField]
    private RobotController robotControl;
    [SerializeField]
    private SplineAnimate traceAnimate;

    [SerializeField]
    private int idx = 0;

    [SerializeField]
    float noiseRange = 0.1f;

    private bool enableAutoDrive = false;

    private SplineContainer tmpCourse = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(enableAutoDrive)
        {
            if(!traceAnimate.IsPlaying
                || (traceAnimate.IsPlaying && !robotControl.GetAutoDrive()))
            {
                idx += 1;
                if(idx >= courseSwitch.CountCourseSetting())
                {
                    idx = 0;
                }

                traceAnimate.Pause();
                robotControl.SetAutoDrive(false);

                robotControl.ResetPos();
                traceAnimate.Restart(false);

                var course = courseSwitch.EnableCourseSetting(idx);

                if(tmpCourse)
                {
                    Destroy(tmpCourse.gameObject);
                }            
                tmpCourse = Instantiate(course);
                tmpCourse.gameObject.name = "traceline_random";

                RandomCourse(tmpCourse);

                traceAnimate.Container = tmpCourse;

                robotControl.SetAutoDrive(true);
                traceAnimate.Play();
            }
        }
    }

    private void RandomCourse(SplineContainer course)
    {
        var spline = course.Spline;

        for(int i=2; i<spline.Knots.Count(); i++)
        {
            var p = spline[i];
            var d = Random.insideUnitCircle * noiseRange;

            p.Position.x += d.x;
            p.Position.z += d.y;

            spline[i] = p;
        }
    }

    public void ChangeAutoDriveState(Toggle sw)
    {
        if (sw.isOn)
        {
            // if(traceAnimate.IsPlaying)
            // {
            //     traceAnimate.Pause();
            //     robotControl.SetAutoDrive(false);
            // }

            // robotControl.ResetPos();
            // traceAnimate.Restart(false);

            // robotControl.SetAutoDrive(true);
            // traceAnimate.Play();

            this.enableAutoDrive = true;
        }
        else {
            this.enableAutoDrive = false;

            traceAnimate.Pause();
            robotControl.SetAutoDrive(false);
        }
    }
}
