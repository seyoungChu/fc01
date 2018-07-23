using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventTest : MonoBehaviour {
    
    public void AnimationFinished(int index)
    {
        Debug.Log("Animation Finished!! " + index);
    }
}
