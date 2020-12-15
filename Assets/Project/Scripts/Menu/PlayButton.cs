using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private Text errorMessageField;

    public void OnClick(InputField field)
    {
        if (field.text.Length < 3)
        {
            errorMessageField.text = "Nickname must be longer than two characters";
        }
        else
        {
            errorMessageField.text = "";
            mainMenu.SetActive(false);
            gameMenu.SetActive(true);
        }
    }
}
