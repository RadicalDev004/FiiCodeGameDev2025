using UnityEngine;
using UnityEngine.UI;

public class ManaUI : MonoBehaviour
{
    public GameObject manaBar;

    private void Start()
    {
        if (ManaSystem.Instance != null)
        {
            ManaSystem.Instance.OnManaChanged += UpdateManaUI;
        }
        else
        {
            Debug.LogError("Mana system null");
        }
    }

    private void UpdateManaUI(float manaPercent)
    {
        manaBar.GetComponent<Image>().fillAmount = manaPercent;
    }

    private void OnDestroy()
    {
        ManaSystem.Instance.OnManaChanged -= UpdateManaUI;
    }
}
