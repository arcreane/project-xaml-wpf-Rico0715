using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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

namespace ProjetWebstartCSharp
{
    /// <summary>
    /// Logique d'interaction pour MesRecettes.xaml
    /// </summary>
    public partial class MesRecettes : Window
    {
        public MesRecettes()
        {
            InitializeComponent();
            ChargerRecettes();
        }


        private void ChargerRecettes()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MaConnexion"].ConnectionString;
                string query = @"
        SELECT 
            R.Id AS RecetteId,
            R.Nom AS RecetteNom,
            I.NomIngre AS IngredientNom,
            RI.Quantite AS Quantite,
            R.Description AS DescriptionRecette
        FROM 
            Recettes R
        JOIN 
            RecetteIngredients RI ON R.Id = RI.RecetteId
        JOIN 
            Ingredients I ON RI.IngredientId = I.Id;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    var recetteDict = new Dictionary<int, Recette>();

                    while (reader.Read())
                    {
                        int recetteId = (int)reader["RecetteId"];
                        string recetteNom = reader["RecetteNom"].ToString();
                        string ingredientNom = reader["IngredientNom"].ToString();
                        string descriptionRecette = reader["DescriptionRecette"].ToString();
                        int quantite = (int)reader["Quantite"];

                        if (!recetteDict.TryGetValue(recetteId, out Recette recette))
                        {
                            recette = new Recette
                            {
                                Id = recetteId,
                                Nom = recetteNom,
                                Description = descriptionRecette,
                                Ingredients = new List<string>(),
                                Quantites = new List<int>() 
                            };
                            recetteDict[recetteId] = recette;
                        }
                        recette.Ingredients.Add($"{ingredientNom} ({quantite})");
                        recette.Quantites.Add(quantite); 
                    }

                    listBoxRecettes.ItemsSource = recetteDict.Values;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des recettes : " + ex.Message);
            }
        }


        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxRecettes.SelectedItem is Recette selectedRecette)
            {
                DetailsRecette detailsWindow = new DetailsRecette(selectedRecette);
                detailsWindow.Show();
            }
        }

    }
}
