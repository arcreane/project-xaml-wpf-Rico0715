using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Windows.Shapes;
using System.Configuration;



namespace ProjetWebstartCSharp
{
    /// <summary>
    /// Logique d'interaction pour MonStock.xaml
    /// </summary>
    public partial class MonStock : Window
    {
        public MonStock()
        {
            InitializeComponent();
            ChargerIngredients();

            
        }
        private void ChargerIngredients()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MaConnexion"].ConnectionString;
            string query = "SELECT * FROM Ingredients"; 

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();
                    adapter.Fill(dataTable);
                   
                    dataGridIngredients.ItemsSource = dataTable.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors du chargement des ingrédients : " + ex.Message);
                }
            }
        }


        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
          
            string searchText = textBoxSearch.Text.Trim().ToLower();

          
            if (!string.IsNullOrEmpty(searchText))
            {
                
               
                (dataGridIngredients.ItemsSource as DataView).RowFilter = $"NomIngre LIKE '*{searchText}*'";
            }
            else
            {
                
                (dataGridIngredients.ItemsSource as DataView).RowFilter = "";
            }
        }

        private void watermarkedTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            watermarkedTxt.Visibility = System.Windows.Visibility.Collapsed;
            textBoxSearch.Visibility = System.Windows.Visibility.Visible;

        }

       

        private void ButtonAddIngredient(object sender, RoutedEventArgs e)
        {
            
            AddIngredient addIngredientWindow = new AddIngredient();
            
            addIngredientWindow.Show();
            
            this.Close();
        }
    }
}
