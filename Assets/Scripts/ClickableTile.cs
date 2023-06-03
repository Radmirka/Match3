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
                // ������ ��������� ������
                TileGenerator.SelectedTile = this.gameObject;
                // ��������� �������� ���� �������
                TileGenerator.originalColor = spriteRenderer.color;
                // �������� ���� ������� �� �����
                spriteRenderer.color = Color.gray;
                isSelected = true;
            }
            else
            {
                // ������ ��������� ������
                GameObject secondSelectedTile = this.gameObject;
                // �������� ������� ��������� �������� � �������
                Vector2Int firstTilePos = TileGenerator.SelectedTile.GetComponent<ClickableTile>().Position;
                Vector2Int secondTilePos = Position;

                // ���������, �������� �� ������ ������ �������� �� ����������� ��� ���������
                if (IsNeighbour(firstTilePos, secondTilePos))
                {
                    // ������ ������� �������
                    SwapSprites(TileGenerator.SelectedTile, secondSelectedTile);

                    // ��������� �������� � �������
                    int firstTileValue = TileGenerator.tileMatrix.GetValue(firstTilePos.x, firstTilePos.y);
                    int secondTileValue = TileGenerator.tileMatrix.GetValue(secondTilePos.x, secondTilePos.y);
                    TileGenerator.tileMatrix.UpdateTileValue(firstTilePos.x, firstTilePos.y, secondTileValue);
                    TileGenerator.tileMatrix.UpdateTileValue(secondTilePos.x, secondTilePos.y, firstTileValue);

                    // ������� ����� �� ��������
                    TileGenerator.SelectedTile = null;
                    spriteRenderer.color = TileGenerator.originalColor;
                    isSelected = false;

                }
                else
                {
                    // ������ �� ������, ���� ������ ������ �� �������� ��������
                }
            }
        }
        else
        {
            // ����� ����� �� �������
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

        // �������� ������� ��������� �������� � �������
        Vector2Int firstTilePos = firstSprite.GetComponent<ClickableTile>().Position;
        Vector2Int secondTilePos = secondSprite.GetComponent<ClickableTile>().Position;

        // �������� ����� ������� ��������
        yield return new WaitForSeconds(0.1f);

        // �������� ����������� ��������
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

        // ������ ������� �������
        Sprite tempSprite = firstRenderer.sprite;
        firstRenderer.sprite = secondRenderer.sprite;
        secondRenderer.sprite = tempSprite;

        // ��������������� ������� ��������
        firstSprite.transform.position = firstStartPosition;
        secondSprite.transform.position = secondStartPosition;

        // ��������������� ���� ��������
        firstSprite.GetComponent<ClickableTile>().spriteRenderer.color = TileGenerator.originalColor;
        secondSprite.GetComponent<ClickableTile>().spriteRenderer.color = TileGenerator.originalColor;

        // ��������� � ������� ����� �� ���� � ����� ��������
        TileGenerator.CheckLines();


    }

    private void SwapSprites(GameObject firstSprite, GameObject secondSprite)
    {
        StartCoroutine(AnimateSwap(firstSprite, secondSprite));
    }


}
