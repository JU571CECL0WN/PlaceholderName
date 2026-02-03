using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [Header("UI States")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject inGameUI;

    public static CanvasManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Start(){
        menuUI.SetActive(true);
        inGameUI.SetActive(false);
    }

    public void StartGame(){
        Debug.Log("CanvasManager StartGame");
        menuUI.SetActive(false);
        inGameUI.SetActive(true);
    }
}
