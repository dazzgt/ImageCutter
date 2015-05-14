using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageCutter
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Init
        public MainWindow()
        {
            InitializeComponent();
            Log = new ObservableCollection<string>();
            DataContext = this;
        }
        #endregion

        #region Methods
        private void Cut(string source)
        {
            try
            {
                Log.Clear();
                Log.Add("Source: " + source);
                string path = source.Substring(0, source.Length - 4);
                Log.Add("Path: " + path);
                if (Directory.Exists(path))
                {
                    System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(path);

                    foreach (FileInfo file in downloadedMessageInfo.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in downloadedMessageInfo.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                    Directory.Delete(path);
                    Log.Add("Delete directory ...");
                }
                Directory.CreateDirectory(path);
                Log.Add("Create directory ... ");
                Bitmap bmp = new Bitmap(source);
                for (int y = TileMargin; y <= bmp.Height; y += TileHeight + Spacing)
                    for (int x = TileMargin; x <= bmp.Width; x += TileWidth + Spacing)
                    {
                        Bitmap cuted = bmp.Clone(new System.Drawing.Rectangle(x, y, TileWidth, TileHeight), bmp.PixelFormat);
                        bool flag = false;
                        for (int xx = 0; xx < cuted.Width; xx++)
                            for (int yy = 0; yy < cuted.Height; yy++)
                                if (cuted.GetPixel(xx, yy).A != 0)
                                    flag = true;
                        if (flag)
                            cuted.Save(path + "\\tile" + y / (TileHeight + Spacing) + 1 + "_" + x / (TileWidth + Spacing) + 1 + ".png", ImageFormat.Png);
                    }
                Log.Add("All OK ... ");
            }
            catch (Exception ex)
            {
                Log.Add(ex.Message);
            }
        }
        #endregion

        #region Properties
        private int _tileMargin = 0;
        private int _tileWidth = 15;
        private int _tileHieght = 15;
        private int _spacing = 2;
        public int TileMargin
        {
            get { return _tileMargin; }
            set
            {
                _tileMargin = value;
                OnPropertyChanged("Margin");
            }
        }
        public int TileWidth
        {
            get { return _tileWidth; }
            set
            {
                _tileWidth = value;
                OnPropertyChanged("Width");
            }
        }
        public int TileHeight
        {
            get { return _tileHieght; }
            set
            {
                _tileHieght = value;
                OnPropertyChanged("Height");
            }
        }
        public int Spacing
        {
            get { return _spacing; }
            set
            {
                _spacing = value;
                OnPropertyChanged("Spacing");
            }
        }
        public ObservableCollection<string> Log { get; set; }
        #endregion

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(params string[] property)
        {
            if (PropertyChanged != null)
                foreach (var prop in property)
                    PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() ?? false)
            {
                Cut(dlg.FileName);
            }
        }
    }
}
