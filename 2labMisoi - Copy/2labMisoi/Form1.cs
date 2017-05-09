using System;
using System.Drawing;
using System.Windows.Forms;

namespace _2labMisoi
{
    public partial class Form1 : Form
    {
        private const string pathToKarpov = @"D:\kkkkkkkk.jpg";
        private static double _binarizationThreshhold;
        private static string _path;
        private static Bitmap _bitmap;
        private static Bitmap _forBinarization;
        private static ProcessingImage _proccessingImage;

        public Form1()  
        {
            InitializeComponent();
        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                _path = openFileDialog1.FileName;
                _bitmap = new Bitmap(_path);
                _forBinarization = new Bitmap(_bitmap);
                pictureBox1.Image = _bitmap;
                pictureBox1.Update();
            }
            else
            {
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var bitmap = new Bitmap(_path);

            _proccessingImage = new ProcessingImage(bitmap);
            _proccessingImage.Binarization(_binarizationThreshhold, pictureBox1);
            _proccessingImage.Perimeter();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_path == null)
            {
                MessageBox.Show("Choose the image", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                if (comboBox2.SelectedIndex == -1)
                {
                    MessageBox.Show("Choose the binarization threshold", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    Bitmap bitmap = new Bitmap(_bitmap);
                    _forBinarization = new Bitmap(_bitmap);
                    _forBinarization = bitmap.MedianFilter(comboBox2.SelectedIndex * 2 + 1);
                    pictureBox1.Image = _forBinarization;
                    pictureBox1.Update();
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (_path == null)
            {
                MessageBox.Show("Choose the image", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                if (comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("Choose the binarization threshold", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {   
                    var bitmap = new Bitmap(_forBinarization);
                    _binarizationThreshhold = Convert.ToDouble(comboBox1.Text);  
                    _proccessingImage = new ProcessingImage(bitmap);
                    _proccessingImage.Binarization(_binarizationThreshhold, pictureBox1);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _proccessingImage.BinaryImageToBytes();
            _proccessingImage.SplitImageIntoAreas();
            _proccessingImage.CalculateAreas();
            _proccessingImage.Perimeter();
            _proccessingImage.Сompactness();
            _proccessingImage.ToPaint(pictureBox1);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int classes;
            Int32.TryParse(textBox1.Text, out classes);

            var kMeans = new KMeans(_proccessingImage.GetObjectsFromImage(), classes);
            _proccessingImage.ToPaint(pictureBox1, kMeans.CalculateAreas());
        }

        private void button6_Click(object sender, EventArgs e)
        { 
        }

        private void ImageProccessingAlgorithm_Click(object sender, EventArgs e)
        {
            var tempImage = new Bitmap(_forBinarization);
            double red = 0, blue = 0, green = 0;

            for (var x = 0; x < tempImage.Height; x++)
            {
                for (var y = 0; y < tempImage.Width; y++)
                {
                    
                    double r, b, g;

                    var pixel = _bitmap.GetPixel(y, x);

                    r = Convert.ToDouble(pixel.R);
                    g = Convert.ToDouble(pixel.G);
                    b = Convert.ToDouble(pixel.B);

                    if ( r == 0 && g == 0 && b == 0 )
                    {
                        tempImage.SetPixel(y, x, Color.FromArgb(0, 0, 0));
                        continue;
                    }

                    try
                    {
                        red = r / (r + g + b) * 255;
                        blue = b / (r + g + b) * 255;
                        green = g / (r + g + b) * 255;
                    }
                    catch (DivideByZeroException)
                    {

                    }


    
                    tempImage.SetPixel(y, x, Color.FromArgb(Convert.ToInt32(red), Convert.ToInt32(green), Convert.ToInt32(blue)));
                    pictureBox1.Image = tempImage;
                    red = green = blue = 0;

                }
            }

            pictureBox1.Image = tempImage;
            pictureBox1.Refresh();
            //Thread.Sleep(2000);

            var skinImage = new Bitmap(tempImage);

            for (var i = 0; i < tempImage.Height; i++)
            {
                for (var j = 0; j < tempImage.Width; j++)
                {
                    double r, b, g;

                    var pixel = skinImage.GetPixel(j, i);

                    r = Convert.ToDouble(pixel.R);
                    g = Convert.ToDouble(pixel.G);
                    b = Convert.ToDouble(pixel.B);

                    red = r / (r + g + b);
                    blue = b / (r + g + b);
                    green = g / (r + g + b);

                    if (red >= 0.2 && red <= 0.46 && green <= 2 * red - 0.4 || red > 0.46 && red <= 0.8 && green <= -red + 1)
                        skinImage.SetPixel(j, i, Color.FromArgb((int)(red * 255), (int)(green * 255), (int)(blue * 255)));
                    else
                        skinImage.SetPixel(j, i, Color.Black);
                }
            }
            pictureBox1.Image = skinImage;

            pictureBox1.Refresh();

            //Thread.Sleep(2000);

            try
            {
                _binarizationThreshhold = Convert.ToDouble(comboBox1.Text);
            }
            catch (Exception ex)
            {
                _binarizationThreshhold = 0.5;
            }

            _binarizationThreshhold = 0.3;
            
            Binarization(skinImage, _binarizationThreshhold);
            pictureBox1.Image = skinImage;
            pictureBox1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void Binarization(Bitmap _bitmap, double threshhold)
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
        }
    }
}
