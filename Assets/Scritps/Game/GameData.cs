using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Serializers;
using UnityEngine;

public static class GameData
{
    private static List<bool[][]> s_GameGrids;


    static GameData()
    {
        s_GameGrids = LoadGrids();
    }

    public static List<bool[][]> GameGrids
    {
        get { return s_GameGrids; }
        set { s_GameGrids = value; }
    }

    public static void SaveGrids()
    {
        Debug.Log(s_GameGrids.Count);
        

        SaveGame.Save("Grids2.xml", s_GameGrids, new SaveGameXmlSerializer());

        //SaveGame.Save("Grids2.xml", s_GameGrids, SaveGamePath.Streaming);

        //SaveGame.Load()

        //BetterStreamingAssets.OpenText()
    }

    public static List<bool[][]> LoadGrids()
    {
        BetterStreamingAssets.Initialize();
        //string[] paths = BetterStreamingAssets.GetFiles("\\", "*.xml", SearchOption.AllDirectories);


        //if (paths.Length > 0)
        //{
        //    Debug.Log(paths[0]);
        //}

        //BetterStreamingAssets.

        //List<bool[][]> grids = SaveGame.Load<List<bool[][]>>("Grids.xml", SaveGamePath.Streaming);


        //XmlSerializer serializerTest = new XmlSerializer(typeof(List<bool[][]>));
        //TextWriter writer = new StreamWriter(Application.streamingAssetsPath + "/Grids2.xml");
        //serializerTest.Serialize(writer, grids);
        //writer.Close();

        using (var stream = BetterStreamingAssets.OpenRead("Grids2.xml"))
        {
            XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<bool[][]>));
            //serializer.Deserialize(stream);
            return (List<bool[][]>)serializer.Deserialize(stream);
            //Debug.Log(typeof());

        }

        //SaveGame.Save("Grids.xml", SaveGame.Load<List<bool[][]>>("Grids", SaveGamePath.Streaming), SaveGamePath.Streaming);

        //using (Stream stream = BetterStreamingAssets.OpenRead("Grids2.xml"))
        //{
        //    DataContractSerializer  serializer = new DataContractSerializer(typeof(List<bool[][]>));
        //    //serializer.Deserialize(stream);
        //    return (List<bool[][]>) serializer.ReadObject(stream);

        //    //return serializer. (stream) as List<bool[][]>;
        //}


        return SaveGame.Load<List<bool[][]>>("Grids2.xml", SaveGamePath.Streaming);
    }

    public static void SaveLevel(int i_Level)
    {
        SaveGame.Save("Level", i_Level, SaveGamePath.PersistentDataPath);
    }

    public static int LoadLevel()
    {
        return SaveGame.Load<int>("Level", SaveGamePath.PersistentDataPath);
    }

    public static void SaveHighScore(int i_Score)
    {
        SaveGame.Save("HighScore", i_Score, SaveGamePath.PersistentDataPath);
    }

    public static int LoadHighScore()
    {
        return SaveGame.Load<int>("HighScore", SaveGamePath.PersistentDataPath);
    }
}