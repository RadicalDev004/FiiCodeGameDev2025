using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagicScroll : Editable
{
    public List<GameObject> Runes = new();
    public List<string> RunesNames = new() { "blue", "orange", "purple1", "yellow", "light_blue", "purple2" };

    public List<float> CorrectAnswer = new(), CorrectAnswer2 = new();

    public Vector3 MovePos1, MovePos2;

    public Transform SolvedPos;

    private void Awake()
    {
        ValidateCode = Validate;
    }

    public bool Validate(List<string> code)
    {
        if(code.Count != 2)
        {
            Debug.LogError("Failed validation at length " + code.Count);
            return false;
        }
        foreach(var cd in code)
        {
            if (!RunesNames.Contains(cd))
            {
                Debug.LogError("Failed validation at type " + cd);
                return false;
            }
        }

        StartCoroutine(SwapRunes(Runes[RunesNames.IndexOf(code[0])], Runes[RunesNames.IndexOf(code[1])]));

        return true;
    }

    private IEnumerator SwapRunes(GameObject r1, GameObject r2)
    {
        Vector3 pr1 = r1.transform.localPosition;
        Vector3 pr2 = r2.transform.localPosition;

        Tween.LocalPosition(r1.transform, MovePos1, 1, 0, Tween.EaseInOut);
        Tween.LocalPosition(r2.transform, MovePos2, 1, 0, Tween.EaseInOut);

        yield return new WaitForSeconds(2);

        Tween.LocalPosition(r1.transform, pr2, 1, 0, Tween.EaseInOut);
        Tween.LocalPosition(r2.transform, pr1, 1, 0, Tween.EaseInOut);

        yield return new WaitForSeconds(2);

        List<float> newPos = Runes.Select(rune => rune.transform.localPosition.x).ToList();

        if(newPos.SequenceEqual(CorrectAnswer) || newPos.SequenceEqual(CorrectAnswer2))
        {
            OnGlitchSolve();
            Tween.LocalPosition(transform, SolvedPos.localPosition, 2, 0, Tween.EaseInOut);
            Tween.LocalRotation(transform, SolvedPos.localRotation, 2, 0, Tween.EaseInOut);
        }
    }
}
