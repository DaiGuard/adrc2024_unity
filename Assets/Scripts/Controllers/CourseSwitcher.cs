using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CourseSwitcher : MonoBehaviour
{
    [SerializeField]
    List<GameObject> rightObjects;

    [SerializeField]
    List<GameObject> leftObjects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchRightLeft(bool flag)
    {
        foreach(var obj in leftObjects)
        {
            obj.SetActive(!flag);
        }

        foreach(var obj in rightObjects)
        {
            obj.SetActive(flag);
        }
    }

    public void RightLeftChanged(Toggle sw)
    {
        if(sw.isOn)
        {
            SwitchRightLeft(true);
        }
        else {
            SwitchRightLeft(false);
        }
    }
}
