using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// todo: All pickups should be objects with box collider.
// We only update the HUD when the objects are actually picked up

public class MoveAnimation
{
    public MoveAnimation(PuzzleGrid puzzleGrid)
    {
        this.puzzleGrid = puzzleGrid;
    }

    private PuzzleGrid puzzleGrid;
    private bool isMove = false;
    private bool isFall = false;
    private Vector3 startPos;
    private Vector3 firstMovePos;
    private Vector3 lastMovePos;
    private float timer = 0;

    public void AnimatePlayer(Vector3 firstMovePos, Vector3 lastMovePos, bool isMove, bool isFall)
    {
        timer = 0;
        startPos = puzzleGrid.Player.transform.position;
        this.firstMovePos= firstMovePos;
        this.lastMovePos= lastMovePos;
        this.isMove = isMove;
        this.isFall = isFall;
    }

    public void Update()
    {
        if (isMove)
        {
            UpdateMove();
        }
        else if (isFall)
        {
            UpdateFall();
        }
    }

    public void FinishAnimation()
    {
        if (isMove || isFall)
        {
            isMove = false;
            isFall = false;
            puzzleGrid.Player.SetGravity(false);
            puzzleGrid.Player.transform.position = lastMovePos;
        }
    }

    private void UpdateMove()
    {
        float speed = 20;
        timer += Time.deltaTime * speed;
        puzzleGrid.Player.transform.position = Vector3.Lerp(startPos, firstMovePos, timer);
        if (timer >= 1f)
        {
            isMove = false;
        }
    }

    private void UpdateFall()
    {
        if (!puzzleGrid.Player.Gravity)
        {
            puzzleGrid.Player.SetGravity(true);
        }

        if (puzzleGrid.Player.transform.position.y <= lastMovePos.y)
        {
            puzzleGrid.Player.SetGravity(false);
            puzzleGrid.Player.transform.position = lastMovePos;
            isFall = false;
        }
    }
}

public partial class PlayScreen
{
    private void UpdateVisualsAfterMoveV2(Move move, Vector2Int prevPlayerPos)
    {
        bool isMovePlayer = false;
        bool isFallPlayer = false;
        Vector3 playerPosFirstMove = puzzleGrid.Player.transform.position;

        if (move.Type == MoveTypes.MOVE)
        {
            MovePlayer movePlayer = (MovePlayer)move;
            Vector2Int newPos = movePlayer.OriginalPos + Utils.MoveDirToVector(movePlayer.Direction);
            playerPosFirstMove = new Vector3(newPos.x + 0.5f, newPos.y + 0.5f);
            isMovePlayer = true;
        }
        else if (move.Type == MoveTypes.MINE)
        {
            MineBlock mineBlock = (MineBlock)move;
            Vector2Int minePos = prevPlayerPos + Utils.MoveDirToVector(mineBlock.Direction);

            // todo: Update later after slight delay
            puzzleGrid.Tilemap.SetTile(minePos.Vector3Int(), null);
        }
        else if (move.Type == MoveTypes.PLACE_BLOCK)
        {
            PlaceBlock placeBlock = (PlaceBlock)move;
            TileBase blockTile = m_tileIds.GetIdToTileDict()[(int)TileType.SOFT_BLOCK];
            Vector2Int pos = boardLogic.State.PlayerPos + Utils.MoveDirToVector(placeBlock.Direction);

            // todo: Update later after slight delay
            puzzleGrid.Tilemap.SetTile(pos.Vector3Int(), blockTile);
        }
        else if (move.Type == MoveTypes.PLACE_LADDER)
        {
            PlaceBlock placeBlock = (PlaceBlock)move;
            TileBase ladderTile = m_tileIds.GetIdToTileDict()[(int)TileType.SOFT_LADDER];
            Vector2Int pos = boardLogic.State.PlayerPos + Utils.MoveDirToVector(placeBlock.Direction);

            // todo: Update later after slight delay
            puzzleGrid.Tilemap.SetTile(pos.Vector3Int(), ladderTile);
        }

        // Additional Moves
        Vector3 playerPosFinalMove = playerPosFirstMove;

        if (move.AdditionalMoves != null)
        {
            foreach (AdditionalMove additionalMove in move.AdditionalMoves)
            {
                if (additionalMove.Type == AdditionalMoveTypes.FALL_PLAYER)
                {
                    FallPlayer fallPlayer = (FallPlayer)additionalMove;
                    playerPosFinalMove = new Vector3(playerPosFirstMove.x, fallPlayer.ToY + 0.5f);
                    isFallPlayer = true;
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

        if (playerPosFirstMove.x > puzzleGrid.Player.transform.position.x)
            puzzleGrid.Player.SetFacingRight(true);
        else if (playerPosFirstMove.x < puzzleGrid.Player.transform.position.x)
            puzzleGrid.Player.SetFacingRight(false);

        // Animate moving player
        moveAnimation.AnimatePlayer(playerPosFirstMove, playerPosFinalMove, isMovePlayer, isFallPlayer);

        // Update HUD - todo: Update pickups only when touched by player
        int numMoves = boardLogic.State.Moves.Count;
        int numPicks = boardLogic.State.Inventory[TileType.PICK];
        int numSoftBlocks = boardLogic.State.Inventory[TileType.SOFT_BLOCK];
        int numLadders = boardLogic.State.Inventory[TileType.SOFT_LADDER];

        hud.UpdateState(numMoves, numPicks, numSoftBlocks, numLadders);
    }
}
