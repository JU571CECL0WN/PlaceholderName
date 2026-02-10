using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private GameObject currencyUI;
    [SerializeField] private GameObject playerMenuUI;
    [SerializeField] private GameObject cellMenuUI;

    public static InGameUIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void EnterInGame()
    {
        Debug.Log("InGameUI EnterInGame");

        // Estado inicial limpio
        currencyUI.SetActive(true);
        playerMenuUI.SetActive(false);
        cellMenuUI.SetActive(false);
    }

        public void ShowPlayerMenu()
    {
        playerMenuUI.SetActive(true);
        cellMenuUI.SetActive(false);
    }

    public void ShowCellMenu()
    {
        cellMenuUI.SetActive(true);
        playerMenuUI.SetActive(false);
    }

    public void HideMenus()
    {
        playerMenuUI.SetActive(false);
        cellMenuUI.SetActive(false);
    }
}
