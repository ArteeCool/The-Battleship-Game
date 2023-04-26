using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToScene : MonoBehaviour    
{
    private enum Scenes
    {
        PVPScene,
        Menu
    }

    [SerializeField] private Scenes scene;

    public void Move()
    {
        SceneManager.LoadScene(scene.ToString());
    }
}
