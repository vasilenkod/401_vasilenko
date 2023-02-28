using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Collections.ObjectModel;
using ImageProccessing;
using System.Text.Json;

namespace WpfApp;


//В этом классе храним выход нейросети
public class Image
{

    private string path;
    private string descr;
    private byte[] byteImage;

    public string Path
    {
        get { return this.path; }
    }
    public string Descr
    {
        get { return this.descr; }
    }

    public byte[] ByteImage
    {
        get { return this.byteImage; }  
    }
    public Image(string path, List<(float, string)> lst)
    {
        this.path = path;

        string str = "This image could be:\n";
        for (int i = 0; i < lst.Count; i++)
            str += "   " + lst[i].Item2 + ": " + lst[i].Item1.ToString() + "\n";

        this.descr = str;

        byteImage = File.ReadAllBytes(path);


    }
}

public partial class MainWindow : Window
{
    ShuffleNet cls;
    CancellationToken token;
    CancellationTokenSource source;
    ObservableCollection<Image> images = new();
    List<byte[]> processedImage = new List<byte[]>();
    public MainWindow()
    {
        InitializeComponent();
        this.cls = new ShuffleNet();
        ListViewImage.ItemsSource = images;

    }


    //Кнопка Select images
    private async void OpenFolder(object sender, RoutedEventArgs e)
    {
        Files_selection.IsEnabled = false;
        ProgressBar.Value = 0;

        this.source = new CancellationTokenSource();
        this.token = source.Token;

        Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
        dialog.Multiselect = true;

        var result = dialog.ShowDialog();
        int len = dialog.FileNames.Length;
        if (len == 0)
        {
            Files_selection.IsEnabled = true;
            return;
        }

        //Запускаем нужное количество Task для обработки полученных изображений
        int step = 100 / len;
        var tasks = new List<Task<List<(string, float)>>>();
        var paths = dialog.FileNames;
        if (result == true)
        {
            for (int i = 0; i < len; i++)
            {
                var path = dialog.FileNames[i];
                byte[] imageBytes = File.ReadAllBytes(path);
                
                var tmp = cls.proccessImage(path, token);
                tasks.Add(tmp);
                ProgressBar.Value += step;
            }
        }
        bool flag = true;


        //Ждем завершения обработки и сохраняем полученные результаты
        for (int i = 0; i < len; i++)
        {
            await tasks[i];
            var ans = new List<(float, string)>();
            for (int j = 0; j < tasks[i].Result.Count; j++)
                ans.Add((tasks[i].Result[j].Item2, tasks[i].Result[j].Item1));
            ans.Sort();
            ans.Reverse();
            if (ans[0].Item1 == 0)
            {
                flag = false;
                break;
            }


            var img = new Image(paths[i], ans);
            images.Add(img);

            //var img = new Image(paths[i], ans);
            //if(!processedImage.Contains(img.ByteImage))
            //{
            //    images.Add(img);
            //    processedImage.Add(img.ByteImage);
            //    using (FileStream fs = new FileStream("C:\\Users\\dimav\\OneDrive\\Рабочий стол\\Programming\\Lab\\JSON\\Images.json", FileMode.OpenOrCreate))
            //    {
            //        await JsonSerializer.SerializeAsync<Image>(fs, img);
            //    }

            //}
            ProgressBar.Value += step;
        }
        if (flag)
            ProgressBar.Value = 1000;
        Files_selection.IsEnabled = true;
    }


    //Кнопка Cancel
    private void Stop(object sender, RoutedEventArgs e)
    {
        source.Cancel();
        MessageBox.Show("Cancelled");
    }

}
