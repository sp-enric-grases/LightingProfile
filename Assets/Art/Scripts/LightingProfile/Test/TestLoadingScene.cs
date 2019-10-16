using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestLoadingScene : MonoBehaviour
{
    [System.Serializable]
    public class SceneState
    {
        public string scene;
        [HideInInspector]
        public bool state;
    }

    public List<SceneState> scenes;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) ManageScene(KeyCode.Alpha0);
        if (Input.GetKeyDown(KeyCode.Alpha1)) ManageScene(KeyCode.Alpha1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ManageScene(KeyCode.Alpha2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ManageScene(KeyCode.Alpha3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ManageScene(KeyCode.Alpha4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ManageScene(KeyCode.Alpha5);
        if (Input.GetKeyDown(KeyCode.Alpha6)) ManageScene(KeyCode.Alpha6);
        if (Input.GetKeyDown(KeyCode.Alpha7)) ManageScene(KeyCode.Alpha7);
        if (Input.GetKeyDown(KeyCode.Alpha8)) ManageScene(KeyCode.Alpha8);
        if (Input.GetKeyDown(KeyCode.Alpha9)) ManageScene(KeyCode.Alpha9);
    }

    private void ManageScene(KeyCode code)
    {
        int i = (int)code - (int)KeyCode.Alpha0;

        if (scenes[i].state)
            SceneManager.UnloadSceneAsync(scenes[i].scene);
        else
            SceneManager.LoadSceneAsync(scenes[i].scene, LoadSceneMode.Additive);

        scenes[i].state = !scenes[i].state;
    }
}
