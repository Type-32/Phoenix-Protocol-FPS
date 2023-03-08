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
    void FixedUpdate()
    {
        if (MenuManager.Instance.openedMainMenu)
        {
            transform.position = Vector3.Slerp(transform.position, originPoint.transform.position, Time.fixedDeltaTime * 2.5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, originPoint.transform.rotation, Time.fixedDeltaTime * 2.5f);
        }
        else if (MenuManager.Instance.openedMultiplayerMenu || MenuManager.Instance.openedCreateRoomMenu || MenuManager.Instance.openedRoomMenu || MenuManager.Instance.openedFindRoomMenu)
        {
            transform.position = Vector3.Slerp(transform.position, multiplayerPoint.transform.position, Time.fixedDeltaTime * 2.5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, multiplayerPoint.transform.rotation, Time.fixedDeltaTime * 2.5f);
        }
        else if (MenuManager.Instance.openedLoadoutSelectionMenu)
        {
            transform.position = Vector3.Slerp(transform.position, loadoutPoint.transform.position, Time.fixedDeltaTime * 2.5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, loadoutPoint.transform.rotation, Time.fixedDeltaTime * 2.5f);
        }
        else if (MenuManager.Instance.openedShopMenu)
        {
            transform.position = Vector3.Slerp(transform.position, shopPoint.transform.position, Time.fixedDeltaTime * 2.5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, shopPoint.transform.rotation, Time.fixedDeltaTime * 2.5f);
        }
        else
        {
            transform.position = Vector3.Slerp(transform.position, originPoint.transform.position, Time.fixedDeltaTime * 2.5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, originPoint.transform.rotation, Time.fixedDeltaTime * 2.5f);
        }
    }
}
