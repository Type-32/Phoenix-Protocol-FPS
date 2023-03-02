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
    [SerializeField] public string menuName;
    [SerializeField] public int menuID = -1;
    [SerializeField] bool selfManagable = false;
    void Awake()
    {
        MenuManager.OnMenuToggled += ReceiveInstruction;
        MenuManager.OnSearchMenu += SearchedInstruction;
    }
    void ReceiveInstruction(bool state, string name, int id)
    {
        //if (selfManagable) return;
        if (id != -1 && id == menuID)
        {
            gameObject.SetActive(state);
        }
        if (name != "null" && menuName == name)
        {
            gameObject.SetActive(state);
        }
        OnReceivedInstruction?.Invoke(gameObject.activeInHierarchy);
    }
    MenuIdentifier SearchedInstruction(string name, int id)
    {
        if (id != -1 && id == menuID)
        {
            return this;
        }
        if (name != "null" && (name != "true" && name != "false") && menuName == name)
        {
            return this;
        }
        if (name == "true" || name == "false")
        {
            if (gameObject.activeInHierarchy == bool.Parse(name)) return this;
        }
        OnReceivedInstruction?.Invoke(gameObject.activeInHierarchy);
        return null;
    }
    public void SetID(int id) => menuID = id;
    public void SetName(string name) => menuName = name;
}