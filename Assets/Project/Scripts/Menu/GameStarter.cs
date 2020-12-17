using Assets.Project.CodeNameData;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private InputField Player1Code;
    [SerializeField] private InputField Player1Name;

    [SerializeField] private InputField Player2Code;
    [SerializeField] private InputField Player2Name;

    [SerializeField] private InputField Player3Code;
    [SerializeField] private InputField Player3Name;

    [SerializeField] private InputField Player4Code;
    [SerializeField] private InputField Player4Name;

    public void StartGame()
    {
        Data.Player1Code = Player1Code.text;
        Data.Player1Name = Player1Name.text;

        Data.Player2Code = Player2Code.text;
        Data.Player2Name = Player2Name.text;

        Data.Player3Code = Player3Code.text;
        Data.Player3Name = Player3Name.text;

        Data.Player4Code = Player4Code.text;
        Data.Player4Name = Player4Name.text;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
