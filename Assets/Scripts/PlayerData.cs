using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public float[] position;

    public PlayerData(Transform playerTransform)
    {
        position = new float[3];
        position[0] = playerTransform.position.x;
        position[1] = playerTransform.position.y;
        position[2] = playerTransform.position.z;
    }
}
