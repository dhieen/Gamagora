using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class LanguageDataExtractor
{
    public enum LANGUAGES { EN, FR, GE, SP, IT }
    public struct Data
    {
        public List<string> inputs;
        public string output;
    }

    public List<Data> data;
    public char[] 

    public void AddDataFromFile (string path)
    {
        string[] lines = File.ReadAllLines(path);
        string output = lines[0];
        for (int i = 1; i < lines.Length;i++)
        {
            char c;
            int c_count = lines[i].Length - lines[i].Remove(c).Length;
        }
    }
}
