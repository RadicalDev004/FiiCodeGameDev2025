using System.Collections;
using System.Collections.Generic;
using Pixelplacement;
using System.Globalization;
using System.Linq;
using UnityEngine;
using TMPro;
using System;

public class TileManager : Editable
{
    public MyTile OirginalTile;
    public List<MyTile> tiles = new();
    public float tileSize = 1;
    public int Side;
    public TMP_Text T_Level;
    public int Level;
    public bool Loading = false;
    public GameObject LoadingSign;
    public ParticleSystem Congrats;

    [TextArea(15, 15)]
    public string Prompt;

    private void Awake()
    {
        T_Level.text = "Level: " + Level;
        ValidateCode = Validate;
        Ref.ActionAfterTime(2, StartPuzzle);
    }

    public void StartPuzzle()
    {
        GenerateGameFromAPI(5, "easy");
    }

    public bool Validate(List<string> code)
    {
        Debug.Log(string.Join(", ", code));
        if (code.Count != 2)
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

        if (0 > values[0] || values[0] > Side)
        {
            Debug.LogError("Failed validation at incorrect value " + values[0]);
            return false;
        }

        if (0 > values[1] || values[1] > Side)
        {
            Debug.LogError("Failed validation at incorrect value " + values[1]);
            return false;
        }

        MyTile mt = tiles.Find(t => t.X == values[0] && t.Y == values[1]);
        mt.RotateTile();
        Block = true;

        bool res = IsPuzzleSolved();

        Ref.ActionAfterTime(2, () => {
            Block = false;
            if (res)
            {
                if (Level + 1 == 25)
                {
                    T_Level.text = "Completed!";
                    foreach (var tile in tiles)
                    {
                        Destroy(tile.gameObject);
                    }
                    tiles.Clear();
                    return;
                }
                Level++;
                T_Level.text = "Level: " + Level;
                Congrats.Play();
                Debug.Log(Congrats.isEmitting + " " + Time.timeScale);
                GenerateGameFromAPI(5 + Level / 5, DifficultyFromLevel(Level));
            }
        });

        return true;
    }

    public void ReloadCurrentLevel()
    {
        GenerateGameFromAPI(5 + Level / 5, DifficultyFromLevel(Level));
    }

    public void Receivemap(string json)
    {
        LoadingSign.SetActive(false);
        Debug.Log(json);
        try
        {
            PuzzleData data = PuzzleDecoder.ParsePuzzleJson(json);
            GenerateTiles(data);
        }
        catch (Exception)
        {
            ReloadCurrentLevel();
        }
        
        
    }

    public void GenerateGameFromAPI(int tilesCnt, string difficulty)
    {
        if (Loading) return;
        
        LoadingSign.SetActive(true);
        Loading = true;
        
        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject);
        }
        tiles.Clear();

        string prmpt = Prompt;
        prmpt = prmpt.Replace("{{size}}", tilesCnt.ToString());
        prmpt = prmpt.Replace("{{difficulty}}", difficulty);
        Debug.Log(prmpt);
        OpenAIChat.RequestChat(prmpt, Receivemap);
    }

    public void GenerateTiles(PuzzleData pz)
    {
        Congrats.Stop();
        Side = pz.size;

        float totalWidth = pz.size * tileSize;
        float totalHeight = pz.size * tileSize;

        Vector3 origin = -new Vector3(totalWidth, totalHeight, 0) / 2f + new Vector3(tileSize, tileSize, 0) / 2f;

        for (int x = 0; x < pz.size; x++)
        {
            for (int y = 0; y < pz.size; y++)
            {
                Vector3 tilePos = origin + new Vector3(x * tileSize, y * tileSize, -0.075f);
                TileData td = pz.tiles.Find(tile => tile.x == x && tile.y == y);
                if(x == pz.start.x && y == pz.start.y)
                {
                    GenerateTile(tilePos, x, y, 5);
                    continue;
                }
                if (x == pz.end.x && y == pz.end.y)
                {
                    GenerateTile(tilePos, x, y, 6);
                    continue;
                }
                GenerateTile(tilePos, x, y, td.type, td.rotation);
            }
        }
        Loading = false;
    }

    public void GenerateTile(Vector3 tilePos, int x, int y, int type, int rot = 0)
    {
        MyTile newTile = Instantiate(OirginalTile, transform);
        newTile.gameObject.SetActive(true);
        newTile.Create(tilePos, x, y, type, rot);
        tiles.Add(newTile);
    }

    public bool IsPuzzleSolved()
    {
        MyTile start = tiles.Find(t => t.Type == 5);
        MyTile end = tiles.Find(t => t.Type == 6);

        if (start == null || end == null)
        {
            Debug.LogError("Start or End tile is missing.");
            return false;
        }

        HashSet<(int, int)> visited = new HashSet<(int, int)>();
        return DFS(start.X, start.Y, end.X, end.Y, visited);
    }

    private bool DFS(int x, int y, int endX, int endY, HashSet<(int, int)> visited)
    {
        if (x == endX && y == endY) return true;
        visited.Add((x, y));
        Debug.Log("Visiting " +  x + ", " + y);

        MyTile current = tiles.Find(t => t.X == x && t.Y == y);
        if (current == null || current.Type == 0) return false;

        Debug.Log("Options: " + string.Join(", ", GetOpenDirections(current.Type, current.Rotation)));

        foreach (int dir in GetOpenDirections(current.Type, current.Rotation))
        {
            Vector2Int delta = directions[dir];
            int nx = x + delta.x;
            int ny = y + delta.y;

            if (visited.Contains((nx, ny))) continue;

            MyTile neighbor = tiles.Find(t => t.X == nx && t.Y == ny);
            if (neighbor == null || neighbor.Type == 0) continue;

            int oppositeDir = (dir + 2) % 4;
            if (GetOpenDirections(neighbor.Type, neighbor.Rotation).Contains(oppositeDir))
            {
                if (DFS(nx, ny, endX, endY, visited)) return true;
            }
        }

        return false;
    }

    private readonly Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(1, 0) 
    };

    private List<int> GetOpenDirections(int type, int rotation)
    {

        List<int> baseDirs = type switch
        {
            1 => new List<int> { 0, 2 },            
            2 => new List<int> { 0, 3 },            
            3 => new List<int> { 0, 1, 3 },         
            4 => new List<int> { 0, 1, 2, 3 },      
            5 => new List<int> { 0, 1, 2, 3 },      
            6 => new List<int> { 0, 1, 2, 3 },      
            _ => new List<int>()                   
        };


        for (int i = 0; i < rotation / 90; i++)
        {
            for (int j = 0; j < baseDirs.Count; j++)
            {
                baseDirs[j] = (baseDirs[j] + 1) % 4;
            }
        }

        return baseDirs;
    }

    public string DifficultyFromLevel(int lvl)
    {
        if (lvl < 10) return "easy";
        if (lvl < 20) return "medium";
        return "hard";
    }
}
