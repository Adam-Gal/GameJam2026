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

    void Start()
    {
        SetActiveCharacter(currentCharacter);
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame && unlockedCharacters >= 1) SwitchCharacter(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame && unlockedCharacters >= 2) SwitchCharacter(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame && unlockedCharacters >= 3) SwitchCharacter(2);

        if (Keyboard.current.tabKey.wasPressedThisFrame)
            SwitchCharacter((currentCharacter + 1) % characterArray.Length);
    }

    public void SwitchCharacter(int newIndex)
    {
        if (newIndex == currentCharacter) return;
        if (newIndex < 0 || newIndex >= characterArray.Length) return;

        GameObject oldChar = characterArray[currentCharacter];
        GameObject newChar = characterArray[newIndex];

        if (!oldChar || !newChar) return;

        // uloženie pozície
        Vector3 savedPos = oldChar.transform.position;
        Quaternion savedRot = oldChar.transform.rotation;

        // === OLD CHARACTER ===
        Rigidbody oldRb = oldChar.GetComponent<Rigidbody>();
        if (oldRb)
        {
            oldRb.linearVelocity = Vector3.zero;
            oldRb.angularVelocity = Vector3.zero;
            oldRb.isKinematic = true;
        }

        oldChar.SetActive(false);

        // === NEW CHARACTER ===
        newChar.transform.SetPositionAndRotation(savedPos, savedRot);
        newChar.SetActive(true);

        Rigidbody newRb = newChar.GetComponent<Rigidbody>();
        if (newRb)
        {
            newRb.isKinematic = false;
            newRb.linearVelocity = Vector3.zero;
            newRb.angularVelocity = Vector3.zero;
        }

        // === CAMERA ===
        if (cameraFollower)
            cameraFollower.SetTarget(newChar.transform, true); // instant snap pri switchi

        currentCharacter = newIndex;
    }

    private void SetActiveCharacter(int index)
    {
        for (int i = 0; i < characterArray.Length; i++)
            if (characterArray[i])
                characterArray[i].SetActive(false);

        if (characterArray[index])
        {
            characterArray[index].SetActive(true);

            if (cameraFollower)
                cameraFollower.SetTarget(characterArray[index].transform, true);
        }

        currentCharacter = index;
    }
}
