using UnityEngine.SceneManagement;

public class LoadingHandler : Singleton<LoadingHandler>
{
    /// <summary>
    /// </summary>
    private void Start()
    {
        SceneManager.LoadScene("Menu");
    }
}