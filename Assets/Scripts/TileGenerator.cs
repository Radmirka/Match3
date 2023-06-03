using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileGenerator : MonoBehaviour
{
    public GameObject emptyTilePrefab; // ������ ������ ������ (���� � �������)
    public GameObject spriteTilePrefab; // ������ ������ � ��������� �������� (������� � �������)
    public Sprite[] tileSprites; // ������ �������� ��� ������

    public int templateIndex = 0; // ������ �������

    private FieldTemplateManager templateManager; // �������� �������� ����

    public GameObject firstSelectedTile; // ������ ��������� ������
    public Color originalColor; // �������� ���� �������

    public GameObject selectedTile;

    public GameObject SelectedTile
    {
        get { return selectedTile; }
        set { selectedTile = value; }
    }

    public TileMatrix tileMatrix;


    private Dictionary<Vector2Int, ClickableTile> clickableTiles;

    private void Start()
    {
        templateManager = new FieldTemplateManager();
        clickableTiles = new Dictionary<Vector2Int, ClickableTile>();
        GenerateTiles();
    }

    public void GenerateTiles()
    {
        FieldTemplate template = templateManager.GetTemplateByIndex(templateIndex); // �������� ������ ���� �� ���������� �������

        int rows = template.layout.GetLength(0);
        int columns = template.layout.GetLength(1);

        RectTransform parentRectTransform = GetComponent<RectTransform>();
        float tileSizeX = parentRectTransform.rect.width / columns;
        float tileSizeY = parentRectTransform.rect.height / rows;

        float offsetX = -parentRectTransform.rect.width / 2 + tileSizeX / 2;
        float offsetY = parentRectTransform.rect.height / 2 - tileSizeY / 2;

        tileMatrix = new TileMatrix(rows, columns); // �������� ���������� TileMatrix

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int? tileType = template.layout[i, j];

                if (tileType.HasValue && tileType >= 0 && tileType < tileSprites.Length)
                {
                    GameObject newTile = Instantiate(spriteTilePrefab, transform);

                    RectTransform tileRectTransform = newTile.GetComponent<RectTransform>();
                    tileRectTransform.sizeDelta = new Vector2(tileSizeX, tileSizeY);
                    tileRectTransform.anchoredPosition = new Vector2(offsetX + j * tileSizeX, offsetY - i * tileSizeY);

                    Sprite sprite = tileSprites[tileType.Value];
                    newTile.GetComponent<SpriteRenderer>().sprite = sprite;

                    // ���������� ��������� �������� ������ � TileMatrix
                    tileMatrix.SetValue(i, j, tileType.Value);

                    // ��������� ��������� �����
                    ClickableTile clickableTile = newTile.AddComponent<ClickableTile>();
                    clickableTile.TileGenerator = this;
                    clickableTile.Position = new Vector2Int(i, j);
                }
                else
                {
                    GameObject newTile = Instantiate(emptyTilePrefab, transform);

                    RectTransform tileRectTransform = newTile.GetComponent<RectTransform>();
                    tileRectTransform.sizeDelta = new Vector2(tileSizeX, tileSizeY);
                    tileRectTransform.anchoredPosition = new Vector2(offsetX + j * tileSizeX, offsetY - i * tileSizeY);

                    // ��������� �������� ������ ������ � TileMatrix
                    tileMatrix.SetValue(i, j, -1);
                }
            }
        }
    }

    public void CheckLines()
    {
        // Check horizontal lines
        for (int row = 0; row < tileMatrix.Rows; row++)
        {
            int count = 1;
            for (int col = 1; col < tileMatrix.Columns; col++)
            {
                int currentValue = tileMatrix.GetValue(row, col);
                int previousValue = tileMatrix.GetValue(row, col - 1);

                if (currentValue == previousValue)
                {
                    count++;
                    if (count >= 3 && col == tileMatrix.Columns - 1)
                    {
                        // Remove line of three or more sprites
                        for (int i = col - count + 1; i <= col; i++)
                        {
                            ClickableTile tile = GetClickableTile(row, i);
                            tile.TileGenerator.tileMatrix.UpdateTileValue(row, i, 100);
                            tile.spriteRenderer.sprite = null;
                        }
                    }
                }
                else
                {
                    if (count >= 3)
                    {
                        // Remove line of three or more sprites
                        for (int i = col - count; i < col; i++)
                        {
                            ClickableTile tile = GetClickableTile(row, i);
                            tile.TileGenerator.tileMatrix.UpdateTileValue(row, i, 100);
                            tile.spriteRenderer.sprite = null;
                        }
                    }
                    count = 1;
                }
            }
        }

        // Check vertical lines
        for (int col = 0; col < tileMatrix.Columns; col++)
        {
            int count = 1;
            for (int row = 1; row < tileMatrix.Rows; row++)
            {
                int currentValue = tileMatrix.GetValue(row, col);
                int previousValue = tileMatrix.GetValue(row - 1, col);

                if (currentValue == previousValue)
                {
                    count++;
                    if (count >= 3 && row == tileMatrix.Rows - 1)
                    {
                        // Remove line of three or more sprites
                        for (int i = row - count + 1; i <= row; i++)
                        {
                            ClickableTile tile = GetClickableTile(i, col);
                            tile.TileGenerator.tileMatrix.UpdateTileValue(i, col, 100);
                            tile.spriteRenderer.sprite = null;
                        }
                    }
                }
                else
                {
                    if (count >= 3)
                    {
                        // Remove line of three or more sprites
                        for (int i = row - count; i < row; i++)
                        {
                            ClickableTile tile = GetClickableTile(i, col);
                            tile.TileGenerator.tileMatrix.UpdateTileValue(i, col, 100);
                            tile.spriteRenderer.sprite = null;
                        }
                    }
                    count = 1;
                }
            }
        }

        StartCoroutine(ShiftTilesAfterDelay());
    }

    private IEnumerator ShiftTilesAfterDelay()
    {
        yield return new WaitForSeconds(1.0f); // Adjust the delay duration as needed

        StartCoroutine(ShiftTiles());
    }



    public List<ClickableTile> FindEmptyTiles()
    {
        List<ClickableTile> emptyTiles = new List<ClickableTile>();

        foreach (ClickableTile clickableTile in FindObjectsOfType<ClickableTile>())
        {
            if (clickableTile.spriteRenderer.sprite == null)
            {
                emptyTiles.Add(clickableTile);
            }
        }

        return emptyTiles;
    }

    public IEnumerator ShiftTiles()
    {
        // �������� ������ ������ ��������
        List<ClickableTile> emptyTiles = FindEmptyTiles();

        // �������� �� ������ ������ ������ � �������� �������
        for (int i = emptyTiles.Count - 1; i >= 0; i--)
        {
            ClickableTile emptyTile = emptyTiles[i];
            int row = emptyTile.Position.x;
            int col = emptyTile.Position.y;

            // ���������� ������� ���� ������ ������ �����
            for (int j = row; j > 0; j--)
            {
                ClickableTile currentTile = GetClickableTile(j, col);
                ClickableTile previousTile = GetClickableTile(j - 1, col);

                // ������ ������� � �������� � �������
                currentTile.spriteRenderer.sprite = previousTile.spriteRenderer.sprite;
                tileMatrix.UpdateTileValue(j, col, tileMatrix.GetValue(j - 1, col));

                // �������� ��� �������� �������� �������
                yield return new WaitForSeconds(0.03f);

                // ������� ������ � �������� � ������� ��� ���������� ������
                previousTile.spriteRenderer.sprite = null;
                tileMatrix.UpdateTileValue(j - 1, col, -1);
            }
        }

        FillEmptyTiles();

        yield return new WaitForSeconds(0.5f); // �������� 0.5 ������� ����� ���������� ������ ������ ����� ��������� �����
        CheckLines();
    }


    public void FillEmptyTiles()
    {
        List<ClickableTile> emptyTiles = FindEmptyTiles();

        foreach (ClickableTile emptyTile in emptyTiles)
        {
            int row = emptyTile.Position.x;
            int col = emptyTile.Position.y;

            // ���������� ��������� ������ ��� ������ ����������� �� ������� ��������
            int randomIndex = Random.Range(0, tileSprites.Length);

            // ������������� ��������� ����������� �� ������ ������
            emptyTile.spriteRenderer.sprite = tileSprites[randomIndex];

            // ��������� �������� � �������
            tileMatrix.UpdateTileValue(row, col, randomIndex);

        }
    }


    private ClickableTile GetClickableTile(int row, int col)
    {
        GameObject tileObject = transform.GetChild(row * tileMatrix.Columns + col).gameObject;
        return tileObject.GetComponent<ClickableTile>();
    }
}