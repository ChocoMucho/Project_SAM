using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public static class Utils
{
    /// <summary>
    /// Fisher-Yates �˰����� ����� ����Ʈ�� ���� �Լ�
    /// </summary>
    public static void ShuffleList<T>(List<T> list)
    {
        int length = list.Count;
        for(int i = length - 1; 0 < i; --i)
        {
            int randomIndex = Random.Range(0, i+1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
