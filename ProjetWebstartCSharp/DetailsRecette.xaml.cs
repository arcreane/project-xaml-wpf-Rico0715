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
    public partial class DetailsRecette : Window
    {
        private int _recetteId;

        public DetailsRecette(Recette recette)
        {
            InitializeComponent();
            _recetteId = recette.Id; // Stocke l'ID de la recette actuelle

            AfficherDetailsRecette(recette);
        }

        private void AfficherDetailsRecette(Recette recette)
        {
            
            txtNomRecette.Text = recette.Nom;

            txtDescriptionRecette.Text = recette.Description;



            List<string> ingredientsAvecQuantites = new List<string>();
            for (int i = 0; i < recette.Ingredients.Count; i++)
            {
                string ingredient = recette.Ingredients[i];
                int quantite = recette.Quantites[i];
                string ingredientAvecQuantite = $"{ingredient} - {quantite}";
                ingredientsAvecQuantites.Add(ingredientAvecQuantite);
            }

           
            listBoxIngredients.ItemsSource = ingredientsAvecQuantites;
           
            listBoxIngredients.ItemsSource = recette.Ingredients;

            
            float prixTotal = CalculerPrixTotal(recette.Id);
            txtPrixTotal.Text = prixTotal.ToString("C2");
        }

        private float CalculerPrixTotal(int recetteId)
        {
            float prixTotal = 0;

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MaConnexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT SUM(PrixTotal) AS PrixTotalRecette
                        FROM RecetteIngredients
                        WHERE RecetteId = @RecetteId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@RecetteId", recetteId);
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        prixTotal = Convert.ToSingle(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du calcul du prix total : " + ex.Message);
            }

            return prixTotal;
        }

        private void MettreAJourStock(int recetteId)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MaConnexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string updateStockQuery = @"
                        UPDATE I
SET I.Quantite = I.Quantite - RI.Quantite
FROM Ingredients AS I
JOIN RecetteIngredients AS RI ON I.Id = RI.IngredientId
WHERE RI.RecetteId = @RecetteId;";

                    SqlCommand updateStockCommand = new SqlCommand(updateStockQuery, connection);
                    updateStockCommand.Parameters.AddWithValue("@RecetteId", recetteId);

                    updateStockCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la mise à jour du stock : " + ex.Message);
            }
        }

        private void ConsommerRecette_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Utilisez l'ID de la recette stockée pour mettre à jour le stock
                MettreAJourStock(_recetteId);
                MessageBox.Show("Stock mis à jour avec succès !");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la consommation de la recette : " + ex.Message);
            }
        }
    }
}
