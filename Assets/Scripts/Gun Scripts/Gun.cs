using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item
{
    public GunManager gun;
    public override abstract void Use();
    public override abstract void InitializeStart();
    public override abstract void InitializeAwake();
    public GameObject bulletImpactPrefab;
}
