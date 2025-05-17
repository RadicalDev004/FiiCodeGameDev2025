using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PuzzleData
{
    public int size;
    public Position start;
    public Position end;
    public List<TileData> tiles;
}

[System.Serializable]
public class Position
{
    public int x;
    public int y;
}

[System.Serializable]
public class TileData
{
    public int x;
    public int y;
    public int type;
    public int rotation;
}

public class PuzzleDecoder : MonoBehaviour
{
    /// <summary>
    /// Parses the JSON string into a PuzzleData object.
    /// </summary>
    /// <param name="json">The JSON string returned from the OpenAI API</param>
    /// <returns>A structured PuzzleData object</returns>
    public static PuzzleData ParsePuzzleJson(string json)
    {
        return JsonUtility.FromJson<PuzzleData>(json);
    }
}
