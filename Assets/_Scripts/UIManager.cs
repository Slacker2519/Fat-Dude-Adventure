using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject InGameUI;

    public GameObject Menu;
    public Button PlayBtn;
    public Button ExitBtn;

    public GameObject Pause;
    public Button Continue;
    public Button HomeBtn;

    public GameObject Lose;
    public Button Replay;
    public Button BackBtn;

    public GameObject Win;
    public Button Again;
    public Button MainBtn;

    // Start is called before the first frame update
    void Start()
    {
        InGameUI.SetActive(false);
        Menu.SetActive(true);
        Pause.SetActive(false); 
        Lose.SetActive(false); 
        Win.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
