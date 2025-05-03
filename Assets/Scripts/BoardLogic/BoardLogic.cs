using System;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    // Explicitly set values. They must equal values in smart contracts.
    NONE = 0,
    SOFT_BLOCK = 1,
    HARD_BLOCK = 2,
    SOFT_LADDER = 3,
    HARD_LADDER = 4,
    PICK = 5,
    DOOR = 6,
    KEY = 7,

    CRYSTAL = 10
}

enum ObjectType
{

}

public enum MoveTypes
{
    // Explicitly set values. They must equal values in smart contracts.
    MOVE = 0,
    MINE = 1,
    PLACE_BLOCK = 2,
    PLACE_LADDER = 3,
}

public enum AdditionalMoveTypes
{
    FALL_PLAYER,
    PICKUP_ITEM,
    KILL_PLAYER,
}

public enum MoveDirs
{
    // Explicitly set values. They must equal values in smart contracts.
    RIGHT = 1,
    LEFT = 2,
    UP = 3,
    DOWN = 4,
    RIGHT_UP = 5,
    RIGHT_DOWN = 6,
    LEFT_UP = 7,
    LEFT_DOWN = 8,
    WAIT = 9,        // Store move type wait here so we have more free space in MoveType (0-9)
}

public abstract class AdditionalMove
{
    public AdditionalMoveTypes Type { get; set; }
    public AdditionalMove(AdditionalMoveTypes moveType)
    {
        this.Type = moveType;
    }

    public abstract void Execute(BoardState state);
    public abstract void Undo(BoardState state);
}

public abstract class Move
{
    public MoveTypes Type { get; set; }
    public MoveDirs Direction { get; set; }
    public List<AdditionalMove> AdditionalMoves { get; set; }

    public Move(MoveTypes moveType, MoveDirs moveDir)
    {
        this.Type = moveType;
        this.Direction = moveDir;
        
    }

    public abstract void Execute(BoardState state);
	public abstract void Undo(BoardState state);

    public AdditionalMove AddAdditionalMove(AdditionalMove additionalMove)
    {
        if (AdditionalMoves == null)
        {
            AdditionalMoves = new List<AdditionalMove>(1);
        }
        AdditionalMoves.Add(additionalMove);
        return additionalMove;
    }

    public bool HasAdditionalMoves()
    {
        return AdditionalMoves != null && AdditionalMoves.Count > 0;
    }

    public void UndoAdditionalMoves(BoardState state)
    {
        if (AdditionalMoves == null) return;

        for (int i = AdditionalMoves.Count - 1; i >= 0; --i)
        {
            AdditionalMoves[i].Undo(state);
        }
    }
}

class MovePlayer : Move
{
    public Vector2Int OriginalPos { get; set; }

    public MovePlayer(Vector2Int originalPos, MoveDirs moveDir)
        : base(MoveTypes.MOVE, moveDir)
    {
        this.OriginalPos = originalPos;
    }

    public override void Execute(BoardState state)
    {
        Vector2Int newPos = Utils.MoveDirToVector(Direction);
        if (newPos.x != state.PlayerPos.x)
        {
            state.PlayerFacingRight = newPos.x > state.PlayerPos.x;
        }

        state.PlayerPos += newPos;
    }

    public override void Undo(BoardState state)
    {
        if (state.PlayerPos.x != OriginalPos.x)
        {
            state.PlayerFacingRight = OriginalPos.x > state.PlayerPos.x;
        }

        state.PlayerPos = OriginalPos;
    }
}

class MineBlock : Move
{
    public TileType BlockType { get; set; }

    public MineBlock(TileType blockType, MoveDirs moveDir)
        : base(MoveTypes.MINE, moveDir)
    {
        BlockType = blockType;
    }

    public override void Execute(BoardState state)
    {
        state.SetBlock(state.PlayerPos + Utils.MoveDirToVector(Direction), TileType.NONE);

        if (BlockType == TileType.DOOR)
        {
            state.DecInventory(TileType.KEY);
        }
        else
        {
            state.IncInventory(BlockType);
            state.DecInventory(TileType.PICK);
        }
    }

    public override void Undo(BoardState state)
    {
        Vector2Int pos = state.PlayerPos + Utils.MoveDirToVector(Direction);
        state.SetBlock(pos, BlockType);

        if (BlockType == TileType.DOOR)
        {
            state.IncInventory(TileType.KEY);
        }
        else
        {
            state.DecInventory(BlockType);
            state.IncInventory(TileType.PICK);
        }
    }
}

class PlaceBlock : Move
{
    public TileType BlockType { get; set; }

    public PlaceBlock(TileType blockType, MoveDirs moveDir)
        : base(Utils.BlockTypeToMoveType(blockType), moveDir)
    {
        BlockType = blockType;
    }

    public override void Execute(BoardState state)
    {
        Vector2Int pos = state.PlayerPos + Utils.MoveDirToVector(Direction);
        state.SetBlock(pos, BlockType);
        state.DecInventory(BlockType);
    }

    public override void Undo(BoardState state)
    {
        Vector2Int pos = state.PlayerPos + Utils.MoveDirToVector(Direction);
        state.SetBlock(pos, BlockType);
        state.IncInventory(BlockType);
    }
}

//
// -- Additional Moves --
//
class FallPlayer : AdditionalMove
{
    public int FromY { get; set; }
    public int ToY { get; set; }

    public FallPlayer(int fromY, int toY)
        : base(AdditionalMoveTypes.FALL_PLAYER)
    {
        this.FromY = fromY;
        this.ToY = toY;
    }

    public override void Execute(BoardState state)
    {
        state.PlayerPos = new Vector2Int(state.PlayerPos.x, ToY);
    }

    public override void Undo(BoardState state)
    {
        state.PlayerPos = new Vector2Int(state.PlayerPos.x, FromY);
    }
}

class PickupItem : AdditionalMove
{
    public TileType Item { get; set; }
    public Vector2Int Position { get; set; }

    public PickupItem(TileType item, Vector2Int position)
        : base(AdditionalMoveTypes.PICKUP_ITEM)
    {
        Item = item;
        Position = position;
    }

    public override void Execute(BoardState state)
    {
        state.SetBlock(Position, TileType.NONE);
        state.IncInventory(Item);
    }

    public override void Undo(BoardState state)
    {
        state.SetBlock(Position, Item);
        state.DecInventory(Item);
    }
}

class KillPlayer : AdditionalMove
{
    public KillPlayer()
        : base(AdditionalMoveTypes.KILL_PLAYER)
    {
    }

    public override void Execute(BoardState state)
    {
        state.PlayerAlive = false;
    }

    public override void Undo(BoardState state)
    {
        state.PlayerAlive = true;
    }
}

static class Utils
{
    public static MoveDirs VectorToMoveDir(Vector2Int vector)
    {
        if (vector.y == 0)
        {
            if (vector.x == 1)
                return MoveDirs.RIGHT;
            else if (vector.x == -1)
                return MoveDirs.LEFT;
        }
        else if (vector.y == -1)
        {
            if (vector.x == 0)
                return MoveDirs.UP;
            else if (vector.x == 1)
                return MoveDirs.RIGHT_UP;
            else if (vector.x == -1)
                return MoveDirs.LEFT_UP;
        }
        else if (vector.y == 1)
        {
            if (vector.x == 0)
                return MoveDirs.DOWN;
            else if (vector.x == 1)
                return MoveDirs.RIGHT_DOWN;
            else if (vector.x == -1)
                return MoveDirs.LEFT_DOWN;
        }

        return MoveDirs.WAIT;
    }

    public static Vector2Int MoveDirToVector(MoveDirs moveDir)
    {
        switch (moveDir)
        {
            case MoveDirs.RIGHT:         return Vector2Int.right;
            case MoveDirs.LEFT:          return Vector2Int.left;
            case MoveDirs.UP:            return Vector2Int.up;
            case MoveDirs.DOWN:          return Vector2Int.down;
            case MoveDirs.RIGHT_UP:      return new Vector2Int(1, 1);
            case MoveDirs.RIGHT_DOWN:    return new Vector2Int(1, -1);
            case MoveDirs.LEFT_UP:       return new Vector2Int(-1, 1);
            case MoveDirs.LEFT_DOWN:     return new Vector2Int(-1, -1);
            case MoveDirs.WAIT: throw new Exception("Cannot convert MoveDir.WAIT to Vector");
            default:
                throw new Exception($"MoveDir enum {moveDir} not handled");
        }
    }

    public static MoveTypes BlockTypeToMoveType(TileType blockType)
    {
        switch (blockType)
        {
            case TileType.SOFT_BLOCK: return MoveTypes.PLACE_BLOCK;
            case TileType.SOFT_LADDER: return MoveTypes.PLACE_LADDER;
            default:
                throw new Exception($"BlockTypes enum {blockType} not handled");
        }
    }
}

public class BoardState
{
    public TileType[,] Tiles { get; set; }
    public Dictionary<TileType, int> Inventory { get; set; } = new();
    public Vector2Int PlayerPos { get; set; }
    public Vector2Int ExitPos { get; set; }
    public bool PlayerFacingRight { get; set; } = true;
    public bool PlayerAlive { get; set; } = true;

    public List<Move> Moves { get; set; } = new();


    public BoardState(Puzzle puzzle)
    {
        Tiles = new TileType[Constants.LEVEL_HEIGHT, Constants.LEVEL_WIDTH];


        for (int y = 0; y < Constants.LEVEL_HEIGHT; ++y)
        {
            for (int x = 0; x < Constants.LEVEL_WIDTH; ++x)
            {
                Tiles[y, x] = (TileType)puzzle.Tiles[y, x];
            }
        }

        PlayerPos = puzzle.Player;
        ExitPos = puzzle.Exit;
        Tiles[puzzle.Crystal.y, puzzle.Crystal.x] = TileType.CRYSTAL;

        Inventory.Add(TileType.PICK, 0);
        Inventory.Add(TileType.CRYSTAL, 0);
        Inventory.Add(TileType.SOFT_BLOCK, 0);
        Inventory.Add(TileType.SOFT_LADDER, 0);
    }

    public const int U128_TOTAL_DIGITS = 39;

    // Init from serialized string
    public BoardState(string[] u128s)
    {
        Tiles = new TileType[Constants.LEVEL_HEIGHT, Constants.LEVEL_WIDTH];


        int u128sIdx = 0;
        int idSubIdx = U128_TOTAL_DIGITS - 2;

        for (int y = Constants.LEVEL_HEIGHT - 1; y >= 0; --y)
        {
            for (int x = 0; x < Constants.LEVEL_WIDTH; ++x)
            {
                int tileId = int.Parse(u128s[u128sIdx].Substring(idSubIdx, 2));
                Tiles[y,x] = (TileType)tileId;

                idSubIdx -= 2;
                if (idSubIdx <= 0)
                {
                    ++u128sIdx;
                    idSubIdx = U128_TOTAL_DIGITS - 2;
                }
            }
        }

        {
            --idSubIdx;
            (int x, int y) = decodeObjXY(u128s[u128sIdx].Substring(idSubIdx, 3));
            PlayerPos = new Vector2Int(x, y);

            idSubIdx -= 3;
            (x, y) = decodeObjXY(u128s[u128sIdx].Substring(idSubIdx, 3));
            ExitPos = new Vector2Int(x, y);

            idSubIdx -= 3;
            (x, y) = decodeObjXY(u128s[u128sIdx].Substring(idSubIdx, 3));
            Tiles[y,x] = TileType.CRYSTAL;
        }

        Inventory.Add(TileType.PICK, 0);
        Inventory.Add(TileType.CRYSTAL, 0);
        Inventory.Add(TileType.SOFT_BLOCK, 0);
        Inventory.Add(TileType.SOFT_LADDER, 0);
    }

    private (int x, int y) decodeObjXY(string data)
    {
        int quadrant = charToInt(data[2]);

        int y = charToInt(data[1]);
        if (quadrant > 2 && y < 4) y += 10;

        int x = charToInt(data[0]);
        if (quadrant % 2 == 0) x += 10;

        return (x, y);
    }

    public int charToInt(char value)
    {
        return ((int)value) - 48;
    }

    public TileType BlockAt(Vector2Int pos)
    {
        return Tiles[pos.y,pos.x];
    }

    public void SetBlock(Vector2Int pos, TileType block)
    {
        Tiles[pos.y,pos.x] = block;
    }

    public void IncInventory(TileType block)
    {
        Inventory[block]++;
    }

    public void DecInventory(TileType block)
    {
        Inventory[block]--;
    }

    public int LevelWidth => Constants.LEVEL_WIDTH;
    public int LevelHeight => Constants.LEVEL_HEIGHT;
}

class BoardLogic
{
    public BoardState State { get; set; }

    private HashSet<TileType> m_mineableBlocks;
    private HashSet<TileType> m_solidBlocks;
    private HashSet<TileType> m_ladderBlocks;

    public BoardLogic()
    {
        m_mineableBlocks = new HashSet<TileType> { TileType.SOFT_BLOCK, TileType.SOFT_LADDER };
        m_solidBlocks = new HashSet<TileType> { TileType.SOFT_BLOCK, TileType.HARD_BLOCK, TileType.DOOR };
        m_ladderBlocks = new HashSet<TileType> { TileType.SOFT_LADDER, TileType.HARD_LADDER };
    }

    public bool CanMovePlayer(MoveDirs dir)
    {
        Vector2Int pos = State.PlayerPos + Utils.MoveDirToVector(dir);
        if (IsOutOfBounds(pos)) return false;

        TileType blockType = State.BlockAt(pos);

        if (IsSolid(blockType)) return false;

        if (dir == MoveDirs.UP && !IsLadderAt(State.PlayerPos))
            return false;

        return true;
    }

    public bool CanMineBlock(MoveDirs dir)
    {
        Vector2Int pos = State.PlayerPos + Utils.MoveDirToVector(dir);
        if (IsOutOfBounds(pos)) return false;

        TileType blockType = State.BlockAt(pos);

        if (blockType == TileType.DOOR)
        {
            return State.Inventory[TileType.KEY] > 0;
        }
        else
        {
            return m_mineableBlocks.Contains(blockType) && State.Inventory[TileType.PICK] > 0;
        }
    }

    public bool CanPlaceBlock(TileType blockType, MoveDirs dir)
    {
        Vector2Int pos = State.PlayerPos + Utils.MoveDirToVector(dir);
        if (IsOutOfBounds(pos)) return false;

        return State.BlockAt(pos) == TileType.NONE && State.Inventory[blockType] > 0;
    }

    public Move MovePlayer(MoveDirs moveDir)
    {
        Move move = new MovePlayer(State.PlayerPos, moveDir);
        move.Execute(State);

        FallAndPickup(move);
        State.Moves.Add(move);
        return move;
    }

    public Move MineBlock(MoveDirs dir)
    {
        Vector2Int pos = State.PlayerPos + Utils.MoveDirToVector(dir);        
        Move move = new MineBlock(State.BlockAt(pos), dir);
        move.Execute(State);

        FallAndPickup(move);
        State.Moves.Add(move);
        return move;
    }

    public Move PlaceBlock(TileType blockType, MoveDirs dir)
    {
        Move move = new PlaceBlock(blockType, dir);
        move.Execute(State);
        State.Moves.Add(move);
        return move;
    }

    // -- Privates --
    private bool IsOutOfBounds(Vector2Int pos)
    {
        return pos.x < 0 || pos.y < 0 || pos.x >= State.LevelWidth || pos.y >= State.LevelHeight;
    }

    private bool IsSolid(TileType blockType)
    {
        return m_solidBlocks.Contains(blockType);
    }

    private bool CanStandOn(TileType blockType)
    {
        return IsSolid(blockType) || m_ladderBlocks.Contains(blockType);
    }    

    private bool IsLadderAt(Vector2Int pos)
    {
        return m_ladderBlocks.Contains(State.BlockAt(pos));
    }

    private void EncounterObstacles(Move move, TileType tile, int playerYPos)
    {
        if (!State.PlayerAlive) return;

        if (tile == TileType.PICK)
        {
            move.AddAdditionalMove(new PickupItem(TileType.PICK, new Vector2Int(State.PlayerPos.x, playerYPos))).Execute(State);
        }
        else if (tile == TileType.CRYSTAL)
        {
            move.AddAdditionalMove(new PickupItem(TileType.CRYSTAL, new Vector2Int(State.PlayerPos.x, playerYPos))).Execute(State);
        }
    }

    private void FallAndPickup(Move move)
    {        
        TileType tile = State.Tiles[State.PlayerPos.y,State.PlayerPos.x];

        // Exit early if we are on a ladder
        if (tile == TileType.SOFT_LADDER) return;
        EncounterObstacles(move, tile, State.PlayerPos.y);

        // Fall to end of level or until we are standing on something
        int playerEndY = State.PlayerPos.y;

        while (playerEndY > 0)
        {
            tile = State.Tiles[playerEndY - 1, State.PlayerPos.x];

            // Break if we've finally found something we can stand on
            if (CanStandOn(tile)) break;
            --playerEndY;

            EncounterObstacles(move, tile, playerEndY);
        }

        if (playerEndY < State.PlayerPos.y)
        {
            move.AddAdditionalMove(new FallPlayer(State.PlayerPos.y, playerEndY)).Execute(State);
        }
    }
}
