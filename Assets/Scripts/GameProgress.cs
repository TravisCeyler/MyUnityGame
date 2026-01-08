using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance;

    [Header("Main Scene Progress")]
    public bool talkedToBoss = false;

    [Header("Medieval Code Progress")]
public int[] medievalCode = new int[5] { -1, -1, -1, -1, -1 };

[Header("Main Scene Progress")]
    public bool talkedToKing = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
