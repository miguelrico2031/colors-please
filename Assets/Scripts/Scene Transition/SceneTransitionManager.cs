
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour, ISceneTransitionService
{

    public void TransitionToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
