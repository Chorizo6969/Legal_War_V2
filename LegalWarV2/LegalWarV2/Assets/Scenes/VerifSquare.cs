using UnityEngine;

public class VerifSquare : MonoBehaviour
{
    [SerializeField] private GameObject _square;
    [SerializeField] private GameObject _camera;

    private void Update()
    {
        if (Vector3.Angle(_camera.transform.forward, _square.transform.position - _camera.transform.position) > 45f)
            print("nik");
        else
            print("good");
    }
}
