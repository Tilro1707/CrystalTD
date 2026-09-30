using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    [SerializeField] private CrystalManager crystalManager;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private BuildManager buildManager;
    [SerializeField] private GameObject rewardPanel;

    [SerializeField] private TextMeshProUGUI textSlot1;
    [SerializeField] private TextMeshProUGUI textSlot2;
    [SerializeField] private TextMeshProUGUI textSlot3;

    public enum RewardType
    {
        FireCrystal,
        NatureCrystal,
        LightningCrystal,
        Heal,
        ExtraTower,
    }

    private List<RewardType> currentRewards;

    public void ShowRewards()
    {
        rewardPanel.SetActive(true);
        currentRewards = new List<RewardType>();

        while (currentRewards.Count < 3)
        {
            RewardType newReward = (RewardType)Random.Range(0, 5);

            if (!currentRewards.Contains(newReward))
            {
                currentRewards.Add(newReward);
            }
        }

        textSlot1.SetText(GetRewardText(currentRewards[0]));
        textSlot2.SetText(GetRewardText(currentRewards[1]));
        textSlot3.SetText(GetRewardText(currentRewards[2]));
    }

    private string GetRewardText(RewardType reward)
    {
        switch (reward)
        {
            case RewardType.FireCrystal:
                return "+1 Fire Crystal";
            case RewardType.NatureCrystal:
                return "+1 Nature Crystal";
            case RewardType.LightningCrystal:
                return "+1 Lightning Crystal";
            case RewardType.Heal:
                return "+3 Lives";
            case RewardType.ExtraTower:
                return "+1 Tower";
            default:
                return "Error";
        }
    }

    public void ChooseReward(int slot)
    {
        if (currentRewards == null || slot < 0 || slot >= currentRewards.Count)
            return;

        RewardType reward = currentRewards[slot];

        switch (reward)
        {
            case RewardType.FireCrystal:
                crystalManager.AddCrystal(CrystalType.Fire);
                break;
            case RewardType.NatureCrystal:
                crystalManager.AddCrystal(CrystalType.Nature);
                break;
            case RewardType.LightningCrystal:
                crystalManager.AddCrystal(CrystalType.Lightning);
                break;
            case RewardType.Heal:
                waveManager.GainLife(3);
                break;
            case RewardType.ExtraTower:
                buildManager.AddTower();
                break;
        }

        AudioController.Instance.PlayReward();
        rewardPanel.SetActive(false);
        currentRewards = null;
        waveManager.RewardChosen();
    }
}
