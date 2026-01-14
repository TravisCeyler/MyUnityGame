using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FootstepAudio : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSource;
    public CharacterController controller;

    [Header("Footstep Sounds")]
    public AudioClip grassWalk;
    public AudioClip grassRun;

    public AudioClip stoneWalk;
    public AudioClip stoneRun;

    public AudioClip sandWalk;
    public AudioClip sandRun;

    public AudioClip woodwalk;
    public AudioClip woodRun;

    [Header("Settings")]
    public float walkStepDelay = 0.5f;
    public float runStepDelay = 0.3f;
    public float rayDistance = 1.5f;

    private float stepTimer;

    void Start()
    {
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (!controller) controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!controller.isGrounded || controller.velocity.magnitude < 0.1f)
        {
            stepTimer = 0f;
            return;
        }

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            PlayFootstep(isRunning);
            stepTimer = isRunning ? runStepDelay : walkStepDelay;
        }
    }

    void PlayFootstep(bool isRunning)
    {
        AudioClip clip = GetTerrainClip(isRunning);

        if (clip == null) return;

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(clip);
    }

    AudioClip GetTerrainClip(bool isRunning)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance))
        {
            switch (hit.collider.tag)
            {
                case "Grass":
                    return isRunning ? grassRun : grassWalk;

                case "Stone":
                    return isRunning ? stoneRun : stoneWalk;

                case "Sand":
                    return isRunning ? sandRun : sandWalk;
                case "Wood":
                    return isRunning ? woodRun : woodwalk;
            }
        }

        return null;
    }
}
