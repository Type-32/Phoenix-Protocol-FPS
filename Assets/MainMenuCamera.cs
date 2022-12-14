using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [SerializeField] GameObject originPoint;
    [SerializeField] GameObject loadoutPoint;
    [SerializeField] GameObject multiplayerPoint;
    [SerializeField] GameObject shopPoint;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = originPoint.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (MainMenuUIManager.instance.openedMainMenu)
        {
            transform.position = Vector3.Slerp(transform.position, originPoint.transform.position, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, originPoint.transform.rotation, Time.deltaTime * 5f);
        }
        else if (MainMenuUIManager.instance.openedMultiplayerMenu || MainMenuUIManager.instance.openedCreateRoomMenu || MainMenuUIManager.instance.openedRoomMenu || MainMenuUIManager.instance.openedFindRoomMenu)
        {
            transform.position = Vector3.Slerp(transform.position, multiplayerPoint.transform.position, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, multiplayerPoint.transform.rotation, Time.deltaTime * 5f);
        }
        else if (MainMenuUIManager.instance.openedLoadoutSelectionMenu)
        {
            transform.position = Vector3.Slerp(transform.position, loadoutPoint.transform.position, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, loadoutPoint.transform.rotation, Time.deltaTime * 5f);
        }
        else if (MainMenuUIManager.instance.openedShopMenu)
        {
            transform.position = Vector3.Slerp(transform.position, shopPoint.transform.position, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, shopPoint.transform.rotation, Time.deltaTime * 5f);
        }
        else
        {
            transform.position = Vector3.Slerp(transform.position, originPoint.transform.position, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, originPoint.transform.rotation, Time.deltaTime * 5f);
        }
    }
}
