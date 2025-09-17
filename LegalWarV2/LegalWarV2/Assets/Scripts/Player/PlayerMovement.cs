using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Mouvement du Perso (ZQSD, cam, sprint et saut)
/// </summary>
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _speed;
    [SerializeField] private CharacterController _cc;
    [SerializeField] private float _jumpHeight = 2f;
    private float _gravity = -9.81f;

    [Header("Camera")]
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private float _mouseSensitivity = 100f;

    [Header("AnimationRef")]
    [SerializeField] private PlayerAnim _playerAnimRef;

    [Header("Others")]
    [SerializeField] private PlayerVisual _playerVisualRef;
    [SerializeField] private GameObject _canvasRef;

    private Vector2 _inputVector;
    private Vector2 _lookVector;
    private Vector3 _velocity;
    private float _xRotation = 0f;

    private void Start()
    {
        PlayerData playerData = LegalWarNetworkManager.Instance.GetPlayerDataFromClientId(OwnerClientId);
        _playerVisualRef.SetColorPlayer(LegalWarNetworkManager.Instance.GetPlayerColor(playerData.meshID));

        if (!IsOwner)
            return;
        _canvasRef.SetActive(true);
    }

    #region LinkToInputSystem
    private InputSystem_Politician _controls;

    private void Awake()
    {
        _controls = new InputSystem_Politician();

        _controls.Game.Move.performed += ctx => Movement(ctx);
        _controls.Game.Move.canceled += ctx => Movement(ctx);

        _controls.Game.Camera.performed += ctx => Look(ctx);
        _controls.Game.Camera.canceled += ctx => Look(ctx);

        _controls.Game.Jump.performed += ctx => Jump(ctx);
    }

    private void OnEnable() => _controls.Enable();

    private void OnDisable() => _controls.Disable();

    public void DisableControls() => _controls.Disable();

    public void EnableControls() => _controls.Enable();

    #endregion

    #region InputSection
    public void Movement(InputAction.CallbackContext ctx)
    {
        _inputVector = ctx.ReadValue<Vector2>();

        if (ctx.performed)
            _playerAnimRef.TransitionTo(AnimEnum.IsWalking, true);

        else if (ctx.canceled)
            _playerAnimRef.TransitionTo(AnimEnum.IsWalking, false);


    }

    public void Look(InputAction.CallbackContext ctx) { _lookVector = ctx.ReadValue<Vector2>(); }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && _cc.isGrounded)
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

        else if (ctx.canceled && _velocity.y > 0)
            _velocity.y *= 0.5f;
    }
    #endregion

    private void Update()
    {
        if (!IsOwner)
            return;
        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        Vector3 move = transform.right * _inputVector.x + transform.forward * _inputVector.y;
        _cc.Move(move * _speed * Time.deltaTime);

        if (_cc.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
            _playerAnimRef.TransitionTo(AnimEnum.IsJumping, false);
        }
    
        _velocity.y += _gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        float mouseX = _lookVector.x * _mouseSensitivity * Time.deltaTime;
        float mouseY = _lookVector.y * _mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 37f);
        _playerCamera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }


    [ClientRpc]
    public void TeleportClientRpc(Vector3 position, ClientRpcParams clientRpcParams = default)
    {
        // Ce code s'exécute sur le client ciblé
        Debug.Log($"TeleportClientRpc reçu sur client {NetworkManager.Singleton.LocalClientId}: {position}");
        var cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        transform.position = position;
        if (cc != null) cc.enabled = true;
    }

    [ClientRpc]
    public void EnableControlsClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        EnableControls();
        Debug.Log("EnableControlsClientRpc exécuté (owner)");
    }

    [ClientRpc]
    public void DisableControlsClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        DisableControls();
    }
}
