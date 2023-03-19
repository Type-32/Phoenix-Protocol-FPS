using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Update Log Data", menuName = "Project Phoenix/Informatics/New Update Log Data", order = 1)]
public class UpdateLogData : ScriptableObject
{
    public string version = "";
    public string description = "";
}
