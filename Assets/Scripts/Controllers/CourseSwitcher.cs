using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;


public class CourseSwitcher : MonoBehaviour
{
    [System.Serializable]
    struct CourseSetting
    {
        [SerializeField]
        public SplineContainer traceline;
        [SerializeField]
        public List<GameObject> enableObjects;
    }

    [SerializeField]
    private SplineContainer enableTraceLine;

    [SerializeField]
    private int enableIdx = 0;

    [SerializeField]
    private List<CourseSetting> courseSettings;

    // Start is called before the first frame update
    void Start()
    {
        EnableCourseSetting(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int CountCourseSetting()
    {
        return courseSettings.Count;
    }

    public SplineContainer EnableCourseSetting(int id)
    {
        enableIdx = id;

        for(int i=0; i<courseSettings.Count; i++)
        {
            var setting = courseSettings[i];

            if(i == enableIdx)
            {
                enableTraceLine = setting.traceline;
                foreach(var obj in setting.enableObjects)
                {
                    obj.SetActive(true);
                }
            }
            else
            {
                foreach(var obj in setting.enableObjects)
                {
                    obj.SetActive(false);
                }
            }
        }

        return enableTraceLine;
    }
}
