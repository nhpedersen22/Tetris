namespace Tetris;

public class GameState {
    private Block currentBlock;

    public Block CurrentBlock {
        get => currentBlock;
        private set {
            currentBlock = value;
            currentBlock.Reset();
        }
    }

    public GameGrid GameGrid { get; }
    public BlockQueue BlockQueue { get; }
    public bool GameOver { get; private set; }

    public GameState() {
        GameGrid = new GameGrid(22, 10);
        BlockQueue = new BlockQueue();
        CurrentBlock = BlockQueue.GetAndUpdate();
    }

    /**
        Checks if the current block is in a legal position.
    */
    private bool BlockFits() {
        foreach (Position p in CurrentBlock.TilePositions()) {
            if (!GameGrid.IsEmpty(p.Row, p.Column)) {
                return false;
            }
        }

        return true;
    }

    /*
        For all Rotate and Move methods, the strategy is as follows:
        1. Rotate/Move the Block.
        2. If this results in an illegal position, Rotate/Move it back.
    */

    public void RotateBlockCW() {
        CurrentBlock.RotateCW();

        if (!BlockFits()) {
            CurrentBlock.RotateCCW();
        }
    }
    
    public void RotateBlockCCW() {
        CurrentBlock.RotateCCW();

        if (!BlockFits()) {
            CurrentBlock.RotateCW();
        }
    }

    public void MoveBlockLeft() {
        CurrentBlock.Move(0, -1);
        
        if (!BlockFits()) {
            CurrentBlock.Move(0, 1);
        }
    }

    public void MoveBlockRight() {
        CurrentBlock.Move(0, 1);
        
        if (!BlockFits()) {
            CurrentBlock.Move(0, -1);
        }
    }

    /**
        If either of the hidden rows on the top aren't empty,
        the game is lost.
    */
    private bool IsGameOver() {
        return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
    }

    /**
        This will be called when the CurrentBlock cannot move down.
    */
    private void PlaceBlock() {
        foreach (Position p in CurrentBlock.TilePositions()) {
            GameGrid[p.Row, p.Column] = CurrentBlock.Id;
        }

        GameGrid.ClearFullRows();

        if (IsGameOver()) {
            GameOver = true;
        } else {
            CurrentBlock = BlockQueue.GetAndUpdate();
        }
    }

    public void MoveBlockDown() {
        CurrentBlock.Move(1, 0);

        if(!BlockFits()) {
            CurrentBlock.Move(-1, 0);
            PlaceBlock();
        }
    }
}