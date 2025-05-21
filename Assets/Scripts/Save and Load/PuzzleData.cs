using System.Collections.Generic;

[System.Serializable]
public class PuzzleData
{
    public List<PuzzleState> gamePuzzleStates;

    public PuzzleData(List<PuzzleState> puzzleStates)
    {
        gamePuzzleStates = new List<PuzzleState>(puzzleStates);
    }

    public PuzzleData()
    {
        gamePuzzleStates = new List<PuzzleState>();
    }
}

[System.Serializable]
public class PuzzleState
{
    public string gamePuzzleName;
    public bool[] gamePuzzleSupports = new bool[3];
    public int gamePuzzlePoints;
    public bool gameIsPuzzleComplete;

    public PuzzleState(string puzzleName, bool[] puzzleSupports, int puzzlePoints, bool isPuzzleComplete)
    {
        gamePuzzleName = puzzleName;
        gamePuzzleSupports = (bool[])puzzleSupports.Clone();
        gamePuzzlePoints = puzzlePoints;
        gameIsPuzzleComplete = isPuzzleComplete;
    }

    public PuzzleState()
    {
        gamePuzzleName = "";
        gamePuzzlePoints = 0;
        gameIsPuzzleComplete = false;
    }
}