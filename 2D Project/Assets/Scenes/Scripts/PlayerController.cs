using UnityEngine;
using UnityEngine.Audio;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class PlayerController : MonoBehaviour
{
    [Tooltip("Units per second")]
    public float moveSpeed = 2f;
    public AudioSource audioSource;
    void Start()
    {
        // If not assigned manually, grab the AudioSource on the same GameObject
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                // Add a default AudioSource so Play() calls won't throw
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0f; // 2D sound by default
            }
        }
    }

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        Vector2 input = Vector2.zero;
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) input.x = -1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) input.x = 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) input.y = -1f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) input.y = 1f;
            // Only play when space is pressed down this frame
            if (kb.spaceKey != null && kb.spaceKey.wasPressedThisFrame)
                PlayAudioIfPossible();
        }
        if (input != Vector2.zero)
        {
            transform.Translate(new Vector3(input.x, input.y, 0f) * moveSpeed * Time.deltaTime, Space.World);
        }
#else
        // Old Input Manager (or Both) — use GetAxisRaw
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 delta = new Vector3(x, y, 0f) * moveSpeed * Time.deltaTime;
        if (delta != Vector3.zero)
        {
            transform.Translate(delta, Space.World);
        }
        // Play sound on space press (old Input Manager)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayAudioIfPossible();
        }
#endif
    }

    private void PlayAudioIfPossible()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("[PlayerController] No AudioSource attached or assigned to play audio.");
            return;
        }
        if (audioSource.clip == null)
        {
            Debug.LogWarning("[PlayerController] AudioSource has no AudioClip assigned. Assign a clip in the Inspector.");
            return;
        }
        audioSource.Play();
        Debug.Log($"[PlayerController] Playing AudioClip '{audioSource.clip.name}'");
    }
}
