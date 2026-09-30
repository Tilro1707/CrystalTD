using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class BuildManager : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap pathTilemap;
    [SerializeField] private Tilemap blockedTilemap;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private CrystalManager crystalManager;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GameUIManager gameUIManager;

    [SerializeField] private TextMeshProUGUI towerText;
    private int currentTowers = 2;

    private readonly HashSet<Vector3Int> occupiedCells = new();

    private void Start()
    {
        UpdateTowerUI();
    }

    private void OnBuildClick(InputValue value)
    {

        if (gameUIManager.IsTutorialOpen())
            return;

        if (!value.isPressed)
            return;

        if (!waveManager.IsBuildPhase)
            return;

        if (crystalManager.HasSelectedCrystal())
            return;

        Vector2 worldPosition = GetMousePos();
        Vector3Int cellPosition = grid.WorldToCell(worldPosition);

        if (CanBuild(cellPosition) &&
            !occupiedCells.Contains(cellPosition) &&
            currentTowers > 0)
        {
            Vector3 buildPosition = grid.GetCellCenterWorld(cellPosition);

            Instantiate(towerPrefab, buildPosition, Quaternion.identity);
            occupiedCells.Add(cellPosition);
            currentTowers--;

            AudioController.Instance.PlayTowerPlace();
            UpdateTowerUI();
        }
    }

    private Vector2 GetMousePos()
    {
        return Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );
    }

    private bool CanBuild(Vector3Int cellPosition)
    {
        bool hasGround = groundTilemap.HasTile(cellPosition);
        bool isPath = pathTilemap.HasTile(cellPosition);
        bool isBlocked = blockedTilemap.HasTile(cellPosition);

        return hasGround && !isPath && !isBlocked;
    }

    public void AddTower()
    {
        currentTowers++;
        UpdateTowerUI();
    }

    private void UpdateTowerUI()
    {
        towerText.text = $"Placeable Towers: {currentTowers}";
    }
}
