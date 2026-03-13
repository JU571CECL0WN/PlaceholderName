using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private GameObject currencyUI;
    [SerializeField] private GameObject playerMenuUI;
    [SerializeField] private CellMenuUI cellMenuUI;

    public static InGameUIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void EnterInGame()
    {
        currencyUI.SetActive(true);
        playerMenuUI.SetActive(true);
        cellMenuUI.gameObject.SetActive(false);
    }

    public void ShowPlayerMenu()
    {
        playerMenuUI.SetActive(true);
        cellMenuUI.gameObject.SetActive(false);
    }

    public void ShowCellCreatePanel()
    {
        cellMenuUI.gameObject.SetActive(true);
        cellMenuUI.ShowCreatePanel();
        playerMenuUI.SetActive(false);
    }

    public void ShowCellUpgradePanel()
    {
        cellMenuUI.gameObject.SetActive(true);
        cellMenuUI.ShowUpgradePanel();
        playerMenuUI.SetActive(false);
    }

    public void HideMenus()
    {
        playerMenuUI.SetActive(false);
        cellMenuUI.gameObject.SetActive(false);
    }
}
