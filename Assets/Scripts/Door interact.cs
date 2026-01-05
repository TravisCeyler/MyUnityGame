using UnityEngine;


    public class DoorInteract : MonoBehaviour
    {
        public bool isOpen = false;
        public float openAngle = 90f;
        public float openSpeed = 2f;
        public Transform doorHinge; // Reference to the part of the door that rotates (usually the pivot)

        private bool playerNearby = false;

        void Update()
        {
            if (playerNearby && Input.GetKeyDown(KeyCode.F))
            {
                isOpen = !isOpen; // Toggle door
            }

            // Rotate the door
            float targetAngle = isOpen ? openAngle : 0f;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            doorHinge.localRotation = Quaternion.Slerp(doorHinge.localRotation, targetRotation, Time.deltaTime * openSpeed);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerNearby = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerNearby = false;
            }
        }
    }
