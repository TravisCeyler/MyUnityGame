using UnityEngine;
using UnityEngine.Playables;
using StarterAssets; // if you're using Starter Assets package

public class CutsceneTrigger : MonoBehaviour
{
    public PlayableDirector timeline;
    public GameObject player;
    //private ThirdPersonController controller;

    void Start()
    {
       // controller = player.GetComponent<ThirdPersonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           // controller.enabled = false; // disable movement
            timeline.Play();
            timeline.stopped += OnCutsceneEnd;
        }
    }

    private void OnCutsceneEnd(PlayableDirector director)
    {
       // controller.enabled = true; // re-enable player control
    }
}
