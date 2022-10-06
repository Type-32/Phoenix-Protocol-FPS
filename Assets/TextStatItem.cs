using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextStatItem : MonoBehaviour
{
    public Text message;
    public Text points;
    public void SetInfo(string msg, int pts)
    {
        message.text = msg;

        string temp = "";
        if (pts > 0) temp = "+" + pts.ToString();
        else if(pts == 0) temp = pts.ToString();
        else temp = "-" + Mathf.Abs(pts).ToString();
        points.text = temp;
    }
}
