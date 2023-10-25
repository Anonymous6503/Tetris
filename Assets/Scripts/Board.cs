using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public Tilemap _tilemap;
    public TetrominoData[] _tetrominos;

    public Shape _activeShape;
    public Vector3Int _spawnPosition;

    public Vector2Int boardSize = new Vector2Int(10,20);

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }
    private void Awake()
    {
        this._tilemap = GetComponentInChildren<Tilemap>();
        this._activeShape = GetComponentInChildren<Shape>();
        for (int i = 0; i < _tetrominos.Length; i++)
        {
            this._tetrominos[i].Initialize();
        }
    }


    private void Start()
    {
        SpawnShapes();
    }

    public void SpawnShapes()
    {
        int random = Random.Range(0, _tetrominos.Length);
        TetrominoData data = this._tetrominos[random];
        
        this._activeShape.Initialize(this._spawnPosition, data, this);

        if (IsValidPosition(this._activeShape, this._spawnPosition))
        {
            SetShape(_activeShape);
        }
        else
        {
            GameOver();
        }
        
    }

    public void SetShape(Shape shape)
    {
        for (int i = 0; i < shape._cells.Length; i++)
        {
            Vector3Int tilePos = shape._cells[i] + shape._spawnPos;
            this._tilemap.SetTile(tilePos,shape._data._tile);
        }
    }
    
    public void ClearShape(Shape shape)
    {
        for (int i = 0; i < shape._cells.Length; i++)
        {
            Vector3Int tilePos = shape._cells[i] + shape._spawnPos;
            this._tilemap.SetTile(tilePos,null);
        }
    }

    public bool IsValidPosition(Shape shape, Vector3Int position)
    {
        RectInt bounds = this.Bounds;
        for (int i = 0; i < shape._cells.Length; i++)
        {
            Vector3Int tilePos = shape._cells[i] + position;

            if (!bounds.Contains((Vector2Int) tilePos))
                return false;
            
            if (this._tilemap.HasTile(tilePos))
                return false;
        }

        return true;
    }

    public void ClearRow()
    {
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (CheckRow(row))
                ClearLine(row);
            else
                row++;
        }
    }

    public void ClearLine(int row)
    {
        RectInt bounds = this.Bounds;

        for (int i = bounds.xMin; i < bounds.xMax; i++)
        {
            Vector3Int pos = new Vector3Int(i, row, 0);
            this._tilemap.SetTile(pos, null);
        }

        while (row < bounds.yMax)
        {
            for (int i = bounds.xMin; i < bounds.xMax; i++)
            {
                Vector3Int pos = new Vector3Int(i, row + 1, 0);
                TileBase tileBase = this._tilemap.GetTile(pos);

                pos = new Vector3Int(i, row, 0);
                
                this._tilemap.SetTile(pos, tileBase);
            }

            row++;
        }
    }

    public bool CheckRow(int row)
    {
        RectInt bound = this.Bounds;

        for (int i = bound.xMin; i < bound.xMax; i++)
        {
            Vector3Int pos = new Vector3Int(i, row, 0);
            if (!this._tilemap.HasTile(pos))
                return false;
        }

        return true;
    }

    public void GameOver()
    {
        Debug.Log("GameOver");
    }
}
