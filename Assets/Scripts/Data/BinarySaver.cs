﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Random = System.Random;

//Based off persistence from: https://unity3d.com/learn/tutorials/topics/scripting/introduction-saving-and-loading

public class BinarySaver : MonoBehaviour
{

  public static BinarySaver S { get; private set; }

  public OverallProgress progress;
  const string folderName = "BinaryProgressData";
  const string fileExtension = ".dat";

  //TODO: Move this to GameManager (Save after level completion.)

  private void Awake()
  {
    S = this;
    DontDestroyOnLoad(this.gameObject);

    //Check if there is a current most recent save. If not, instantiate one.
    OverallProgress p = LoadMostRecent();

    if (p == null) p = CreateDefaultSave();

    progress = p;
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.S)) {
      Save(RandomString(5));
      DebugCurrentProgress(progress);
    }

    if (Input.GetKeyDown(KeyCode.L)) {
      Load();
      DebugCurrentProgress(progress);
    }
  }

  public void Save(string saveName = null)
  {
    string folderPath = Path.Combine(Application.persistentDataPath, folderName);
    if (!Directory.Exists(folderPath))
      Directory.CreateDirectory(folderPath);

    if (saveName != null) progress.saveName = saveName;
    string dataPath = Path.Combine(folderPath, progress.saveName + fileExtension);
    SaveProgress(progress, dataPath);
    Debug.Log("Progress Saved.");
  }

  public void Load(string saveName = null)
  {
    string[] filePaths = GetFilePaths();

    if (saveName ==  null) { //Just load the first save.
      if (filePaths.Length > 0) progress = LoadProgress(filePaths[0]);
    }
    else { //Search for specific save and load it.
      foreach (string path in filePaths) {
        if (path.Equals(saveName)) {
          progress = LoadProgress(path);
          break;
        }
      }
    } 
  }

  //Loads the Save that has been most recently saved. (Will most likely result in loading the previous user's save.)
  private OverallProgress LoadMostRecent()
  {
    string folderPath = Path.Combine(Application.persistentDataPath, folderName);
    FileInfo[] files = new DirectoryInfo(folderPath).GetFiles("*.*");
    string latestFile = "";
    if (files.Length <= 0) return null;
    else {
      DateTime lastModified = DateTime.MinValue;
      foreach (FileInfo file in files) {
        if (file.LastWriteTime > lastModified) {
          lastModified = file.LastWriteTime;
          latestFile = file.Name;
        }
      }
      return LoadProgress(latestFile);
    }
  }

  //Saves the Progress Data to the directory.
  private void SaveProgress(OverallProgress data, string path)
  {
    BinaryFormatter binaryFormatter = new BinaryFormatter();

    using (FileStream fileStream = File.Open(path, FileMode.OpenOrCreate)) {
      binaryFormatter.Serialize(fileStream, data);
    }
  }

  //Loads the Progress Data from the directory.
  private OverallProgress LoadProgress(string path)
  {
    BinaryFormatter binaryFormatter = new BinaryFormatter();

    using (FileStream fileStream = File.Open(path, FileMode.Open)) {
      return (OverallProgress)binaryFormatter.Deserialize(fileStream);
    }
  }

  //Returns a list of the file paths from the BinaryProgressData directory.
  static string[] GetFilePaths()
  {
    string folderPath = Path.Combine(Application.persistentDataPath, folderName);

    return Directory.GetFiles(folderPath, fileExtension);
  }

  public OverallProgress CreateDefaultSave(string saveName = "Atticus")
  {

    OverallProgress p = new OverallProgress();
    p.saveName = saveName;
    LevelProgress level1 = new LevelProgress();
    LevelProgress level2 = new LevelProgress();
    LevelProgress level3 = new LevelProgress();
    level1.level = 1;
    level2.level = 2;
    level3.level = 3;
    p.levels = new LevelProgress[3];
    p.levels[0] = level1;
    p.levels[1] = level2;
    p.levels[2] = level3;

    return p;
  }


  /*
   * 
   * For Testing Purposes.
   * 
   * 
   * */

  private void DebugCurrentProgress(OverallProgress progress)
  {
    Debug.Log("SAVE: " + progress.saveName);

    foreach (LevelProgress level in progress.levels) {
      Debug.Log("Level " + level.level +
                "\n" + "Completed? :" + level.completed +
                "\n" + "Time Taken :" + level.timeSpent +
                "\n" + "Total Time :" + level.timeOverall);
    }
  }

  // Based on solution found here. https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings

  private static Random random = new Random();
  public static string RandomString(int length)
  {
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    return new string(Enumerable.Repeat(chars, length)
      .Select(s => s[random.Next(s.Length)]).ToArray());
  }


}
