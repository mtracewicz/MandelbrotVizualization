using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageBuilderLibrary
{
    public class ImageBuilder
    {
        /// <summary>
        /// Variable holding base value of size from which image sizeis created in 3x2 ratio
        /// </summary>
        private int Size;
        /// <summary>
        /// Buffer in which image will be stored
        /// </summary>
        public byte[] ImageBuffer
        {
            get;
            private set;
        }
        /// <summary>
        /// Dwo dimensional boolan array where each [i,j] represents if  (i,j) in 
        /// Cartesian coordinate system belongs to Mandelbrot set
        /// </summary>
        private bool[,] _BelongingsArray;
        /// <summary>
        /// Dwo dimensional boolan array where each [i,j] represents if  (i,j) in 
        /// Cartesian coordinate system belongs to Mandelbrot set.
        /// Set checks if array sieze is proper and sets Size and  and ImageBuffer size to fit.
        /// </summary>
        public bool[,] BelongingsArray {
            private get
            {
                return _BelongingsArray;
            }
            set
            {
                if ((value.GetLength(0) / 3) != (value.GetLength(1) / 2))
                {
                    throw new ArgumentException("Array need to be of size [3*x,2*x]");
                }
                this.Size = value.GetLength(0) / 3;
                this._BelongingsArray = value;
                this.ImageBuffer = new byte[4 * (this.Width + this.Height * this.Width)];
            }
        }
        /// <summary>
        /// Base constructor which checks size, saves arrayss and initailizes ImageBuffer to propper size
        /// </summary>
        /// <param name="Array"></param>
        public ImageBuilder(bool[,] Array)
        {
            if ((Array.GetLength(0) / 3) != (Array.GetLength(1) / 2))
            {
                throw new ArgumentException("Array need to be of size [3*x,2*x]");
            }
            this.Size = Array.GetLength(0) / 3;
            this.BelongingsArray = Array;
            this.ImageBuffer = new byte[4 * (this.Width + this.Height * this.Width)];
        }
        /// <summary>
        /// Property representing width of image
        /// </summary>
        public int Width
        {
            get
            {
                return 3 * this.Size;

            }
        }
        /// <summary>
        /// Property representing height of image
        /// </summary>
        public int Height
        {
            get
            {
                return 2 * this.Size;
            }
        }
        /// <summary>
        /// Sets pixel which coordinates are (x,y) to RGBA value in ImageBuffer
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="R">Red</param>
        /// <param name="G">Green</param>
        /// <param name="B">Blue</param>
        /// <param name="A">Alpha</param>
        private void SetPixel(int x, int y, byte R, byte G, byte B, byte A)
        {
            int offset = 4 * (x + y * this.Width);
            ImageBuffer[offset] = B;
            ImageBuffer[offset + 1] = G;
            ImageBuffer[offset + 2] = R;
            ImageBuffer[offset + 3] = A;
        }
        /// <summary>
        /// Generates image into ImageBuffer based on BelongingsArray
        /// </summary>
        private void GenerateImage()
        {
            for (int j = 0; j < this.Height; j++)
            {
                for (int i = 0; i < this.Width; i++)
                {

                    if (BelongingsArray[i, j])
                    {
                        this.SetPixel(i, j, 255, 0, 0, 255);
                    }
                    else
                    {
                        this.SetPixel(i, j, 0, 0, 0, 255);
                    }
                }
            }
        }
        /// <summary>
        /// Saves image stored in ImageBuffer into file
        /// </summary>
        /// <param name="fileName">File name under which the image will be saved</param>
        public void SaveImage(String fileName)
        {
            this.GenerateImage();
            unsafe
            {
                fixed (byte* ptr = ImageBuffer)
                {
                    using Bitmap image = new Bitmap(this.Width, this.Height, 4 * this.Width,
                       PixelFormat.Format32bppRgb, new IntPtr(ptr));
                    image.Save(fileName);
                }
            }
        }
    }
}