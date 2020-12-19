using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameMenu;

    public void OnClick()
    {

        mainMenu.SetActive(false);
        gameMenu.SetActive(true);
    }
}
