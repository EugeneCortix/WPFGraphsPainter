using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Diagnostics;
using System.Windows.Forms;

using CheckBox = System.Windows.Controls.CheckBox;
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, ObservableCollection<dataValues>> _data = new Dictionary<string, ObservableCollection<dataValues>>();
        ObservableCollection<dataValues> dataValuesObject;
        List<CheckBox> checkBoxes = new List<CheckBox>();
        Dictionary<string, Brush> colorPens = new Dictionary<string, Brush>();
        double xScale = 0;
        double yScale = 0;
        double kx;
        double ky;
        double xSubZero = 0;
        double ySubZero = 0;

        double zoomMax = 5;
        double zoomMin = 1;
        double zoomSpeed = 0.001;
        double zoom = 1;

        double xmax = 0;
        double ymax = 0;
        double xmin = 0;
        double ymin = 0;

        public MainWindow()
        {
            InitializeComponent();

            dataValuesObject = new ObservableCollection<dataValues> 
            { 
            new dataValues{X = 0,Y = 0},
            new dataValues{X = 1,Y = 2}
            };
            dataValuesGrid.ItemsSource = dataValuesObject;

           // BoxOfDatasOnGrid.Items.Add("Исходные данные");
            BoxOfDatasOnGrid.SelectedIndex = 0;
            drawWay.Items.Add("Draw as a line");
            drawWay.Items.Add("Draw as a spline");
            drawWay.SelectedIndex = 0;

            string inname = "Исходные данные";
            _data.Add(inname, dataValuesObject);
            CheckBox newCheckBox = new CheckBox();
            newCheckBox.Content = inname;
            listOfFiles.Items.Add(newCheckBox);
            checkBoxes.Add(newCheckBox);
            BoxOfDatasOnGrid.Items.Add(inname);
            Random rnd = new Random();
            colorPens.Add(inname, new SolidColorBrush(Color.FromRgb(Convert.ToByte(rnd.Next(0, 255)),
                                                                                     Convert.ToByte(rnd.Next(0, 255)),
                                                                                     Convert.ToByte(rnd.Next(0, 255)))));


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dataValuesObject.Add(new dataValues { X = 4, Y = 16 });
            repaint();
        }

        

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog(); 
            saveFileDialog.Filter = "Text Files | *.txt";
            // Открытие диалога
            saveFileDialog.Title = "Save Data as...";
            if (saveFileDialog.ShowDialog() == false)
                return;

            // Сохранение текста. Создание строки
            string data = "";

            // Добавления в строку значений из таблицы
            for (int i = 0; i < dataValuesObject.Count; i++)
            {
                data += dataValuesObject[i].X.ToString() +'\t' + dataValuesObject[i].Y.ToString() + '\n';
            }
            // Сохранение результата в файл
            System.IO.File.WriteAllText(saveFileDialog.FileName, data);

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open File";
            if (openFileDialog.ShowDialog() == false)
                return;
            // Name shorting
            string lastname = "";
            int ls = 0;
            for (int i = 0; i < openFileDialog.FileName.Length; i++) if (openFileDialog.FileName[i] == '\\') ls = i;
            for (int j = ls + 1; j < openFileDialog.FileName.Length; j++) lastname += openFileDialog.FileName[j];

            if (_data.ContainsKey(lastname))
            {
                MessageBox.Show("Такой файл уже открыт!");
            }
            else
            {
                _data.Add(lastname, readData(openFileDialog.FileName));
                CheckBox newCheckBox = new CheckBox();
                newCheckBox.Content = lastname;
                listOfFiles.Items.Add(newCheckBox);
                checkBoxes.Add(newCheckBox);
                BoxOfDatasOnGrid.Items.Add(lastname);

                //Colors
                Random rnd = new Random();
                colorPens.Add(lastname, new SolidColorBrush(Color.FromRgb(Convert.ToByte(rnd.Next(0, 255)), 
                                                                                         Convert.ToByte(rnd.Next(0, 255)),
                                                                                         Convert.ToByte(rnd.Next(0, 255)))));
                
            }

        }


        private ObservableCollection<dataValues> readData(string filename)
        {

            // Читаем файл в строку
            string[] fileText = System.IO.File.ReadAllLines(filename);

            ObservableCollection<dataValues> Data;
            Data = new ObservableCollection<dataValues>();
            foreach (string data in fileText)
            {
                string[] datas;
                datas = data.Split('\t');
                Data.Add(new dataValues { X = Convert.ToDouble(datas[0]), Y = Convert.ToDouble(datas[1]) });
            }
            return Data;
        }

        private void BoxOfDatasOnGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BoxOfDatasOnGrid.SelectedIndex == 0) dataValuesGrid.ItemsSource = dataValuesObject; else
            {
                dataValuesGrid.ItemsSource = _data[BoxOfDatasOnGrid.SelectedItem.ToString()];
            }
            repaint();
        }

        private void drawLine(string g)
        {
            for (int i = 1; i < _data[g].Count; i++)
            {
                // System.Diagnostics.Debugger.Break();
                double x1 = (_data[g][i - 1].X - xSubZero) * kx;
                double y1 = (_data[g][i - 1].Y - ySubZero) * ky;
                double x2 = (_data[g][i].X - xSubZero) * kx;
                double y2 = (_data[g][i].Y - ySubZero) * ky;


                Line newline = new Line()
                {
                    X1 = x1,
                    Y1 = 300 - y1,
                    X2 = x2,
                    Y2 = 300 - y2,
                    Stroke = colorPens[g],
                    StrokeThickness = 3
                };
                graphCanvas.Children.Add(newline);
            }
        }

        private void drawSline (string g)
        {
            if (_data[g].Count < 3)
            {
                drawLine(g);
                return;
            }
            Path spline = new Path();
            spline.Stroke  = colorPens[g];
            spline.StrokeThickness = 3;
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point((_data[g][0].X - xSubZero) * kx, 
                300 - (_data[g][0].Y - ySubZero) * ky);

            PointCollection p = new PointCollection();
            for (int i = 1; i < _data[g].Count; i ++)
            {
                p.Add(new Point((_data[g][i].X - xSubZero) * kx,
                   300 - (_data[g][i].Y - ySubZero) * ky));
            }
            PolyQuadraticBezierSegment myBezierSegment = new PolyQuadraticBezierSegment();
            myBezierSegment.Points = p;
            figure.Segments.Add(myBezierSegment);

            geometry.Figures.Add(figure);
            spline.Data = geometry;
            graphCanvas.Children.Add(spline);
        }

        private void paint(string g)
        {
            draw(g);
            Label leglab = new Label();
            leglab.Content = g;
            leglab.Background = (colorPens[g]);
            stackLegend.Children.Add(leglab);
            
        }

        private void draw(string g)
        {
            if (drawWay.SelectedIndex == 0)
                drawLine(g);
            else
                drawSline(g);
        }

        private void recountScale(string g)
        {
            

            for (int i = 0; i < _data[g].Count; i++)
            {
                if (_data[g][i].X > xmax) xmax = _data[g][i].X;
                if (_data[g][i].Y > ymax) ymax = _data[g][i].Y;
                if (_data[g][i].X < xmin) xmin = _data[g][i].X;
                if (_data[g][i].Y < ymin) ymin = _data[g][i].Y;

            }

            if (xmax - xmin > xScale) xScale = xmax - xmin;
            if (ymax - ymin > yScale) yScale = ymax - ymin;

            kx = graphCanvas.ActualWidth / xScale;
            ky = graphCanvas.ActualHeight / yScale;

            if (xmin < xSubZero) xSubZero = xmin;
            if (ymin < ySubZero) ySubZero = ymin;
        }

        private void makegrid()
        {
            for (int i = 0; i <= xScale; i++)
            {
                Line newline = new Line()
                {
                    X1 = i*kx,
                    Y1 = 300,
                    X2 = i*kx,
                    Y2 = 0,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };
                graphCanvas.Children.Add(newline);
            }
            for (int i = 0; i <= yScale; i++)
            {
                Line newline = new Line()
                {
                    X1 = 0,
                    Y1 = i*ky,
                    X2 = 400,
                    Y2 = i*ky,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };
                graphCanvas.Children.Add(newline);
            }

            // Labels
            if(zoom == 1)
            {
                labelxl.Content = (xmin).ToString();
                labelxm.Content = ((xmax - xmin) / 2).ToString();
                labelxr.Content = (xmax).ToString();
                labelyb.Content = (ymin).ToString();
                labelym.Content = ((ymax - ymin) / 2).ToString();
                labelyt.Content = (ymax).ToString();
                overCanvas.Children.Clear();
            }
        }

       // private void

        private void ListOfFiles_MouseMove(object sender, MouseEventArgs e)
        {
            repaint();
        }

        private void repaint()
        {
            var items = listOfFiles.Items;

            graphCanvas.Children.Clear();
            stackLegend.Children.Clear();

            xScale = 0;
            yScale = 0;
            xSubZero = 0;
            ySubZero = 0;
            xmax = 0;
            xmin = 0;
            ymin = 0;
            ymax = 0;

            foreach (CheckBox c in checkBoxes)
            {
                if (c.IsChecked == true)
                {
                    string s = (c.Content).ToString();
                    recountScale(s);
                };
            }
            makegrid();

            foreach (CheckBox c in checkBoxes)
            {
                if (c.IsChecked == true)
                {
                    string s = (c.Content).ToString();
                    paint(s);
                };
            }
        }

        private void overlay()
        {
            //Height="300" Margin="256,46,0,0" VerticalAlignment="Top" Width="403"
            Rectangle rect1 = new Rectangle();
            Rectangle rect2 = new Rectangle();
            Rectangle rect3 = new Rectangle();
            Rectangle rect4 = new Rectangle();

            Brush wBrush = new SolidColorBrush(Colors.White);

            // Left
            rect1.Width = 256;
            rect1.Height = 420;
            rect1.Fill = wBrush;
            overCanvas.Children.Add(rect1);
            //устанавливаем расположение
            Canvas.SetTop(rect1, 0);
            Canvas.SetLeft(rect2, 0);

            //Height="420" VerticalAlignment="Top" Width="906
            // Top
            rect2.Width = 906;
            rect2.Height = 46;
            rect2.Fill = wBrush;
            overCanvas.Children.Add(rect2);
            //устанавливаем расположение
            Canvas.SetTop(rect2, 0);
            Canvas.SetLeft(rect2, 0);

            // Bottom
            rect3.Width = 906;
            rect3.Height = 420-346;
            rect3.Fill = wBrush;
            overCanvas.Children.Add(rect3);
            //устанавливаем расположение
            Canvas.SetTop(rect3, 346);
            Canvas.SetLeft(rect3, 0);

            // Right
            rect4.Width = 906-(256 + 403);
            rect4.Height = 420;
            rect4.Fill = wBrush;
            overCanvas.Children.Add(rect4);
            //устанавливаем расположение
            Canvas.SetTop(rect3, 0);
            Canvas.SetLeft(rect3, 256+403);
        }

        private void GraphCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
             zoom += zoomSpeed * e.Delta; // Ajust zooming speed (e.Delta = Mouse spin value )
             if (zoom < zoomMin) { zoom = zoomMin; } // Limit Min Scale
             if (zoom > zoomMax) { zoom = zoomMax; } // Limit Max Scale

             Point mousePos = e.GetPosition(graphCanvas);


             if (zoom > 1)
             {
                 graphCanvas.RenderTransform = new ScaleTransform(zoom, zoom, mousePos.X, mousePos.Y); // transform Canvas size from mouse position
                overlay(); // Hide extra
            }
             else
             {
                 graphCanvas.RenderTransform = new ScaleTransform(zoom, zoom); // transform Canvas size
             }

                labelxl.Content = "";
                labelxm.Content = "";
                labelxr.Content = "";
                labelyb.Content = "";
                labelym.Content = "";
                labelyt.Content = "";

            // graphCanvas.Width = 400;
            // graphCanvas.Height = 300;
          //  overlay();
            ListOfFiles_MouseMove(sender, e);
        }

        private void StackLegend_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //ChooseColor(sender, e);

            //25
            double h = e.GetPosition(stackLegend).Y;
            int i = Convert.ToInt32(Math.Floor(h / 25.0));
            string s = (((sender as StackPanel)).Children[i] as Label)?.Content?.ToString();
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            colorDialog.ShowDialog();
            System.Drawing.Color color1 = colorDialog.Color;
            Color color = Colors.Red;
            color.A = color1.A;
            color.R = color1.R;
            color.G = color1.G;
            color.B = color1.B;
            Brush brush = new SolidColorBrush(color);
            colorPens[s] = brush;
            ((sender as StackPanel).Children[i] as Label).Background = (colorPens[s]);
            draw(s);
        }

        private void DrawWay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            repaint();
        }
    }
    public class dataValues
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
