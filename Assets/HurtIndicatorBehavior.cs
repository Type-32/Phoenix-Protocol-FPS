using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HurtIndicatorBehavior : MonoBehaviour
{
    public UIManager manager;
    public CanvasGroup localCanvasGroup;

    private float maxTimer = 8f;
    private float timer = 6f;

    private RectTransform rectTrans;

    private Action unReg = null;
    private IEnumerator cooldown;

    private Transform target;
    private Transform player;

    public Vector3 targetPos = Vector3.zero;
    public Quaternion targetRot = Quaternion.identity;
    void Awake()
    {
        manager = GetComponentInParent<UIManager>();
    }
    public void RegisterData(Transform target, Transform player, Action unRegister)
    {
        this.target = target;
        this.player = player;
        this.unReg = unRegister;
        StartCoroutine(RotateToTarget());
        StartTimer();
    }
    private void StartTimer()
    {
        if(cooldown != null)
        {
            StopCoroutine(cooldown);
        }
        cooldown = Countdown();
        StartCoroutine(cooldown);
    }
    IEnumerator RotateToTarget()
    {
        while (enabled)
        {
            if (target)
            {
                targetPos = target.position;
                targetRot = target.rotation;
            }
            Vector3 dir = player.position - targetPos;
            targetRot = Quaternion.LookRotation(dir);
            targetRot.z = targetRot.y;
            targetRot.x = targetRot.y = 0;

            Vector3 northDir = new Vector3(0, 0, player.eulerAngles.y);
            rectTrans.localRotation = targetRot * Quaternion.Euler(northDir);

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
