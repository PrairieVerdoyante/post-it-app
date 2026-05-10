using post_it_app.textHandling;
using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace post_it_app
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                PostItLibrary.initialiseDatabase();

                var postIts = PostItLibrary.getAll();

                foreach (PostIt pit in postIts)
                {
                    if (pit.Text == "")
                    {
                        PostItLibrary.deletePostIt(pit.Id);
                    } else
                    {
                        Add_new_postit(pit.Text, pit.Id, pit.PosX, pit.PosY);
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // todo max 36 postits
        private void Add_new_postit(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Canvas)
            {
                Point position = e.GetPosition(this);

                Add_new_postit(posX:position.X,posY:position.Y);
            }
        }

        private void Add_new_postit(string name = "", int id = 0, double posX=0, double posY=0)
        {

            try
            {
                // add in db only if id isnt defined.
                if (id == 0)
                {
                    // ajouter nouveau postit
                    id = PostItLibrary.storeNew(name, posX, posY);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Border postItBorder = new Border
            {
                Width = 130,
                Height = 130,
                Background = Brushes.Yellow,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(4),
                CornerRadius = new CornerRadius(4),
                Tag = id
            };

            TextBox tb = new TextBox
            {
                TextWrapping = TextWrapping.Wrap,
                Background = Brushes.Yellow,
                BorderBrush = Brushes.Yellow,
                Height = 120,
                Width = 120,
                FontSize = 16,
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Text = name.ToString(),
                // associate id with db
                Tag = id
            };

            postItBorder.Child = tb;

            // wpPostIts

            Canvas.SetLeft(postItBorder, posX);
            Canvas.SetTop(postItBorder, posY);

            canvas.Children.Add(postItBorder);

            // wpPostIts.Children.Add(tb);
            tb.TextChanged += TextBox_Update;

            postItBorder.MouseLeftButtonDown += CanvasMouseLeftButtonDown;
            postItBorder.MouseLeftButtonUp += CanvasMouseLeftButtonUp;
            postItBorder.MouseMove += CanvasMouseMove;
            
        }

        private Dictionary<TextBox, DispatcherTimer> saveTimers = new Dictionary<TextBox, DispatcherTimer>();

        private void TextBox_Update(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            int id = (int)tb.Tag;


            if (tb == null)
                return;

            if (!(tb.Tag is int))
                return;


            // introduce timer to avoid bd overload
            if (!saveTimers.ContainsKey(tb))
            {
                var timer = new DispatcherTimer
                {
                    // every 500ms
                    Interval = TimeSpan.FromMilliseconds(500)
                };

                timer.Tick += (s, args) =>
                {
                    timer.Stop();

                    try
                    {
                        // position handling
                        var border = tb.Parent as Border;
                        double pX = 0;
                        double pY = 0;
                        if (border != null)
                        {
                            pX = Canvas.GetLeft(border);
                            if (double.IsNaN(pX)) pX = 0;
                            pY = Canvas.GetTop(border);
                            if (double.IsNaN(pY)) pY = 0;
                        }
                        
                        PostItLibrary.editPostIt(id, tb.Text, pX, pY);

                        // delete postit if empty
                        if (tb.Text == "")
                        {
                            PostItLibrary.deletePostIt(id);

                            if (border != null)
                            {
                                canvas.Children.Remove(border);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                };

                saveTimers[tb] = timer;
            }

            saveTimers[tb].Stop();
            saveTimers[tb].Start();
        }

        private Point mousePosition;
        private Border draggedBorder;
        private void CanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (sender is Border border)
            {
                draggedBorder = border;
                mousePosition = e.GetPosition(canvas);

                border.CaptureMouse();
                Panel.SetZIndex(border, 100);
            }
        }
        private void CanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (draggedBorder != null)
            {
                Point position = e.GetPosition(canvas);

                double offsetX = position.X - mousePosition.X;
                double offsetY = position.Y - mousePosition.Y;

                double left = Canvas.GetLeft(draggedBorder);
                double top = Canvas.GetTop(draggedBorder);

                if (double.IsNaN(left)) left = 0;
                if (double.IsNaN(top)) top = 0;

                Canvas.SetLeft(draggedBorder, left + offsetX);
                Canvas.SetTop(draggedBorder, top + offsetY);

                mousePosition = position;

                TextBox tb = draggedBorder.Child as TextBox;

                if (tb != null && tb.Tag is int id)
                {
                    // Exemple d'appel avec texte et position
                    double posX = Canvas.GetLeft(draggedBorder);
                    double posY = Canvas.GetTop(draggedBorder);
                    PostItLibrary.editPostIt(id, tb.Text, posX, posY);
                }

                draggedBorder.ReleaseMouseCapture();
                Panel.SetZIndex(draggedBorder, 0);
                draggedBorder = null;
            }
            
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (draggedBorder != null && draggedBorder.IsMouseCaptured)
            {
                Point position = e.GetPosition(canvas);

                double offsetX = position.X - mousePosition.X;
                double offsetY = position.Y - mousePosition.Y;

                double left = Canvas.GetLeft(draggedBorder);
                double top = Canvas.GetTop(draggedBorder);

                if (double.IsNaN(left)) left = 0;
                if (double.IsNaN(top)) top = 0;

                Canvas.SetLeft(draggedBorder, left + offsetX);
                Canvas.SetTop(draggedBorder, top + offsetY);

                mousePosition = position;
            }
            
        }

    }
    }

