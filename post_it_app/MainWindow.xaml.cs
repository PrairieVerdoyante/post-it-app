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

                var postIts = PostItLibrary.getAll();

                
                foreach (PostIt pit in postIts)
                {
                    Add_new_postit(pit.Text, pit.Id);
                }
                

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Add_new_postit(object sender, RoutedEventArgs e)
        {
            Add_new_postit();
        }
        private void Add_new_postit(string name="", int id=0)
        {
            
            // get db last inserted id
            try
            {
                // TODO: position non mise à jour.

                // add in db only if id isnt defined.
                if (id == 0)
                {
                    // ajouter nouveau postit
                    id = PostItLibrary.storeNew(name, 0, 0);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            TextBox tb = new TextBox
            {
                Background = Brushes.Yellow,
                BorderBrush = Brushes.Black,
                Height = 120,
                Width = 120,
                FontSize = 16,
                Margin = new Thickness(5),
                Text = name.ToString(),
                // associate id with db
                Tag = id
            };
            wpPostIts.Children.Add(tb);
            // text changed
            tb.TextChanged += TextBox_Update;

        }

        private void TextBox_Update(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                string content = tb.Text;

                PostItLibrary.editPostIt((int)tb.Tag, content);
            }
        }        
   }

}

