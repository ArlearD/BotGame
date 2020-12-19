using Assets.Scripts.Economy.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EconomyManagment : MonoBehaviour
{
    public void OnLeonardoManagment()
    {
        PlayersData.CurrentPlayer = CurrentPlayerEconomy.Leonardo;
        SceneManager.LoadScene(2);
    }

    public void OnRaphaelManagment()
    {
        PlayersData.CurrentPlayer = CurrentPlayerEconomy.Raphael;
        SceneManager.LoadScene(2);
    }

    public void OnDonatelloManagment()
    {
        PlayersData.CurrentPlayer = CurrentPlayerEconomy.Donatello;
        SceneManager.LoadScene(2);
    }

    public void OnMichelangeloManagment()
    {
        PlayersData.CurrentPlayer = CurrentPlayerEconomy.Michelangelo;
        SceneManager.LoadScene(2);
    }
}
