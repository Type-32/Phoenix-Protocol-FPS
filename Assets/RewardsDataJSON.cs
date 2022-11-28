using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class RewardsDataJSON
{
    public List<string> ownedRewards;
    internal RewardsDataJSON()
    {
        ownedRewards = new List<string>();
    }
}
