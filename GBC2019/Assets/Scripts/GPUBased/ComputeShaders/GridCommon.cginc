#ifndef GRID_COMMON_INCLUDED
#define GRID_COMMON_INCLUDED
struct BlockStruct
{
    int ID;
    int creationID;
    int type;
    int subType;
    int dead;
    int left;
    int right;
    int up;
    int down;
    int centerX;
    int centerY;
    int velocityX;
    int velocityY;
    int moveTicks;
    int currentTick;
    int canMove;
    int attachToBlockId;
    int attatched;
    int absorbPriority;
    int absorbType;
    int ignoreType;
    int canAbsorb;
    int CanBeAbsorbed;
    int KillNonMatching;
    int KillableByNonMatching;
    int centerCellSquareDistance;
    int centerGridID;
    int centerCellID;
    int rotateDirection;
    int aboutToDie;
    int isGrenade;
    int cellCount;
};

struct CellStruct
{
    int ID;
    int gridPosX;
    int gridPosY;
    int parentBlockID;
    int dying;
    int dead;
    int left;
    int right;
    int up;
    int down;
    int rotTicks;
    float lastX;
    float lastY;
    int connectedToCenter;
    int isImpactDeath;
    float velocityX;
    float velocityY;
    float timeSinceImpact;
};

struct GridCell
{
    int cells[10];
};
#endif // GRID_COMMON_INCLUDED
