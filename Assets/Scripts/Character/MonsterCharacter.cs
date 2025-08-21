using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCharacter : CharacterBase
{
    StatComponentBase _player;
    StatComponentBase _monster;

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
        _player = Player.gameObject.GetComponent<StatComponentBase>();
        if (_player != null)
        {
            _player.statValues[StatType.Health].BaseValue -= _monster.statValues[StatType.Attack].BaseValue;
            Debug.Log("Player damaged: " + _monster.statValues[StatType.Attack].BaseValue + ". Remaining Health: " + _player.statValues[StatType.Health].BaseValue);
        }
    }

    void HitboxOn()
    {
        //히트박스 활성화
    }

    void HitboxOff()
    {
        //히트박스 비활성화
    }
}
