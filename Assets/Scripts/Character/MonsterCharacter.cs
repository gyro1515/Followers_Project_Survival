using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCharacter : MonoBehaviour
{
    StatComponentBase _player;
    StatComponentBase _monster;
    public bool isAttacked = false;

    private void Awake()
    {
        _monster = GetComponent<StatComponentBase>();
    }
    void OnTriggerEnter(Collider Player)
    {
        if (!Player.gameObject.CompareTag("Player"))
        {
            return;
        }
        if (isAttacked)
        {
            return;
        }
        _player = Player.gameObject.GetComponent<StatComponentBase>();
        if (_player != null)
        {
            _player.statValues[StatType.Health].BaseValue -= _monster.statValues[StatType.Attack].BaseValue;
            Debug.Log("Player damaged: " + _monster.statValues[StatType.Attack].BaseValue + ". Remaining Health: " + _player.statValues[StatType.Health].BaseValue);
        }
    }
}
