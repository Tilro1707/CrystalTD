using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CrystalManager : MonoBehaviour
{
    private CrystalType selectedCrystal = CrystalType.None;

    [SerializeField] private int natureCount;
    [SerializeField] private int fireCount;
    [SerializeField] private int lightningCount;

    [SerializeField] private TMP_Text natureCountText;
    [SerializeField] private TMP_Text fireCountText;
    [SerializeField] private TMP_Text lightningCountText;

    [SerializeField] private Button natureButton;
    [SerializeField] private Button fireButton;
    [SerializeField] private Button lightningButton;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GameUIManager gameUIManager;

    private void Start()
    {
        UpdateCrystalUI();
        UpdateSelectionUI();
    }

    private void SelectCrystal(CrystalType type)
    {
        if (!waveManager.IsBuildPhase)
            return;

        if (gameUIManager.IsTutorialOpen())
            return;

        bool selected = false;

        switch (type)
        {
            case CrystalType.Fire:
                if (fireCount > 0)
                {
                    selectedCrystal = type;
                    selected = true;
                }
                break;

            case CrystalType.Nature:
                if (natureCount > 0)
                {
                    selectedCrystal = type;
                    selected = true;
                }
                break;

            case CrystalType.Lightning:
                if (lightningCount > 0)
                {
                    selectedCrystal = type;
                    selected = true;
                }
                break;
        }

        if (selected)
        {
            UpdateSelectionUI();
            AudioController.Instance.PlayCrystalSelect();
        }
    }

    private void TryAddCrystalToTower(Tower tower)
    {
        if (selectedCrystal == CrystalType.None)
            return;

        if (tower.AddCrystal(selectedCrystal))
        {
            switch (selectedCrystal)
            {
                case CrystalType.Fire:
                    fireCount--;
                    break;
                case CrystalType.Nature:
                    natureCount--;
                    break;
                case CrystalType.Lightning:
                    lightningCount--;
                    break;
            }

            UpdateCrystalUI();
            selectedCrystal = CrystalType.None;
            UpdateSelectionUI();
        }
    }

    private void OnBuildClick(InputValue input)
    {
        if (!input.isPressed)
            return;

        if (gameUIManager.IsTutorialOpen())
            return;

        if (!waveManager.IsBuildPhase)
            return;

        Vector2 worldPosition = GetMousePos();
        Collider2D hit = Physics2D.OverlapPoint(worldPosition);

        if (hit != null)
        {
            Tower tower = hit.GetComponent<Tower>();

            if (tower != null)
            {
                TryAddCrystalToTower(tower);
            }
            else
            {
                ClearSelection();
            }
        }
        else
        {
            ClearSelection();
        }
    }

    private Vector2 GetMousePos()
    {
        return Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );
    }

    private void ClearSelection()
    {
        selectedCrystal = CrystalType.None;
        UpdateSelectionUI();
    }

    public void SelectFire()
    {
        SelectCrystal(CrystalType.Fire);
    }

    public void SelectNature()
    {
        SelectCrystal(CrystalType.Nature);
    }

    public void SelectLightning()
    {
        SelectCrystal(CrystalType.Lightning);
    }

    public bool HasSelectedCrystal()
    {
        return selectedCrystal != CrystalType.None;
    }

    private void UpdateCrystalUI()
    {
        natureCountText.text = natureCount.ToString();
        fireCountText.text = fireCount.ToString();
        lightningCountText.text = lightningCount.ToString();

        natureButton.interactable = natureCount > 0;
        fireButton.interactable = fireCount > 0;
        lightningButton.interactable = lightningCount > 0;
    }

    private void UpdateSelectionUI()
    {
        SetButtonColor(natureButton, selectedCrystal == CrystalType.Nature);
        SetButtonColor(fireButton, selectedCrystal == CrystalType.Fire);
        SetButtonColor(lightningButton, selectedCrystal == CrystalType.Lightning);
    }

    private void SetButtonColor(Button button, bool selected)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = selected ? selectedColor : normalColor;
        colors.selectedColor = selected ? selectedColor : normalColor;
        button.colors = colors;
    }

    public void AddCrystal(CrystalType type)
    {
        switch (type)
        {
            case CrystalType.Fire:
                fireCount++;
                break;
            case CrystalType.Nature:
                natureCount++;
                break;
            case CrystalType.Lightning:
                lightningCount++;
                break;
        }

        UpdateCrystalUI();
    }
}
