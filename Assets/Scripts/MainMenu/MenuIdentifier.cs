using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class MenuIdentifier : MonoBehaviour
{
    public delegate void ChangeState(bool value);
    public event ChangeState OnReceivedInstruction;
    [SerializeField] string menuName;
    [SerializeField] int menuID = -1;
    [SerializeField] bool selfManagable = false;
    void Start()
    {
        //MenuManager.ToggleMenu += ReceiveInstruction;
    }
    void ReceiveInstruction(bool state, bool closeOtherMenus, int id, string name)
    {
        if (selfManagable) return;
        if (id != -1 && id == menuID)
        {
            gameObject.SetActive(state);
        }
        if (name != null && menuName == name)
        {
            gameObject.SetActive(state);
        }
        if (closeOtherMenus) gameObject.SetActive(false);
        OnReceivedInstruction?.Invoke(gameObject.activeInHierarchy);
    }
    public void SetID(int id) => menuID = id;
    public void SetName(string name) => menuName = name;
}