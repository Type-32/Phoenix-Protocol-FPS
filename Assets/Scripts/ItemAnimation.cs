using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAnimation : MonoBehaviour
{
        [Header("Global References")]
        public ItemStats stats;
        public ItemManager item;
        public Animator animate;
        public GameObject itemModel;

        [Space]
        public bool rotationX = true;
        public bool rotationY = true;
        public bool rotationZ = true;

        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Quaternion originRotation;

        private float mouseInputX;
        private float mouseInputY;

        private Vector3 currentPosition;
        private Vector3 targetPosition;

        private float swayIntensityValve = 0f;
        private float maxSwayIntensityValve = 0f;
        private float rotSwayIntensityValve = 0f;
        private float maxRotSwayIntensityValve = 0f;

        Vector3 rotationalRecoil;
        Vector3 positionalRecoil;
        Vector3 Rot;
        public float walkBobAmount = 0.3f;
        public float sprintBobAmount = 0.6f;
        public float walkBobSpeed = 9f;
        public float sprintBobSpeed = 15f;
        public float returnDuration = 4f;

        private float defaultYPos = 0f;
        private float defaultXPos = 0f;
        private float timer;

        private float returnSpeed = 1f;
        private float snappiness = 6f;

        private float xPosDelay = 0f;
        private void Start()
        {
            initialRotation = originRotation = itemModel.transform.localRotation;
            initialPosition = targetPosition = itemModel.transform.localPosition;
            defaultXPos = itemModel.transform.localPosition.x;
            defaultYPos = itemModel.transform.localPosition.y;
        }


        void Update()
        {
        if (!item.player.stats.mouseMovementEnabled) return;
            WeaponBob();
            if (item.player.stats.isSliding || item.player.stats.isSprinting)
            {
            }
            else
            {
                swayIntensityValve = Mathf.Lerp(swayIntensityValve, (stats.swayIntensity), Time.deltaTime * 7f);
                maxSwayIntensityValve = Mathf.Lerp(maxSwayIntensityValve, stats.maxSwayIntensity, Time.deltaTime * 7f);
                rotSwayIntensityValve = Mathf.Lerp(rotSwayIntensityValve, (stats.rotSwayIntensity), Time.deltaTime * 7f);
                maxRotSwayIntensityValve = Mathf.Lerp(maxRotSwayIntensityValve, stats.maxRotSwayIntensity, Time.deltaTime * 7f);
            }
            CalculateSway();
            MoveSway();
            TiltSway();
        }

        private void CalculateSway()
        {
            mouseInputX = -Input.GetAxis("Mouse X") - (Input.GetAxis("Horizontal") * 1.8f);
            mouseInputY = -Input.GetAxis("Mouse Y") - (item.player.controls.playerInput.y * 1.2f);
        }
        private void MoveSway()
        {
            float moveX = Mathf.Clamp(mouseInputX * swayIntensityValve, -maxSwayIntensityValve, maxSwayIntensityValve);
            float moveY = Mathf.Clamp(mouseInputY * swayIntensityValve, -maxSwayIntensityValve, maxSwayIntensityValve);
            Vector3 finalPosition = new Vector3(moveX, moveY, 0);
            itemModel.transform.localPosition = Vector3.Lerp(itemModel.transform.localPosition, finalPosition + initialPosition, Time.deltaTime * stats.smoothness);
        }
        private void TiltSway()
        {
            float tiltY = Mathf.Clamp(mouseInputX * rotSwayIntensityValve, -maxRotSwayIntensityValve, maxRotSwayIntensityValve);
            float tiltX = Mathf.Clamp(mouseInputY * rotSwayIntensityValve, -maxRotSwayIntensityValve, maxRotSwayIntensityValve);
            Quaternion finalRotation = Quaternion.Euler(new Vector3(rotationX ? -tiltX : 0f, rotationY ? tiltY : 0f, rotationZ ? tiltY : 0f));
            itemModel.transform.localRotation = Quaternion.Slerp(itemModel.transform.localRotation, finalRotation * initialRotation, Time.deltaTime * stats.rotSmoothness);
        }
        private void UpdateSway()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            //Calculate targeted Rotation
            Quaternion tempRotX = Quaternion.AngleAxis(stats.swayIntensity * mouseX, Vector3.up);
            Quaternion tempRotY = Quaternion.AngleAxis(stats.swayIntensity * mouseY, Vector3.right);
            Quaternion targetRotation = originRotation * tempRotX * tempRotY;

            itemModel.transform.localRotation = Quaternion.Lerp(itemModel.transform.localRotation, targetRotation, Time.deltaTime * stats.smoothness);
        }
        private void UpdateRecoil()
        {
            targetPosition = Vector3.Lerp(targetPosition, transform.localPosition, returnSpeed * Time.deltaTime);
            currentPosition = Vector3.Slerp(currentPosition, targetPosition, snappiness * Time.fixedDeltaTime);
            transform.localPosition = currentPosition;
        }

        public void TriggerWeaponRecoil()
        {
        }
        public void WeaponBob()
        {
            itemModel.transform.localPosition = new Vector3(Mathf.Lerp(itemModel.transform.localPosition.x, defaultXPos, Time.deltaTime * 2), Mathf.Lerp(itemModel.transform.localPosition.y, defaultYPos, Time.deltaTime * 2), itemModel.transform.localPosition.z + (-Input.GetAxis("Vertical") / 70));
            if (!item.player.stats.onGround) return;
            if ((Mathf.Abs(Input.GetAxis("Horizontal")) > 0f || Mathf.Abs(Input.GetAxis("Vertical")) > 0f))
            {
                //defaultXPos + Mathf.Cos(timer) * (player.stats.isSprinting ? sprintBobAmount : walkBobAmount)
                timer += Time.deltaTime * (item.player.stats.isSprinting ? sprintBobSpeed : walkBobSpeed);
                itemModel.transform.localPosition = new Vector3(Mathf.Lerp(itemModel.transform.localPosition.x, defaultXPos + Mathf.Cos(timer / 2) * (item.player.stats.isSprinting ? sprintBobAmount : walkBobAmount), Time.deltaTime * 8), Mathf.Lerp(itemModel.transform.localPosition.y, defaultYPos + Mathf.Sin(timer) * (item.player.stats.isSprinting ? sprintBobAmount : walkBobAmount), Time.deltaTime * 8), itemModel.transform.localPosition.z);
            }
        }

        // Start is called before the first frame update
}
