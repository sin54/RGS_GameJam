using TMPro;
using UnityEngine;

public class InteractPromptUI_NonHold : MonoBehaviour
{
    [SerializeField] private TMP_Text promptText;

    public void SetText(string text)
    {
        promptText.text = text;
    }
}
