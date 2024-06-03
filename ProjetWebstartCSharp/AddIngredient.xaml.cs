using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class AddIngredient : Window
    {
        public AddIngredient()
        {
            InitializeComponent();
        }

        private void ButtonAjouter_Click(object sender, RoutedEventArgs e)
        {
            string nom = textBoxNom.Text.Trim();
            if (!int.TryParse(textBoxQuantite.Text.Trim(), out int quantite))
            {
                MessageBox.Show("Veuillez entrer une quantité valide.");
                return;
            }
            if (!decimal.TryParse(textBoxPrix.Text.Trim(), out decimal prix))
            {
                MessageBox.Show("Veuillez entrer un prix valide.");
                return;
            }
            string unite = (comboBoxUnite.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(unite))
            {
                MessageBox.Show("Veuillez sélectionner une unité.");
                return;
            }

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MaConnexion"].ConnectionString;
                string query = "INSERT INTO Ingredients (NomIngre, Quantite, Prix, Unite) VALUES (@Nom, @Quantite, @Prix, @Unite)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Nom", nom);
                    command.Parameters.AddWithValue("@Quantite", quantite);
                    command.Parameters.AddWithValue("@Prix", prix);
                    command.Parameters.AddWithValue("@Unite", unite);

                    connection.Open();
                    command.ExecuteNonQuery();

                    MessageBox.Show("Ingrédient ajouté avec succès !");
                    this.Close(); // Ferme la fenêtre après l'ajout
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'ajout de l'ingrédient : " + ex.Message);
            }
        }

        private void TextBoxPrix_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Autoriser uniquement les chiffres et le point décimal
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void TextBoxPrix_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static bool IsTextAllowed(string text)
        {
            // Vérifier si le texte contient uniquement des chiffres et des points décimaux
            Regex regex = new Regex("[^0-9.-]+"); // Autorise uniquement les chiffres, le point et le tiret
            return !regex.IsMatch(text);
        }
    }
}
