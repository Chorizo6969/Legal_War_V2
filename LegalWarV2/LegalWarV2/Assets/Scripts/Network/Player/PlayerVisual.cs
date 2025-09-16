using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _bodyMeshRenderer;

    public void SetColorPlayer(Mesh playerMesh)
    {
        _bodyMeshRenderer.sharedMesh = playerMesh;
    }
}
