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

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        Button btAdd;

        private void Add_new_postit(Object sender, RoutedEventArgs e)
        {
            int id = 1;

            TextBox tb = new TextBox
            {
                Background = Brushes.Yellow,
                BorderBrush = Brushes.Black,
                Height = 100,
                Width = 100,
                Margin = new Thickness(5)
            };

            // get db last inserted id
            try
            {
                // TODO: position non mise à jour.
                id = PostItLibrary.storeNew("", 0, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // associate db id with tag
            tb.Tag = id;

            wpPostIts.Children.Add(tb);
            tb.TextChanged += TextBox_Update;

        }

        private void updatePostIt()
        {

        }

        private void textBox_Clicked(object sender, MouseButtonEventArgs e)
        {
            // select a post it and get its id.
            /*
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                string content = tb.Text.ToString();
            }*/
        }

        /***
         * focus loss: we should be able to click on the window to go out of focus (TODO)
         * 
         */
        private void TextBox_Update(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                string content = tb.Text.ToString();

                PostItLibrary.editPostIt(1, content);
            }
        }

        

        
   }

    // find a button by name

}

