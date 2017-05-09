using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace _2labMisoi
{
    public class ProcessingImage
    {
        private static int[,] areas;
        private static Bitmap _bitmap;
        private static List<Region> _regions;

        public ProcessingImage(Bitmap bitmap)
        {
            var r = new Random();
            _regions = new List<Region>();

            for (var i = 0; i < 1000; i++)
                _regions.Add(new Region(Color.FromArgb(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255))));

            _bitmap = bitmap;
        }

        public void Binarization(double threshhold, PictureBox pictureBox)
        {
            for (var x = 0; x < _bitmap.Height; x++)
            {
                for (var y = 0; y < _bitmap.Width; y++)
                {
                    var pixel = _bitmap.GetPixel(y, x);
                    var d = pixel.GetBrightness();

                    if (d > threshhold)
                    {
                        _bitmap.SetPixel(y, x, Color.FromArgb(255, 255, 255));
                    }
                    else
                    {
                        _bitmap.SetPixel(y, x, Color.FromArgb(0, 0, 0));
                    }
                }
            }

            pictureBox.Image = _bitmap;
            pictureBox.Update();
        }

        public void CalculateAreas()
        {
            foreach (var n in _regions)
            {
                n.Area = 0;
            }

            for (var i = 0; i < _regions.Count; i++)
            {
                _regions[i].Area = _regions[i].pixels.Count;
            }
        }

        //получить битовый массив бинарного изображения
        public void BinaryImageToBytes()
        {
            areas = new int[_bitmap.Height, _bitmap.Width];

            for (var x = 0; x < areas.GetLength(0); x++)
            {
                for (var y = 0; y < areas.GetLength(1); y++)
                {
                    areas[x, y] = (int)_bitmap.GetPixel(y, x).GetBrightness();
                }
            }
        }

        public void Perimeter()
        {
            var area = new int[4];

            for (var x = 0; x < areas.GetLength(0); x++)
            {
                for (var y = 0; y < areas.GetLength(1); y++)
                {
                    if (areas[x, y] != 0)
                    {
                        int overXY = 0;
                        int overYX = 0;
                        int underYX = 0;
                        int underXY = 0;

                        if (x == 0)
                            overXY = 0;
                        else
                            overXY = areas[x - 1, y];

                        if (y == 0)
                            overYX = 0;
                        else
                            overYX = areas[x, y - 1];

                        if (x == areas.GetLength(0) - 1)
                            underXY = 0;
                        else
                            underXY = areas[x + 1, y];

                        if (y == areas.GetLength(1) - 1)
                            underYX = 0;
                        else
                            underYX = areas[x, y + 1];

                        area[0] = underYX;
                        area[1] = overXY;
                        area[2] = underXY;
                        area[3] = overYX;

                        for (var j = 0; j < area.Length; j++)
                        {
                            if (area[j] == 0)
                            {
                                _regions[areas[x, y]].Perimeter++;
                               // _regions[areas[x, y]].perimeterCoordinates.Add(new Point(x + 1, y + 1));
                                _regions[areas[x, y]].perimeterCoordinates.Add(new Point(x, y));

                                break;
                            }
                        }
                    }
                }
            }
        }

        public void Сompactness()
        {
            foreach (var n in _regions)
            {
                n.Сompactness = 0;
            }

            for (var i = 0; i < _regions.Count; i++)
            {
                if (_regions[i].Area == 0)
                    continue;
                _regions[i].Сompactness = _regions[i].Perimeter * _regions[i].Perimeter / _regions[i].Area;
            }
        }

        public void SaveIntoFile(string fileName)
        {
            var array = new List<string>();
            for (var x = 0; x < areas.GetLength(0); x++)
            {
                var strBuilder = new StringBuilder();
                for (var y = 0; y < areas.GetLength(1); y++)
                {
                    if (areas[x, y].ToString().Length == 1)
                    {
                        strBuilder.Append(areas[x, y].ToString() + "  ");
                        continue;
                    }

                    if (areas[x, y].ToString().Length == 2)
                    {
                        strBuilder.Append(areas[x, y].ToString() + " ");
                        continue;
                    }

                    strBuilder.Append(areas[x, y].ToString());
                }
                array.Add(strBuilder.ToString());
            }

            using (StreamWriter streamWriter = new StreamWriter(fileName))
            {
                foreach (var a in array)
                {
                    streamWriter.WriteLine(a);
                }
            }
        }

        public void SplitImageIntoAreas()
        {
            byte countOfArea = 0;

            for (var x = 0; x < areas.GetLength(0); x++)
            {
                for (var y = 0; y < areas.GetLength(1); y++)
                {
                    int overX = 0;
                    int overY = 0;

                    if (x == 0)
                        overX = 0;
                    else
                        overX = areas[x - 1, y];

                    if (y == 0)
                        overY = 0;
                    else
                        overY = areas[x, y - 1];


                    if (areas[x, y] == 1)
                    {
                        if (overX == 0 && overY == 0)
                        {
                            countOfArea++;
                            areas[x, y] = countOfArea;
                            _regions[countOfArea].pixels.Add(new Point(x, y));
                            continue;
                        }

                        if (overX == overY)
                        {
                            areas[x, y] = overX;
                            _regions[overX].pixels.Add(new Point(x, y));
                            continue;
                        }

                        if (overX != 0 && overY == 0 || overX == 0 && overY != 0)
                        {
                            if (overX != 0)
                                areas[x, y] = overX;
                            if (overY != 0)
                                areas[x, y] = overY;

                            _regions[areas[x, y]].pixels.Add(new Point(x, y));
                            continue;
                        }

                        if (overX != overY)
                        {
                            int over = overX;

                            areas[x, y] = countOfArea;
                            _regions[countOfArea].pixels.Add(new Point(x, y));

                            if (overX == countOfArea)
                                over = overY;

                            for (var i = 0; i < _regions[over].pixels.Count; i++)
                            {
                                _regions[countOfArea].pixels.Add(_regions[over].pixels[i]);
                            }

                            for (var i = 0; i < _regions[countOfArea].pixels.Count; i++)
                            {
                                areas[_regions[countOfArea].pixels[i].X, _regions[countOfArea].pixels[i].Y] = countOfArea;
                            }

                            while (_regions[over].pixels.Count != 0)
                            {
                                _regions[over].pixels.RemoveAt(0);
                            }
                        }
                    }
                }
            }

            SaveIntoFile("output.txt");

            for (var i = 0; i < _regions.Count; i++)
            {
                if (_regions[i].pixels.Count < 100)
                {
                    for (var j = 0; j < _regions[i].pixels.Count; j++)
                        areas[_regions[i].pixels[j].X, _regions[i].pixels[j].Y] = 0;

                    _regions[i].Area = 0;
                }
            }
        }

        public void ToPaint(PictureBox pictureBox, List<Region> regions)
        {
            var bitmap = new Bitmap(_bitmap.Width, _bitmap.Height);

            for (var x = 0; x < _bitmap.Height; x++)
            {
                for (var y = 0; y < _bitmap.Width; y++)
                {
                    bitmap.SetPixel(y, x, Color.FromArgb(0, 0, 0));
                }
            }

            for (var i = 0; i < regions.Count; i++)
            {
                for (var j = 0; j < regions[i].pixels.Count; j++)
                {
                    bitmap.SetPixel(regions[i].pixels[j].Y, regions[i].pixels[j].X, regions[i]._color);

                }
            }

            for (var i = 0; i < _regions.Count; i++)
            {
                for (var j = 0; j < _regions[i].perimeterCoordinates.Count; j++)
                {
                    bitmap.SetPixel(_regions[i].perimeterCoordinates[j].Y, _regions[i].perimeterCoordinates[j].X, Color.White);
                }
            }

            pictureBox.Image = bitmap;
            pictureBox.Update();

        }

        public void ToPaint(PictureBox pictureBox)
        {
            var bitmap = new Bitmap(_bitmap.Width, _bitmap.Height);

            for (var x = 0; x < _bitmap.Height; x++)
            {
                for (var y = 0; y < _bitmap.Width; y++)
                {
                    bitmap.SetPixel(y, x, Color.FromArgb(0, 0, 0));
                }
            }

            for (var i = 0; i < _regions.Count; i++)
            {
                if (_regions[i].pixels.Count < 150 && _regions[i].pixels.Count > 0)
                {
                    _regions[i]._color = Color.Black;
                    while (_regions[i].pixels.Count > 0)
                    {
                        _regions[i].pixels.RemoveAt(0);
                    }
                }

                for (var j = 0; j < _regions[i].pixels.Count; j++)
                {
                    bitmap.SetPixel(_regions[i].pixels[j].Y, _regions[i].pixels[j].X, _regions[i]._color);
                }
            }
            
            for (var i = 0; i < _regions.Count; i++)
            {
                for (var j = 0; j < _regions[i].perimeterCoordinates.Count; j++)
                {
                    bitmap.SetPixel(_regions[i].perimeterCoordinates[j].Y, _regions[i].perimeterCoordinates[j].X, Color.White);
                }
            }


            //for (var i = 0; i < _regions.Count; i++)
            //{
            //    if (_regions[i].pixels.Count >= 150)
            //        Debug.WriteLine("{0}, {1}, {2}", i, _regions[i].Area, _regions[i].Perimeter);
            //}

            pictureBox.Image = bitmap;
            pictureBox.Update();
        }

        public List<Region> GetObjectsFromImage()
        {
            var regions = new List<Region>();

            for (var i = 0; i < _regions.Count; i++)
            {
                if (_regions[i].pixels.Count >= 150)
                {
                    regions.Add(_regions[i]);
                }
            }

            return regions;
        }
    }
}
