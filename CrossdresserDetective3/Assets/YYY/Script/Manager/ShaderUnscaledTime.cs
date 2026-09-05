using UnityEngine;

public class ShaderUnscaledTime : MonoBehaviour
{
    private static readonly int UnscaledTimeID =
        Shader.PropertyToID("_UnscaledTime");

    private void Update()
    {
        Shader.SetGlobalFloat(UnscaledTimeID, Time.unscaledTime);
    }
}