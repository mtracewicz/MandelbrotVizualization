using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    class Mandelbrot
    {
        /// <summary>
        /// How many number will be checked in Mandelbrot sequence to check if point bellongs to set
        /// </summary>
        public int N;
        /// <summary>
        /// Dwo dimensional boolan array where each [i,j] represents if  (i,j) in 
        /// Cartesian coordinate system belongs to Mandelbrot set
        /// </summary>
        public bool[,] BelongingsArray
        {
            get;
            private set;
        }
        private int _size;
        /// <summary>
        /// Base size of image from which its real size will be generated in 3x2 ratio.
        /// Set will also resize BelongingsArray and check if given size isn't to big if it is it will be set to 5000
        /// </summary>
        public int Size
        {
            set
            {
                _size = value;
                try
                {
                    this.BelongingsArray = new bool[this.Width, this.Height];
                    int tmp = 4 * (this.Width + this.Height * this.Width);
                }
                catch (Exception)
                {
                    _size = 5000;
                    this.BelongingsArray = new bool[this.Width, this.Height];
                }
            }
            get
            {
                return _size;
            }
        }
        /// <summary>
        /// Image Width
        /// </summary>
        private int Width
        {
            get
            {
                return 3 * this.Size;

            }
        }
        /// <summary>
        /// Image Height
        /// </summary>
        private int Height
        {
            get
            {
                return 2 * this.Size;
            }
        }
        /// <summary>
        /// Base constructor which sets accuracy and base image size (if it's to big then it is shrinked to 5000) and
        /// creates BelongingsArray of propper size
        /// </summary>
        /// <param name="N">Accouracy</param>
        /// <param name="Size">Base image size</param>
        public Mandelbrot(int N, int Size)
        {
            this.N = N;
            this._size = Size;
            try
            {
                this.BelongingsArray = new bool[this.Width, this.Height];
                int tmp = 4 * (this.Width + this.Height * this.Width);
            }
            catch (Exception)
            {
                _size = 5000;
                this.BelongingsArray = new bool[this.Width, this.Height];
            }
        }
        /// <summary>
        /// This method checks Mandelbrot sequence for given (x,y) point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true if point bellongs to Mandelbrot set, false otherwise</returns>
        private bool CheckSequence(double x, double y)
        {
            Complex z = new Complex(0, 0);
            Complex p = new Complex(x, y);
            for (int i = 1; i <= N; i++)
            {
                z = z * z + p;
                if (z.Magnitude >= 2.0)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Times execution of parallel BelongingsArray fill
        /// </summary>
        /// <returns>Timespan of FillArayParallel execution </returns>
        public TimeSpan ParallelFillArrayWithTiming()
        {
            DateTime start = DateTime.Now;
            this.FillArrayParallel();
            return (DateTime.Now - start);
        }
        /// <summary>
        /// Method which fills BelongingsArray using parallel for
        /// </summary>
        private void FillArrayParallel()
        {
            for (int i = 0; i < this.Height; i++)
            {
                double y = (double)i / this.Size - 1.0;
                Parallel.For(0, this.Width, (j, state) =>
                {
                    double x = (double)j / this.Size - 2.0;
                    this.BelongingsArray[j, i] = this.CheckSequence(x, y);
                });
            }
        }
        /// <summary>
        /// Times execution of sequential BelongingsArray fill
        /// </summary>
        /// <returns>Timespan of FillAray execution </returns>
        public TimeSpan SequentailFillArrayWithTiming()
        {
            DateTime start = DateTime.Now;
            this.FillArray();
            return (DateTime.Now - start);
        }
        /// <summary>
        /// Method which fills BelongingsArray
        /// </summary>
        private void FillArray()
        {
            for (int i = 0; i < this.Height; i++)
            {
                double y = (double)i / this.Size - 1.0;
                for (int j = 0; j < this.Width; j++)
                {
                    double x = (double)j / this.Size - 2.0;
                    this.BelongingsArray[j, i] = this.CheckSequence(x, y);
                };
            }
        }
    }
}
