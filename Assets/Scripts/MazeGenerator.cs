using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
        public bool useable = false;
        public int level = 0;
        public bool end = false;
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;
        private bool obligatory;

        public int ProbabilityOfSpawning(int x, int y) {
            // 0 - cannot spawn 1 - can spawn 2 - HAS to spawn
            if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y) {
                return obligatory ? 2 : 1;
            }
            return 0;
        }
    }

    public Vector2Int size;
    public int levels;
    public Rule[] rooms;
    public Vector2 offset;
    List<Cell> board;

    // Start is called before the first frame update
    void Start() {
        //Random.InitState(42);
        //Random.InitState(7);

        MazeMaker();
    }

    void MazeMaker() {
        board = new List<Cell>();
        int sizeOfGrid = size.x * levels - levels + 1;
        for (int i = 0; i < sizeOfGrid; i++) {
            for (int j = 0; j < sizeOfGrid; j++) {
                board.Add(new Cell());
            }
        }

        ValidateCell(size.x, levels);
        int currentCell = 0;
        board[currentCell].end = true;

        Stack<int> path = new Stack<int>();
        int breakVal = sizeOfGrid * (size.x - 1) + size.x - 1;
        bool backtracking = false;
        List<int> levelEnds = new List<int>(new int[levels]);
        Debug.Log("initial list: " + string.Join(", ", levelEnds) + " breakVal: " + breakVal);

        //the DFS algorithm
        while (currentCell != board.Count) {
            board[currentCell].visited = true;
            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0) {
                if (!backtracking && currentCell != board.Count - 1) {
                    int L = (int)Mathf.Floor(currentCell / breakVal);
                    levelEnds[L]++;
                    board[currentCell].end = true;
                    backtracking = true;
                }
                if (path.Count == 0) {
                    break;
                } else {
                    currentCell = path.Pop();
                }
            } else {
                backtracking = false;
                path.Push(currentCell);
                int newCell = neighbors[Random.Range(0, neighbors.Count)];
                if (newCell > currentCell) {     //down or right
                    if (newCell - 1 == currentCell) {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    } else {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                } else {                       //up or left
                    if (newCell + 1 == currentCell) {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    } else {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }
            }
        }
        //add final room
        board[board.Count - 1].visited = true;
        board[board.Count - 1].end = true;

        Debug.Log("final list: " + string.Join(", ", levelEnds));
        AddCycles();
        GenerateTheMaze();

        Occlusion occlusionCall = GameObject.Find("GameObject").GetComponent<Occlusion>();
        occlusionCall.getObjects();
    }


    // used to add cycles to the DFS plot
    void AddCycles() {
        for (int cell = 0; cell < board.Count; cell++) {
            int exitCount = board[cell].status.Where(c => c).Count();

            if (exitCount > 1) {
                //generate connection with 60% chance
                int chance = Random.Range(0, 100);
                if (chance < 60) {
                    // get a neighbor with no connection
                    // create connection and update doors/walls
                    List<int> neighbors = new List<int>();
                    int sizeOfGrid = size.x * levels - levels + 1;

                    //check up neighbor
                    if (cell >= 1 && cell - sizeOfGrid >= 0 && board[(cell - sizeOfGrid)].visited && board[cell - sizeOfGrid].status.Where(c => c).Count() > 1) {
                        neighbors.Add((cell - sizeOfGrid));
                    }

                    //check down neighbor
                    if (cell <= (sizeOfGrid * sizeOfGrid) - 1 && cell + sizeOfGrid < board.Count && board[(cell + sizeOfGrid)].visited && board[cell + sizeOfGrid].status.Where(c => c).Count() > 1) {
                        neighbors.Add((cell + sizeOfGrid));
                    }

                    //check right neighbor
                    if (cell <= (sizeOfGrid * sizeOfGrid) - 1 && (cell + 1) % sizeOfGrid != 0 && board[(cell + 1)].visited && board[cell + 1].status.Where(c => c).Count() > 1) {
                        neighbors.Add((cell + 1));
                    }

                    //check left neighbor
                    if (cell >= 1 && cell % sizeOfGrid != 0 && board[(cell - 1)].visited && board[cell - 1].status.Where(c => c).Count() > 1) {
                        neighbors.Add((cell - 1));
                    }
                    if (neighbors.Count > 0) {
                        int connect = neighbors[Random.Range(0, neighbors.Count)];
                        if (connect > cell) {     //down or right
                            if (connect - 1 == cell) {
                                board[cell].status[2] = true;
                                cell = connect;
                                board[cell].status[3] = true;
                            } else {
                                board[cell].status[1] = true;
                                cell = connect;
                                board[cell].status[0] = true;
                            }
                        } else {                       //up or left
                            if (connect + 1 == cell) {
                                board[cell].status[3] = true;
                                cell = connect;
                                board[cell].status[2] = true;
                            } else {
                                board[cell].status[0] = true;
                                cell = connect;
                                board[cell].status[1] = true;
                            }
                        }
                    }
                }
            }
        }
    }


    //this actually generates and places the rooms and doors after the DFS is run
    void GenerateTheMaze() {            
        int sizeOfGrid = size.x * levels - levels + 1;
        for (int i = 0; i < sizeOfGrid; i++) {
            for (int j = 0; j < sizeOfGrid; j++) {
                Cell currentCell = board[(i + j * sizeOfGrid)];
                if (currentCell.visited) {
                    int randomRoom = -1;
                    List<int> availableRooms = new List<int>();

                    // all rooms with one entrance will generate as a puzzle room
                    // assuming the puzzle room is the first in the room list right now
                    int exitCount = currentCell.status.Where(c => c).Count();
                    if (currentCell.end) {
                        randomRoom = 1;
                    }

                    // current room list:
                    // 0-Puzzle room - not using right now
                    // 1-Lava room - for locations with only one entrance
                    // 2-start room - start and restart destination
                    // 3-Connection rooms - located at the points between levels
                    // 4-End room - final destination with portal
                    // 5-Cross room - default if nothing else is available
                    // 6-Shire room - level 1 only
                    // 7-Forest room - level 2 only
                    // 8-Swamp room - level 3 only
                    // 9+-Any other rooms

                    for (int k = 5; k < rooms.Length; k++) {  //the k=? designates when in the list any room can spawn
                        int p = rooms[k].ProbabilityOfSpawning(i, j);
                        if (p == 2) {
                            randomRoom = k;
                            break;
                        } else if (p == 1) {
                            availableRooms.Add(k);
                        }
                    }

                    //assigning start room
                    if (i == j && i == 0) {
                        randomRoom = 2;
                    }

                    //assigning connection rooms to the breakpoints between levels
                    List<int> levNums = new List<int>();
                    for (int l = 1; l < levels; l++) {
                        levNums.Add((size.x - 1) * l);
                    }
                    if (i == j && levNums.Contains(i)) {
                        randomRoom = 3;
                    }

                    //assigning end room
                    if (i == j && i == sizeOfGrid - 1) {
                        randomRoom = 4;
                    }

                    //if no room is found it defaults as a corridor 
                    if (randomRoom == -1) {
                        if (availableRooms.Count > 0) {
                            randomRoom = availableRooms[Random.Range(0, availableRooms.Count)];
                        } else {
                            randomRoom = 5;
                        }
                    }

                    var newRoom = Instantiate(rooms[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehavior>();
                    newRoom.UpdateRoom(currentCell.status);
                    newRoom.name += " " + i + "-" + j;
                }
            }
        }
    }

    List<int> CheckNeighbors(int cell) {    // checks and returns which neighbors are on the board, useable, and not visited
        List<int> neighbors = new List<int>();
        int sizeOfGrid = size.x * levels - levels + 1;

        //check up neighbor
        if (cell >= 1 && cell - sizeOfGrid >= 0 && !board[(cell - sizeOfGrid)].visited && board[cell - sizeOfGrid].useable) {
            neighbors.Add((cell - sizeOfGrid));
        }

        //check down neighbor
        if (cell <= (sizeOfGrid * sizeOfGrid) - 1 && cell + sizeOfGrid < board.Count && !board[(cell + sizeOfGrid)].visited && board[cell + sizeOfGrid].useable) {
            neighbors.Add((cell + sizeOfGrid));
        }

        //check right neighbor
        if (cell <= (sizeOfGrid * sizeOfGrid) - 1 && (cell + 1) % sizeOfGrid != 0 && !board[(cell + 1)].visited && board[(cell + 1)].useable) {
            neighbors.Add((cell + 1));
        }

        //check left neighbor
        if (cell >= 1 && cell % sizeOfGrid != 0 && !board[(cell - 1)].visited && board[(cell - 1)].useable) {
            neighbors.Add((cell - 1));
        }

        if (neighbors.Count == 0) {
            return new List<int>();
        }
        return neighbors;
    }

    bool CheckNeighborsVVV(int cell, List<int> prevPath) {        // 0-3 --> up, down, right, left
        // searches if theres a visited neighbor connected to the main maze thats not an end room
        int sizeOfGrid = size.x * levels - levels + 1;

        //check up neighbor
        if (board[(cell - sizeOfGrid)].visited && !prevPath.Contains(cell - sizeOfGrid) && !board[cell - sizeOfGrid].end) {
            board[cell].status[0] = true;
            board[cell - sizeOfGrid].status[1] = true;
            return true;
        }

        //check down neighbor
        if (board[(cell + sizeOfGrid)].visited && !prevPath.Contains(cell + sizeOfGrid) && !board[cell + sizeOfGrid].end) {
            board[cell].status[1] = true;
            board[cell + sizeOfGrid].status[0] = true;
            return true;
        }

        //check right neighbor
        if (board[(cell + 1)].visited && !prevPath.Contains(cell + 1) && !board[cell + 1].end) {
            board[cell].status[2] = true;
            board[cell + 1].status[3] = true;
            return true;
        }

        //check left neighbor
        if (board[(cell - 1)].visited && !prevPath.Contains(cell - 1) && !board[cell - 1].end) {
            board[cell].status[3] = true;
            board[cell - 1].status[2] = true;
            return true;
        }

        return false;
    }

    // this method constricts the possible area of the maze into the stepping diagonal
    // pattern depending on the number of levels specified
    void ValidateCell(int S, int L) {    
        int B = (L * S) - L + 1;
        int dash = 2 * S - 1;
        int current = 0;

        //first row
        for (int ss = 0; ss < S; ss++) {
            board[current].useable = true;
            current++;
        }
        current += B - S;

        // starting through the levels
        for (int l = 0; l < L; l++) {   
            int scoot = l * (S - 1);       

            //middle rows of each level
            for (int s = 0; s < S - 2; s++) {  
                current += scoot;
                for (int ss = 0; ss < S; ss++) {
                    board[current].useable = true;
                    board[current].level = l;
                    current++;
                }
                current += B - S - scoot;   
            }
            // adding the dash row
            if (l != L - 1) {
                current += scoot;
                for (int d = 0; d < dash; d++) {
                    int lev = l;
                    if (d >= S) {
                        lev++;
                    }
                    board[current].useable = true;
                    board[current].level = lev;
                    current++;
                }
                current += B - dash - scoot;
            }
        }

        //last row
        current += B - S;
        for (int ss = 0; ss < S; ss++) {
            board[current].useable = true;
            board[current].level = L - 1;
            current++;
        }
    }




    // this method was developed to add additional dead ends to a maze that did not have enough
    // unused now since the DFS is run until the maze is full
    void AddDeadEnds(List<int> endCounts, int bV) {
        //adding extra dead ends to maze
        int sizeOfGrid = size.x * levels - levels + 1;

        // basically running some mini DFSs to pathfind to a room thats been visited
        // room must also be part of the greater maze and not an end room

        for (int l = 0; l < levels; l++) {
            int lim1 = bV * l;
            int lim2 = (bV * (l + 1)) - 1;
            List<int> pathTravelled = new List<int>();
            while (endCounts[l] < 3) {
            // add rooms in this level
            End:
                int newRoom = Random.Range(lim1, lim2);
                if (board[newRoom].useable && !board[newRoom].visited) {
                    pathTravelled.Add(newRoom);
                    board[newRoom].visited = true;
                    board[newRoom].end = true;

                    //checking if a neighbor is visited and updating doors if so
                    while (!CheckNeighborsVVV(newRoom, pathTravelled)) {

                        List<int> neighbors = CheckNeighbors(newRoom);
                        int nextRoom = neighbors[Random.Range(0, neighbors.Count)];
                        //right
                        if (nextRoom - 1 == newRoom && !pathTravelled.Contains(nextRoom)) {
                            board[newRoom].status[2] = true;
                            board[nextRoom].status[3] = true;
                            newRoom = nextRoom;
                            pathTravelled.Add(newRoom);
                            board[newRoom].visited = true;
                            //down
                        } else if (nextRoom - sizeOfGrid == newRoom && !pathTravelled.Contains(nextRoom)) {
                            board[newRoom].status[1] = true;
                            board[nextRoom].status[0] = true;
                            newRoom = nextRoom;
                            pathTravelled.Add(newRoom);
                            board[newRoom].visited = true;
                            //left
                        } else if (nextRoom + 1 == newRoom && !pathTravelled.Contains(nextRoom)) {
                            board[newRoom].status[3] = true;
                            board[nextRoom].status[2] = true;
                            newRoom = nextRoom;
                            pathTravelled.Add(newRoom);
                            board[newRoom].visited = true;
                            //up
                        } else if (nextRoom + sizeOfGrid == newRoom && !pathTravelled.Contains(nextRoom)) {
                            board[newRoom].status[0] = true;
                            board[nextRoom].status[1] = true;
                            newRoom = nextRoom;
                            pathTravelled.Add(newRoom);
                            board[newRoom].visited = true;
                        } else {
                            Debug.Log("dead end fail: " + l + " at: " + nextRoom);

                            //if all of the neighbors are in pathTravelled weve got a cycle
                            //delete the path travelled and start again with a new deadend room
                            foreach (var cel in pathTravelled) {
                                board[cel].visited = false;
                                board[cel].end = false;
                                board[cel].status = new bool[4];
                                pathTravelled = new List<int>();
                            }
                            goto End;
                        }
                    }
                    //Debug.Log("dead end success!! at: " + newRoom);
                    pathTravelled = new List<int>();
                    endCounts[l]++;
                }
            }
        }
    }
}
