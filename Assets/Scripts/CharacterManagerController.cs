using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterManagerController : MonoBehaviour
{
    public static CharacterManagerController Instance { get; private set; }

    [Header("Characters")]
    public GameObject[] characterArray;
    public int currentCharacter = 0;
    public int unlockedCharacters = 1;

    [Header("Camera")]
    [SerializeField] private CameraFollower cameraFollower;

    private bool _subscribed;

    public GameObject CurrentCharacterGameObject => characterArray[currentCharacter];

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetActiveCharacter(currentCharacter);
        Subscribe();
    }

    void OnEnable() => Subscribe();
    void OnDisable() => Unsubscribe();
    void OnDestroy() => Unsubscribe();

    private void Subscribe()
    {
        if (_subscribed || InputManager.Instance == null) return;

        InputManager.Instance.OnOne += HandleOne;
        InputManager.Instance.OnTwo += HandleTwo;
        InputManager.Instance.OnThree += HandleThree;
        _subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_subscribed) return;

        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnOne -= HandleOne;
            InputManager.Instance.OnTwo -= HandleTwo;
            InputManager.Instance.OnThree -= HandleThree;
        }
        _subscribed = false;
    }

    private void HandleOne(bool pressed)
    {
        if (pressed && unlockedCharacters >= 1)
            SwitchCharacter(0);
    }

    private void HandleTwo(bool pressed)
    {
        if (pressed && unlockedCharacters >= 2)
            SwitchCharacter(1);
    }

    private void HandleThree(bool pressed)
    {
        if (pressed && unlockedCharacters >= 3)
            SwitchCharacter(2);
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            int nextIndex = (currentCharacter + 1) % characterArray.Length;
            SwitchCharacter(nextIndex);
        }
    }

    public void SwitchCharacter(int newIndex)
    {
        if (newIndex == currentCharacter) return;
        if (newIndex < 0 || newIndex >= characterArray.Length) return;

        GameObject oldChar = characterArray[currentCharacter];
        GameObject newChar = characterArray[newIndex];
        if (!oldChar || !newChar) return;

        Vector3 savedPos = oldChar.transform.position;
        Quaternion savedRot = oldChar.transform.rotation;

        // deaktivuje starý charakter a jeho Rigidbody
        Rigidbody2D oldRb2D = oldChar.GetComponent<Rigidbody2D>();
        if (oldRb2D != null)
        {
            oldRb2D.linearVelocity = Vector2.zero;
            oldRb2D.angularVelocity = 0f;
            oldRb2D.isKinematic = true;
        }

        oldChar.SetActive(false);

        // aktivuje nový charakter
        newChar.transform.SetPositionAndRotation(savedPos, savedRot);
        newChar.SetActive(true);

        Rigidbody2D newRb2D = newChar.GetComponent<Rigidbody2D>();
        if (newRb2D != null)
        {
            newRb2D.isKinematic = false;
            newRb2D.linearVelocity = Vector2.zero;
            newRb2D.angularVelocity = 0f;
        }

        cameraFollower?.SetTarget(newChar.transform, true);

        currentCharacter = newIndex;
    }

    private void SetActiveCharacter(int index)
    {
        // deaktivuje všetky charaktery
        for (int i = 0; i < characterArray.Length; i++)
        {
            if (characterArray[i])
            {
                Rigidbody2D rb2D = characterArray[i].GetComponent<Rigidbody2D>();
                if (rb2D) rb2D.isKinematic = true;
                characterArray[i].SetActive(false);
            }
        }

        // aktivuje len aktuálny
        GameObject activeChar = characterArray[index];
        if (activeChar)
        {
            activeChar.SetActive(true);
            Rigidbody2D rb2D = activeChar.GetComponent<Rigidbody2D>();
            if (rb2D) rb2D.isKinematic = false;

            cameraFollower?.SetTarget(activeChar.transform, true);
        }

        currentCharacter = index;
    }
}
