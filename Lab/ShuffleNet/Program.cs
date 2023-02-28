using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Linq;
using SixLabors.ImageSharp.Processing;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ImageProccessing;

namespace Program;

class Program
{
    static void Main(string[] args)
    {
        PrintProccessingImages();


    }
    static void PrintProccessingImages()
    {
        var cls = new ShuffleNet();


        CancellationTokenSource source_1 = new CancellationTokenSource();
        CancellationToken token_1 = source_1.Token;
        CancellationTokenSource source_2 = new CancellationTokenSource();
        CancellationToken token_2 = source_2.Token;

        var img1 = cls.proccessImage("C:\\Users\\dimav\\OneDrive\\Рабочий стол\\Programming\\Lab\\img\\img1.jpg", token_1);
        var img2 = cls.proccessImage("C:\\Users\\dimav\\OneDrive\\Рабочий стол\\Programming\\Lab\\img\\img2.jpg", token_2);

        Task.WaitAll();
        img1.Wait();
        img2.Wait();

        
        PrintImage(img1);
        PrintImage(img2);
        


    }
    static void PrintImage(Task<List<(string, float)>> img)
    {
        string str = "";
        for (int i = 0; i < img.Result.Count; i++)
            str += img.Result[i].Item1 + "  " + img.Result[i].Item2.ToString() + "\n";
        Console.WriteLine(str);
    }

    
}
