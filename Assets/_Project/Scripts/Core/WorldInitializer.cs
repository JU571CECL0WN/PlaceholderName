using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldInitializer : MonoBehaviour
{
    [SerializeField] GridManager gridManagerPrefab;
    [SerializeField] GridSelector gridSelectorPrefab;
    [SerializeField] GridGenerator gridGeneratorPrefab;

    [SerializeField] Tilemap floor;
    [SerializeField] Tilemap walls;
    [SerializeField] Tilemap doors;

    void Start(){
        var gridManager = Instantiate(gridManagerPrefab);

        var selector = Instantiate(gridSelectorPrefab);
        selector.Initialize(gridManager);
        
        var generator = Instantiate(gridGeneratorPrefab);
        generator.Initialize(floor, walls, doors);
        generator.Generate();
    }
}