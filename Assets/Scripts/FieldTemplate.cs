using System.Collections.Generic;
using UnityEngine;

// �������� ����� ��� ������������� ������� ����
public class FieldTemplate
{
    public int?[,] layout; // ������ ��� �������� ������� ����

    public FieldTemplate(int?[,] template)
    {
        layout = template;
    }
}

// �������� �����, ������� ����� ��������� ������� � ���������� �������� ����
// �������� �����, ������� ����� ��������� ������� � ���������� �������� ����
public class FieldTemplateManager
{
    private List<FieldTemplate> templates; // ������ ��������� ��������

    public FieldTemplateManager()
    {
        templates = new List<FieldTemplate>();
        // ����� �� ������ �������� ���� ����������� ������� ����
        templates.Add(new FieldTemplate(new int?[,] {
            { 1, 1, -1, 1, 1, 1, 0, 1 },
            { 1, 1, 0, 1, 1, 1, 0, 1 },
            { 1, 1, 0, 1, 1, 1, 0, 1 },
            { 1, 1, 0, 1, 1, 1, 0, 1 },
            { 1, 1, 0, 1, 1, 1, 0, 1 },
            { 1, 1, 0, 1, 1, 1, 0, 1 },
            { 0, 1, 1, 1, 1, 1, 0, 1 },
            { 1, 0, 1, 1, 1, 1, 0, 1 }
        }));
        templates.Add(new FieldTemplate(new int?[,] {
            { 1, 0, 1, 0 },
            { 0, 1, 0, 1 },
            { 1, 0, 1, 0 },
            { 0, 1, 0, 1 }
        }));
        templates.Add(new FieldTemplate(new int?[,] {
            { -1, 0, 1, 2, 3, 4, 5, 0 },
            { 1, -1, 1, 2, 3, 4, 5, 0 },
            { 1, 2, -1, 3, 4, 5, 0, 1 },
            { 2, 3, 4, -1, 5, 0, 1, 2 },
            { 3, 4, 5, 0, -1, 1, 2, 3 },
            { 4, 5, 0, 1, 2, -1, 3, 4 },
            { 5, 0, 1, 2, 3, 4, -1, 5 },
            { 0, 1, 2, 3, 4, 5, 0, -1 }
        }));
        templates.Add(new FieldTemplate(new int?[,] {
            { 0, 0, 1, 2, 3, 4, 5, 0 },
            { 1, 1, 1, 2, 3, 4, 5, 0 },
            { 1, 2, 2, 3, 4, 5, 0, 1 },
            { 2, 3, 4, 3, 5, 0, 1, 2 },
            { 3, 4, 5, 0, 4, 1, 2, 3 },
            { 4, 5, 0, 1, 2, 5, 3, 4 },
            { 5, 0, 2, 2, 3, 4, 0, 5 },
            { 0, 1, 2, 3, 4, 5, 0, 1 }
        }));templates.Add(new FieldTemplate(new int?[,] {
            { 0, 0, 1, 0, 0, 4, 5, 0 },
            { 1, 1, 0, 2, 3, 4, 5, 0 },
            { 1, 2, 2, 3, 4, 5, 0, 1 },
            { 2, 3, 4, 3, 5, 0, 1, 2 },
            { 3, 4, 5, 0, 4, 1, 2, 3 },
            { 4, 5, 0, 1, 2, 5, 3, 4 },
            { 5, 0, 2, 2, 3, 4, 0, 5 },
            { 0, 1, 2, 3, 4, 5, 0, 1 }
        }));
    }

    public FieldTemplate GetTemplateByIndex(int index)
    {
        // ���������, ��� ������ ��������� � ���������� ���������
        if (index >= 0 && index < templates.Count)
        {
            return templates[index];
        }
        else
        {
            Debug.LogError("Invalid template index!");
            return null;
        }
    }
}