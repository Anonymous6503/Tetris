using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Shape : MonoBehaviour
{
    public Board _board;
    public TetrominoData _data;
    public Vector3Int _spawnPos;
    public Vector3Int[] _cells;

    public int _rotationIndex;

    public float _stepDelay = 1f;
    public float _lockDelay = 0.5f;

    public float _stepTime;
    public float _lockTime;
    
    public void Initialize(Vector3Int spawnPos, TetrominoData data, Board board)
    {
        this._board = board;
        this._data = data;
        this._spawnPos = spawnPos;
        this._rotationIndex = 0;
        this._stepTime = Time.time + _stepDelay;
        this._lockTime = 0;

            Debug.Log("Creating New Cell");
        if (this._cells != null)
        {
            this._cells = new Vector3Int[_data._cells.Length];
            Debug.Log("New Cell Created");
        }
        

        for (int i = 0; i < _data._cells.Length; i++)
        {
            Debug.Log(i);
            this._cells[i] = (Vector3Int)_data._cells[i];
        }
    }

    void Update()
    {
        this._board.ClearShape(this);

        this._lockTime += Time.deltaTime;
        
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveShape(Vector2Int.left);
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            MoveShape(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveShape(Vector2Int.down);
        }
        if(Input.GetKeyDown(KeyCode.Space))
            HardDropShape();

        if (Input.GetKeyDown(KeyCode.Q))
            Rotate(-1);
        else if (Input.GetKeyDown(KeyCode.E))
            Rotate(1);
        if (Time.time >= _stepTime)
            Step();
        
        this._board.SetShape(this);
    }

    public void Step()
    {
        this._stepTime = Time.time + _stepDelay;

        MoveShape(Vector2Int.down);

        if (this._lockTime >= this._lockDelay)
        {
            Lock () ;
        }
    }

    public void Lock()
    {
        this._board.SetShape(this);
        this._board.ClearRow();
        this._board.SpawnShapes();
    }

    public bool MoveShape(Vector2Int translationAmount)
    {
        Vector3Int newPosition = this._spawnPos;

        newPosition.x += translationAmount.x;
        newPosition.y += translationAmount.y;

        bool canMove = _board.IsValidPosition(this, newPosition);
        if (canMove)
        {
            this._spawnPos = newPosition;
            this._lockTime = 0;
        }

        return canMove;
    }


    public void HardDropShape()
    {
        while (MoveShape(Vector2Int.down))
        {
            continue;
        }
        Lock();
    }

    public void Rotate(int direction)
    {
        int lastRotation = this._rotationIndex;
        this._rotationIndex = wrap(this._rotationIndex + direction, 4,0);
        PerformRotation(direction);

        if (!PerformWallKick(_rotationIndex, direction))
        {
            this._rotationIndex = lastRotation;
            PerformRotation(-direction);
        }
    }

    public void PerformRotation(int direction)
    {
        for (int i = 0; i < this._cells.Length; i++)
        {
            Vector3 cell = this._cells[i];

            int x, y;

            switch (this._data._tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data._rotationMatrix[0] * direction) + (cell.y * Data._rotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data._rotationMatrix[2] * direction) + (cell.y * Data._rotationMatrix[3] * direction));
                    break;
                
                default:
                    x = Mathf.RoundToInt((cell.x * Data._rotationMatrix[0] * direction) + (cell.y * Data._rotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data._rotationMatrix[2] * direction) + (cell.y * Data._rotationMatrix[3] * direction));
                    break;
            }

            this._cells[i] = new Vector3Int(x, y, 0);
        }
    }

    public int wrap(int input, int max, int min)
    {
        /*if (input < min)
            return max - (min - input) % (max - min);
        return min + (input - min) % (max - min);*/

        if (input < min)
            return max - (min - input) % (max - min);
        return min + (input - min) % (max - min);
        
    }

    public bool PerformWallKick(int rotationalIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationalIndex, rotationDirection);

        for (int i = 0; i < this._data._wallKicks.GetLength(1);i++)
        {
            Vector2Int translation = this._data._wallKicks[wallKickIndex, 1];
            if (MoveShape(translation))
                return true;
        }

        return false;
    }

    public int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return wrap(wallKickIndex, this._data._wallKicks.GetLength(0), 0);
    }
}
