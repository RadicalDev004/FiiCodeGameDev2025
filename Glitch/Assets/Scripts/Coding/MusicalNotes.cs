using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class MusicalNotes : Editable
{
    [Header("Particular")]
    public List<int> CorrectOrder = new();
    public List<GameObject> Notes = new();
    public Material Org, Wrong;
    public List<Material> PlayingMaterials = new();

    private void Awake()
    {
        ValidateCode = Validate;
    }

    public bool Validate(List<string> code)
    {
        if(code.Count > 1)
        {
            Debug.LogError("Failed validation at length " + code.Count);
            return false;
        }

        string input = code.Count > 0 ? code[0] : null;
        string pattern = @"^(?:notes\[(\d+)\](?:,\s*|$))+$";
        List<int> allNotes = new() { 0 };
        if (string.IsNullOrEmpty(input))
        {

        }
        else if (Regex.IsMatch(input, pattern))
        {
            Match match = Regex.Match(input, pattern);

            foreach (Capture capture in match.Groups[1].Captures.Cast<Capture>())
            {
                int vl = int.Parse(capture.Value);
                if(vl < 0 || vl >= Notes.Count)
                {
                    Debug.LogError("Failed validation at value " + vl);
                    return false;
                }
                allNotes.Add(int.Parse(capture.Value));
            }
        }
        else
        {
            Debug.LogError("Failed validation at format " + input);
            return false;
        }

        StartCoroutine(PlayNotes(new() { CorrectOrder, allNotes}, 0.5f));

        

        return true;
    }

    private IEnumerator PlayNotes(List<List<int>> notes, float time)
    {
        Block = true;
        for(int i = 0; i < notes.Count; i++)
        {
            var list = notes[i];
            for(int j = 0; j < list.Count; j++)
            {
                var note = list[j];
                Notes[note].GetComponent<MeshRenderer>().material = (i > 0 && note != notes[0][j]) ? Wrong : PlayingMaterials[note];
                yield return new WaitForSeconds(time);
                Notes[note].GetComponent<MeshRenderer>().material = Org;
                yield return new WaitForSeconds(time);
            }
            for(int j = list.Count; i > 0 && j < notes[0].Count; j++)
            {
                Notes.All(x => x.GetComponent<MeshRenderer>().material = Wrong);
                yield return new WaitForSeconds(time);
                Notes.All(x => x.GetComponent<MeshRenderer>().material = Org);
                yield return new WaitForSeconds(time);
            }
            yield return new WaitForSeconds(1);
        }

        if (notes[0].SequenceEqual(notes[1]))
        {
            OnGlitchSolve();
            for(int i = 0; i < Notes.Count; i++)
            {
                Notes[i].GetComponent<MeshRenderer>().material = PlayingMaterials[i];
            }
        }
        Block = false;
    }
}
