using TMPro;
using UnityEngine;

public class UI_DialogueLogRowView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _sourceText;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _speakerText;
    [SerializeField] private TextMeshProUGUI _bodyText;

    public void Render(DialogueLogEntry entry)
    {
        SetText(_sourceText, GetSourceLabel(entry.Source));
        SetText(_titleText, entry.Title);
        SetText(_speakerText, entry.SpeakerName);

        if (_bodyText != null)
        {
            _bodyText.text = entry.Text;
        }
    }

    private static void SetText(TextMeshProUGUI target, string value)
    {
        if (target == null)
        {
            return;
        }

        bool hasValue = !string.IsNullOrWhiteSpace(value);
        target.gameObject.SetActive(hasValue);

        if (hasValue)
        {
            target.text = value;
        }
    }

    private static string GetSourceLabel(EDialogueLogSource source)
    {
        switch (source)
        {
            case EDialogueLogSource.WeekFeedback:
                return "\uC8FC\uAC04 \uACB0\uACFC";

            case EDialogueLogSource.EventStep:
                return "\uC774\uBCA4\uD2B8";

            case EDialogueLogSource.ChoiceResult:
                return "\uC120\uD0DD \uACB0\uACFC";

            case EDialogueLogSource.Ending:
                return "\uC5D4\uB529";
        }

        return string.Empty;
    }
}
