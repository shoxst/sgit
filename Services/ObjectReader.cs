using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace sgit
{
  public static class ObjectReader
  {
    public static SortedDictionary<string, string> GetBlobHashFromCommit(string hash)
    {
      var dict = new Dictionary<string, string>();
      var commit = ReadCommitObject(hash);
      GetBlobHashFromTree(dict, commit.TreeHash, "");
      return new SortedDictionary<string, string>(dict);
    }

    public static void GetBlobHashFromTree(Dictionary<string, string> dict, string hash, string dirPath)
    {
      var filePath = PathUtil.GetObjectFilePath(hash);
      var cData = File.ReadAllBytes(filePath);
      var data = new List<byte>(CompressUtil.Decompress(cData));
      var type = Encoding.UTF8.GetString(data.GetRange(0, 4).ToArray());

      if (type != "tree")
      {
        throw new Exception("This object is not tree object");
      }

      int offset = data.IndexOf(0b00) + 1;
      while (offset < data.Count)
      {
        var meta = new List<byte>();
        foreach (var b in data.GetRange(offset, data.Count - offset))
        {
          if (b == 0b00)
          {
            break;
          }
          else
          {
            offset++;
            meta.Add(b);
          }
        }
        offset++;
        var cols = Encoding.UTF8.GetString(meta.ToArray()).Split(' ');
        if (cols[0] == "100644")
        {
          string sgitFilePath = dirPath + cols[1];
          string blobHash = HashUtil.GetString(data.GetRange(offset, 20).ToArray());
          dict.Add(sgitFilePath, blobHash);
          offset += 20;
        }
        else if (cols[0] == "40000")
        {
          string dirName = cols[1];
          string treeHash = HashUtil.GetString(data.GetRange(offset, 20).ToArray());
          GetBlobHashFromTree(dict, treeHash, $"{dirPath}{dirName}/");
          offset += 20;
        }
      }
    }

    public static CommitObject ReadCommitObject(string hash)
    {
      var filePath = PathUtil.GetObjectFilePath(hash);
      var cData = File.ReadAllBytes(filePath);
      var data = CompressUtil.Decompress(cData);
      var str = Encoding.UTF8.GetString(data);
      var lines = str.Split('\n');

      if(lines[0].Split(' ')[0] != "commit")
      {
        throw new Exception("This object is not commit object");
      }
      
      string treeHash = null;
      List<string> parents = new List<string>();
      UserInfo author = null;
      UserInfo committer = null;
      var sb = new StringBuilder();

      int cnt = 0;
      for (; ; ++cnt)
      {
        var cols = lines[cnt].Split(' ');
        if (cols[0] == "commit")
        {
          treeHash = cols[2];
        }
        if (cols[0] == "parent")
        {
          parents.Add(cols[1]);
        }
        if (cols[0] == "author")
        {
          var list = lines[cnt];
          int i = list.IndexOf(' ');
          int j = list.IndexOf('<');
          var name = list.Substring(i+1,j-i-2);
          var items = list.Substring(j, list.Length - j).Split(' ');
          author = new UserInfo(name, items[0].Trim('<', '>'), Int32.Parse(items[1]), items[2]);
        }
        if (cols[0] == "committer")
        {
          var list = lines[cnt];
          int i = list.IndexOf(' ');
          int j = list.IndexOf('<');
          var name = list.Substring(i+1,j-i-2);
          var items = list.Substring(j, list.Length - j).Split(' ');
          committer = new UserInfo(name, items[0].Trim('<', '>'), Int32.Parse(items[1]), items[2]);
          break;
        }
      }
      for (int i = cnt + 2; i < lines.Length - 1; i++)
      {
        sb.Append($"{lines[i]}\n");
      }
      return new CommitObject(treeHash, parents, author, committer, sb.ToString());
    }

    public static string ReadBlobObject(string hash)
    {
      var filePath = PathUtil.GetObjectFilePath(hash);
      var cData = File.ReadAllBytes(filePath);
      var data = CompressUtil.Decompress(cData);
      var type = Encoding.UTF8.GetString(data.Take(4).ToArray());

      if (type != "blob")
      {
        throw new Exception("This object is not blob object");
      }

      var nullIndex = Array.IndexOf(data, (byte)0b00);
      var fileContent = Encoding.UTF8.GetString(data.Skip(nullIndex + 1).Take(data.Length - nullIndex - 1).ToArray());
      return fileContent;
    }
  }
}
