using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    public UnityAction<Character> OnEventRaised;//通过事件订阅实现跨场景传输脚本
    public void RaiseEvent(Character character) 
    {
        OnEventRaised?.Invoke(character);//事件的调用
    }
}
