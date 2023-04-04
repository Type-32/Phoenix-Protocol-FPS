using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Throwables Data", menuName = "Project Phoenix/Loadouts/New Throwables Data", order = 1)]
public class ThrowablesData : ItemData
{
    public GameObject throwablesPrefab;
    public WeaponType weaponType;

    [Space]
    [Header("Stats")]
    [Range(0f, 200f)] public float damagePerUnit = 30f;
    public float throwForce = 4f;
    public float effectiveRange = 8f;
    public float explosiveDelay = 3f;

    [Space]
    [Header("Parameters")]
    public bool isExplosive = true;
    public bool isGrenade = true;
    public bool isArmorPenetrative = false;
    public bool isReactive = false;

    [Space]
    [Header("Audio Clips")]
    public List<AudioClip> contactClips = new List<AudioClip>();
    public List<AudioClip> explodeClips = new List<AudioClip>();
    public List<AudioClip> aftermathClips = new List<AudioClip>();

}
