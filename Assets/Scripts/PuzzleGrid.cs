using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Provides conveniance for other objects to access tilemap and objects
/// Can store in Puzzle and load Puzzle
/// </summary>
public class PuzzleGrid : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Transform objectsParent;
    [SerializeField] private Player player;
    [SerializeField] private GameObject exit;
    [SerializeField] private GameObject crystal;
    [SerializeField] private Camera mainCamera;

    public Tilemap Tilemap { get { return tilemap; } }
    public Transform ObjectsParent { get { return objectsParent; } }
    public Player Player { get { return player; } }
    public GameObject Exit { get { return exit; } }
    public GameObject Crystal { get { return crystal; } }

    public Puzzle ToPuzzle()
    {
        Puzzle puzzle = new Puzzle();

        for (int y = 0; y < Constants.LEVEL_HEIGHT; ++y)
        {
            for (int x = 0; x < Constants.LEVEL_WIDTH; ++x)
            {
                var tilePos = new Vector3Int(x, y, 0);

                TileBase tile = tilemap.GetTile(tilePos);
                puzzle.Tiles[y, x] = GM.LevelSerializer.GetIdForTile(tile);
            }
        }

        puzzle.Player = ObjPosToVector2Int(player.gameObject);
        puzzle.Exit = ObjPosToVector2Int(exit);
        puzzle.Crystal = ObjPosToVector2Int(crystal);
        return puzzle;
    }

    public void LoadPuzzle(Puzzle puzzle)
    {
        for (int y = 0; y < Constants.LEVEL_HEIGHT; ++y)
        {
            for (int x = 0; x < Constants.LEVEL_WIDTH; ++x)
            {
                var tilePos = new Vector3Int(x, y, 0);

                tilemap.SetTile(tilePos, null); // Always set to null first to prevent unwanted rotation

                int tileId = puzzle.Tiles[y, x];
                if (tileId > 0)
                {
                    tilemap.SetTile(tilePos, GM.LevelSerializer.GetTileForTileId(tileId));
                }
            }
        }

        player.transform.position = new Vector3(puzzle.Player.x + 0.5f, puzzle.Player.y + 0.5f);
        exit.transform.position = new Vector3(puzzle.Exit.x + 0.5f, puzzle.Exit.y + 0.5f);
        crystal.transform.position = new Vector3(puzzle.Crystal.x + 0.5f, puzzle.Crystal.y + 0.5f);

        player.gameObject.SetActive(true);
        exit.SetActive(true);
        crystal.SetActive(true);
    }

    public void BuildTilemapBorder(int borderSize, TileBase borderTile)
    {
        // todo: Look at also using m_tilemap.BoxFill

        for (int x = -borderSize; x < Constants.LEVEL_WIDTH + borderSize; ++x)
        {
            for (int y = -borderSize; y < Constants.LEVEL_HEIGHT + borderSize; ++y)
            {
                if (x >= 0 && x < Constants.LEVEL_WIDTH && y >= 0 && y < Constants.LEVEL_HEIGHT) continue;

                tilemap.SetTile(new Vector3Int(x, y, 0), borderTile);
            }
        }

        tilemap.CompressBounds();
    }

    public Vector3Int MousePositionToTilePosition()
    {
        Vector3 screenWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 zeroCellCenterWorld = tilemap.GetCellCenterWorld(Vector3Int.zero);
        Vector3 paintTileWorldPosition = screenWorldPosition - zeroCellCenterWorld;
        Vector3Int tilePosition = new(Mathf.RoundToInt(paintTileWorldPosition.x), Mathf.RoundToInt(paintTileWorldPosition.y), 0);
        return tilePosition;
    }

    private Vector2Int ObjPosToVector2Int(GameObject obj)
    {
        return new Vector2Int(Mathf.FloorToInt(obj.transform.position.x), Mathf.FloorToInt(obj.transform.position.y));
    }
}