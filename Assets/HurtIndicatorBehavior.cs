using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HurtIndicatorBehavior : MonoBehaviour
{
    public UIManager manager;
    public CanvasGroup localCanvasGroup;

    private float maxTimer = 6f;
    private float timer = 5f;

    private RectTransform rectTrans;
    protected RectTransform Rect
    {
        get
        {
            if(rectTrans == null)
            {
                rectTrans = GetComponent<RectTransform>();
                if(rectTrans == null)
                {
                    rectTrans = gameObject.AddComponent<RectTransform>();
                }
            }
            return rectTrans;
        }
    }

    private Action unReg = null;
    private IEnumerator IE_cooldown = null;

    private UIManager.ValueTransform target;
    private Transform player = null;

    private Vector3 targetPos = Vector3.zero;
    private Quaternion targetRot = Quaternion.identity;
    void Awake()
    {
        manager = GetComponentInParent<UIManager>();
        //rectTrans = GetComponent<RectTransform>();
    }
    public void RegisterData(UIManager.ValueTransform target, Transform player, Action unRegister)
    {
        this.target = target;
        this.player = player;
        unReg = unRegister;
        StartCoroutine(RotateToTarget());
        StartTimer();
    }
    private void StartTimer()
    {
        if(IE_cooldown != null)
        {
            StopCoroutine(IE_cooldown);
        }
        IE_cooldown = Countdown();
        StartCoroutine(IE_cooldown);
    }
    IEnumerator RotateToTarget()
    {
        while (enabled)
        {
            //if (target)
            //{
                targetPos = target.position;
                targetRot = target.rotation;
            //}
            Vector3 dir = player.position - targetPos;
            targetRot = Quaternion.LookRotation(dir);
            targetRot.z = -targetRot.y;
            targetRot.x = 0;
            targetRot.y = 0;

            Vector3 northDir = new Vector3(0, 0, player.eulerAngles.y);
            Rect.localRotation = targetRot * Quaternion.Euler(northDir);

            yield return null;
        }
    }
    private IEnumerator Countdown()
    {
        while(localCanvasGroup.alpha < 1.0f)
        {
            localCanvasGroup.alpha += 4 * Time.deltaTime;
            yield return null;
        }
        while(timer > 0)
        {
            timer--;
            yield return new WaitForSeconds(1f);
        }
        while(localCanvasGroup.alpha > 0.0f)
        {
            localCanvasGroup.alpha -= 2 * Time.deltaTime;
            yield return null;
        }
        unReg();
        Destroy(gameObject);
    }
    public void Restart()
    {
        timer = maxTimer;
        StartTimer();
    }
}
