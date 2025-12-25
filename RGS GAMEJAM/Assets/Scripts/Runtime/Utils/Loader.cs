using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
public static class Loader
{
    public enum Scene
    {
        StartScene = 0,
        ClientConnect,
        Lobby,
        GameScene,
        HostConnect
    }

    private static AsyncOperation loadingOperation;

    private class LoaderRunner : MonoBehaviour { }

    //씬 비동기로드
    public static void Load(Scene scene)
    {
        GameObject runnerObj = new GameObject("LoaderRunner");
        LoaderRunner runner = runnerObj.AddComponent<LoaderRunner>();
        runner.StartCoroutine(LoadAsync(scene, runnerObj));
    }

    private static IEnumerator LoadAsync(Scene scene, GameObject runnerObj)
    {
        loadingOperation = SceneManager.LoadSceneAsync(scene.ToString());
        loadingOperation.allowSceneActivation = true;

        while (!loadingOperation.isDone)
        {
            yield return null;
        }

        UnityEngine.Object.Destroy(runnerObj);
    }
}
