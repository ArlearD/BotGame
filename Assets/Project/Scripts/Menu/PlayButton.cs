using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private Text errorMessageField;
    [SerializeField] private InputField nickName;

    public void OnClick(InputField field)
    {
        if (field.text.Length < 2)
        {
            errorMessageField.text = "Nickname must be longer than one characters";
        }
        else
        {
            errorMessageField.text = "";
            mainMenu.SetActive(false);
            gameMenu.SetActive(true);
            nickName.text = field.text;
        }
    }
}
