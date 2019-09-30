using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestLoadingScene : MonoBehaviour
{
    [System.Serializable]
    public class SceneState
    {
        public string scene;
        public bool state;
    }

    public List<SceneState> scenes;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0)) ManageScene(KeyCode.Keypad0);
        if (Input.GetKeyDown(KeyCode.Keypad1)) ManageScene(KeyCode.Keypad1);
        if (Input.GetKeyDown(KeyCode.Keypad2)) ManageScene(KeyCode.Keypad2);
        if (Input.GetKeyDown(KeyCode.Keypad3)) ManageScene(KeyCode.Keypad3);
        if (Input.GetKeyDown(KeyCode.Keypad4)) ManageScene(KeyCode.Keypad4);
        if (Input.GetKeyDown(KeyCode.Keypad5)) ManageScene(KeyCode.Keypad5);
        if (Input.GetKeyDown(KeyCode.Keypad6)) ManageScene(KeyCode.Keypad6);
        if (Input.GetKeyDown(KeyCode.Keypad7)) ManageScene(KeyCode.Keypad7);
        if (Input.GetKeyDown(KeyCode.Keypad8)) ManageScene(KeyCode.Keypad8);
        if (Input.GetKeyDown(KeyCode.Keypad9)) ManageScene(KeyCode.Keypad9);
    }

    private void ManageScene(KeyCode code)
    {
        int i = (int)code - (int)KeyCode.Keypad0;
        Debug.Log(i);

        if (scenes[i].state)
            SceneManager.UnloadSceneAsync(scenes[i].scene);
        else
            SceneManager.LoadSceneAsync(scenes[i].scene, LoadSceneMode.Additive);

        scenes[i].state = !scenes[i].state;
    }
}
