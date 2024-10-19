using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    public Button startButton;
    public Button ShopButton;
    public Button OptionButton;

    private UnityAction action;

    void Start()
    {
        //unityaction을 사용한 이벤트 연결 방식
        action = () => OnStartClick();
        startButton.onClick.AddListener(action);

        // 무명 메세드를 활용한 이벤트 연결 방식
        OptionButton.onClick.AddListener(delegate { OnButtonClick(OptionButton.name); });

        // 람다식을 활용한 이벤트 연결 방식
        ShopButton.onClick.AddListener(() => OnButtonClick (ShopButton.name));
    }
    public void OnButtonClick(string msg)
    {
        Debug.Log($"Click Button: {msg}");
    }

    public void OnStartClick()
    {
        SceneManager.LoadScene("Level_01");
        SceneManager.LoadScene("Play", LoadSceneMode.Additive);
    }
}
