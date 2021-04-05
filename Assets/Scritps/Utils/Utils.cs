using System.Collections.Generic;
using UnityEngine;

public static class Utils 
{
    public static int s_GridWidth;
    public static int s_GridHeight;

    public static Vector3[] s_Directions = { Vector3.up, Vector3.right, Vector3.down, Vector3.left };
    public static Color[] s_GameColors;

    public static bool s_IsLevelFinished = false;
    public static bool s_IsGamePaused = false;
    public static int s_CurrentLevel = 1;

    public static bool IsFloatInRange(float i_Number, float i_MinInRange, float i_MaxInRange)
    {
        return i_Number >= i_MinInRange && i_Number <= i_MaxInRange;
    }

    public static T[][] Init2DArrayRows<T>(int i_Rows , int i_Cols)
    {
        T[][] newArray = new T[i_Rows][];

        for (int i = 0; i < i_Rows; i++)
        {
            newArray[i] = new T[i_Cols];
        }

        return newArray;
    }

    public static void ShuffleList<T>(List<T> i_List)
    {
        System.Random rng = new System.Random();
        int numberOfGrids = i_List.Count;
        while (numberOfGrids > 1)
        {
            numberOfGrids--;
            int nextGrid = rng.Next(numberOfGrids + 1);
            T value = i_List[nextGrid];
            i_List[nextGrid] = i_List[numberOfGrids];
            i_List[numberOfGrids] = value;
        }
    }

    public static bool OnMiddle(float i_Row, float i_Col)
    {
        return i_Row >= s_GridWidth / 2 - 1
               && i_Row <= s_GridWidth / 2 + 1
               && i_Col >= s_GridHeight / 2 - 1
               && i_Col <= s_GridHeight / 2 + 1;
    }

    public static bool OnBorder(float i_Row, float i_Col)
    {
        return i_Row == s_GridWidth - 1 ||
               i_Row == 0 ||
               i_Col == s_GridHeight - 1 ||
               i_Col == 0;
    }

    public static bool OnGrid(float i_Row, float i_Col)
    {
        return i_Row >= 0 && i_Row < s_GridWidth &&
               i_Col >= 0 && i_Col < s_GridHeight;
    }
}
