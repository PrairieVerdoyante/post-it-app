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
            TextBox tb = new TextBox
            {
                Background = Brushes.Yellow,
                BorderBrush = Brushes.Black,
                Height = 100,
                Width = 100,
                Margin = new Thickness(5)                
            };

            try
            {
                PostItLibrary.storeNew("");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            wpPostIts.Children.Add(tb);

            // TODO: 
            // récupérer la modification
            // récupérer id
            // modifier la bonne table

            /*
            tb.TextChanged += (s, e) =>
            {
                PostItLibrary.storeNew(tb.Text);
            };
            */

        }

        

        
   }

    // find a button by name

}

