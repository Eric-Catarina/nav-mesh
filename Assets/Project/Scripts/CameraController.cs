
using UnityEngine;

/*
    This file has a commented version with details about how each line works. 
    The commented version contains code that is easier and simpler to read. This file is minified.
*/

/// <summary>
/// Camera movement script for third person games.
/// This Script should not be applied to the camera! It is attached to an empty object and inside
/// it (as a child object) should be your game's MainCamera.
/// </summary>
public class CameraController : MonoBehaviour
{

    [Tooltip("Enable to move the camera by holding the right mouse button. Does not work with joysticks.")]
    public bool clickToMoveCamera = false;
    [Tooltip("Enable zoom in/out when scrolling the mouse wheel. Does not work with joysticks.")]
    public bool canZoom = true;
    [Space]
    [Tooltip("The higher it is, the faster the camera moves. It is recommended to increase this value for games that uses joystick.")]
    public float sensitivity = 5f;

    [Tooltip("Camera Y rotation limits. The X axis is the maximum it can go up and the Y axis is the maximum it can go down.")]
    public Vector2 cameraLimit = new Vector2(-45, 40);

    float mouseY;
    float offsetDistanceY = 1.5f;
    float distanciaAtras = 4f;

    Transform alvo;

    public void DefinirAlvo(Transform novoAlvo)
    {
        alvo = novoAlvo;
    }

    void Start()
    {
        if (alvo == null)
        {
            GameObject jogador = GameObject.FindWithTag("Player");
            if (jogador != null)
                alvo = jogador.transform;
        }

        if (!clickToMoveCamera)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        if (alvo == null)
            return;

        if (canZoom && Input.GetAxis("Mouse ScrollWheel") != 0f)
            Camera.main.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * sensitivity * 2;

        if (!clickToMoveCamera || Input.GetAxisRaw("Fire2") != 0f)
        {
            mouseY -= Input.GetAxis("Mouse Y") * sensitivity;
            mouseY = Mathf.Clamp(mouseY, cameraLimit.x, cameraLimit.y);
        }

        transform.position = alvo.position + Vector3.up * offsetDistanceY;
        transform.rotation = Quaternion.Euler(0f, alvo.eulerAngles.y, 0f);

        if (Camera.main != null)
        {
            Camera.main.transform.localPosition = new Vector3(0f, 0f, -distanciaAtras);
            Camera.main.transform.localRotation = Quaternion.Euler(mouseY, 0f, 0f);
        }
    }
}