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

        DrawingGroup drawingGroup = new DrawingGroup();





        public MainWindow()
        {
            InitializeComponent();




            dataValuesObject = new ObservableCollection<dataValues> 
            { 
            new dataValues{X = 0,Y = 0},
            new dataValues{X = 1,Y = 1}
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
            double xpr = dataValuesObject[dataValuesObject.Count - 1].X + 1;
            double ypr = xpr*xpr;
            dataValuesObject.Add(new dataValues { X = xpr, Y = ypr });
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
            GeometryDrawing myGeometryDrawing = new GeometryDrawing();
            GeometryGroup lines = new GeometryGroup();
            myGeometryDrawing.Pen = new Pen(colorPens[g], 3);
            

            for (int i = 1; i < _data[g].Count; i++)
            {
                // System.Diagnostics.Debugger.Break();
                double x1 = (_data[g][i - 1].X - xSubZero) * kx;
                double y1 = (_data[g][i - 1].Y - ySubZero) * ky;
                double x2 = (_data[g][i].X - xSubZero) * kx;
                double y2 = (_data[g][i].Y - ySubZero) * ky;


              


                lines.Children.Add(new LineGeometry(new Point(x1, graphImage.Height - y1), new Point(x2, graphImage.Height - y2)));
            }

            myGeometryDrawing.Geometry = lines;

            drawingGroup.Children.Add(myGeometryDrawing);

            graphImage.Source = new DrawingImage(drawingGroup);
        }

        private void drawSline (string g)
        {
            GeometryDrawing myGeometryDrawing = new GeometryDrawing();
            GeometryGroup splines = new GeometryGroup();
            myGeometryDrawing.Pen = new Pen(colorPens[g], 3);
            if (_data[g].Count < 3)
            {
                drawLine(g);
                return;
            }

            for(int i = 1; i < _data[g].Count; i++)
            {
                Point startPoint = new Point((_data[g][i -1 ].X - xSubZero) * kx,
                   300 - (_data[g][i - 1].Y - ySubZero) * ky);

                double dx = (_data[g][i].X - _data[g][i - 1].X) * (1.0 / 3.1);

                Point p1 = new Point((_data[g][i - 1].X  - xSubZero + dx) * kx ,
                   300 - (_data[g][i - 1].Y - ySubZero) * ky);

                Point p2 = new Point((_data[g][i].X  - xSubZero - dx) * kx ,
                                    300 - (_data[g][i].Y  - ySubZero) * ky);

                Point endPoint = new Point((_data[g][i].X - xSubZero) * kx,
                                    300 - (_data[g][i].Y - ySubZero) * ky);
                BezierSegment bz = new BezierSegment();
                bz.Point1 = p1;
                bz.Point2 = p2;
                bz.Point3 = endPoint;
                PathFigure pf = new PathFigure();
                pf.Segments.Add(bz);
                PathGeometry pg = new PathGeometry();
                pf.StartPoint = startPoint;
                pg.Figures.Add(pf);
                splines.Children.Add(pg);
            }

            //PathFigure pf = new PathFigure();
            //  pf.Segments.Add(myBezierSegment);
            //  PathGeometry pg = new PathGeometry();
            //  pf.StartPoint = new Point((_data[g][0].X - xSubZero) * kx,
          /*  300 - (_data[g][0].Y - ySubZero) * ky);
            pg.Figures.Add(pf);
            splines.Children.Add(pg);*/
            myGeometryDrawing.Geometry = splines;

            drawingGroup.Children.Add(myGeometryDrawing);
            graphImage.Source = new DrawingImage(drawingGroup);
        }


        private void paint(string g)
        {
            draw(g);

            Label leglab = new Label();
            leglab.Content = g;
            leglab.Background = (colorPens[g]);
            stackLegend.Children.Add(leglab);
            zoomPic();
            
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

            kx = graphImage.Width / xScale;
            ky = graphImage.Height / yScale;

            if (xmin < xSubZero) xSubZero = xmin;
            if (ymin < ySubZero) ySubZero = ymin;
        }

        private void makegrid()
        {
            GeometryDrawing myGeometryDrawing = new GeometryDrawing();
            GeometryGroup lines = new GeometryGroup();
            myGeometryDrawing.Pen = new Pen(Brushes.LightGray, 1);

            for (int i = 0; i <= xScale; i++)
            {

                lines.Children.Add(new LineGeometry(new Point(i * kx, graphImage.Height), new Point(i * kx, 0)));
            }
            for (int i = 0; i <= yScale; i++)
            {

                lines.Children.Add(new LineGeometry(new Point(0, i * ky), new Point(graphImage.Width, i * ky)));
            }

            myGeometryDrawing.Geometry = lines;

            drawingGroup.Children.Add(myGeometryDrawing);


            graphImage.Source = new DrawingImage(drawingGroup);
            overlay();
            // Labels
            if (zoom == 1)
            {
                labelxl.Content = (xmin).ToString();
                labelxm.Content = ((xmax + xmin) / 2).ToString();
                labelxr.Content = (xmax).ToString();
                labelyb.Content = (ymin).ToString();
                labelym.Content = ((ymax + ymin) / 2).ToString();
                labelyt.Content = (ymax).ToString();
                overCanvas.Children.Clear();
            }
        }

        private void ListOfFiles_MouseMove(object sender, MouseEventArgs e)
        {
            repaint();
        }

        private void repaint()
        {
            
            var items = listOfFiles.Items;

            drawingGroup.Children.Clear();
            graphImage.Source = new DrawingImage(drawingGroup);

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
            GeometryDrawing ar = new GeometryDrawing();

            ar.Brush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            ar.Pen = new Pen(ar.Brush, 3);
            ar.Geometry = new RectangleGeometry(new Rect(new Size(400 * zoom, 300 * zoom)));

            drawingGroup.Children.Add(ar);
        }
        

        private void StackLegend_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //ChooseColor(sender, e);

            //25
            double h = e.GetPosition(stackLegend).Y;
            int i = Convert.ToInt32(Math.Floor(h / 25.0));
            string s = (((sender as StackPanel)).Children[i] as Label)?.Content?.ToString();
            //System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            ColorDialog colorDialog = new ColorDialog();

                colorDialog.ShowDialog();
            //colorDialog.ShowDialog.DialogResult;
            
            System.Drawing.Color color1 = colorDialog.Color;
            /*if (colorDialog.ShowDialog() == DialogResult.Cancel)
                return;*/
            Color color = Colors.Red;
            color.A = color1.A;
            color.R = color1.R;
            color.G = color1.G;
            color.B = color1.B;
            Brush brush = new SolidColorBrush(color);
            colorPens[s] = brush;
            ((sender as StackPanel).Children[i] as Label).Background = (colorPens[s]);
            // draw(s);
            repaint();
        }

        private void DrawWay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            repaint();
        }

        private void graphImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoom += zoomSpeed * e.Delta; // Ajust zooming speed (e.Delta = Mouse spin value )
            if (zoom < zoomMin) { zoom = zoomMin; } // Limit Min Scale
            if (zoom > zoomMax) { zoom = zoomMax; } // Limit Max Scale


            labelxl.Content = "";
            labelxm.Content = "";
            labelxr.Content = "";
            labelyb.Content = "";
            labelym.Content = "";
            labelyt.Content = "";

            ListOfFiles_MouseMove(sender, e);

        }

        private void zoomPic()
        {
            Point mousePos = Mouse.GetPosition(graphImage);

            //drawingGroup.Transform = new ScaleTransform(zoom, zoom, mousePos.X, mousePos.Y);

            // drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(new Size(graphImage.Width, graphImage.Height)));



            if (zoom > 1)
            {
                //drawingGroup.Transform = new ScaleTransform(1/zoom, 1/zoom, mousePos.X, mousePos.Y);

                graphImage.RenderTransform = new ScaleTransform(zoom, zoom, mousePos.X, mousePos.Y); // transform Canvas size from mouse position
            }
            else
            {
                //drawingGroup.Transform = new ScaleTransform(zoom, zoom);
                graphImage.RenderTransform = new ScaleTransform(zoom, zoom); // transform Canvas size
            }

              graphImage.Source = new DrawingImage(drawingGroup);
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
            rect3.Width = window.Width;
            rect3.Height = window.Height - 46 - graphImage.Height;
            rect3.Fill = wBrush;
            overCanvas.Children.Add(rect3);
            //устанавливаем расположение
            Canvas.SetTop(rect3, graphImage.Height+46);
            Canvas.SetLeft(rect3, 0);

            // Right
            rect4.Width = window.Width - (256 + 403);
            rect4.Height = 420;
            rect4.Fill = wBrush;
            overCanvas.Children.Add(rect4);
            //устанавливаем расположение
            Canvas.SetTop(rect4, 0);
            Canvas.SetLeft(rect4, 252 + graphImage.Width);
        }
    }
    public class dataValues
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
