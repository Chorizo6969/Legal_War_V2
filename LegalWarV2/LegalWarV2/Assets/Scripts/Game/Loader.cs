using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        Lobby,
        LegalWar,
        TitleScreen
    }

    private static Scene _targetScene;

    public static void Load(Scene targetScene)
    {
        _targetScene = targetScene;
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void Quit() => Application.Quit();

    public static void StartGame() => SceneManager.LoadScene(Scene.Lobby.ToString());
}
