using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogsItem : MonoBehaviour
{
    public Text content;
    public Text version;
    public void SetInfo(string _content, string _version)
    {
        content.text = _content;
        version.text = _version;
    }
}
