using UnityEngine;

public class Recoil : MonoBehaviour
{
    [SerializeField] PlayerControllerManager player;
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private Vector3 currentPosition;
    private Vector3 targetPosition;
    //public float recoilX;
    //public float recoilY;
    //public float recoilZ;

    [Header("Camera Recoil Generic Stats")]
    public float snappiness;
    public float returnSpeed;
    public float positionSnappiness;
    public float positionReturnSpeed;
    void Update()
    {
        if (!player.pv.IsMine) return;
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        targetPosition = Vector3.Lerp(targetPosition, Vector3.zero, positionReturnSpeed * Time.deltaTime);
        currentPosition = Vector3.Slerp(currentPosition, targetPosition, positionSnappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
        transform.localPosition = currentPosition;
    }

    public void RecoilFire(float verticalRecoil, float horizontalRecoil, float sphericalShake, float positionRecoilRetaliation, float positionRecoilVertical, float positionTransitionalSnappiness, float positionRecoilReturnSpeed, float transitionalSnappiness, float recoilReturnSpeed)
    {
        snappiness = transitionalSnappiness;
        returnSpeed = recoilReturnSpeed;
        positionSnappiness = positionTransitionalSnappiness;
        positionReturnSpeed = positionRecoilReturnSpeed;
        targetRotation += new Vector3(-verticalRecoil, Random.Range(-horizontalRecoil, horizontalRecoil), Random.Range(-sphericalShake, sphericalShake));
        targetPosition += new Vector3(0f, 0f, -positionRecoilRetaliation);
        player.ui.AddReticleSize((verticalRecoil * positionRecoilRetaliation) * 20f);
    }
}
