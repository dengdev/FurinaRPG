using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : Singleton<SceneController>
{
    Gameobject
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transionType)
        {
            case transitionPoint.transionType.SameScene:
                break;
            case transitionPoint.transionType.DifferentScene:
                break;
        }
    }

    IEnumerator Transition(string sceneName, TransitionDestination.DestionationTag destionationTag)
    {

    }
}
