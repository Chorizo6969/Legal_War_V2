using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;

public class VivoxManager : MonoBehaviour
{
    public static VivoxManager Instance;

    private async void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        await InitVivox();
        await JoinTestChannel(); // On rejoint un canal de test
    }

    private async Task InitVivox()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        await VivoxService.Instance.InitializeAsync();

        await VivoxService.Instance.LoginAsync(new LoginOptions()
        {
            DisplayName = "Player_" + AuthenticationService.Instance.PlayerId
        });

        Debug.Log("Vivox prêt et joueur connecté !");
    }

    private async Task JoinTestChannel()
    {
        string channelName = "GlobalTest"; // Nom du canal (même pour tous les joueurs)

        await VivoxService.Instance.JoinGroupChannelAsync(channelName, ChatCapability.AudioOnly);

        Debug.Log($"Rejoint le canal vocal : {channelName}");
    }

    public async Task LeaveTestChannel()
    {
        string channelName = "GlobalTest"; // Nom du canal (même pour tous les joueurs)

        await VivoxService.Instance.LeaveChannelAsync(channelName);
    }

    public async Task JoinProximityChannel()
    {
        string channelName = "InGameChannel";

        Channel3DProperties props = new Channel3DProperties(
            audibleDistance: 12, //On entends ma voix jusqu'a 32m
            conversationalDistance: 2, //Voix pleine jusqu'a 1m
            audioFadeIntensityByDistanceaudio: 1f, //Fade intensity = 1
            audioFadeModel: AudioFadeModel.InverseByDistance
        );

        ChannelOptions channelOptions = new ChannelOptions { MakeActiveChannelUponJoining = true }; //Quand j'entre dans ce canal il devient prio pour capter mon micro

        await VivoxService.Instance.JoinPositionalChannelAsync(channelName, ChatCapability.AudioOnly, props, channelOptions);

        Debug.Log($"Requested join positional channel: {channelName}"); //Affiché
    }
}
