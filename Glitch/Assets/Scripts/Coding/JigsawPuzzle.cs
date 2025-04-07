using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class JigsawPuzzle : Editable
{
    public List<GameObject> PuzzlePieces = new();
    public List<GameObject> CorrectPuzzleLocations = new();
    public List<bool> Answers = new();
    

    private void Awake()
    {
        ValidateCode = Validate;
    }

    public bool Validate(List<string> code)
    {
        Debug.LogWarning(string.Join(",", code) + "\n" + string.Join(",", Ref.Code.AddedCode));
        if(code.Count != 8)
        {
            Debug.LogError("Failed validation at length " + code.Count);
            return false;
        }

        List<float> values = new();
        foreach (string s in code)
        {
            string str = s.Replace(',', '.');
            if (float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
            {
                Debug.LogWarning(value);
                values.Add(value);
            }
            else
            {
                Debug.LogError("Failed validation at parse " + s);
                return false;
            }
        }

        StartCoroutine(MoveAllPieces(values));

        return true;
    }

    private IEnumerator MoveAllPieces(List<float> values)
    {
        Block = true;
        for (int i = 0; i < values.Count; i += 2)
        {
            var x = values[i];
            var z = values[i + 1];

            if (x.ToString() != Ref.Code.AddedCode[i] || z.ToString() != Ref.Code.AddedCode[i + 1])
            {
                var time = 1;
                StartCoroutine(MovePiece(i / 2, new(x, z), time));
                yield return new WaitForSeconds(time * 3);
            }
        }

        yield return new WaitForSeconds(0.5f);
        Block = false;
        foreach (var ans in Answers)
        {
            if (!ans) yield break;
        }
        OnGlitchSolve();
    }

    private IEnumerator MovePiece(int ind, Vector2 pos, float time)
    {
        Vector3 initialPos = PuzzlePieces[ind].transform.localPosition;
        Tween.LocalPosition(PuzzlePieces[ind].transform, PuzzlePieces[ind].transform.localPosition + new Vector3(0, 0.2f, 0), time, 0, Tween.EaseInOut);
        yield return new WaitForSeconds(time);

        Tween.LocalPosition(PuzzlePieces[ind].transform, new Vector3(pos.x, 0.2f, pos.y), time, 0, Tween.EaseInOut);
        yield return new WaitForSeconds(time);

        Tween.LocalPosition(PuzzlePieces[ind].transform, new Vector3(pos.x, 0, pos.y), time, 0, Tween.EaseInOut);
        yield return new WaitForSeconds(time);

        if (Mathf.Abs(CorrectPuzzleLocations[ind].transform.localPosition.x - pos.x) < 0.002f && Mathf.Abs(CorrectPuzzleLocations[ind].transform.localPosition.z - pos.y) < 0.002f)
        {
            Answers[ind] = true;
        }
        else
        {
            Answers[ind] = false;
            //Tween.LocalPosition(PuzzlePieces[ind].transform, initialPos, time, 0, Tween.EaseInOut);
        }

    }
}
