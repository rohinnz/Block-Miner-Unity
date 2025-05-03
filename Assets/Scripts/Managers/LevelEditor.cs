using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;


public class LevelEditor : MonoBehaviour  // todo: Rename to puzzle editor
{
    [SerializeField] private TileBase m_unbreakableTile;
    [SerializeField] private Toolbar m_toolbar;
    [SerializeField] private SpriteRenderer m_previewBrush;
    [SerializeField] private PuzzleGrid puzzleGrid;

    // Place to hide sprites offscreen. todo: Confirm this is cheaper than just hiding the object
    private readonly Vector3 HIDE_POS = new Vector3(0, 1000, 0);

    public string[] LatestSerializedString { get; private set; }

    private void Awake()
    {
    }

    private void Start()
    {
        puzzleGrid.BuildTilemapBorder(5, m_unbreakableTile);
        AdjustCameraZoomAndPosition();

        m_toolbar.SelectionChangedEvent += OnSelectionChanged;
    }

    private void OnDestroy()
    {
        m_toolbar.SelectionChangedEvent -= OnSelectionChanged;
    }

    private void OnSelectionChanged(ToolbarButton selectedButton)
    {
        m_previewBrush.sprite = selectedButton.Sprite;
    }

    private void Update()
    {
        Vector3Int tilePosition = puzzleGrid.MousePositionToTilePosition();

        if (IsTilePositionWithinBounds(tilePosition))
        {
            // Show preview brush
            Vector3 objPos = new Vector3(tilePosition.x + 0.5f, tilePosition.y + 0.5f, 0);
            m_previewBrush.transform.position = objPos;

            if (Input.GetMouseButton(0)) // Left click
            {
                puzzleGrid.Tilemap.SetTile(tilePosition, null);

                switch (m_toolbar.SelectedButton.GetBrushType())
                {
                    case ToolbarButton.BrushType.Eraser:
                        HideExistingObjectAt(objPos);
                        break;
                    case ToolbarButton.BrushType.Tile:
                        puzzleGrid.Tilemap.SetTile(tilePosition, m_toolbar.SelectedButton.Tile);
                        HideExistingObjectAt(objPos);
                        break;
                    case ToolbarButton.BrushType.Object:
                        GameObject o = m_toolbar.SelectedButton.NextSpriteObject();
                        if (!IsObjectAlreadyPlacedAt(objPos))
                        {
                            HideExistingObjectAt(objPos);
                            o.SetActive(true);
                            o.transform.position = objPos;
                        }
                        break;
                }
            }
        }
        else
        {
            // Hide preview brush
            m_previewBrush.transform.position = HIDE_POS;
        }
    }

    private bool IsObjectAlreadyPlacedAt(Vector3 position)
    {
        foreach (var o in m_toolbar.SelectedButton.SpriteObjects)
        {
            if (o.transform.position == position) return true;
        }

        return false;
    }

    private void HideExistingObjectAt(Vector3 position)
    {
        GameObject existingObject = ObjectAtPosition(position);
        if (existingObject != null)
        {
            existingObject.SetActive(false);
        }
    }

    private GameObject ObjectAtPosition(Vector3 position)
    {
        foreach (Transform child in puzzleGrid.ObjectsParent)
        {
            if (child.gameObject.activeSelf && child.transform.position == position)
                return child.gameObject;
        }

        return null;
    }

    bool IsTilePositionWithinBounds(Vector3Int p)
    {
        return p.x >= 0 && p.x < Constants.LEVEL_WIDTH && p.y >= 0 && p.y < Constants.LEVEL_HEIGHT;
    }

    private void AdjustCameraZoomAndPosition()
    {
        //
        // 1. Adjust zoom
        //
        float boardHeight = (float)Constants.LEVEL_HEIGHT;
        float boardWidth = (float)Constants.LEVEL_WIDTH;
        float screenAspect = (float)Screen.height / (float)Screen.width;

        boardHeight += 1; // Add 1 to height to account for the toolbar

        if ((boardHeight / boardWidth) > screenAspect)
        {
            // todo: Account for toolbar on side

            Camera.main.orthographicSize = boardHeight * 0.5f;
        }
        else
        {
            // todo: Account for toolbar on top

            Camera.main.orthographicSize = boardWidth * screenAspect * 0.5f;
        }

        //
        // 2. Adjust position
        //
        float camXPos = (boardWidth * .5f) - 0.5f;
        float camYPos = Camera.main.orthographicSize - 0.5f;
        Camera.main.transform.position = new Vector3(camXPos, camYPos, Camera.main.transform.position.z);
        Debug.Log(string.Format("Camera orthographic size set to {0}", Camera.main.orthographicSize));
    }

    public void OnSaveClick()
    {
        Puzzle puzzle = puzzleGrid.ToPuzzle();
        string[] dataStrs = GM.LevelSerializer.SerializePuzzle(puzzle);
        GM.LevelSerializer.SaveJSONToDisk(dataStrs);
    }

    public void OnLoadClick()
    {
        Puzzle puzzle = GM.LevelSerializer.LoadCurrentLevel();
        puzzleGrid.LoadPuzzle(puzzle);
    }

    public void OnClickTest()
    {

    }

    public void OnClickMint()
    {
        Puzzle puzzle = puzzleGrid.ToPuzzle();
        MintPuzzle(puzzle);
    }

    private async void MintPuzzle(Puzzle puzzle)
    {
        // todo: Move function to BlockchainManager

        string[] data = GM.LevelSerializer.SerializePuzzle(puzzle);
        string uri = "test";

        string puzzleExistsResponse = await GM.Blockchain.PuzzleExists(data);
        print("OnClickMintPuzzle() response: " + puzzleExistsResponse);

        if (bool.Parse(puzzleExistsResponse))
        {
            Debug.LogError("Puzzle Already Exists!");
            return;
        }

        string mintFee = await GM.Blockchain.GetMintFee();
        string response = await GM.Blockchain.MintPuzzle(data, uri, mintFee);
        print("OnClickMintPuzzle() response: " + response);
    }
}
