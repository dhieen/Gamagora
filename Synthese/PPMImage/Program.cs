using System;
using System.Diagnostics;
using System.IO;


namespace TPSynthese
{
    class Program
    {
        static void Main(string[] args)
        {
            string folder = "D:/TPOutputs";            
            Tests.DrawExample2(1000,1000,200).SaveFile (folder + "/Image.ppm");
        }
    }       
}
