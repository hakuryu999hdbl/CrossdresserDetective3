using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

[Serializable]
[DisplayName("跳过点 / Skip Point")]
[CustomStyle("SignalEmitter")]
public class TimelineSkipPoint : Marker
{
    [Tooltip("编辑用备注。正常播放经过此标记不会暂停或跳转。")]
    public string description = "下一段";
}
