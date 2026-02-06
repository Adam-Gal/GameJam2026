using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterManagerController : MonoBehaviour
{
    public int currentCharacter = 0;
    public GameObject[] characterArray;
    public GameObject target;

    void Start()
    {
        if (characterArray.Length > 0)
        {
            SetActiveCharacter(currentCharacter);
        }
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SwitchCharacter(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SwitchCharacter(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SwitchCharacter(2);
        if (Keyboard.current.tabKey.wasPressedThisFrame) SwitchCharacter((currentCharacter + 1) % characterArray.Length);
    }

    public void SetActiveCharacter(int index)
    {
        for (int i = 0; i < characterArray.Length; i++)
        {
            if (characterArray[i] != null)
                characterArray[i].SetActive(false);
        }
        
        if (index >= 0 && index < characterArray.Length && characterArray[index] != null)
        {
            characterArray[index].SetActive(true);
            target = characterArray[index];
            currentCharacter = index;
        }
    }

    public void SwitchCharacter(int newIndex)
    {
        SetActiveCharacter(newIndex);
    }
}
