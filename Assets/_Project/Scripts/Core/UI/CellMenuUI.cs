using UnityEngine;

public class CellMenuUI : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private GameObject exitButton;
    [SerializeField] private GameObject createPanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject rootPanel;

    public static CellMenuUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ShowCreatePanel()
    {
        rootPanel.SetActive(true);
        createPanel.SetActive(true);
        upgradePanel.SetActive(false);
    }

    public void ShowUpgradePanel()
    {
        rootPanel.SetActive(true);
        upgradePanel.SetActive(true);
        createPanel.SetActive(false);
    }

    public void OnExitPressed()
    {
        InGameUIManager.Instance.ShowPlayerMenu(); 
        rootPanel.SetActive(false);
    }
}
