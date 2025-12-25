using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InteractionPromptUI_Hold : MonoBehaviour
{
    [SerializeField] private Image interactionProgressUI;
    [SerializeField] private TMP_Text promptText;
    public void SetText(string text)
    {
        promptText.text = text;
    }
    public void SetFillAmount(float amount)
    {
        interactionProgressUI.fillAmount = amount;
    }

}
