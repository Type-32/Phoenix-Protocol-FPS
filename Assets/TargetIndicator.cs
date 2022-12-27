using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    public Image indicatorImage;
    public Image offscreenImage;
    public Sprite objective, friendly, hostile, supply;
    public float outOfSightOffset = 45f;
    private GameObject target;
    private new Camera camera;
    private RectTransform holderRect;
    private RectTransform rect;
    public CanvasGroup canvasGroup;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    public void InitializeIndicator(GameObject target, UIManager.TargetIndicatorType type, Color color, Camera cam, RectTransform holderRect)
    {
        this.target = target;
        camera = cam;
        this.holderRect = holderRect;
        switch (type)
        {
            case UIManager.TargetIndicatorType.Hostile:
                indicatorImage.sprite = hostile;
                break;
            case UIManager.TargetIndicatorType.Objective:
                indicatorImage.sprite = objective;
                break;
            case UIManager.TargetIndicatorType.Friendly:
                indicatorImage.sprite = friendly;
                break;
            case UIManager.TargetIndicatorType.Supply:
                indicatorImage.sprite = supply;
                break;
            case UIManager.TargetIndicatorType.None:
                indicatorImage.sprite = null;
                break;
        }
        indicatorImage.color = offscreenImage.color = color;
    }
    public void UpdateTargetIndicator()
    {
        if (target == null)
        {
            Destroy(this.gameObject);
            canvasGroup.alpha = 0f;
            return;
        }
        canvasGroup.alpha = 1f;
        SetPosition();
    }
    protected void SetPosition()
    {
        Vector3 iPos = camera.WorldToScreenPoint(new Vector3(target.transform.position.x, target.transform.position.y + 1f, target.transform.position.z));
        if (iPos.z >= 0f & iPos.x <= holderRect.rect.width * holderRect.rect.height & iPos.y <= holderRect.rect.height * holderRect.localScale.x & iPos.x >= 0f & iPos.y >= 0f)
        {
            iPos.z = 0f;
            TargetOutOfSight(false, iPos);
        }
        else if (iPos.z >= 0f)
        {
            iPos = OutOfRangeIndicatorPosB(iPos);
            TargetOutOfSight(true, iPos);
        }
        else
        {
            iPos *= -1f;
            iPos = OutOfRangeIndicatorPosB(iPos);
            TargetOutOfSight(true, iPos);
        }
        rect.position = iPos;
    }
    private Vector3 OutOfRangeIndicatorPosB(Vector3 iPos)
    {
        iPos.z = 0f;
        Vector3 canvasCenter = new Vector3(holderRect.rect.width / 2f, holderRect.rect.height / 2f, 0f) * holderRect.localScale.x;
        iPos -= canvasCenter;
        float divX = (holderRect.rect.width / 2f - outOfSightOffset) / Mathf.Abs(iPos.x);
        float divY = (holderRect.rect.height / 2f - outOfSightOffset) / Mathf.Abs(iPos.y);
        if (divX < divY)
        {
            float angle = Vector3.SignedAngle(Vector3.right, iPos, Vector3.forward);
            iPos.x = Mathf.Sign(iPos.x) * (holderRect.rect.width * 0.5f - outOfSightOffset) * holderRect.localScale.x;
            iPos.y = Mathf.Tan(Mathf.Deg2Rad * angle) * iPos.x;
        }
        else
        {
            float angle = Vector3.SignedAngle(Vector3.up, iPos, Vector3.forward);
            iPos.y = Mathf.Sign(iPos.y) * (holderRect.rect.height / 2f - outOfSightOffset) * holderRect.localScale.y;
            iPos.x = -Mathf.Tan(Mathf.Deg2Rad * angle) * iPos.y;
        }
        iPos += canvasCenter;
        return iPos;
    }
    private void TargetOutOfSight(bool oos, Vector3 iPos)
    {
        if (oos)
        {
            if (!offscreenImage.gameObject.activeSelf) offscreenImage.gameObject.SetActive(true);
            if (indicatorImage.isActiveAndEnabled) indicatorImage.enabled = false;
            offscreenImage.rectTransform.rotation = Quaternion.Euler(RotOutOfSightTargetIndicator(iPos));
        }
        else
        {
            if (offscreenImage.gameObject.activeSelf) offscreenImage.gameObject.SetActive(false);
            if (!indicatorImage.isActiveAndEnabled) indicatorImage.enabled = true;
        }
    }
    private Vector3 RotOutOfSightTargetIndicator(Vector3 iPos)
    {
        Vector3 canvasCenter = new Vector3(holderRect.rect.width / 2f, holderRect.rect.height / 2f, 0f) * holderRect.localScale.x;
        float angle = Vector3.SignedAngle(Vector3.up, iPos - canvasCenter, Vector3.forward);
        return new Vector3(0f, 0f, angle);
    }
}
