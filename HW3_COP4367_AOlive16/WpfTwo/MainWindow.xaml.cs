//
// (c) Copyright 2013 Dr.Thomas Fernandez
//        All rights reserved.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfTwo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TabControl rightTabControl;
        List<double> tabScales = new List<double>();
        ScrollBar scaleScrollBar;
        Random rand = new Random();
        bool isPoly = false;
        double ref_X = 13000.0;
        double ref_Y = 4000.0;
        List<Button> sideButton = new List<Button>();
        List<Button> toolButton = new List<Button>();
        static Color apBaseColor = Colors.Transparent;
        //static Color apBaseColor = Colors.Green;
        //Grid rootGrid;
        static int[] customColors = new int[16];

        double randD
        {
            get
            {
                return rand.NextDouble();
            }
        }

        Point canvasCenterPoint
        {
            get
            {
                return new Point(CurrentCanvas().Width / 2.0, CurrentCanvas().Height / 2.0);
            }
        }



        public MainWindow()
        {
            InitializeComponent();
            MakeTabsAndCanvases();
            SetupLayout();
            this.Closing += MainWindow_Closing;
            this.Loaded += MainWindow_Loaded;
        }

        private void SetupLayout()
        {
            mainWindow.Background = ApShadeBrush(0.3f);
            sideButton.Clear();
            toolButton.Clear();
            rootGrid = new Grid();
            mainWindow.Content = rootGrid;
            //rootGrid.Height = ClientHeight;
            //rootGrid.Width = ClientWidth;
            rootGrid.Background = ApShadeBrush(0.5f);

            //Setup the Colummns
            ColumnDefinition cd = new ColumnDefinition();
            cd.Width = new GridLength(100, GridUnitType.Pixel);
            rootGrid.ColumnDefinitions.Add(cd);

            cd = new ColumnDefinition();
            cd.Width = new GridLength(0.9, GridUnitType.Star);
            rootGrid.ColumnDefinitions.Add(cd);

            //Setup the Rows
            RowDefinition rd = new RowDefinition();
            rd.Height = new GridLength(40, GridUnitType.Pixel);
            rootGrid.RowDefinitions.Add(rd);

            rd = new RowDefinition();
            rootGrid.RowDefinitions.Add(rd);

            rootGrid.Children.Add(rightTabControl);
            SetTabColors();


            //Add Buttons
            StackPanel stackPanel = new StackPanel();
            stackPanel.Background = new SolidColorBrush(ApShadeColor(0.3f));
            Grid.SetColumn(stackPanel, 0);
            Grid.SetRow(stackPanel, 1);
            rootGrid.Children.Add(stackPanel);

            DropShadowEffect dse = new DropShadowEffect();
            dse.ShadowDepth = 5;
            dse.Color = ApShadeColor(0.2f);
            dse.Direction = -45.0;

            Button b;

            for (int i = 0; i < 16; i++)
            {
                b = new Button();
                b.Effect = dse;
                b.Margin = new Thickness(5.0);
                b.Background = new SolidColorBrush(ApShadeColor(0.4f + 0.03f * i));
                b.Content = i.ToString();
                stackPanel.Children.Add(b);
                b.Click += b_Click;
                sideButton.Add(b);
            }

            //Make Toolbar

            ToolBar toolBar = new ToolBar();
            toolBar.Background = new SolidColorBrush(ApShadeColor(0.8f));

            for (int x = 0; x <= 20; x++)
            {
                b = new Button();
                b.Margin = new Thickness(1.0);
                b.Effect = dse;
                b.Content = x.ToString("000");
                b.Background = new SolidColorBrush(ApShadeColor(0.4f + 0.03f * x));
                toolBar.Items.Add(b);
                b.Click += b_Click;
                toolButton.Add(b);
            }

            Label lab = new Label();
            lab.Content = "Scale:";
            toolBar.Items.Add(lab);


            //Add ScrollBar to ToolBar

            scaleScrollBar = new ScrollBar();
            scaleScrollBar.Orientation = Orientation.Horizontal;
            scaleScrollBar.Width = 100;
            scaleScrollBar.Value = 0;
            scaleScrollBar.Minimum = -4;
            scaleScrollBar.Maximum = 6;
            scaleScrollBar.LargeChange = 0.1;
            scaleScrollBar.SmallChange = 0.01;
            toolBar.Items.Add(scaleScrollBar);
            scaleScrollBar.ValueChanged += sb_ValueChanged;

            Grid.SetColumn(toolBar, 0);
            Grid.SetRow(toolBar, 0);
            Grid.SetColumnSpan(toolBar, 2);

            rootGrid.Children.Add(toolBar);
            relabelButtons();
        }
        private void MakeTabsAndCanvases()
        {
            //Make Tabs on Right

            rightTabControl = new TabControl();
            rightTabControl.SelectionChanged += rightTabControl_SelectionChanged;
            Grid.SetColumn(rightTabControl, 1);
            Grid.SetRow(rightTabControl, 1);

            for (int i = 0; i < 5; i++)
            {
                //Add Tabs Scroll Viewers and Canvases to tabs
                TabItem ti = new TabItem();
                ti.Header = "Tab" + i.ToString();
                //ti.Background = ApShadeBrush(0.6f + 0.05f * i);
                rightTabControl.Items.Add(ti);
                ScrollViewer sv = new ScrollViewer();
                Canvas canvas = new Canvas();
                canvas.ClipToBounds = true;
                canvas.Height = 10800;
                canvas.Width = 16200;
                sv.Content = canvas;

                canvas.Background = Brushes.White;

                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                ti.Content = sv;
                Transform t = new ScaleTransform(1.0/16.0, 1.0/16.0);
                canvas.RenderTransform = t;
                tabScales.Add(-4.0);
            }
        }
        private void SetTabColors()
        {
            for (int i = 0; i < 5; i++)
            {
                ((TabItem)(rightTabControl.Items[i])).Background = ApShadeBrush(0.6f + 0.05f * i);             
            }
        }

        void rightTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            scaleScrollBar.Value = tabScales[rightTabControl.SelectedIndex];
        }
        void sb_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double rawScale=((ScrollBar)sender).Value;
            double scale = Math.Pow(2.0, rawScale);
            Transform t = new ScaleTransform(scale, scale);
            CurrentCanvas().RenderTransform = t;
            setCurrentTabScale(rawScale);
        }
        private double getCurrentTabScale()
        {
            return tabScales[rightTabControl.SelectedIndex];
        }
        private void setCurrentTabScale(double d)
        {
            tabScales[rightTabControl.SelectedIndex] = d;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StreamReader sr = new StreamReader(@"../../CustomColors.txt");
                for (int i = 0; i < 16; i++)
                {
                    customColors[i] = Convert.ToInt32(sr.ReadLine());
                }
                sr.Close();
            }
            catch
            {
            }

        }
        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StreamWriter sw = new StreamWriter(@"../../CustomColors.txt");

            for (int i = 0; i < 16; i++)
            {
                sw.WriteLine(customColors[i]);
            }
            sw.Close();

        }

        private static Color GetColorFromDialog()
        {
            Color c = Colors.Transparent;
            System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
            cd.AllowFullOpen = true;
            cd.FullOpen = true;
            cd.CustomColors = customColors;
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                c.A = cd.Color.A;
                c.R = cd.Color.R;
                c.G = cd.Color.G;
                c.B = cd.Color.B;
                customColors = cd.CustomColors;
            }
            return c;
        }
        private SolidColorBrush RandSolidBrush()
        {
            return new SolidColorBrush(randColor());
        }
        private Color randColor()
        {
            Color c = new Color();
            c.ScR = (float)rand.NextDouble();
            c.ScG = (float)rand.NextDouble();
            c.ScB = (float)rand.NextDouble();
            c.ScA = 1.0f;
            return c;
        }
        private Color randApShadeColor()
        {
            float v = (float)rand.NextDouble();
            return ApShadeColor(v);
        }
        private SolidColorBrush ApShadeBrush(float v)
        {
            return new SolidColorBrush(ApShadeColor(v));
        }
        private static Color ApShadeColor(float v)
        {
            Color c = new Color();
            c.ScR = v;
            c.ScG = v;
            c.ScB = v;
            c.ScA = 1.0f;
            if (apBaseColor != Colors.Transparent)
            {
                c.ScR = (c.ScR + apBaseColor.ScR) / 2.0f;
                c.ScG = (c.ScG + apBaseColor.ScG) / 2.0f;
                c.ScB = (c.ScB + apBaseColor.ScB) / 2.0f;
            }
            return c;
        }

 
        private Canvas CurrentCanvas()
        {
            return ((Canvas)(((ScrollViewer)(((TabItem)(rightTabControl.Items[rightTabControl.SelectedIndex])).Content)).Content));
        }
        private Polygon MakePoly(int sides, ref Point center, double radius, int skip, double startAngle, Brush fillBrush)
        {
            Polygon p = new Polygon();
            //p.FillRule = FillRule.Nonzero;
            double angle = startAngle;
            for (int i = 0; i < sides; i++)
            {
                angle += skip * ((Math.PI * 2) / sides);

                Point po = new Point();
                po.X = Math.Cos(angle) * radius;
                po.Y = Math.Sin(angle) * radius;
                po.X += center.X;
                po.Y += center.Y;
                p.Points.Add(po);
            }
            if (shadowOn)
            {
                DropShadowEffect dse = new DropShadowEffect();
                dse.Color = Colors.Black;
                dse.Direction = -45;
                dse.ShadowDepth = radius / 10;
                dse.Opacity = 0.5;
                dse.BlurRadius = radius / 20;
                p.Effect = dse;
            }
            p.Fill = fillBrush;
            return p;
        }

        private Polygon MakePoly(int sides, ref Point center, double radius, int skip, double startAngle, Brush fillBrush, Brush edgeBrush)
        {
            Polygon p = new Polygon();
            p.StrokeThickness = radius / 50.0;
            p.Stroke = edgeBrush;
            //p.FillRule = FillRule.Nonzero;
            double angle = startAngle;
            for (int i = 0; i < sides; i++)
            {
                angle += skip * ((Math.PI * 2) / sides);

                Point po = new Point();
                po.X = Math.Cos(angle) * radius;
                po.Y = Math.Sin(angle) * radius;
                po.X += center.X;
                po.Y += center.Y;
                p.Points.Add(po);
            }
            if (shadowOn)
            {
                DropShadowEffect dse = new DropShadowEffect();
                dse.Color = Colors.Black;
                dse.Direction = -45;
                dse.ShadowDepth = radius / 10;
                dse.Opacity = 0.5;
                dse.BlurRadius = radius / 20;
                p.Effect = dse;
            }
            p.Fill = fillBrush;
            return p;
        }

        bool shadowOn = true;

        private void AddRandomCircle()
        {
            Ellipse elip = new Ellipse();
            elip.Height = randD * 1000.0;
            elip.Width = elip.Height;
            elip.Fill = new SolidColorBrush(randColor());
            if (shadowOn)
            {
                DropShadowEffect dse = new DropShadowEffect();
                dse.Color = Colors.Black;
                dse.Direction = -45;
                dse.ShadowDepth = 50;
                dse.Opacity = 0.8;
                dse.BlurRadius = 30;
                elip.Effect = dse;
            }
            Canvas.SetTop(elip, randD * CurrentCanvas().Height);
            Canvas.SetLeft(elip, randD * CurrentCanvas().Width);
            CurrentCanvas().Children.Add(elip);
        }

        Color colorMix(Color c1, Color c2, float v)
        {
            Color c = new Color();
            c.ScA = 1f;
            c.ScR = c1.ScR * v + c2.ScR * (1f - v);
            c.ScG = c1.ScG * v + c2.ScG * (1f - v);
            c.ScB = c1.ScB * v + c2.ScB * (1f - v);
            return c;
        }

        static double distance(Point p1,Point p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        Point randPoint()
        {
            return new Point(randD*2.0-1.0, randD*2.0-1.0);
        }

        Point randPoint(double d)
        {
            return new Point(randD*d*2-(d), randD*d*2-d);
        }



        Color colorMixPlus(Color c1, Color c2, Color c3, Color c4, Point p)
        {
            Point p1 = new Point(1, 1);
            Point p2 = new Point(-1, 1);
            Point p3 = new Point(1, -1);
            Point p4 = new Point(-1, -1);
            float d1 = (float)distance(p, p1);
            float d2 = (float)distance(p, p2);
            float d3 = (float)distance(p, p3);
            float d4 = (float)distance(p, p4);
            float dt = d1 + d2 + d3 + d4;

            Color c = new Color();
            c.ScA = 1.0f;
            c.ScR = (c1.ScR * d1 + c2.ScR * d2 + c3.ScR * d3 + c4.ScR * d4) / dt;
            c.ScG = (c1.ScG * d1 + c2.ScG * d2 + c3.ScG * d3 + c4.ScG * d4) / dt;
            c.ScB = (c1.ScB * d1 + c2.ScB * d2 + c3.ScB * d3 + c4.ScB * d4) / dt;
            return c;
        }


        Color colorMixPlus(Color c1, Color c2, Color c3, Color c4, Point p, float alfa)
        {
            Point p1 = new Point(1, 1);
            Point p2 = new Point(-1, 1);
            Point p3 = new Point(1, -1);
            Point p4 = new Point(-1, -1);
            float d1 = (float)distance(p, p1);
            float d2 = (float)distance(p, p2);
            float d3 = (float)distance(p, p3);
            float d4 = (float)distance(p, p4);
            float dt = d1 + d2 + d3 + d4;

            Color c = new Color();
            c.ScA = alfa;
            c.ScR = (c1.ScR * d1 + c2.ScR * d2 + c3.ScR * d3 + c4.ScR * d4) / dt;
            c.ScG = (c1.ScG * d1 + c2.ScG * d2 + c3.ScG * d3 + c4.ScG * d4) / dt;
            c.ScB = (c1.ScB * d1 + c2.ScB * d2 + c3.ScB * d3 + c4.ScB * d4) / dt;
            return c;
        }


        void relabelButtons()
        {
            sideButton[0].Content = "Circle";
            sideButton[1].Content = "10 Circles";
            sideButton[2].Content = "Poly";
            sideButton[3].Content = "RandPoly";
            sideButton[4].Content = "Flower";
            sideButton[5].Content = "Flower+";
            sideButton[6].Content = "Flower+Edge";
            sideButton[7].Content = "Flower+Edge2";
            sideButton[8].Content = "OpenImg";
            sideButton[9].Content = "Reflect";
            sideButton[10].Content = "GPUstress";

            toolButton[0].Content = "Clear";
            toolButton[1].Content = "CanvasColor";
            toolButton[2].Content = "RandCanvasColor";
            toolButton[18].Content = "S-Off";
            toolButton[19].Content = "S-On";
            toolButton[20].Content = "SetApColor";
        }
        void b_Click(object sender, RoutedEventArgs e)
        {
            Button curButton = (Button)sender;
            string bText = (string)curButton.Content;
            switch (bText)
            {
                case "Poly":
                    int sides = 13;
                    Point center = new Point(3000.0, 4000.0);
                    double radius = 5000.0;
                    int skip = 7;
                    double startAngle = 0.0;
                    Brush fillBrush = Brushes.Blue;

                    Polygon p = MakePoly(sides, ref center, radius, skip, startAngle, fillBrush);

                    CurrentCanvas().Children.Add(p);
                    if (isPoly == false)
                    {
                        isPoly = true;
                    }
                    break;

                case "RandPoly":
                    sides = 13;
                    center = new Point(400.0, 400.0);
                    radius = 100.0;
                    //int skip = 7;
                    startAngle = 0.0;
                    fillBrush = Brushes.Blue;

                    radius = randD * 3000.0;
                    center = new Point(randD * CurrentCanvas().Width, randD * CurrentCanvas().Height);

                    for (int skipX = 1; skipX < (sides / 2); skipX++)
                    {
                        fillBrush = RandSolidBrush();
                        Polygon p2 = MakePoly(sides, ref center, radius, skipX, startAngle, fillBrush);

                        CurrentCanvas().Children.Add(p2);
                    }
                    break;
                case "Flower":
                    sides = rand.Next(7, 23);
                    startAngle = Math.PI / 2.0;
                    CurrentCanvas().Children.Clear();

                    //radius = canvasCenterPoint.X;
                    center = canvasCenterPoint;
                    double offset = 2000.0;
                    center.X += randD * offset - randD * offset;
                    center.Y += randD * offset - randD * offset;
                    radius = Math.Sqrt(canvasCenterPoint.X * canvasCenterPoint.X + canvasCenterPoint.Y * canvasCenterPoint.Y)+offset;
                    while (radius > 40.0)
                    {
                        for (int skipX = 1; skipX < (sides / 2); skipX++)
                        {
                            fillBrush = RandSolidBrush();
                            Polygon p2 = MakePoly(sides, ref center, radius, skipX, startAngle, fillBrush);

                            CurrentCanvas().Children.Add(p2);
                        }
                        radius /= 2.0;
                    }
                    break;

                case "Flower+":
                    Color c1 = randColor();
                    Color c2 = randColor();
                    sides = (int)Math.Sqrt(rand.Next(7 * 7, 30 * 30));
                    startAngle = -(Math.PI / 2.0);
                    CurrentCanvas().Children.Clear();

                    CurrentCanvas().Background = new SolidColorBrush(colorMix(c1, c2, (float)randD));


                    radius = Math.Sqrt(canvasCenterPoint.X * canvasCenterPoint.X + canvasCenterPoint.Y * canvasCenterPoint.Y);
                    center = canvasCenterPoint;
                    while (radius > 40.0)
                    {
                        for (int skipX = 1; skipX < (sides / 2); skipX++)
                        {
                            fillBrush = new SolidColorBrush(colorMix(c1, c2, (float)randD));
                            Polygon p2 = MakePoly(sides, ref center, radius, skipX, startAngle, fillBrush);

                            CurrentCanvas().Children.Add(p2);
                        }
                        radius /= 2.0;
                    }
                    break;
                case "Flower+Edge":
                    c1 = randColor();
                    c2 = randColor();
                    Color c3 = randColor();
                    Color c4 = randColor();
                    sides = (int)Math.Sqrt(rand.Next(7 * 7, 30 * 30));
                    startAngle = -(Math.PI / 2.0);
                    CurrentCanvas().Children.Clear();

                    CurrentCanvas().Background = new SolidColorBrush(colorMix(c1, c2, (float)randD));

                    Brush edgeBrush;

                    //radius = Math.Sqrt(canvasCenterPoint.X * canvasCenterPoint.X + canvasCenterPoint.Y * canvasCenterPoint.Y);
                    center = canvasCenterPoint;
                    offset = 2000.0;
                    center.X += randD * offset - randD * offset;
                    center.Y += randD * offset - randD * offset;
                    radius = Math.Sqrt(canvasCenterPoint.X * canvasCenterPoint.X + canvasCenterPoint.Y * canvasCenterPoint.Y)+offset;
                    while (radius > 40.0)
                    {
                        for (int skipX = 1; skipX < (sides / 2); skipX++)
                        {
                            fillBrush = new SolidColorBrush(colorMix(c1, c2, (float)randD));
                            edgeBrush = new SolidColorBrush(colorMix(c3, c4, (float)randD));
                            Polygon p2 = MakePoly(sides, ref center, radius, skipX, startAngle, fillBrush, edgeBrush);

                            CurrentCanvas().Children.Add(p2);
                        }
                        radius /= 2.0;
                    }
                    break;

                case "OpenImg":
                    CurrentCanvas().Children.Clear();
                    BitmapImage theImage = new BitmapImage(new Uri(@"../../nerd.gif", UriKind.Relative));            
                    ImageBrush myImageBrush = new ImageBrush(theImage);
                    CurrentCanvas().Background = myImageBrush;
                    break;
                case "Reflect":
                    if (isPoly == true)
                    {
                        int sidesRef = 13;
                        Point centerRef = new Point(ref_X, ref_Y);
                        double radiusRef = 5000.0;
                        int skipRef = 7;
                        double startAngleRef = 0.0;
                        Brush fillBrushRef = Brushes.Blue;

                        Polygon Ref = MakePoly(sidesRef, ref centerRef, radiusRef, skipRef, startAngleRef, fillBrushRef);

                        CurrentCanvas().Children.Add(Ref);
                    }
                    

                    break;
                case "GPUstress":
                   double _secondDuration = 0.0;
                   for (int i = 0; i < 10000; i++)
                    {
                        
                        int sides3 = 13;
                        Point center3 = new Point(3000.0, 4000.0);
                        double radius3 = 5000.0;
                        int skip3 = 7;
                        double startAngle3 = 0.0;
                        Brush fillBrush3 = Brushes.Blue;

                        Polygon p3 = MakePoly(sides3, ref center3, radius3, skip3, startAngle3, fillBrush3);

                        CurrentCanvas().Children.Add(p3);
                        sides = 13;
                        center = new Point(400.0, 400.0);
                        radius = 100.0;
                        //int skip = 7;
                        startAngle = 0.0;
                        fillBrush = Brushes.Blue;

                        radius = randD * 3000.0;
                        center = new Point(randD * CurrentCanvas().Width, randD * CurrentCanvas().Height);

                        for (int skipX = 1; skipX < (sides / 2); skipX++)
                        {
                            fillBrush = RandSolidBrush();
                            Polygon p2 = MakePoly(sides, ref center, radius, skipX, startAngle, fillBrush);

                            CurrentCanvas().Children.Add(p2);
                        }
                        sides = rand.Next(7, 23);
                        startAngle = Math.PI / 2.0;
                        CurrentCanvas().Children.Clear();

                        //radius = canvasCenterPoint.X;
                        center = canvasCenterPoint;
                        double offset3 = 2000.0;
                        center.X += randD * offset3 - randD * offset3;
                        center.Y += randD * offset3 - randD * offset3;
                        radius = Math.Sqrt(canvasCenterPoint.X * canvasCenterPoint.X + canvasCenterPoint.Y * canvasCenterPoint.Y) + offset3;
                        while (radius > 40.0)
                        {
                            for (int skipX = 1; skipX < (sides / 2); skipX++)
                            {
                                fillBrush = RandSolidBrush();
                                Polygon p2 = MakePoly(sides, ref center, radius, skipX, startAngle, fillBrush);

                                CurrentCanvas().Children.Add(p2);
                            }
                            radius /= 2.0;
                        }
                        _secondDuration += .008;


                    }

                   _secondDuration /= 5.6;
                   Math.Round(_secondDuration, 3);
                    string testResult = "it took your GPU ";
                     testResult += _secondDuration.ToString();
                     testResult +=" seconds to process 10,000 random images!!";
                    MessageBoxResult result = MessageBox.Show(testResult, "GPU Results");
                    break;



                case "Flower+Edge2":
                    //c1 = randColor();
                    //c2 = randColor();
                    //c3 = randColor();
                    //c4 = randColor();
                    c1 = Colors.Red;
                    c2 = Colors.Blue;
                    c3 = Colors.Green;
                    c4 = Colors.Blue;
                    //Point colorP1 = randPoint(0.5);
                    //Point colorP2 = randPoint();
                    //Point colorP1 = new Point(1, 1);
                    //Point colorP2 = randPoint();
                    sides = (int)Math.Sqrt(rand.Next(7 * 7, 30 * 30));
                    startAngle = -(Math.PI / 2.0);
                    CurrentCanvas().Children.Clear();

                    CurrentCanvas().Background = new SolidColorBrush(colorMix(c1, c2, (float)randD));

                    
                    //radius = Math.Sqrt(canvasCenterPoint.X * canvasCenterPoint.X + canvasCenterPoint.Y * canvasCenterPoint.Y);
                    center = canvasCenterPoint;

                    offset = 2000.0;
                    center.X += randD * offset - randD * offset;
                    center.Y += randD * offset - randD * offset;
                    radius = Math.Sqrt(canvasCenterPoint.X * canvasCenterPoint.X + canvasCenterPoint.Y * canvasCenterPoint.Y)+offset;
                    while (radius > 40.0)
                    {
                        c4 = randColor();
                        for (int skipX = 1; skipX < (sides / 2); skipX++)
                        {
                            Point colorP1 = randPoint(3);
                            fillBrush = new SolidColorBrush(colorMixPlus(c1, c2, c3, c4, colorP1, 0.9f));
                            edgeBrush = new SolidColorBrush(colorMixPlus(c4, c3, c2, c1, colorP1, 0.9f));
                            //edgeBrush = new SolidColorBrush(colorMix(c3, c4, (float)randD));
                            Polygon p2 = MakePoly(sides, ref center, radius, skipX, startAngle, fillBrush, edgeBrush);

                            CurrentCanvas().Children.Add(p2);
                        }
                        radius /= 2.0;
                    }
                    break;

                case "S-Off":
                    shadowOn = false;
                    break;
                case "S-On":
                    shadowOn = true;
                    break;
                case "CanvasColor":
                    Color c = GetColorFromDialog();
                    if (c != Colors.Transparent) CurrentCanvas().Background = new SolidColorBrush(c);
                    break;
                case "RandCanvasColor":
                    CurrentCanvas().Background = RandSolidBrush();
                    break;
                case "Clear":
                    CurrentCanvas().Children.Clear();
                    break;
                case "Circle":
                    AddRandomCircle();
                    break;
                case "10 Circles":
                    for (int i = 0; i < 10; i++)
                    {
                        AddRandomCircle();
                    }
                    break;
                case "SetApColor":
                    apBaseColor = GetColorFromDialog();
                    rootGrid.Children.Remove(rightTabControl);
                    SetupLayout();
                    break;
            }
        }




    } //end class
} //end namespace
