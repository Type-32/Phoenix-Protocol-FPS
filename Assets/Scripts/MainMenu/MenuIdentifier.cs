using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class MenuIdentifier : MonoBehaviour
{
    public delegate void ChangeState(bool value, string name = null);
    public event ChangeState OnReceivedInstruction;
    public GameObject menuObject;
    [SerializeField] public string menuName;
    [SerializeField] public int menuID = -1;
    [SerializeField] bool selfManagable = false;
    void Awake()
    {
        MenuManager.OnMenuToggled += ReceiveInstruction;
        MenuManager.OnSearchMenu += SearchedInstruction;
        OnReceivedInstruction += MenuManager.OnInstructedMenuIdentifier;
    }
    public void ReceiveInstruction(bool state, string name, int id)
    {
        //if (selfManagable) return;
        if (id != -1 && id == menuID)
        {
            if (!state && !menuObject.activeInHierarchy)
                return;
            else
            {
                menuObject.SetActive(state);
                //Debug.Log($"MenuIdentifier {menuID} is Invoked.");
            }
        }
        if (name != "null" && menuName == name)
        {
            if (!state && !menuObject.activeInHierarchy)
                return;
            else
            {
                menuObject.SetActive(state);
                //Debug.Log($"MenuIdentifier {menuName} is Invoked.");
            }
        }
        OnReceivedInstruction?.Invoke(menuObject.activeInHierarchy, menuName);
        if (state && name == "main") MenuManager.instance.SetQuitButtonState(true);
        else MenuManager.instance.SetQuitButtonState(false);

    }
    public MenuIdentifier SearchedInstruction(string name, int id)
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
            if (menuObject.activeSelf == bool.Parse(name)) return this;
        }
        OnReceivedInstruction?.Invoke(menuObject.activeInHierarchy, menuName);
        return null;
    }
    public void SetID(int id) => menuID = id;
    public void SetName(string name) => menuName = name;
}