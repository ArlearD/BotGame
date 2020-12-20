using Assets.Scripts.Economy.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EconomyManagment : MonoBehaviour
{
    public void OnLeonardoManagment()
    {
        PlayersData.CurrentPlayer = EconomyPlayerType.Leonardo;
        SceneManager.LoadScene(2);
    }

    public void OnRaphaelManagment()
    {
        PlayersData.CurrentPlayer = EconomyPlayerType.Raphael;
        SceneManager.LoadScene(2);
    }

    public void OnDonatelloManagment()
    {
        PlayersData.CurrentPlayer = EconomyPlayerType.Donatello;
        SceneManager.LoadScene(2);
    }

    public void OnMichelangeloManagment()
    {
        PlayersData.CurrentPlayer = EconomyPlayerType.Michelangelo;
        SceneManager.LoadScene(2);
    }
}
