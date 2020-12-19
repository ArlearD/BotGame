using UnityEngine;
using UnityEngine.SceneManagement;

public class EconomyController : MonoBehaviour
{
    [SerializeField] Economy.Player player;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnExitButton()
    {
        player.SaveData();

        SceneManager.LoadScene(0);
    }
}
