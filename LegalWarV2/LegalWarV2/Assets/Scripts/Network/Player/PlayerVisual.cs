using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _bodyMeshRenderer;

    private Mesh _skin;

    //private void Awake()
    //{
    //    _skin = new Material(_bodyMeshRenderer.material);
    //    foreach (MeshRenderer mesh in _othersBodyPartsMeshRenderers)
    //    {
    //        mesh.material = _skin;
    //    }
    //}

    public void SetColorPlayer(Mesh playerMesh)
    {
        _bodyMeshRenderer.sharedMesh = playerMesh;
    }
}
