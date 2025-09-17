using Unity.Netcode;

public class ShowPlayerUI : NetworkBehaviour
{
    private void Start()
    {
        if (!IsOwner)
            return;
        gameObject.SetActive(true);
    }
}
