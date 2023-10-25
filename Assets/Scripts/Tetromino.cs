

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    I,
    O,
    T,
    L,
    J,
    S,
    Z,
}

[System.Serializable]
public struct TetrominoData
{
    public Tetromino _tetromino;
    public Tile _tile;
    public Vector2Int[] _cells;

    public Vector2Int[,] _wallKicks;

    public void Initialize()
    {
        this._cells = Data._cells[this._tetromino];
        this._wallKicks = Data._wallKicks[this._tetromino];
    }
}