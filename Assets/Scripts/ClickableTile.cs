using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClickableTile : MonoBehaviour
{
    public TileGenerator TileGenerator { get; set; }
    public Vector2Int Position { get; set; }

    public SpriteRenderer spriteRenderer;
    private bool isSelected = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        if (!isSelected)
        {
            if (TileGenerator.SelectedTile == null)
            {
                // Первый выбранный спрайт
                TileGenerator.SelectedTile = this.gameObject;
                // Сохраняем исходный цвет спрайта
                TileGenerator.originalColor = spriteRenderer.color;
                // Изменяем цвет спрайта на серый
                spriteRenderer.color = Color.gray;
                isSelected = true;
            }
            else
            {
                // Второй выбранный спрайт
                GameObject secondSelectedTile = this.gameObject;
                // Получаем позиции выбранных спрайтов в матрице
                Vector2Int firstTilePos = TileGenerator.SelectedTile.GetComponent<ClickableTile>().Position;
                Vector2Int secondTilePos = Position;

                // Проверяем, является ли второй спрайт соседним по горизонтали или вертикали
                if (IsNeighbour(firstTilePos, secondTilePos))
                {
                    // Меняем спрайты местами
                    SwapSprites(TileGenerator.SelectedTile, secondSelectedTile);

                    // Обновляем значения в матрице
                    int firstTileValue = TileGenerator.tileMatrix.GetValue(firstTilePos.x, firstTilePos.y);
                    int secondTileValue = TileGenerator.tileMatrix.GetValue(secondTilePos.x, secondTilePos.y);
                    TileGenerator.tileMatrix.UpdateTileValue(firstTilePos.x, firstTilePos.y, secondTileValue);
                    TileGenerator.tileMatrix.UpdateTileValue(secondTilePos.x, secondTilePos.y, firstTileValue);

                    // Снимаем выбор со спрайтов
                    TileGenerator.SelectedTile = null;
                    spriteRenderer.color = TileGenerator.originalColor;
                    isSelected = false;

                }
                else
                {
                    // Ничего не делаем, если второй спрайт не является соседним
                }
            }
        }
        else
        {
            // Снять выбор со спрайта
            TileGenerator.SelectedTile = null;
            spriteRenderer.color = TileGenerator.originalColor;
            isSelected = false;
        }
    }

    private bool IsNeighbour(Vector2Int pos1, Vector2Int pos2)
    {
        int distanceX = Mathf.Abs(pos1.x - pos2.x);
        int distanceY = Mathf.Abs(pos1.y - pos2.y);

        return (distanceX == 1 && distanceY == 0) || (distanceX == 0 && distanceY == 1);
    }

    private IEnumerator AnimateSwap(GameObject firstSprite, GameObject secondSprite)
    {
        SpriteRenderer firstRenderer = firstSprite.GetComponent<SpriteRenderer>();
        SpriteRenderer secondRenderer = secondSprite.GetComponent<SpriteRenderer>();

        // Получаем позиции выбранных спрайтов в матрице
        Vector2Int firstTilePos = firstSprite.GetComponent<ClickableTile>().Position;
        Vector2Int secondTilePos = secondSprite.GetComponent<ClickableTile>().Position;

        // Задержка перед началом анимации
        yield return new WaitForSeconds(0.1f);

        // Анимация перемещения спрайтов
        Vector3 firstStartPosition = firstSprite.transform.position;
        Vector3 secondStartPosition = secondSprite.transform.position;
        float animationDuration = 0.2f;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            firstSprite.transform.position = Vector3.Lerp(firstStartPosition, secondStartPosition, t);
            secondSprite.transform.position = Vector3.Lerp(secondStartPosition, firstStartPosition, t);
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        // Меняем спрайты местами
        Sprite tempSprite = firstRenderer.sprite;
        firstRenderer.sprite = secondRenderer.sprite;
        secondRenderer.sprite = tempSprite;

        // Восстанавливаем позиции спрайтов
        firstSprite.transform.position = firstStartPosition;
        secondSprite.transform.position = secondStartPosition;

        // Восстанавливаем цвет спрайтов
        firstSprite.GetComponent<ClickableTile>().spriteRenderer.color = TileGenerator.originalColor;
        secondSprite.GetComponent<ClickableTile>().spriteRenderer.color = TileGenerator.originalColor;

        // Проверяем и удаляем линии из трех и более спрайтов
        TileGenerator.CheckLines();


    }

    private void SwapSprites(GameObject firstSprite, GameObject secondSprite)
    {
        StartCoroutine(AnimateSwap(firstSprite, secondSprite));
    }


}
