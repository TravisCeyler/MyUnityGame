using UnityEngine;

public class LevelTracker : MonoBehaviour
{
    public bool[] levelCompleted = new bool[10];

    public void CompleteLevel(int levelNumber)
    {
        levelCompleted[levelNumber] = true;
        Debug.Log("LEVEL " + levelNumber + " COMPLETED!");
    }
}
