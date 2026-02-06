using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterManagerController : MonoBehaviour
{
    [Header("Characters")]
    public GameObject[] characterArray;
    public int currentCharacter = 0;
    public int unlockedCharacters = 1;

    [Header("Camera")]
    [SerializeField] private CameraFollower cameraFollower;

    private bool _subscribed;

    void Start()
    {
        SetActiveCharacter(currentCharacter);
        Subscribe();
    }

    void OnEnable()
    {
        Subscribe();
    }

    void OnDisable()
    {
        Unsubscribe();
    }

    void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (_subscribed)
        {
            return;
        }

        if (InputManager.Instance == null)
        {
            return;
        }
        InputManager.Instance.OnOne += HandleOne;
        InputManager.Instance.OnTwo += HandleTwo;
        InputManager.Instance.OnThree += HandleThree;
        _subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_subscribed)
        {
            return;
        }
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
        if (!pressed)
        {
            return;
        }

        if (unlockedCharacters >= 1)
        {
            SwitchCharacter(0);
        }
    }

    private void HandleTwo(bool pressed)
    {
        if (!pressed)
        {
            return;
        }

        if (unlockedCharacters >= 2)
        {
            SwitchCharacter(1);
        }
    }

    private void HandleThree(bool pressed)
    {
        if (!pressed)
        {
            return;
        }

        if (unlockedCharacters >= 3)
        {
            SwitchCharacter(2);
        }
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            SwitchCharacter((currentCharacter + 1) % characterArray.Length);
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
        Quaternion savedRot = Quaternion.identity;

        Rigidbody oldRb = oldChar.GetComponent<Rigidbody>();
        if (oldRb)
        {
            oldRb.linearVelocity = Vector3.zero;
            oldRb.angularVelocity = Vector3.zero;
            oldRb.isKinematic = true;
        }

        oldChar.SetActive(false);

        newChar.transform.SetPositionAndRotation(savedPos, savedRot);
        newChar.SetActive(true);

        Rigidbody newRb = newChar.GetComponent<Rigidbody>();
        if (newRb)
        {
            newRb.isKinematic = false;
            newRb.linearVelocity = Vector3.zero;
            newRb.angularVelocity = Vector3.zero;
        }

        if (cameraFollower)
            cameraFollower.SetTarget(newChar.transform, true);

        currentCharacter = newIndex;
    }


    private void SetActiveCharacter(int index)
    {
        for (int i = 0; i < characterArray.Length; i++)
        {
            if (characterArray[i])
            {
                characterArray[i].SetActive(false);
            }
        }

        if (characterArray[index])
        {
            characterArray[index].SetActive(true);

            if (cameraFollower)
            {
                cameraFollower.SetTarget(characterArray[index].transform, true);
            }
        }

        currentCharacter = index;
    }
}
