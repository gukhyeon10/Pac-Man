
enum EObjectType
{
    WALL = 0,
    ITEM = 1,
    CHARACTER = 2,
}

public enum EDirect
{
    EAST = 0,
    WEST = 1,
    SOUTH = 2,
    NORTH = 3,
}

enum EWall
{
    DEFAULT = 0,
    CURVE = 1,
    LINE = 2,
    POP = 3,
    EDGE = 4,
    RIGHTDOOR = 5,
    LEFTDOOR = 6,
    CENTERDOOR = 7,
}

enum ECharacter
{
    PAC = 0,
    BLINKY = 1,
    PINKY = 2,
    INKY = 3,
    CLYDE = 4,
}

enum EItem
{
    NORMAL = 0,
    SUPER = 1,
    CHERRY = 2,
    BERRY = 3,
    PEAR = 4,
    APPLE = 5,
    MELON = 6,
}

enum EPanel
{
    START = 0,
    KEY = 1,
    UI = 2,
    STAGE_FAIL = 3,
    STAGE_CLEAR = 4,
}

enum EResult
{
    GAME_OVER = 0,
    TIME_OVER = 1,
    STAGE_CLEAR = 2,
    GAME_CLEAR = 3,
}

public struct PrimNode
{
    public int row, col;
    public bool isCheck, isNear;
    public bool isUp, isDown, isLeft, isRight;
    public int parentNodeDirect;
}

public struct BfsNode
{
    private bool isVisited;
    private (int preRow, int preCol) preCoord;
    private (int row, int col) coord;
    
    public bool IsVisited => isVisited;
    public (int preRow, int preCol) PreCoord => preCoord;
    public (int row, int col) Coord => coord;

    public void Modify((int row, int col) coord, (int preRow, int preCol) preCoord, bool isVisited)
    {
        this.coord = coord;
        this.preCoord = preCoord;
        this.isVisited = isVisited;
    }
}
