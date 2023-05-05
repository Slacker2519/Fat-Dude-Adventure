using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] Button _playBtn;
    [SerializeField] Button _exitBtn;

    // Start is called before the first frame update
    void Start()
    {
        _playBtn?.onClick.AddListener(() => SceneManager.LoadScene(1));
        _playBtn?.onClick.AddListener(() => Time.timeScale = 1);

        _exitBtn?.onClick.AddListener(() => Application.Quit());
    }
}
