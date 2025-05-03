using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Responsible for puzzle gameplay
/// </summary>
public partial class PlayScreen : MonoBehaviour
{
    [SerializeField] private HUD hud;
    [SerializeField] private TileIds m_tileIds;
    [SerializeField] private TileBase m_unbreakableTile;
    [SerializeField] private PuzzleGrid puzzleGrid;

    private BoardLogic boardLogic = new();
    private MoveAnimation moveAnimation;

    private void Awake()
    {
        moveAnimation = new MoveAnimation(puzzleGrid);
    }

    private void Start()
    {
        puzzleGrid.BuildTilemapBorder(10, m_unbreakableTile);
        AdjustCameraZoomAndPosition();
        LoadLevel();
    }

    private void Update()
    {
        moveAnimation.Update();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TryMovePlayer(MoveDirs.LEFT);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            TryMovePlayer(MoveDirs.RIGHT);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            TryMovePlayer(MoveDirs.UP);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            TryMovePlayer(MoveDirs.DOWN);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Return to previous scene
            GM.Screens.TransitionToPreviousScreen();
        }
        else if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Vector3Int tilePosition = puzzleGrid.MousePositionToTilePosition();
            Vector2Int delta = new Vector2Int(tilePosition.x - boardLogic.State.PlayerPos.x, boardLogic.State.PlayerPos.y - tilePosition.y);
            MoveDirs moveDir = Utils.VectorToMoveDir(delta);
            Vector2Int prevPlayerPos = boardLogic.State.PlayerPos;


            if (moveDir == MoveDirs.WAIT)
            {
                // Invalid move
            }
            else if (boardLogic.CanMineBlock(moveDir))
            {
                Move move = boardLogic.MineBlock(moveDir);
                UpdateVisualsAfterMove(move, prevPlayerPos);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (boardLogic.CanPlaceBlock(TileType.SOFT_BLOCK, moveDir))
                {
                    Move move = boardLogic.PlaceBlock(TileType.SOFT_BLOCK, moveDir);
                    UpdateVisualsAfterMove(move, prevPlayerPos);
                }
            }
            else if (boardLogic.CanPlaceBlock(TileType.SOFT_LADDER, moveDir))
            {
                Move move = boardLogic.PlaceBlock(TileType.SOFT_LADDER, moveDir);
                UpdateVisualsAfterMove(move, prevPlayerPos);
            }
        }
    }

    private void TryMovePlayer(MoveDirs moveDir)
    {
        if (boardLogic.CanMovePlayer(moveDir))
        {
            moveAnimation.FinishAnimation();

            Vector2Int prevPlayerPos = boardLogic.State.PlayerPos;
            Move move = boardLogic.MovePlayer(moveDir);
            UpdateVisualsAfterMoveV2(move, prevPlayerPos);
        }
    }

    private void UpdateVisualsAfterMove(Move move, Vector2Int prevPlayerPos)
    {
        // todo: If there are any pickup item moves, need to process when the player actually touches it

        if (move.Type == MoveTypes.MOVE)
        {
            MovePlayer movePlayer = (MovePlayer)move;
            Vector2Int newPos = movePlayer.OriginalPos + Utils.MoveDirToVector(movePlayer.Direction);
            puzzleGrid.Player.transform.position = new Vector3(newPos.x + 0.5f, newPos.y + 0.5f);
        }
        else if (move.Type == MoveTypes.MINE)
        {
            MineBlock mineBlock = (MineBlock)move;
            Vector2Int minePos = prevPlayerPos + Utils.MoveDirToVector(mineBlock.Direction);
            puzzleGrid.Tilemap.SetTile(minePos.Vector3Int(), null);
        }
        else if (move.Type == MoveTypes.PLACE_BLOCK)
        {
            PlaceBlock placeBlock = (PlaceBlock)move;
            TileBase blockTile = m_tileIds.GetIdToTileDict()[(int)TileType.SOFT_BLOCK];
            Vector2Int pos = boardLogic.State.PlayerPos + Utils.MoveDirToVector(placeBlock.Direction);
            puzzleGrid.Tilemap.SetTile(pos.Vector3Int(), blockTile);
        }
        else if (move.Type == MoveTypes.PLACE_LADDER)
        {
            PlaceBlock placeBlock = (PlaceBlock)move;
            TileBase ladderTile = m_tileIds.GetIdToTileDict()[(int)TileType.SOFT_LADDER];
            Vector2Int pos = boardLogic.State.PlayerPos + Utils.MoveDirToVector(placeBlock.Direction);
            puzzleGrid.Tilemap.SetTile(pos.Vector3Int(), ladderTile);
        }

        if (move.AdditionalMoves != null)
        {
            foreach (AdditionalMove additionalMove in move.AdditionalMoves)
            {
                if (additionalMove.Type == AdditionalMoveTypes.FALL_PLAYER)
                {
                    FallPlayer fallPlayer = (FallPlayer)additionalMove;
                    puzzleGrid.Player.transform.position = new Vector3(puzzleGrid.Player.transform.position.x, fallPlayer.ToY + 0.5f);
                }
                else if (additionalMove.Type == AdditionalMoveTypes.PICKUP_ITEM)
                {
                    PickupItem pickupItem = (PickupItem)additionalMove;
                    if (pickupItem.Item == TileType.PICK)
                    {
                        puzzleGrid.Tilemap.SetTile(pickupItem.Position.Vector3Int(), null);
                    }
                    else if (pickupItem.Item == TileType.CRYSTAL)
                    {
                        puzzleGrid.Crystal.SetActive(false);
                    }
                }
            }
        }

        // Update HUD
        int numMoves = boardLogic.State.Moves.Count;
        int numPicks = boardLogic.State.Inventory[TileType.PICK];
        int numSoftBlocks = boardLogic.State.Inventory[TileType.SOFT_BLOCK];
        int numLadders = boardLogic.State.Inventory[TileType.SOFT_LADDER];

        hud.UpdateState(numMoves, numPicks, numSoftBlocks, numLadders);
    }

    private void LoadLevel()
    {
        PlayScreenArgs args = GM.Screens.GetPlayScreenArgs();
        Puzzle puzzle = null;

#if UNITY_EDITOR
        // If args not set, load level 1
        if (args == null)
        {
            args = new PlayScreenArgs();
            args.IsStory = true;
            args.IslandNumber = 1;
            args.PuzzleNumber = 99; // Puzzle 99 of Island 1 is a special puzzle for level testing
        }
#endif

        if (args.IsStory)
        {
            StoryPuzzle storyPuzzle = GM.Screens.GetStoryPuzzle(args.IslandNumber, args.PuzzleNumber);
            puzzle = GM.LevelSerializer.LoadFromSerializedStrings(storyPuzzle.Data);
        }

        puzzleGrid.LoadPuzzle(puzzle);
        boardLogic.State = new BoardState(puzzle);
        hud.UpdateState(0, 0, 0, 0);
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
}
