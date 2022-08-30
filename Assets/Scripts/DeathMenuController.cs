using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMenuController : MonoBehaviour
{
    public UIManager ui;
    public GameObject respawnDeathIndicator;
    public GameObject loadoutDeathIndicator;
    public DeathCamLookScript deathCam;
    public bool playerIsDead = false;
    private int maxRespawnDelay = 3;
    public int respawnDelay;
}
