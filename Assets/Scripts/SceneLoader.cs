using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        LoadScenes();
    }

    private static void LoadScenes()
    {
        var activeScene = SceneManager.GetActiveScene();

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            if (activeScene.buildIndex == i) continue;

            if (SceneManager.GetSceneByBuildIndex(i).IsValid()) continue;

            SceneManager.LoadScene(i, LoadSceneMode.Additive);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadScenes());
        }
    }

    private IEnumerator ReloadScenes()
    {
        List<int> sceneIds = new List<int>();
        
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            sceneIds.Add(SceneManager.GetSceneAt(i).buildIndex);
        }
        
        var activeSceneId = SceneManager.GetActiveScene().buildIndex;
        var unloadingScenes = new List<AsyncOperation>();
        
        foreach (var id in sceneIds)
        {
            if (id == activeSceneId)
            {
                continue;
            }
            
            unloadingScenes.Add(SceneManager.UnloadSceneAsync(id));
        }
        
        if(unloadingScenes.Count >= 0)
        {
            yield return new WaitUntil(() => unloadingScenes.TrueForAll(op => op.isDone));
        }

        foreach (var id in sceneIds)
        {
            if (id == activeSceneId)
            {
                SceneManager.LoadScene(id , LoadSceneMode.Single);
                continue;
            } 
            SceneManager.LoadScene(id , LoadSceneMode.Additive);
        }
            
        yield return null;
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(0));
    }
}
