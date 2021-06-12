using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace sgit
{
  public static class Index
  {
    // Key: fileName, Value: hash
    private static SortedDictionary<string, string> dict;
    
    public static void Update(string fileName, string hash)
    {
      Read();
      if (dict.ContainsKey(fileName))
      {
        dict[fileName] = hash;
      }
      else
      {
        dict.Add(fileName, hash);
      }
      Write();
    }

    public static SortedDictionary<string, string> GetIndexes()
    {
      Read();
      return dict;
    }

    public static void Update(Dictionary<string, string> sub, string dirName)
    {
      Read();
      var itemsToRemove = dict.Where(item => item.Key.StartsWith($"{dirName}")).ToArray();
      foreach (var item in itemsToRemove)
      {
        dict.Remove(item.Key);
      }
      foreach (var pair in sub)
      {
        dict.Add(pair.Key, pair.Value);
      }
      Write();
    }

    private static void Read()
    {
      dict = new SortedDictionary<string, string>();
      using (var sr = new StreamReader(PathUtil.SGIT_INDEX))
      {
        string line;
        while ((line = sr.ReadLine()) != null)
        {
          string[] cols = line.Split(' ');
          dict.Add(cols[3], cols[1]);
        }
      }
    }
    
    private static void Write()
    {
      using (var sw = new StreamWriter(PathUtil.SGIT_INDEX))
      {
        foreach (var item in dict)
        {
          sw.WriteLine($"1060644 {item.Value} 0 {item.Key}");
        }
      }
    }
  }
}
