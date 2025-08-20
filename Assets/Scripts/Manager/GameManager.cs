using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    List<PlayerCharacter> players = new List<PlayerCharacter>();

    protected override void Awake()
    {
        base.Awake();
        SetCursorVisibility(false);

        PlayerCharacter player = GetPlayer(0);
        if (player == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Player");
            player = Instantiate(prefab).GetComponent<PlayerCharacter>();
        }

    }

    private void Start()
    {
        InitializeHUD();

    }

    private void InitializeHUD()
    {
        PlayerCharacter player = GetPlayer(0);

        if (player.GetStatComponent<PlayerStatComponent>().statValues.Count > 0)
        {
            StatValue statValue;
            if (player.GetStatComponent<PlayerStatComponent>().statValues.TryGetValue(StatType.Health, out statValue))
            {
                statValue.OnValueChanged += UIManager.Instance.HUDUI.UpdateGaugeBar;
                statValue.RecalculateFinalValue();
            }
            if (player.GetStatComponent<PlayerStatComponent>().statValues.TryGetValue(StatType.Hunger, out statValue))
            {
                statValue.OnValueChanged += UIManager.Instance.HUDUI.UpdateGaugeBar;
                statValue.RecalculateFinalValue();

            }
            if (player.GetStatComponent<PlayerStatComponent>().statValues.TryGetValue(StatType.Stamina, out statValue))
            {
                statValue.OnValueChanged += UIManager.Instance.HUDUI.UpdateGaugeBar;
                statValue.RecalculateFinalValue();

            }
            if (player.GetStatComponent<PlayerStatComponent>().statValues.TryGetValue(StatType.Thirst, out statValue))
            {
                statValue.OnValueChanged += UIManager.Instance.HUDUI.UpdateGaugeBar;
                statValue.RecalculateFinalValue();

            }
        }
    }

    public void AddPlayer(PlayerCharacter player)
    {
        Debug.Assert(player != null);
        players.Add(player);
    }

    public PlayerCharacter GetPlayer(int id)
    {
        if (id >= players.Count)
        {
            return null;
        }

        return players[id];
    }

    public void SetCursorVisibility(bool visible)
    {
        if (visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
