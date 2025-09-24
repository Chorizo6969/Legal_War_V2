using UnityEngine;
using Unity.Services.Vivox;

public class PlayerVoice : MonoBehaviour
{
    [SerializeField] private string channelName = "InGameChannel";
    [SerializeField, Range(0.1f, 1f)] private float updateInterval = 0.25f; // 4x/s par défaut

    private float _timer;

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < updateInterval) return;
        _timer = 0f;

        if (VivoxService.Instance == null) return;

        if (VivoxService.Instance.ActiveChannels.ContainsKey(channelName))
        {
            VivoxService.Instance.Set3DPosition(gameObject, channelName, allowPanning: true);
        }
    }
}
