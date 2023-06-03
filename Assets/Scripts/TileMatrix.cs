using UnityEngine;

public class TileMatrix
{
    private int[,] matrix; // Матрица для хранения числовых значений плиток

    public int Rows { get; private set; } // Количество строк в матрице
    public int Columns { get; private set; } // Количество столбцов в матрице

    public TileMatrix(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        matrix = new int[rows, columns];
    }

    public int GetValue(int row, int column)
    {
        if (IsValidIndex(row, column))
        {
            return matrix[row, column];
        }
        else
        {
            Debug.LogError("Invalid matrix index: (" + row + ", " + column + ")");
            return -1;
        }
    }

    public void SetValue(int row, int column, int value)
    {
        if (IsValidIndex(row, column))
        {
            matrix[row, column] = value;
        }
        else
        {
            Debug.LogError("Invalid matrix index: (" + row + ", " + column + ")");
        }
    }

    public void UpdateTileValue(int row, int column, int value)
    {
        SetValue(row, column, value);
    }

    private bool IsValidIndex(int row, int column)
    {
        return row >= 0 && row < Rows && column >= 0 && column < Columns;
    }

    public void PrintMatrix()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                int value = GetValue(i, j);
                Debug.Log("(" + i + ", " + j + "): " + value);
            }
        }
    }

    public void UpdateMatrix(int row1, int col1, int row2, int col2)
    {
        int temp = matrix[row1, col1];
        matrix[row1, col1] = matrix[row2, col2];
        matrix[row2, col2] = temp;
    }

}
