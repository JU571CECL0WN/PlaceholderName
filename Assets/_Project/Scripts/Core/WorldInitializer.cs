using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldInitializer : MonoBehaviour
{
    [SerializeField] GridManager gridManagerPrefab;
    [SerializeField] GridGenerator gridGeneratorPrefab;
    [SerializeField] GridSelector gridSelectorPrefab;

    void Start(){
        var gridManager = Instantiate(gridManagerPrefab);     
        var generator = Instantiate(gridGeneratorPrefab);
        var selector = Instantiate(gridSelectorPrefab);

        CellData[,] map = generator.Generate();
        gridManager.SetMap(map);
        selector.Initialize(gridManager);
    }
}