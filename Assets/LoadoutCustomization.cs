using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutCustomization : MonoBehaviour
{
    [SerializeField] LoadoutSelectionScript loadoutSelection;
    [SerializeField] LoadoutCustomButtonsHolder customButtonsHolder;
    public GameObject attachmentSelectionItemPrefab;

    [Space]
    public GameObject sightUI;
    public GameObject barrelUI;
    public GameObject underbarrelUI;
    public GameObject leftbarrelUI;
    public GameObject rightbarrelUI;

    [Space]
    public Transform sightUIHolder;
    public Transform barrelUIHolder;
    public Transform underbarrelUIHolder;
    public Transform leftbarrelUIHolder;
    public Transform rightbarrelUIHolder;
    
    [Space]
    public List<GameObject> objectList = new List<GameObject>();

    [HideInInspector] public List<GameObject> sightObjects = new List<GameObject>();
    [HideInInspector] public List<GameObject> barrelObjects = new List<GameObject>();
    [HideInInspector] public List<GameObject> underbarrelObjects = new List<GameObject>();
    [HideInInspector] public List<GameObject> leftbarrelObjects = new List<GameObject>();
    [HideInInspector] public List<GameObject> rightbarrelObjects = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        //AttachmentUIItemInstantiation();
        ToggleAllAttachmentUI(false);
    }
    public void ToggleAllAttachmentUI(bool value)
    {
        for (int i = 0; i <= 4; i++)
        {
            AttachmentSelectionUIToggler(i, value);
        }
    }
    public void AttachmentSelectionUIToggler(int index, bool value)
    {
        switch (index)
        {
            case 0:
                sightUI.SetActive(value);
                break;
            case 1:
                barrelUI.SetActive(value);
                break;
            case 2:
                underbarrelUI.SetActive(value);
                break;
            case 3:
                leftbarrelUI.SetActive(value);
                break;
            case 4:
                rightbarrelUI.SetActive(value);
                break;
        }
        if (sightObjects.Count <= 0) sightUI.SetActive(false);
        if (barrelObjects.Count <= 0) barrelUI.SetActive(false);
        if (underbarrelObjects.Count <= 0) underbarrelUI.SetActive(false);
        if (leftbarrelObjects.Count <= 0) leftbarrelUI.SetActive(false);
        if (rightbarrelObjects.Count <= 0) rightbarrelUI.SetActive(false);
    }
    GameObject temp;
    public void AttachmentUIItemInstantiation()
    {
        for (int i = 0; i < loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].weaponData[loadoutSelection.forSelectedSlot].applicableAttachments.Count; i++)
        {
            //GameObject temp = Instantiate(attachmentSelectionItemPrefab,);
            switch (loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].weaponData[loadoutSelection.forSelectedSlot].applicableAttachments[i].attachmentType)
            {
                case GunAttachments.AttachmentTypes.Sight:
                    temp = Instantiate(attachmentSelectionItemPrefab,sightUIHolder);
                    sightObjects.Add(temp);
                    break;
                case GunAttachments.AttachmentTypes.Barrel:
                    temp = Instantiate(attachmentSelectionItemPrefab, barrelUIHolder);
                    barrelObjects.Add(temp);
                    break;
                case GunAttachments.AttachmentTypes.Underbarrel:
                    temp = Instantiate(attachmentSelectionItemPrefab, underbarrelUIHolder);
                    underbarrelObjects.Add(temp);
                    break;
                case GunAttachments.AttachmentTypes.Sidebarrel_Left:
                    temp = Instantiate(attachmentSelectionItemPrefab, leftbarrelUIHolder);
                    leftbarrelObjects.Add(temp);
                    break;
                case GunAttachments.AttachmentTypes.Sidebarrel_Right:
                    temp = Instantiate(attachmentSelectionItemPrefab, rightbarrelUIHolder);
                    rightbarrelObjects.Add(temp);
                    break;
            }
            temp.GetComponent<LoadoutAttachUIItem>().SetInfo(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].weaponData[loadoutSelection.forSelectedSlot].applicableAttachments[i]);
            objectList.Add(temp);
        }
    }
    public void RemoveAttachmentUIItems()
    {
        for(int i = 0; i < objectList.Count; i++)
        {
            Destroy(objectList[i]);
        }
        objectList.Clear();
        sightObjects.Clear();
        barrelObjects.Clear();
        underbarrelObjects.Clear();
        leftbarrelObjects.Clear();
        rightbarrelObjects.Clear();
    }
    public void BackToCustomizeButton()
    {
        loadoutSelection.ToggleCustomizeButtonsUI(true);
        loadoutSelection.ToggleCustomizeSelectionUI(false);
        ToggleAllAttachmentUI(false);
    }
}
