using Assets.Project.CodeNameData;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private InputField Player1Code;

    [SerializeField] private InputField Player2Code;

    [SerializeField] private InputField Player3Code;

    [SerializeField] private InputField Player4Code;

    public void StartGame()
    {
        Data.Player1Code = Player1Code.text;
        Data.Player1Name = "Leonardo";

        Data.Player2Code = Player2Code.text;
        Data.Player2Name = "Raphael";

        Data.Player3Code = Player3Code.text;
        Data.Player3Name = "Donatello";

        Data.Player4Code = Player4Code.text;
        Data.Player4Name = "Michelangelo";

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}