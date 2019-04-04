using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Leadtools;

namespace Basics_of_Digital_Image_Processing
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int DPI { get; private set; }

        public int BitsPerPixel { get; private set; }
        public byte BrightnessMin { get; private set; }
        public byte BrightnessMax { get; private set; }
        
        public List<byte> brightnessPixels = new List<byte>();
        public List<byte> brightnessPixelsR = new List<byte>();
        public List<byte> brightnessPixelsG = new List<byte>();
        public List<byte> brightnessPixelsB = new List<byte>();
        public string ImagePath { get; private set; }
        public BitmapImage bitmapImage { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        

        private byte[,] getPixelsMatrix(BitmapImage image)
        {
            brightnessPixels.Clear();
            byte[,] pixelsMatrix = new byte[image.PixelHeight, image.PixelWidth];
            byte[] arr = new byte[image.PixelHeight * image.PixelWidth * (image.Format.BitsPerPixel / 8)];
            
            image.CopyPixels(arr, (image.Format.BitsPerPixel / 8) * image.PixelWidth, 0);
            int count = 0;

            for (int i = 0; i < image.PixelHeight; i++)
            {
                for (int j = 0; j < image.PixelWidth; j++)
                {
                    pixelsMatrix[i, j] = arr[count];
                    brightnessPixels.Add(arr[count]);
                    count += 4;                    
                }
            }
            brightnessPixels.Sort();
            BrightnessMin = brightnessPixels[0];
            BrightnessMax = brightnessPixels[brightnessPixels.Count - 1];

            return pixelsMatrix;
        }

        private List<byte[,]> getPixelsMatrixForRGB(BitmapImage image)
        {
            brightnessPixelsR.Clear();
            brightnessPixelsG.Clear();
            brightnessPixelsB.Clear();

            List<byte[,]> result = new List<byte[,]>();

            byte[,] pixelsMatrixComponentR = new byte[image.PixelHeight, image.PixelWidth];
            byte[,] pixelsMatrixComponentG = new byte[image.PixelHeight, image.PixelWidth];
            byte[,] pixelsMatrixComponentB = new byte[image.PixelHeight, image.PixelWidth];           
            byte[] arr = new byte[image.PixelHeight * image.PixelWidth * (image.Format.BitsPerPixel / 8)];

            image.CopyPixels(arr, (image.Format.BitsPerPixel / 8) * image.PixelWidth, 0);
            int count = 0;

            for (int i = 0; i < image.PixelHeight; i++)
            {
                for (int j = 0; j < image.PixelWidth; j++)
                {
                    pixelsMatrixComponentR[i, j] = arr[count];
                    brightnessPixelsR.Add(arr[count]);
                    pixelsMatrixComponentG[i, j] = arr[++count];
                    brightnessPixelsG.Add(arr[count]);
                    pixelsMatrixComponentB[i, j] = arr[++count];
                    brightnessPixelsB.Add(arr[count]);
                    count += 2;
                }
            }

            result.Add(pixelsMatrixComponentR);
            result.Add(pixelsMatrixComponentG);
            result.Add(pixelsMatrixComponentB);

            return result;
        }

        private BitmapSource BitmapFromByteArr(byte[,] img)
        {
            byte[] arr1 = new byte[(img.GetUpperBound(0) + 1) * (img.GetUpperBound(1) + 1) * BitsPerPixel/8];
            int count = 0;
            for(int i = 0; i <= img.GetUpperBound(0); i++)
            {
                for(int j = 0; j <= img.GetUpperBound(1); j++)
                {
                    arr1[count] = img[i, j];
                    arr1[++count] = img[i, j];
                    arr1[++count] = img[i, j];
                    arr1[++count] = 255;
                    count++;
                }
            }
            return BitmapSource.Create(img.GetUpperBound(1) + 1, img.GetUpperBound(0) + 1, bitmapImage.DpiX , bitmapImage.DpiY , PixelFormats.Bgr32, BitmapPalettes.BlackAndWhite, arr1, (BitsPerPixel / 8)* (img.GetUpperBound(1) + 1));

        }

        private BitmapSource BitmapFromByteArr(byte[,] R , byte[,] G , byte[,] B)
        {
            byte[] result = new byte[(R.GetUpperBound(0) + 1) * (R.GetUpperBound(1) + 1) * BitsPerPixel / 8];
            int count = 0;
            for (int i = 0; i <= R.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= R.GetUpperBound(1); j++)
                {
                    result[count] = R[i, j];
                    result[++count] = G[i, j];
                    result[++count] = B[i, j];
                    result[++count] = 255;
                    count++;

                }
            }
            return BitmapSource.Create(R.GetUpperBound(1) + 1, R.GetUpperBound(0) + 1, bitmapImage.DpiX, bitmapImage.DpiY, PixelFormats.Bgra32, null, result, (BitsPerPixel / 8) * (R.GetUpperBound(1) + 1));

        }
        
         

        private byte[,] MinimumFilter(byte[,] mas , int k)
        {
            byte[,] newImage = new byte[mas.GetUpperBound(0) + 1, mas.GetUpperBound(1) + 1];
            int pol = k / 2;
            for (int i = 0; i <= mas.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= mas.GetUpperBound(1); j++)
                {
                    int yy = i - pol;
                    if (yy < 0)
                    {
                        yy = 0;
                    }
                    int xx = j - pol;
                    if (xx < 0)
                    {
                        xx = 0;
                    }
                    int granicaW = j + pol;
                    int granicaH = i + pol;
                    if (granicaW > mas.GetUpperBound(1))
                    {
                        granicaW = mas.GetUpperBound(1);
                    }
                    if (granicaH > mas.GetUpperBound(0))
                    {
                        granicaH = mas.GetUpperBound(0);
                    }

                    List<byte> pixels = new List<byte>();
                    for (int y = yy; y <= granicaH; y++)
                    {
                        for (int x = xx; x <= granicaW; x++)
                        {
                            pixels.Add(mas[y, x]);
                        }
                    }
                    pixels.Sort();
                    newImage[i, j] = pixels[0];
                }
            }
            return newImage;
        }

        private byte[,] MaximumFilter(byte[,] mas, int k)
        {
            byte[,] newImage = new byte[mas.GetUpperBound(0) + 1, mas.GetUpperBound(1) + 1];
            int pol = k / 2;
            for (int i = 0; i <= mas.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= mas.GetUpperBound(1); j++)
                {
                    int yy = i - pol;
                    if (yy < 0)
                    {
                        yy = 0;
                    }
                    int xx = j - pol;
                    if (xx < 0)
                    {
                        xx = 0;
                    }
                    int granicaW = j + pol;
                    int granicaH = i + pol;
                    if (granicaW > mas.GetUpperBound(1))
                    {
                        granicaW = mas.GetUpperBound(1);
                    }
                    if (granicaH > mas.GetUpperBound(0))
                    {
                        granicaH = mas.GetUpperBound(0);
                    }
                    List<byte> pixels = new List<byte>();
                    for (int y = yy; y <= granicaH; y++)
                    {
                        for (int x = xx; x <= granicaW; x++)
                        {
                            pixels.Add(mas[y, x]);
                        }
                    }
                    pixels.Sort();
                    newImage[i, j] = pixels[pixels.Count-1];
                }
            }
            return newImage;
        }

        // именование
        private byte[,] HistogramAlignment(byte[,] img , List<byte> brightnessPixels)
        {
            int[] mas = new int[256];
            foreach(byte b in brightnessPixels)
            {
                if (mas[b] == 0)
                {
                    mas[b] = brightnessPixels.FindAll(item => item == b).Count;
                }
            }
            int[] masCDF = funcCDF(mas);
            int min =  masCDF.Min();

            for(int i = 0; i <= img.GetUpperBound(0); i++)
            {
                for(int j = 0; j <= img.GetUpperBound(1); j++)
                {        
                    img[i, j] = (byte)((double)(masCDF[img[i, j]] - min) / ((img.GetUpperBound(0) + 1) * (img.GetUpperBound(1) + 1) - 1) * 255.0); ;
                }
            }
            return img;
        }

        private int[] funcCDF(int[] mas)
        {
            int[] cdf = new int[256];
            for(int i = 0; i < mas.Length; i++)
            {
                for(int j = 0; j <= i; j++)
                {
                    cdf[i] += mas[j];
                }
            }
            return cdf;
        }

        private byte[,] MedianFilter(byte[,] mas, int k)
        {
            int pol = k / 2;
            for (int i = 0; i <= mas.GetUpperBound(0); i++)
            {
                for(int j = 0; j <= mas.GetUpperBound(1); j++)
                {
                    int yy = i - pol;
                    if(yy < 0)
                    {
                        yy = 0;
                    }
                    int xx = j - pol;
                    if(xx < 0)
                    {
                        xx = 0;
                    }
                    int granicaW = j + pol;
                    int granicaH = i + pol;
                    if (granicaW > mas.GetUpperBound(1))
                    {
                        granicaW = mas.GetUpperBound(1);
                    }
                    if (granicaH > mas.GetUpperBound(0))
                    {
                        granicaH = mas.GetUpperBound(0);
                    }
                    List<byte> pixels = new List<byte>();
                    for(int y = yy ; y <= granicaH; y++)
                    {
                        for(int x = xx;  x <= granicaW; x++)
                        {
                            pixels.Add(mas[y, x]);
                        }
                    }
                    pixels.Sort();
                    mas[i, j] = pixels[pixels.Count / 2];
                }
            }
            return mas;
        }

        private byte[,] LinearСontrast(byte[,] img)
        {
            for (int i = 0; i <= img.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= img.GetUpperBound(1); j++)
                {

                    img[i, j] = (byte)((((double)(img[i, j] - BrightnessMin))/(BrightnessMax - BrightnessMin))*255.0);
                }
            }
            return img;
        }

        

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.DefaultExt = ".jpg";
            openFileDlg.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG|All files (*.*)|*.*";
            openFileDlg.InitialDirectory = @"C:\Users\Nikita Grishin\source\repos\Basics_of_Digital_Image_Processing\Basics_of_Digital_Image_Processing";
            //Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();           
            if (result == true)
            {
                ImagePath = openFileDlg.FileName;
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(System.IO.Path.GetFullPath(openFileDlg.FileName), UriKind.Relative);
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                imageBefore.Source = bitmapImage;
                BitsPerPixel = bitmapImage.Format.BitsPerPixel;
                DPI = (int)bitmapImage.DpiX;
            }
        }

        private void TextBlockClick_MedianFilter(object sender, RoutedEventArgs e)
        {
            byte[,] new_image = new byte[(int)bitmapImage.Height, (int)bitmapImage.Width];
            new_image = getPixelsMatrix(bitmapImage);
            new_image = MedianFilter(new_image,3);
            BitmapSource bit = BitmapFromByteArr(new_image);
            imageAfter.Source = bit;
        }

        private void TextBlockClick_MinimumFilter(object sender, RoutedEventArgs e)
        {
            byte[,] new_image = new byte[(int)bitmapImage.Height, (int)bitmapImage.Width];
            new_image = getPixelsMatrix(bitmapImage);
            new_image = MinimumFilter(new_image, 3);
            BitmapSource bit = BitmapFromByteArr(new_image);
            imageAfter.Source = bit;
        }

        private void TextBlockClick_MaximumFilter(object sender, RoutedEventArgs e)
        {
            byte[,] new_image = new byte[(int)bitmapImage.Height, (int)bitmapImage.Width];
            new_image = getPixelsMatrix(bitmapImage);
            new_image = MaximumFilter(new_image, 3);
            BitmapSource bit = BitmapFromByteArr(new_image);
            imageAfter.Source = bit;
        }

        private void TextBlockClick_LinearСontrast(object sender, RoutedEventArgs e)
        {
            byte[,] new_image = new byte[(int)bitmapImage.Height, (int)bitmapImage.Width];
            new_image = getPixelsMatrix(bitmapImage);
            new_image = LinearСontrast(new_image);
            BitmapSource bit = BitmapFromByteArr(new_image);
            imageAfter.Source = bit;
        }

        private void TextBlockClick_HistogramAlignment(object sender, RoutedEventArgs e)
        {           
            byte[,] new_image = new byte[(int)bitmapImage.Height, (int)bitmapImage.Width];
            new_image = getPixelsMatrix(bitmapImage);
            new_image = HistogramAlignment(new_image, brightnessPixels);
            BitmapSource bit = BitmapFromByteArr(new_image);
            imageAfter.Source = null;
            imageAfter.Source = bit;
        }

        private void TextBlockClick_HistogramAlignmentRGB(object sender, RoutedEventArgs e)
        {
            
            byte[,] matrixCompontentR = new byte[(int)bitmapImage.Height, (int)bitmapImage.Width];
            byte[,] matrixCompontentG = new byte[(int)bitmapImage.Height, (int)bitmapImage.Width];
            byte[,] matrixCompontentB = new byte[(int)bitmapImage.Height, (int)bitmapImage.Width];

            matrixCompontentR = getPixelsMatrixForRGB(bitmapImage)[0];
            matrixCompontentG = getPixelsMatrixForRGB(bitmapImage)[1];
            matrixCompontentB = getPixelsMatrixForRGB(bitmapImage)[2];

            matrixCompontentR = HistogramAlignment(matrixCompontentR, brightnessPixelsR);
            matrixCompontentG= HistogramAlignment(matrixCompontentG, brightnessPixelsG);
            matrixCompontentB = HistogramAlignment(matrixCompontentB, brightnessPixelsB);

            BitmapSource bit = BitmapFromByteArr(matrixCompontentR,matrixCompontentG,matrixCompontentB);
            imageAfter.Source = null;
            imageAfter.Source = bit;
        }

        private void TextBlockClick_HistogramAlignmentHSV(object sender, RoutedEventArgs e)
        {
            List<byte> brightnessPixels = new List<byte>();

            byte[,] matrixCompontentR;
            byte[,] matrixCompontentG;
            byte[,] matrixCompontentB;

            byte[,] matrixCompontentH = new byte[(int)bitmapImage.PixelHeight, (int)bitmapImage.PixelWidth];
            byte[,] matrixCompontentS = new byte[(int)bitmapImage.PixelHeight, (int)bitmapImage.PixelWidth];
            byte[,] matrixCompontentV = new byte[(int)bitmapImage.PixelHeight, (int)bitmapImage.PixelWidth];

            matrixCompontentR = getPixelsMatrixForRGB(bitmapImage)[0];
            matrixCompontentG = getPixelsMatrixForRGB(bitmapImage)[1];
            matrixCompontentB = getPixelsMatrixForRGB(bitmapImage)[2];

            for(int i = 0; i < bitmapImage.PixelHeight; i++)
            {
                for(int j = 0; j < bitmapImage.PixelWidth; j++)
                {
                    var res = Converter.RGBtoHSV(matrixCompontentR[i, j], matrixCompontentG[i, j], matrixCompontentB[i, j]);
                    matrixCompontentH[i, j] = res.Item1;
                    matrixCompontentS[i, j] = res.Item2;
                    matrixCompontentV[i, j] = res.Item3;
                    brightnessPixels.Add(res.Item3);
                }
            }

            matrixCompontentV = HistogramAlignment(matrixCompontentV, brightnessPixels);

            for (int i = 0; i < bitmapImage.PixelHeight; i++)
            {
                for (int j = 0; j < bitmapImage.PixelWidth; j++)
                {
                    var res = Converter.HSVtoRGB(matrixCompontentH[i, j], matrixCompontentS[i, j], matrixCompontentV[i, j]);
                    matrixCompontentR[i, j] = res.Item1;
                    matrixCompontentG[i, j] = res.Item2;
                    matrixCompontentB[i, j] = res.Item3;
                }
            }
            BitmapSource bit = BitmapFromByteArr(matrixCompontentR, matrixCompontentG, matrixCompontentB);
            imageAfter.Source = null;
            imageAfter.Source = bit;
        }
    }
}
