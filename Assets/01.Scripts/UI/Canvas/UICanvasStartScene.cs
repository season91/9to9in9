using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasStartScene : MonoBehaviour, IGUI
{
    public Button btnGameStart;
    
    public GameObject GUIObject => gameObject;

    public void Reset()
    {
        btnGameStart = GetComponentInChildren<Button>();
    }
    
    public void Initialization()
    {
    }

    public void Open()
    {
    }

    public void Close()
    {
    }
    
    public void GameStart()
    {
        _ = UIManager.Instance.OpenScene(SceneType.Main);
    }
}
