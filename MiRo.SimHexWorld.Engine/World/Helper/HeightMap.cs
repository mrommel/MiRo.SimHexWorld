using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MiRo.AiXGameWindow.World.MapCreation.Conversions;

namespace MiRo.AiXGame.Maps
{
    public class HeightMap
    {
        public const byte Overlap = 34;

        private float[][] _heights;
        private int _size, _startX, _startY;

        #region Properties

        public int StartY
        {
            get { return _startY; }
            set { _startY = value; }
        }

        public int StartX
        {
            get { return _startX; }
            set { _startX = value; }
        }

        public float this[int x, int y]
        {
            get { return _heights[x + Overlap][y + Overlap]; }
            set { _heights[x + Overlap][y + Overlap] = value; }
        }

        public int Size
        {
            get { return _size; }
        }

        #endregion

        public HeightMap(int size, int startX = 0, int startY = 0)
        {
            if (size < 2 || (size & (size - 1)) != 0)
            {
                throw new ArgumentException("Size must be bigger than 1 and a power of 2.", "size");
            }

            int realSize = size + (Overlap * 2);

            _size = size;
            _startX = startX;
            _startY = startY;
            _heights = new float[realSize][];

            for (int i = 0; i < realSize; i++) _heights[i] = new float[realSize];

            Perlin.Seed = new Random().Next();
        }

        #region Public Methods

        public float[][] GetOverlappedHeights()
        {
            return _heights;
        }

        public void AlignEdges(HeightMap leftNeighbor, HeightMap rightNeighbor,
                HeightMap topNeighbor, HeightMap bottomNeighbor, int shift = 0)
        {
            int x, y, counter;
            float[][] nHeights;

            int size = this.Size;

            if (leftNeighbor != null)
            {
                nHeights = leftNeighbor.GetOverlappedHeights();
                counter = 0;

                for (x = size + Overlap - shift; x < size + (Overlap * 2); x++)
                {
                    for (y = 0; y < size; y++)
                    {
                        _heights[counter][y] = nHeights[x][y];
                    }

                    counter++;
                }

                x = size - 1;

                for (y = 0; y < size; y++)
                {
                    this[0, y] = leftNeighbor[x, y];
                }
            }
            if (rightNeighbor != null)
            {
                nHeights = rightNeighbor.GetOverlappedHeights();
                counter = 0;

                for (x = size + Overlap - shift; x < size + (Overlap * 2); x++)
                {
                    for (y = 0; y < size; y++)
                    {
                        _heights[x][y] = nHeights[counter][y];
                    }

                    counter++;
                }

                x = size - 1;

                for (y = 0; y < size; y++)
                {
                    this[x, y] = rightNeighbor[0, y];
                }
            }
            if (topNeighbor != null)
            {
                nHeights = topNeighbor.GetOverlappedHeights();
                counter = 0;

                for (y = size + Overlap - shift; y < size + (Overlap * 2); y++)
                {
                    for (x = 0; x < size; x++)
                    {
                        _heights[x][y] = nHeights[x][counter];
                    }

                    counter++;
                }

                y = size - 1;

                for (x = 0; x < size; x++)
                {
                    this[x, y] = topNeighbor[x, 0];
                }
            }
            if (bottomNeighbor != null)
            {
                nHeights = bottomNeighbor.GetOverlappedHeights();
                counter = 0;

                for (y = size + Overlap - shift; y < size + (Overlap * 2); y++)
                {
                    for (x = 0; x < size; x++)
                    {
                        _heights[x][counter] = nHeights[x][y];
                    }

                    counter++;
                }

                y = size - 1;

                for (x = 0; x < size; x++)
                {
                    this[x, 0] = bottomNeighbor[x, y];
                }
            }
        }

        public void SetNoise(float frequency, byte octaves = 1, float persistence = 0.5f, float lacunarity = 2.0f,
                bool additive = false)
        {
            int size = _heights.GetLength(0);
            int startX = _startX - Overlap;
            int startY = _startY - Overlap;
            var fSize = (float)size;

            Parallel.For((long) 0, size, x =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                for (int y = 0; y < size; y++)
                {
                    float value = 0.0f;
                    float currentPersistence = 1.0f;
                    var coord = new Vector2(
                        ((x + startX)),
                        ((y + startY)));
                    coord *= frequency;

                    byte currentOctave;
                    for (currentOctave = 0; currentOctave < octaves; currentOctave++)
                    {
                        var signal = Perlin.Noise2(coord.X, coord.Y);
                        value += signal * currentPersistence;
                        coord *= lacunarity;
                        currentPersistence *= persistence;
                    }

                    _heights[x][y] = (!additive) ? value : _heights[x][y] + value;
                }
            });
        }

        public void Perturb(float frequency, float depth)
        {
            int u, v, i, j;
            int size = _heights.GetLength(0);
            int startX = _startX - Overlap;
            int startY = _startY - Overlap;
            float[][] temp = new float[size][];
            float fSize = (float)size;
            Vector2 coord;

            for (i = 0; i < size; ++i)
            {
                temp[i] = new float[size];

                for (j = 0; j < size; ++j)
                {
                    coord = new Vector2(
                            (i + startX) / fSize,
                            (j + startY) / fSize);

                    coord *= frequency;

                    u = i + (int)(Perlin.Noise3(coord.X, coord.Y, 0.0f) * depth);
                    v = j + (int)(Perlin.Noise3(coord.X, coord.Y, 1.0f) * depth);

                    if (u < 0) u = 0;
                    if (u >= size) u = size - 1;
                    if (v < 0) v = 0;
                    if (v >= size) v = size - 1;

                    temp[i][j] = _heights[u][v];
                }
            }

            _heights = temp;
        }

        public void Erode(float smoothness)
        {
            int size = _heights.GetLength(0);

            Parallel.For(1, size - 1, i =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                int u, v;
                float d_max, d_i, d_h;
                int[] match;

                for (int j = 1; j < size - 1; j++)
                {
                    d_max = 0.0f;
                    match = new[] { 0, 0 };

                    for (u = -1; u <= 1; u++)
                    {
                        for (v = -1; v <= 1; v++)
                        {
                            if (Math.Abs(u) + Math.Abs(v) > 0)
                            {
                                d_i = _heights[i][j] - _heights[i + u][j + v];

                                if (d_i > d_max)
                                {
                                    d_max = d_i;
                                    match[0] = u;
                                    match[1] = v;
                                }
                            }
                        }
                    }

                    if (0 < d_max && d_max <= (smoothness / (float)size))
                    {
                        d_h = 0.5f * d_max;

                        _heights[i][j] -= d_h;
                        _heights[i + match[0]][j + match[1]] += d_h;
                    }
                }
            });
        }

        public void Smoothen()
        {
            int i, j, u, v;
            int size = _heights.GetLength(0);
            float total;

            for (i = 1; i < size - 1; ++i)
            {
                for (j = 1; j < size - 1; ++j)
                {
                    total = 0.0f;

                    for (u = -1; u <= 1; u++)
                    {
                        for (v = -1; v <= 1; v++)
                        {
                            total += _heights[i + u][j + v];
                        }
                    }

                    _heights[i][j] = total / 9.0f;
                }
            }
        }

        public void Normalize()
        {
            int size = _heights.GetLength(0);
            float min = -1f, max = 1f;

            Parallel.For(0, size, x =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                for (int y = 0; y < size; y++)
                {
                    _heights[x][y] = (_heights[x][y] - min) / (max - min);
                }
            });
        }

        public void MakeFlat(float height = 0.0f)
        {
            int x, y;
            int size = _heights.GetLength(0);

            for (x = 0; x < size; x++)
            {
                for (y = 0; y < size; y++)
                {
                    _heights[x][y] = height;
                }
            }
        }

        public void Multiply(float amount)
        {
            int x, y;
            int size = _heights.GetLength(0);

            for (x = 0; x < size; x++)
            {
                for (y = 0; y < size; y++)
                {
                    _heights[x][y] *= amount;
                }
            }
        }

        public void ForEach(Func<float, float> body)
        {
            int size = _heights.GetLength(0);

            Parallel.For(0, size, x =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                for (int y = 0; y < size; y++)
                {
                    _heights[x][y] = body(_heights[x][y]);
                }
            });
        }

        #endregion

        /// <summary>
        /// return the level (height) when <para>percentageBigger</para> is reached
        /// 
        /// for iteration it used the <para>stepGranularity</para>
        /// </summary>
        /// <param name="percentageBigger"></param>
        /// <param name="stepGranularity"></param>
        /// <returns></returns>
        public float FindHeightLevel(float percentageBigger, float stepGranularity)
        {
            float startThreshold = Maximum;
            float endThreshold = Minimum;
            float currentPercentage = 0f;

            while (startThreshold > endThreshold && currentPercentage < percentageBigger)
            {
                int countTiles = 0;

                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        if (this[i, j] > startThreshold)
                            countTiles++;
                    }
                }

                currentPercentage = ((float)countTiles) / (Size * Size);

                startThreshold -= stepGranularity;
            }

            return startThreshold;
        }

        public float Maximum
        {
            get
            {
                float max = float.MinValue;

                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        //if (this[i, j] < min)
                        //    min = this[i, j];

                        if (this[i, j] > max)
                            max = this[i, j];
                    }
                }

                return max;
            }
        }

        public float Minimum
        {
            get
            {
                float min = float.MaxValue;

                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        if (this[i, j] < min)
                            min = this[i, j];
                    }
                }

                return min;
            }
        }

        //public void SaveAsImage(string path, int zoom, LinearColorScale linearColorScale)
        //{
        //    Bitmap btm = new Bitmap(Size * zoom, Size * zoom);

        //    using (Graphics g = Graphics.FromImage(btm))
        //    {
        //        float min = Minimum;
        //        float max = Maximum;
        //        SolidBrush brush = new SolidBrush(System.Drawing.Color.Black);

        //        for (int i = 0; i < Size; i++)
        //        {
        //            for (int j = 0; j < Size; j++)
        //            {
        //                brush.Color = linearColorScale.Interpolate(this[i, j]);
        //                g.FillRectangle(brush, i * zoom, j * zoom, i * (zoom + 1) - 1, j * (zoom + 1) - 1);
        //            }
        //        }

        //        btm.Save(path);
        //        //PictureViewer.Show("Bitmap", btm);
        //    }
        //}
    }
}
