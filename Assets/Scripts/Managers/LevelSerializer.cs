using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using System.Text;
using System.IO;
using System;
using Jint.Parser;

[Serializable]
public class LevelRawData
{
    public string[] Data = null;
}

public class Puzzle
{
    public int[,] Tiles = new int[Constants.LEVEL_HEIGHT, Constants.LEVEL_WIDTH];
    public Vector2Int Player;
    public Vector2Int Exit;
    public Vector2Int Crystal;
}

public class LevelSerializer : MonoBehaviour
{
    [SerializeField]
    private TileIds m_tileIds;

    // The max value of an unsigned 256 bit int is 115792089237316195423570985008687907853269984665640564039457584007913129639935
    // The first digit must be 0, which then gives us 77 digits that can be any value from 0-9

    // This gives us 77 digits which can have a value from 0-9. The first digit should be 1 so it's easy for the smart c can only be
    // a 1 (Or 0, but that then means extra work in the smart contract)
    public const int U128_TOTAL_DIGITS = 78;
    public const int U128_AVAILABLE_DIGITS = 77;

    public const int IDS_PER_U128 = U128_AVAILABLE_DIGITS;

    private Dictionary<TileBase, int> m_tileToIdDict;
    private Dictionary<int, TileBase> m_idToTileDict;

    public int GetIdForTile(TileBase tile)
    {
        if (tile == null)
        {
            return 0;
        }
        else
        {
            return m_tileToIdDict[tile];
        }
    }

    public TileBase GetTileForTileId(int id)
    {
        return m_idToTileDict[id];
    }

    // Start is called before the first frame update
    void Start()
    {
        m_tileToIdDict = m_tileIds.GetTileToIdDict();
        m_idToTileDict = m_tileIds.GetIdToTileDict();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public string ReverseString(string text)
    {
        if (text == null) return null;

        // this was posted by petebob as well 
        char[] array = text.ToCharArray();
        Array.Reverse(array);
        return new String(array);
    }

    public Puzzle LoadCurrentLevel()
    {
        string[] data = LoadJSONFromDisk();
        if (data != null)
        {
            return LoadFromSerializedStrings(data);
        }
        else
        {
            return null;
        }
    }

    public char intToChar(int value)
    {
        return (char)(value + 48);
    }

    public int charToInt(char value)
    {
        return ((int)value) - 48;
    }

    public string[] SerializePuzzle(Puzzle puzzle)
    {
        // IMPORTANT: This code must be kept in sync with smart contract that reads the puzzle data
        const int OBJ_SPACE = 28;
        const int MAIN_OBJ_DIGITS = 3;
        const int NUM_MAIN_OBJS = 3;

        // Add dead space
        const int DEAD_SPACE = OBJ_SPACE - (NUM_MAIN_OBJS * MAIN_OBJ_DIGITS);
        StringBuilder sb = new StringBuilder(10);
        sb.Append("0");
        for (int i = 0; i < DEAD_SPACE; ++i)
            sb.Append("9");

        // Add objs
        sb.Append(encodeObjPos(puzzle.Exit));
        sb.Append(encodeObjPos(puzzle.Player));
        sb.Append(encodeObjPos(puzzle.Crystal));

        List<string> u256s = new List<string>();
        int j = OBJ_SPACE;
        for (int y = Constants.LEVEL_HEIGHT - 1; y >= 0; --y)
        {
            for (int x = Constants.LEVEL_WIDTH - 1; x >= 0; --x)
            {
                sb.Append(puzzle.Tiles[y, x]);

                ++j;
                if (j == U128_AVAILABLE_DIGITS)
                {
                    u256s.Push(sb.ToString());
                    sb.Clear();
                    sb.Append("0");
                    j = 0;
                }
            }
        }

        u256s.Reverse();
        return u256s.ToArray();
    }

    // todo: Explain how this works
    public Puzzle LoadFromSerializedStrings(string[] u128s)
    {
        Puzzle puzzle = new Puzzle();

        int u128sIdx = 0;
        int i = U128_AVAILABLE_DIGITS;

        for (int y = 0; y < Constants.LEVEL_HEIGHT; ++y)
        {
            for (int x = 0; x < Constants.LEVEL_WIDTH; ++x)
            {

                puzzle.Tiles[y, x] = int.Parse(u128s[u128sIdx].Substring(i, 1));

                --i;
                if (i == 0)
                {
                    ++u128sIdx;
                    i = U128_AVAILABLE_DIGITS;
                }
            }
        }

        {
            i -= 2;
            (int x, int y) = decodeObjXY(u128s[u128sIdx].Substring(i, 3));
            puzzle.Crystal = new Vector2Int(x, y);

            i -= 3;
            (x, y) = decodeObjXY(u128s[u128sIdx].Substring(i, 3));
            puzzle.Player = new Vector2Int(x, y);

            i -= 3;
            (x, y) = decodeObjXY(u128s[u128sIdx].Substring(i, 3));
            puzzle.Exit = new Vector2Int(x, y);


        }

        return puzzle;
    }

    private string IntArrayToString(int[] intArray, int size, string firstDigit = null)
    {
        StringBuilder sb = new StringBuilder(size * 2 + 1);

        if (firstDigit != null)
        {
            sb.Append(firstDigit);
        }

        for (int i = size - 1; i >= 0; --i)
        {
            sb.Append(intArray[i].ToString("D2"));
        }
        return sb.ToString();
    }

    private (int x, int y) decodeObjXY(string data)
    {
        print($"Decoding data: {data}");


        int quadrant = charToInt(data[2]);

        int y = charToInt(data[1]);
        if (quadrant > 2 && y < 4) y += 10;

        int x = charToInt(data[0]);
        if (quadrant % 2 == 0) x += 10;

        return (x, y);
    }

    private string encodeObjPos(Vector2Int pos)
    {
        int x = pos.x;
        int y = pos.y;
        int quadrant = 1;

        if (x > 9)
        {
            x %= 10;
            quadrant = 2;
        }
        if (y > 9)
        {
            y %= 10;
            quadrant += 2;
        }

        print($"Encoding data: {x}{y}{quadrant}");

        return $"{x}{y}{quadrant}";
    }

    public void SaveJSONToDisk(string[] data)
    {
        var levelRawData = new LevelRawData
        {
            Data = data
        };

        string json = JsonUtility.ToJson(levelRawData);
        Debug.Log(json);
        string filepath = Application.persistentDataPath + "/savedLevel.json";
        Debug.Log($"Saving json to {filepath}");
        File.WriteAllText(filepath, json);
    }

    public string[] LoadJSONFromDisk()
    {
        string filepath = Application.persistentDataPath + "/savedLevel.json";
        if (File.Exists(filepath))
        {
            string json = File.ReadAllText(filepath);
            Debug.Log(json);
            Debug.Log($"Loading json from {filepath}");
            LevelRawData levelRawData = JsonUtility.FromJson<LevelRawData>(json);
            return levelRawData.Data;
        }
        return null;
    }
}
