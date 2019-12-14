using ImageBuilderLibrary;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Object which allows to generate array of given size which contains information if (x,y) point in
        /// Cartesian coordinate system bellongs to Mandelbrot set with certain N.
        /// </summary>
        private Mandelbrot mandelbrot;

        internal Mandelbrot Mandelbrot { get => mandelbrot; set => mandelbrot = value; }

        /// <summary>
        /// Default constructor it sets default value of N = 2000 and Size = 60 to mandelbrot object
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            getInput(out int N, out int Size);
            this.Mandelbrot = new Mandelbrot(N, Size);
        }
        /// <summary>
        /// Method which gethers user input and trys to parse it to proper type
        /// if it fails it sets values to defoult ones.
        /// </summary>
        /// <param name="N">How many numbers in mandelbrot sequence should be checked ti determin if points bellong to set</param>
        /// <param name="Size">Size from which image size is generated in 3x2 ratio</param>
        private void getInput(out int N, out int Size)
        {
            if (!int.TryParse(NInput.Text, out N))
            {
                N = 2000;
            }
            if (!int.TryParse(SizeInput.Text, out Size))
            {
                Size = 60;
            }
        }
        /// <summary>
        /// Method reads user input using getInput method and sets it as parameters to Mandelbrot object
        /// </summary>
        private void SetInputAsMandelbrotParameters()
        {
            getInput(out Mandelbrot.N, out int Size);
            Mandelbrot.Size = Size;
        }

        /// <summary>
        /// Method which sets isEnabled to state of all buttons in MainWindow
        /// </summary>
        /// <param name="state">true if buttons should be enabled, false otherwise</param>
        private void SetButtonsEnabled(bool state)
        {
            this.SequentailButton.IsEnabled = state;
            this.ParallelButton.IsEnabled = state;
            this.CompareButton.IsEnabled = state;
            this.SaveButton.IsEnabled = state;
        }
        /// <summary>
        /// Method which sets isEnabled to state of all input fields in MainWindow
        /// </summary>
        /// <param name="state">true if input should be enabled, false otherwise</param>
        private void SetInputEnabled(bool state)
        {
            this.NInput.IsEnabled = state;
            this.SizeInput.IsEnabled = state;
        }
        /// <summary>
        /// Method combining setButtonsEnabled and setInputEnabled, effectivly setting isEnabed to all contros in MainWindow
        /// </summary>
        /// <param name="state">true if controls should be enabled, false otherwise</param>
        private void SetAllControlsEnabled(bool state)
        {
            SetButtonsEnabled(state);
            SetInputEnabled(state);
        }
        /// <summary>
        /// Method which sets Visualization to image created from Array
        /// </summary>
        /// <param name="Array">Dwo dimensional boolan array where each [i,j] represents if  (i,j) in 
        /// Cartesian coordinate system belongs to Mandelbrot set</param>
        private void SetImage(bool[,] Array)
        {
            String AppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            String tmpFileName = AppData + "\\tmp_mandelbrot.png";
            ImageBuilder imageBuilder = new ImageBuilder(Array);
            imageBuilder.SaveImage(tmpFileName);
            this.Visualization.Source = CreateBitmap(tmpFileName);
            if (File.Exists(tmpFileName))
            {
                File.Delete(tmpFileName);
            }
        }

        /// <summary>
        /// Method which loads Bitmap from given path and igonrs image cache while doing so.
        /// </summary>
        /// <param name="path">Path to image</param>
        /// <returns>Object of type BitmapImage representing loaded image</returns>
        private BitmapImage CreateBitmap(String path)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            image.UriSource = new Uri(path);
            image.EndInit();
            return image;
        }
        /// <summary>
        /// Method visualizys Mandelbrot set with given input sequentiali filling array which stores information if (x,y)
        /// in Cartesian coordinate system bellong to Mandelbrot set and times it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SequentailButton_Click(object sender, RoutedEventArgs e)
        {
            SetInputAsMandelbrotParameters();
            SetAllControlsEnabled(false);
            Mouse.OverrideCursor = Cursors.Wait;
            this.OutputLabel.Content = "";
            Thread t = new Thread(Sequential);
            t.Start(this.Mandelbrot);
        }
        private void Sequential(object obj)
        {
            Mandelbrot m = obj as Mandelbrot;
            TimeSpan time = m.SequentailFillArrayWithTiming();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                SetImage(m.BelongingsArray);
                this.OutputLabel.Content += "Sekwencyjnie genracja obrazu zajęła: " + time.ToString() + "\n";
                SetAllControlsEnabled(true);
                Mouse.OverrideCursor = null;
            }), null);
        }

        /// <summary>
        /// Method visualizys Mandelbrot set with given input in parllalel filling array which stores information if (x,y)
        /// in Cartesian coordinate system bellong to Mandelbrot set and times it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParallelButton_Click(object sender, RoutedEventArgs e)
        {
            SetInputAsMandelbrotParameters();
            SetAllControlsEnabled(false);
            Mouse.OverrideCursor = Cursors.Wait;
            this.OutputLabel.Content = "";
            Thread t = new Thread(Parallel);
            t.Start(this.Mandelbrot);
        }

        private void Parallel(object obj)
        {
            Mandelbrot m = obj as Mandelbrot;
            TimeSpan time = m.ParallelFillArrayWithTiming();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                SetImage(m.BelongingsArray);
                this.OutputLabel.Content += "Równolegle genracja obrazu zajęła: " + time.ToString() + "\n";
                SetAllControlsEnabled(true);
                Mouse.OverrideCursor = null;
            }), null);
        }
        /// <summary>
        /// Method visualizys Mandelbrot set with given input in parllalel and sequentaily
        /// filling array which stores information if (x,y) in Cartesian coordinate system bellong to Mandelbrot set.
        /// It also times both versions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            SetInputAsMandelbrotParameters();
            SetAllControlsEnabled(false);
            Mouse.OverrideCursor = Cursors.Wait;
            this.OutputLabel.Content = "";
            Thread t = new Thread(() =>
            {
                Thread st = new Thread(Sequential);
                Thread pt = new Thread(Parallel);
                st.Start(this.Mandelbrot);
                pt.Start(new Mandelbrot(Mandelbrot.N, Mandelbrot.Size));
            });
            t.Start();
        }
        /// <summary>
        /// Method which allows to save generated image to file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.Filter = "Image (*.png)|*.png";
            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            saveDialog.Title = "Zapisz plik";
            saveDialog.FileName = "mandelbrot";
            if (saveDialog.ShowDialog() == true)
            {
                ImageBuilder imageBuilder = new ImageBuilder(Mandelbrot.BelongingsArray);
                imageBuilder.SaveImage(saveDialog.FileName);
            }
        }
    }
}
