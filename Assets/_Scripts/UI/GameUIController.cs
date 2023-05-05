using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Security;

public class GameUIController : MonoBehaviour
{
    public PlayerController2_0 PlayerController;

    [Header("InGame")]
    public GameObject InGameUI;
    public Image HpBar;
    public Text HealthNumber;
    public Button PauseBtn;

    [Header("Pause")]
    public GameObject Pause;
    public Button ContinueBtn;
    public Button HomeBtn;

    [Header("Lose")]
    public GameObject Lose;
    public Button Replay;
    public Button BackBtn;

    [Header("Win")]
    public GameObject Win;
    public Button Again;
    public Button MainBtn;

    // Start is called before the first frame update
    void Start()
    {
        InGameUI.SetActive(true);
        Pause.SetActive(false); 
        Lose.SetActive(false); 
        Win.SetActive(false);

        PauseBtn?.onClick.AddListener(() => PauseGame());

        ContinueBtn?.onClick.AddListener(() => ContinueGame());
        HomeBtn?.onClick.AddListener(() => BackMenu());

        Replay?.onClick.AddListener(() => ReplayGame());
        BackBtn?.onClick.AddListener(() => BackMenu());

        Again?.onClick.AddListener(() => ReplayGame());
        MainBtn?.onClick.AddListener(() => BackMenu());
    }

    public void UpdatePlayerHp()
    {
        HpBar.fillAmount = (PlayerController.PlayerCurrentHealth + 1) / PlayerController.PlayerHealth;
        HealthNumber.text = $"x{PlayerController.PlayerCurrentHealth + 1}";
    }

    public void PauseGame()
    {
        Pause.SetActive(true);
        Time.timeScale = 0;
    }

    public void ContinueGame()
    {
        Pause.SetActive(false);
        Time.timeScale = 1;
    }

    public void BackMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void PlayerLoseGame()
    {
        Lose.SetActive(true);
    }

    public void ReplayGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayerWinGame()
    {
        Win.SetActive(true);
        Time.timeScale = 0;
    }
}
