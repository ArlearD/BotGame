using Assets.Project.CodeNameData;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [SerializeField] InputField code1;
    [SerializeField] InputField code2;
    [SerializeField] InputField code3;
    [SerializeField] InputField code4;
    void Start()
    {
        if (Data.Player1Code != null)
        {
            code1.text = Data.Player1Code;
        }
        if (Data.Player1Code != null)
        {
            code2.text = Data.Player2Code;
        }
        if (Data.Player1Code != null)
        {
            code3.text = Data.Player3Code;
        }
        if (Data.Player1Code != null)
        {
            code4.text = Data.Player4Code;
        }
    }

    private void OnEnable()
    {
        Debug.Log("Yay");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
