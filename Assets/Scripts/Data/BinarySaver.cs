﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

//Based off persistence from: https://unity3d.com/learn/tutorials/topics/scripting/introduction-saving-and-loading

public class BinarySaver : MonoBehaviour
{

  public static BinarySaver S { get; private set; }

  public OverallProgress currentSave { get; private set; }
#pragma warning disable 0649

  const string folderName = "BinaryProgressData";
  const string fileExtension = ".dat";

#pragma warning restore 0649

  //TODO: Move this to GameManager (Save after level completion.)

  private void Awake()
  {
    S = this;
    DontDestroyOnLoad(this.gameObject);

    //Check if there is a current most recent save. If not, instantiate one.
    OverallProgress p = LoadMostRecent();

    if (p == null) p = CreateDefaultSave();

    currentSave = p;
  }
  /*
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
  }*/

  public void Save(string saveName = null)
  {
    string folderPath = PathCombine(Application.persistentDataPath, folderName);
    if (!Directory.Exists(folderPath))
      Directory.CreateDirectory(folderPath);

    if (saveName != null) currentSave.saveName = saveName;
    string dataPath = PathCombine(folderPath, currentSave.saveName + fileExtension);
    SaveProgress(currentSave, dataPath);
  }

  public void Load(string saveName = null)
  {
    string[] filePaths = GetFilePaths();

    if (saveName == null) { //Just load the first save.
      if (filePaths.Length > 0) currentSave = LoadProgress(filePaths[0]);
    }
    else { //Search for specific save and load it.
      foreach (string path in filePaths) {
        if (path.Contains(saveName)) {
          currentSave = LoadProgress(path);
          break;
        }
      }
    } 
  }

  //Loads the Save that has been most recently saved. (Will most likely result in loading the previous user's save.)
  private OverallProgress LoadMostRecent()
  {
    string folderPath = PathCombine(Application.persistentDataPath, folderName);
    try {
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
    } catch (Exception e) {
      Debug.Log("Could not Load Most Recent Save (Do any exist?)" + e);
      return null;
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
    string filePath = PathCombine(PathCombine(Application.persistentDataPath, folderName), path);
    BinaryFormatter binaryFormatter = new BinaryFormatter();
    using (FileStream fileStream = File.Open(filePath, FileMode.Open)) {
      return (OverallProgress)binaryFormatter.Deserialize(fileStream);
    }
  }

  //Returns a list of the file paths from the BinaryProgressData directory.
  static string[] GetFilePaths()
  {
    string folderPath = PathCombine(Application.persistentDataPath, folderName);
    Debug.Log("Directory: " + folderPath);

    return Directory.GetFiles(folderPath);
  }

  public void StartNewSave()
  {
    currentSave = CreateRandomSave();
    Save();
  }

  private bool StringInFileList(string testString, string[] files)
  {
    bool stringIn = false;
    foreach (string file in files) {
      Debug.Log(file);
      if (file.Contains(testString)) {
        stringIn = true;
        break;
      }
    }
    return stringIn;
  }

  public OverallProgress CreateRandomSave()
  {

    string saveName = "Atticus";
    string[] files = GetFilePaths();
    while (StringInFileList(saveName,files)) {
      saveName = RandomString(8);
    }
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

  public OverallProgress[] LoadSaves()
  {
    string[] files = GetFilePaths();
    Debug.Log("Number of Files: " + files.Length);
    if (files.Length == 0) return null;

    OverallProgress[] saves = new OverallProgress[files.Length];
    for (int i = 0; i < files.Length; i++) {
      saves[i] = LoadProgress(files[i]);
    }
    return saves;
  }

  public void SaveCompletion(int score)
  {
    String currentLevel = SceneManager.GetActiveScene().name;
    bool InBoss = currentLevel.Contains("3");
    int index;
    if (InBoss) {
       if (currentLevel.Contains("3-3")) { index = 3; }
       else { return; }
    }
    index = Int32.Parse(currentLevel.Substring(currentLevel.Length -1));
    LevelProgress progress = currentSave.levels[index-1];
    progress.completed = true;
    progress.score = (progress.score >= score) ? progress.score : score;
    Save();
    DebugCurrentProgress(currentSave);
  }

  //Based on solution found here: https://stackoverflow.com/questions/53102/why-does-path-combine-not-properly-concatenate-filenames-that-start-with-path-di
  private static string PathCombine(string path1, string path2)
  {
    if (Path.IsPathRooted(path2)) {
      path2 = path2.TrimStart(Path.DirectorySeparatorChar);
      path2 = path2.TrimStart(Path.AltDirectorySeparatorChar);
    }

    return Path.Combine(path1, path2);
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
                "\n" + "Score :" + level.score);
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
