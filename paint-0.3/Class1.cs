using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using System.Windows.Ink;

namespace PaintApp
{
    public partial class MainWindow : Window
    {
        private bool isEraser = false;
        private Color currentColor = Colors.Black;
        private double brushSize = 5;
        private Point lastPoint;

        public MainWindow()
        {
            InitializeComponent();
            inkCanvas.StrokeCollected += InkCanvas_StrokeCollected;
            inkCanvas.PreviewMouseDown += InkCanvas_PreviewMouseDown;
            inkCanvas.PreviewMouseMove += InkCanvas_PreviewMouseMove;
            inkCanvas.PreviewMouseUp += InkCanvas_PreviewMouseUp;
            inkCanvas.PreviewKeyDown += InkCanvas_PreviewKeyDown;
        }

        private void btnColorPicker_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                currentColor = System.Windows.Media.Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                btnColorPicker.Background = new SolidColorBrush(currentColor);
                isEraser = false; // ���������� �������
                btnEraser.Background = Brushes.Transparent;
            }
        }

        private void btnEraser_Click(object sender, RoutedEventArgs e)
        {
            isEraser = !isEraser;
            if (isEraser)
            {
                btnEraser.Background = Brushes.LightGray;
            }
            else
            {
                btnEraser.Background = Brushes.Transparent;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg)|*.jpg";
            if (saveFileDialog.ShowDialog() == true)
            {
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)inkCanvas.ActualWidth, (int)inkCanvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                bmp.Render(inkCanvas);

                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));

                using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
                inkCanvas.Background = new ImageBrush(bitmapImage);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.Strokes.Clear();
            inkCanvas.Background = Brushes.White;
        }

        private void btnBrushSize_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.NumericUpDown numericUpDown = new System.Windows.Forms.NumericUpDown();
            numericUpDown.Value = (decimal)brushSize;
            if (numericUpDown.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                brushSize = (double)numericUpDown.Value;
            }
        }

        private void InkCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            lastPoint = e.GetPosition(inkCanvas);
        }

        private void InkCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(inkCanvas);
                if (isEraser)
                {
                    // ��������� ��������
                    inkCanvas.Strokes.Clear();
                    inkCanvas.StrokeCollected -= InkCanvas_StrokeCollected;
                    inkCanvas.StrokeCollected += InkCanvas_StrokeCollected;
                    inkCanvas.StrokeThickness = brushSize;
                    inkCanvas.StrokeColor = Brushes.White;
                    inkCanvas.StrokeEndCap = System.Windows.Media.PenLineCap.Round;
                    inkCanvas.StrokeLineJoin = System.Windows.Media.PenLineJoin.Round;
                    inkCanvas.StrokeDashCap = System.Windows.Media.PenLineCap.Round;
                    inkCanvas.StrokeDashArray = new DoubleCollection();
                    inkCanvas.Strokes.Add(new Stroke(new StylusPointCollection(new[] { lastPoint, currentPoint })));
                }
                else
                {
                    // ��������� ������
                    inkCanvas.StrokeThickness = brushSize;
                    inkCanvas.StrokeColor = new SolidColorBrush(currentColor);
                    inkCanvas.StrokeEndCap = System.Windows.Media.PenLineCap.Round;
                    inkCanvas.StrokeLineJoin = System.Windows.Media.PenLineJoin.Round;
                    inkCanvas.StrokeDashCap = System.Windows.Media.PenLineCap.Round;
                    inkCanvas.StrokeDashArray = new DoubleCollection();
                    inkCanvas.Strokes.Add(new Stroke(new StylusPointCollection(new[] { lastPoint, currentPoint })));
                }
                lastPoint = currentPoint;
            }
        }

        private void InkCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            // ����� ��������� �������
            if (isEraser)
            {
                isEraser = false;
                btnEraser.Background = Brushes.Transparent;
            }
        }

        private void InkCanvas_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                isEraser = !isEraser;
                if (isEraser)
                {
                    btnEraser.Background = Brushes.LightGray;
                }
                else
                {
                    btnEraser.Background = Brushes.Transparent;
                }
            }
        }

        private void InkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            // ��������� ��������� �������
        }
    }
}
