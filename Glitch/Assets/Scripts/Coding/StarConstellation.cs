using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.Windows;

public class StarConstellation : Editable
{
    public List<Transform> StarPositions;
    public LineRenderer LinePrefab;
    public Transform LinesParent;

    private List<List<(int, int)>> correctSolutions = new()
    {
        new List<(int, int)> { (1, 2), (2, 7), (6, 7), (1, 6), (6, 10), (10, 13), (13, 15) },
        new List<(int, int)> { (8, 11), (8, 9), (9, 11), (9, 12), (11, 14), (12, 14) }
    };

    private Dictionary<int, Transform> starsByNumber = new();
    private List<(int, int)> currentConnections = new();
    private List<LineRenderer> currentLines = new();

    private void Awake()
    {
        ValidateCode = Validate;
        SetPositions();
    }

    void SetPositions()
    {
        for (int i = 0; i < StarPositions.Count; i++)
        {
            starsByNumber[i+1] = StarPositions[i];
        }
    }

    public bool Validate(List<string> code)
    {
        if(code.Count != 2)
        {
            Debug.LogError("Validation failed: Incorrect number of inputs.");
            return false;
        }

        string name1 = code[0].ToLower();
        string name2 = code[1].ToLower();

        if (!int.TryParse(code[0], out int numar1) || !int.TryParse(code[1], out int numar2))
        {
            Debug.LogError("Validation failed: Invalid input 1 or 2");
            return false;
        }
        if((numar1 <= 0 && numar2 >= 0) || (numar1 >= 0 && numar2 <= 0))
        {
            Debug.LogError("Failed validation at incorrect value 1 or 2");
            return false;
        }

        // 1 2 // 0 -1 // -1 0 // 1 
        if (numar1 > 0)
        {
            if (numar1 > starsByNumber.Count || numar2 > starsByNumber.Count)
            {
                Debug.LogError("Failed validation at incorrect value 1 or 2");
                return false;
            }
            Connect(numar1, numar2);
        }
        else
        {
            if (numar1 < -starsByNumber.Count || numar2 < -starsByNumber.Count)
            {
                Debug.LogError("Failed validation at incorrect value 1 or 2");
                return false;
            }
            Remove(-numar1, -numar2);
        }


        if (CheckSolution())
        {
            OnGlitchSolve();
        }

        return true;
    }


    private void Connect(int a, int b)
    {
        if (currentConnections.Contains((a, b)) || currentConnections.Contains((b, a)))
            return;

        Vector3 posA = starsByNumber[a].position + new Vector3(-0.0008f, 0f, 0f);
        Vector3 posB = starsByNumber[b].position + new Vector3(-0.0008f, 0f, 0f);

        var lr = Instantiate(LinePrefab, LinesParent);
        lr.SetPosition(0, posA);
        lr.SetPosition(1, posB);

        currentLines.Add(lr);
        currentConnections.Add((a, b));
    }


    private void Remove(int a, int b)
    {
        for (int i = 0; i < currentConnections.Count; i++)
        {
            var conn = currentConnections[i];
            if ((conn.Item1 == a && conn.Item2 == b) || (conn.Item1 == b && conn.Item2 == a))
            {
                currentConnections.RemoveAt(i);

                Destroy(currentLines[i].gameObject);
                currentLines.RemoveAt(i);
                break;
            }
        }
    }

    private bool CheckSolution()
    {
        return correctSolutions.Any(solution =>
        {
            var solutionSet = new HashSet<(int, int)>(solution);
            var currentSet = new HashSet<(int, int)>(
                currentConnections.Select(p => p.Item1 < p.Item2 ? p : (p.Item2, p.Item1))
            );

            var normalizedSolution = new HashSet<(int, int)>(
                solution.Select(p => p.Item1 < p.Item2 ? p : (p.Item2, p.Item1))
            );

            return currentSet.SetEquals(normalizedSolution);
        });
    }


}
