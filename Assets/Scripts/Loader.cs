using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader 
{
    public enum Scence
    {
        MainMenuScence,
        GameScence,
        LoadingScence,
        LobbyScene,
        CharacterSelectScene,
    }

    private static Scence TargetScence;

    public static void Load(Scence targetScene)
    {
        Loader.TargetScence=targetScene;
        SceneManager.LoadScene(Scence.LoadingScence.ToString());
        
    }

    public static void LoadNetwork(Scence targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallBack()
    {
        SceneManager.LoadScene(TargetScence.ToString());
    }
    
}
