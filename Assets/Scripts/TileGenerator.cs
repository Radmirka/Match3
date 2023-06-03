using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileGenerator : MonoBehaviour
{
    public GameObject emptyTilePrefab; // Префаб пустой плитки (ноль в шаблоне)
    public GameObject spriteTilePrefab; // Префаб плитки с рандомным спрайтом (единица в шаблоне)
    public Sprite[] tileSprites; // Массив спрайтов для плиток

    public int templateIndex = 0; // Индекс шаблона

    private FieldTemplateManager templateManager; // Менеджер шаблонов поля

    public GameObject firstSelectedTile; // Первый выбранный спрайт
    public Color originalColor; // Исходный цвет спрайта

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
        FieldTemplate template = templateManager.GetTemplateByIndex(templateIndex); // Получаем шаблон поля по указанному индексу

        int rows = template.layout.GetLength(0);
        int columns = template.layout.GetLength(1);

        RectTransform parentRectTransform = GetComponent<RectTransform>();
        float tileSizeX = parentRectTransform.rect.width / columns;
        float tileSizeY = parentRectTransform.rect.height / rows;

        float offsetX = -parentRectTransform.rect.width / 2 + tileSizeX / 2;
        float offsetY = parentRectTransform.rect.height / 2 - tileSizeY / 2;

        tileMatrix = new TileMatrix(rows, columns); // Создание экземпляра TileMatrix

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

                    // Сохранение числового значения плитки в TileMatrix
                    tileMatrix.SetValue(i, j, tileType.Value);

                    // Добавляем компонент клика
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

                    // Установка значения пустой плитки в TileMatrix
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
        // Получаем список пустых спрайтов
        List<ClickableTile> emptyTiles = FindEmptyTiles();

        // Проходим по каждой пустой плитке в обратном порядке
        for (int i = emptyTiles.Count - 1; i >= 0; i--)
        {
            ClickableTile emptyTile = emptyTiles[i];
            int row = emptyTile.Position.x;
            int col = emptyTile.Position.y;

            // Перемещаем спрайты ниже пустой плитки вверх
            for (int j = row; j > 0; j--)
            {
                ClickableTile currentTile = GetClickableTile(j, col);
                ClickableTile previousTile = GetClickableTile(j - 1, col);

                // Меняем спрайты и значения в матрице
                currentTile.spriteRenderer.sprite = previousTile.spriteRenderer.sprite;
                tileMatrix.UpdateTileValue(j, col, tileMatrix.GetValue(j - 1, col));

                // Задержка для создания анимации подъема
                yield return new WaitForSeconds(0.03f);

                // Очищаем спрайт и значение в матрице для предыдущей плитки
                previousTile.spriteRenderer.sprite = null;
                tileMatrix.UpdateTileValue(j - 1, col, -1);
            }
        }

        FillEmptyTiles();

        yield return new WaitForSeconds(0.5f); // Задержка 0.5 секунды после заполнения пустых плиток перед проверкой линий
        CheckLines();
    }


    public void FillEmptyTiles()
    {
        List<ClickableTile> emptyTiles = FindEmptyTiles();

        foreach (ClickableTile emptyTile in emptyTiles)
        {
            int row = emptyTile.Position.x;
            int col = emptyTile.Position.y;

            // Генерируем случайный индекс для выбора изображения из массива спрайтов
            int randomIndex = Random.Range(0, tileSprites.Length);

            // Устанавливаем случайное изображение на пустую плитку
            emptyTile.spriteRenderer.sprite = tileSprites[randomIndex];

            // Обновляем значение в матрице
            tileMatrix.UpdateTileValue(row, col, randomIndex);

        }
    }


    private ClickableTile GetClickableTile(int row, int col)
    {
        GameObject tileObject = transform.GetChild(row * tileMatrix.Columns + col).gameObject;
        return tileObject.GetComponent<ClickableTile>();
    }
}