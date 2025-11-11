using UnityEngine;

public class ButtonGroup : MonoBehaviour
{
    public buttionClick[] buttons; // assign in inspector
    public Door door;                  // assign the door

    [Header("Settings")]
    public bool latchMode = false; // if true, door stays open permanently after all pressed

    private bool latched = false;

    void Update()
    {
        if (buttons == null || door == null) return;

        // If already latched, keep door open
        if (latchMode && latched)
        {
            door.OpenDoor();
            return;
        }

        bool allPressed = true;
        foreach (var b in buttons)
        {
            if (b == null) continue;
            if (!b.IsPressed())
            {
                allPressed = false;
                break;
            }
        }

        if (allPressed)
        {
            door.OpenDoor();
            if (latchMode) latched = true; // lock permanently
        }
        else
        {
            if (!latchMode) // only close if in live mode
                door.CloseDoor();
        }
    }
}
