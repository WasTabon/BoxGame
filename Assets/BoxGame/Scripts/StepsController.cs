using System.Collections.Generic;
using UnityEngine;

public class StepsController : MonoBehaviour
{
    public List<AudioClip> footSteps;

    public void PlayFootstep()
    {
        int random = Random.Range(0, footSteps.Count);
        
        MusicController.Instance.PlaySpecificSound(footSteps[random]);
    }
}
