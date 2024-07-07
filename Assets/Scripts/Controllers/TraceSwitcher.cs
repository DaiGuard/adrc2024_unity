using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class TraceSwitcher : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private SplineContainer rightTraceLine;

    [SerializeField]
    private SplineContainer leftTraceLine;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RightLeftTraceChange(bool flag)
    {
        var anime = this.GetComponent<SplineAnimate>();

        if (flag)
        {
            anime.Container = rightTraceLine;
        }
        else
        {
            anime.Container = leftTraceLine;
        }
    }


    public void RightLeftChanged(Toggle sw)
    {
        if(sw.isOn)
        {
            RightLeftTraceChange(true);
        }
        else {
            RightLeftTraceChange(false);
        }
    }
}
