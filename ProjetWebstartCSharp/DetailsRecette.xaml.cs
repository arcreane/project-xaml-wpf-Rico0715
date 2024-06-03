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
        public DetailsRecette(Recette recette)
        {
            InitializeComponent();
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

    }
}
