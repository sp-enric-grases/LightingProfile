using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestLoadingScene : MonoBehaviour
{
    public string sceneToLoad1;
    public string sceneToLoad2;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadSceneAsync(sceneToLoad1, LoadSceneMode.Additive);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadSceneAsync(sceneToLoad2, LoadSceneMode.Additive);
        }
    }
}
