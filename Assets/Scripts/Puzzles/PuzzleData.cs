[System.Serializable]
public class PuzzleData
{
    public bool[] gamePuzzle1Supports = new bool[3];
    public int gamePuzzle1Points = 0;
    public bool[] gamePuzzle2Supports = new bool[3];
    public int gamePuzzle2Points = 0;
    public bool[] gamePuzzle3Supports = new bool[3];
    public int gamePuzzle3Points = 0;
    public bool[] gamePuzzle4Supports = new bool[3];
    public int gamePuzzle4Points = 0;
    public bool[] gamePuzzle5Supports = new bool[3];
    public int gamePuzzle5Points = 0;
    public bool[] gamePuzzle6Supports = new bool[3];
    public int gamePuzzle6Points = 0;
    public bool[] gamePuzzle7Supports = new bool[3];
    public int gamePuzzle7Points = 0;
    public bool[] gamePuzzle8Supports = new bool[3];
    public int gamePuzzle8Points = 0;

    public PuzzleData(bool[] puzzle1Supports, int puzzle1Points, bool[] puzzle2Supports, int puzzle2Points,
        bool[] puzzle3Supports, int puzzle3Points, bool[] puzzle4Supports, int puzzle4Points, 
        bool[] puzzle5Supports, int puzzle5Points, bool[] puzzle6Supports, int puzzle6Points,
        bool[] puzzle7Supports, int puzzle7Points, bool[] puzzle8Supports, int puzzle8Points)
    {
        gamePuzzle1Supports = puzzle1Supports;
        gamePuzzle1Points = puzzle1Points;

        gamePuzzle2Supports = puzzle2Supports;
        gamePuzzle2Points = puzzle2Points;

        gamePuzzle3Supports = puzzle3Supports;
        gamePuzzle3Points = puzzle3Points;

        gamePuzzle4Supports = puzzle4Supports;
        gamePuzzle4Points = puzzle4Points;

        gamePuzzle5Supports = puzzle5Supports;
        gamePuzzle5Points = puzzle5Points;

        gamePuzzle6Supports = puzzle6Supports;
        gamePuzzle6Points = puzzle6Points;

        gamePuzzle7Supports = puzzle7Supports;
        gamePuzzle7Points = puzzle7Points;

        gamePuzzle8Supports = puzzle8Supports;
        gamePuzzle8Points = puzzle8Points;
    }

    public PuzzleData()
    {
        for (int i = 0; i < gamePuzzle1Supports.Length; i++)
        {
            gamePuzzle1Supports[i] = false;
        }
        gamePuzzle1Points = 0;

        for (int i = 0; i < gamePuzzle2Supports.Length; i++)
        {
            gamePuzzle2Supports[i] = false;
        }
        gamePuzzle2Points = 0;

        for (int i = 0; i < gamePuzzle3Supports.Length; i++)
        {
            gamePuzzle3Supports[i] = false;
        }
        gamePuzzle3Points = 0;

        for (int i = 0; i < gamePuzzle4Supports.Length; i++)
        {
            gamePuzzle4Supports[i] = false;
        }
        gamePuzzle4Points = 0;

        for (int i = 0; i < gamePuzzle5Supports.Length; i++)
        {
            gamePuzzle5Supports[i] = false;
        }
        gamePuzzle5Points = 0;

        for (int i = 0; i < gamePuzzle6Supports.Length; i++)
        {
            gamePuzzle6Supports[i] = false;
        }
        gamePuzzle6Points = 0;

        for (int i = 0; i < gamePuzzle7Supports.Length; i++)
        {
            gamePuzzle7Supports[i] = false;
        }
        gamePuzzle7Points = 0;

        for (int i = 0; i < gamePuzzle8Supports.Length; i++)
        {
            gamePuzzle8Supports[i] = false;
        }
        gamePuzzle8Points = 0;
    }
}
