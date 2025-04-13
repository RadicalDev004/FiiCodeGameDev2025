using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject Tab_Credits;
    public RectTransform Credits;
    public Vector3 InitialPos, EndPos;
    public float CreditTimer;
    int CreditTweenID;

    void Start()
    {
        Time.timeScale = 1.0f;
    }


    public void RollCredits()
    {
        Tab_Credits.SetActive(true);       
        Credits.localPosition = InitialPos;

        Tween.Stop(CreditTweenID);
        CreditTweenID = Tween.LocalPosition(Credits, EndPos, CreditTimer, 0).targetInstanceID;

        Ref.ActionAfterTime(CreditTimer + 2, () =>
        {
            StopCredits();
        });
    }
    public void StopCredits()
    {
        Tab_Credits.SetActive(false);
    }
}
