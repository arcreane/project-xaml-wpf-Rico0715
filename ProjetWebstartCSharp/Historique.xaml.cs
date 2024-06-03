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
    /// Logique d'interaction pour Historique.xaml
    /// </summary>
    public partial class Historique : Window
    {
        public Historique()
        {
            InitializeComponent();
            ChargerHistorique();
        }


        private void ChargerHistorique()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MaConnexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT DISTINCT R.Nom
                FROM Recettes AS R
                JOIN RecetteIngredients AS RI ON R.Id = RI.RecetteId
                WHERE RI.Quantite > 0";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    List<string> recettesConsommees = new List<string>();

                    while (reader.Read())
                    {
                        string nomRecette = reader["Nom"].ToString();
                        recettesConsommees.Add(nomRecette);
                    }

                    reader.Close();

                    // Mettre à jour l'affichage dans votre interface utilisateur
                    listBoxRecettes.ItemsSource = recettesConsommees;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement de l'historique : " + ex.Message);
            }
        }
    }
}
