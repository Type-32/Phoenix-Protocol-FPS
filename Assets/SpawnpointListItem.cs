using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using PrototypeLib.OnlineServices.PUNMultiplayer.ConfigurationKeys;

public class SpawnpointListItem : MonoBehaviour
{
    public SpawnpointUI spawnpointUI;
    public Text capitalCharacter;
    public GameObject spawnpointTrackingObject;
    public Button buttonVisual;
    public GameObject selectedVisual;
    public int spawnpointItemIndex = 0;
    public CanvasGroup canvasGroup;
    Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        spawnpointUI = GetComponentInParent<SpawnpointUI>();
        buttonVisual = GetComponent<Button>();
        animator = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        selectedVisual.gameObject.SetActive(false);
        canvasGroup.alpha = (bool)PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RandomRespawn] ? 0f : 1f;
    }

    public void OnClickSpawnpoint()
    {
        spawnpointUI.DeselectEverySpawnpoint();
        spawnpointUI.playerManager.SetSpawnPositionReference(SpawnManager.Instance.SetSpawnpoint(spawnpointItemIndex), spawnpointItemIndex);
        selectedVisual.gameObject.SetActive(true);
        animator.SetTrigger("OnSelect");
    }
    public void OnDeselectSpawnpoint()
    {
        selectedVisual.gameObject.SetActive(false);
    }

    public void TrackObject()
    {
        //if(spawnpointUI.playerManager.cameraObject.View)
        if (spawnpointTrackingObject != null) transform.position = spawnpointUI.playerManager.cameraObject.WorldToScreenPoint(spawnpointTrackingObject.transform.position);
    }

    public void SetCapitalCharacter(string character)
    {
        capitalCharacter.text = character;
    }
    public void SetTrackingObject(GameObject obj)
    {
        spawnpointTrackingObject = obj;
    }
    public void SetIndex(int i)
    {
        spawnpointItemIndex = i;
    }

    private void Update()
    {
        if (spawnpointUI.playerManager.randomSpawnpoint) buttonVisual.interactable = false;
        else buttonVisual.interactable = true;
        animator.SetBool("SelectionActive", selectedVisual.activeSelf);
        TrackObject();
    }
}
