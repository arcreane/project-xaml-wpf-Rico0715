using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace ProjetWebstartCSharp
{
    public partial class SaisieRecette : Window
    {
        private DataTable IngredientsTable { get; set; }
        private Dictionary<string, int> QuantitesSelectionnees { get; set; }
        private Dictionary<string, int> selectedIngredients = new Dictionary<string, int>();

        public SaisieRecette()
        {
            InitializeComponent();
            ChargerIngredients();
            WindowState = WindowState.Maximized;
        }

        private void ChargerIngredients()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MaConnexion"].ConnectionString;
            string query = "SELECT NomIngre, Unite FROM Ingredients";

            IngredientsTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                try
                {
                    connection.Open();
                    adapter.Fill(IngredientsTable);

                    List<Ingredient> ingredients = new List<Ingredient>();
                    foreach (DataRow row in IngredientsTable.Rows)
                    {
                        ingredients.Add(new Ingredient
                        {
                            NomIngredient = row["NomIngre"].ToString(),
                            Unite = row["Unite"].ToString(),
                            Quantite = 0
                        });
                    }
                    ingredients.Sort((x, y) => string.Compare(x.NomIngredient, y.NomIngredient));

                    listBoxIngredients.ItemsSource = ingredients;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors du chargement des ingrédients : " + ex.Message);
                }
            }
        }

        

        private void ButtonEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MaConnexion"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    
                    string insertRecetteQuery = "INSERT INTO Recettes (Nom, Description, TempsPreparation) VALUES (@Nom, @Description, @TempsPreparation); SELECT SCOPE_IDENTITY();";
                    SqlCommand insertRecetteCommand = new SqlCommand(insertRecetteQuery, connection);
                    insertRecetteCommand.Parameters.AddWithValue("@Nom", textBoxNomRecette.Text);
                    insertRecetteCommand.Parameters.AddWithValue("@Description", textBoxDescription.Text);
                    insertRecetteCommand.Parameters.AddWithValue("@TempsPreparation", int.Parse(textBoxTempsPreparation.Text));
                    int recetteId = Convert.ToInt32(insertRecetteCommand.ExecuteScalar());

                    foreach (var ingredient in (List<Ingredient>)listBoxIngredients.ItemsSource)
                    {
                        if (ingredient.Quantite > 0)
                        {
                            string insertIngredientQuery = @"
                                INSERT INTO RecetteIngredients (RecetteId, IngredientId, Quantite) 
                                VALUES (@RecetteId, (SELECT Id FROM Ingredients WHERE NomIngre = @NomIngre), @Quantite);";
                            SqlCommand insertIngredientCommand = new SqlCommand(insertIngredientQuery, connection);
                            insertIngredientCommand.Parameters.AddWithValue("@RecetteId", recetteId);
                            insertIngredientCommand.Parameters.AddWithValue("@NomIngre", ingredient.NomIngredient);
                            insertIngredientCommand.Parameters.AddWithValue("@Quantite", ingredient.Quantite);
                            insertIngredientCommand.ExecuteNonQuery();
                        }
                    }
                    MettreAJourStock(recetteId);


                    MessageBox.Show("Recette enregistrée avec succès !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'enregistrement de la recette : " + ex.Message);
            }
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

        private void textBoxDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            
            textBox.TextChanged -= textBoxDescription_TextChanged;

            
            var lines = textBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].StartsWith("-"))
                {
                    lines[i] = "-" + lines[i];
                }
            }

          
            textBox.Text = string.Join(Environment.NewLine, lines);

           
            textBox.SelectionStart = textBox.Text.Length;

            
            textBox.TextChanged += textBoxDescription_TextChanged;
        }


private void TextBox_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = textBoxSearch.Text.Trim().ToLower();

           
            for (int i = 0; i < listBoxIngredients.Items.Count; i++)
            {
                var ingredient = listBoxIngredients.Items[i] as Ingredient;
                if (ingredient != null && ingredient.NomIngredient.ToLower().Contains(searchText))
                {
                    // Sélection de l'élément correspondant
                    listBoxIngredients.SelectedItem = ingredient;
                    listBoxIngredients.ScrollIntoView(ingredient); 
                    break; 
                }
            }
        }
        private void textBoxTempsPreparation_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void listBoxIngredients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }
    }

    
}
