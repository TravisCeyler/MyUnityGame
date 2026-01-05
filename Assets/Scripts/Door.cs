using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen = false;

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            gameObject.SetActive(false); // Door disappears
        }
    }

     public void CloseDoor()
    {
        if (isOpen)
        {
            isOpen = false;
            gameObject.SetActive(true); // bring door back
        }
    }
}
