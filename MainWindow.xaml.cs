using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using POEPart3;

namespace POEPart3
{
    public partial class MainWindow : Window
    {
        public Recipe recipe;
        public List<Recipe> recipes = new List<Recipe>();

        public MainWindow()
        {
            InitializeComponent();

            // Populate the ListBox with recipe names
            UpdateRecipeLists();

            // Initialize a new recipe object
            recipe = new Recipe();
            recipe.CalorieNotification += DisplayCalorieWarning;
        }

        // Helper method to update recipe lists
        private void UpdateRecipeLists()
        {
            LstAllRecipes.ItemsSource = recipes.Select(r => r.RecipeName).ToList();
            LstRecipesToScale.ItemsSource = recipes.Select(r => r.RecipeName).ToList();
            LstRecipesToReset.ItemsSource = recipes.Select(r => r.RecipeName).ToList();
            LstRecipesToClear.ItemsSource = recipes.Select(r => r.RecipeName).ToList();
        }

        // Event handler for Add Ingredient button
        public void AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            string name = TxtIngredientName.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Ingredient name cannot be empty.");
                return;
            }

            if (int.TryParse(TxtQuantity.Text, out int quantity) && int.TryParse(TxtCalories.Text, out int calories))
            {
                string unitOfMeasurement = TxtUnitOfMeasurement.Text;
                recipe.AddIngredients(name, quantity, unitOfMeasurement, calories);

                // Clear the input fields after adding the ingredient
                TxtIngredientName.Clear();
                TxtQuantity.Clear();
                TxtUnitOfMeasurement.Clear();
                TxtCalories.Clear();
            }
            else
            {
                MessageBox.Show("Please enter valid numeric values for Quantity and Calories.");
            }
        }

        // Event handler for Add Step button
        public void AddStep_Click(object sender, RoutedEventArgs e)
        {
            string description = TxtStepDescription.Text;
            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Step description cannot be empty.");
                return;
            }

            recipe.AddSteps(description);

            // Clear the input field after adding the step
            TxtStepDescription.Clear();
        }

        // Event handler for Display Recipe button
        public void DisplayRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtRecipeName.Text))
            {
                MessageBox.Show("Recipe name cannot be empty.");
                return;
            }

            recipe.RecipeName = TxtRecipeName.Text;
            recipe.FoodGroup = CBFilterFoodGroup.SelectedItem?.ToString(); // Assuming ComboBox is used for food group

            recipe.SortIngredientsAlphabetically(); // Ensure ingredients are sorted
            string recipeDetails = $"Recipe Name: {recipe.RecipeName}\n";
            recipeDetails += $"Food Group: {recipe.FoodGroup}\n\nIngredients:\n";

            foreach (var ingredient in recipe.Ingredients)
            {
                recipeDetails += $"{ingredient.Name}: {ingredient.Quantity} {ingredient.UnitOfMeasurement}, {ingredient.Calories} calories\n";
            }

            recipeDetails += "\nSteps:\n";
            foreach (var step in recipe.Steps)
            {
                recipeDetails += $"{step.Description}\n";
            }

            // Display the complete recipe details in the text block in the second tab
            TxtRecipeDetails.Text = recipeDetails;

            // Add the new recipe to the list and update the ListBox
            recipes.Add(recipe);
            UpdateRecipeLists();

            // Notify the user that the recipe has been added
            MessageBox.Show("Recipe has been added successfully!");

            // Reinitialize the recipe object
            recipe = new Recipe();
            recipe.CalorieNotification += DisplayCalorieWarning;
        }


        // Event handler for ListBox selection changed
        public void LstAllRecipes_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (LstAllRecipes.SelectedItem != null)
            {
                string selectedRecipeName = LstAllRecipes.SelectedItem.ToString();
                DisplayRecipeDetails(selectedRecipeName);
            }
        }

        // Event handler for Display Specific Recipe button
        public void DisplaySpecificRecipe_Click(object sender, RoutedEventArgs e)
        {
            string recipeName = TxtDisplayName.Text;
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                MessageBox.Show("Please enter a recipe name to display.");
                return;
            }

            DisplayRecipeDetails(recipeName);
        }

        // Method to display recipe details
        public void DisplayRecipeDetails(string recipeName)
        {
            var recipe = recipes.FirstOrDefault(r => r.RecipeName == recipeName);
            if (recipe != null)
            {
                string recipeDetails = $"Recipe Name: {recipe.RecipeName}\n\nIngredients:\n";
                foreach (var ingredient in recipe.Ingredients)
                {
                    recipeDetails += $"{ingredient.Name}: {ingredient.Quantity} {ingredient.UnitOfMeasurement}, {ingredient.Calories} calories\n";
                }

                recipeDetails += "\nSteps:\n";
                foreach (var step in recipe.Steps)
                {
                    recipeDetails += $"{step.Description}\n";
                }

                TxtRecipeDetails.Text = recipeDetails;
            }
            else
            {
                MessageBox.Show("Recipe not found.");
            }
        }

        // Event handler for Scale Recipes button
        public void ScaleRecipes_Click(object sender, RoutedEventArgs e)
        {
            var selectedRecipes = LstRecipesToScale.SelectedItems.Cast<string>().ToList();
            if (!selectedRecipes.Any())
            {
                MessageBox.Show("Please select at least one recipe to scale.");
                return;
            }

            if (CmbScalingFactor.SelectedItem == null)
            {
                MessageBox.Show("Please select a scaling factor.");
                return;
            }

            double scalingFactor = Convert.ToDouble((CmbScalingFactor.SelectedItem as ComboBoxItem).Content);
            string scaledRecipeDetails = "";

            foreach (var recipeName in selectedRecipes)
            {
                var recipe = recipes.FirstOrDefault(r => r.RecipeName == recipeName);
                if (recipe != null)
                {
                    scaledRecipeDetails += $"Recipe Name: {recipe.RecipeName} (Scaled by {scalingFactor})\n\nIngredients:\n";
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        scaledRecipeDetails += $"{ingredient.Name}: {ingredient.Quantity * scalingFactor} {ingredient.UnitOfMeasurement}, {ingredient.Calories * scalingFactor} calories\n";
                    }

                    scaledRecipeDetails += "\nSteps:\n";
                    foreach (var step in recipe.Steps)
                    {
                        scaledRecipeDetails += $"{step.Description}\n";
                    }

                    scaledRecipeDetails += "\n";
                }
            }

            TxtScaledRecipes.Text = scaledRecipeDetails;
        }

        // Event handler for Reset Recipes button
        public void ResetRecipes_Click(object sender, RoutedEventArgs e)
        {
            var selectedRecipes = LstRecipesToReset.SelectedItems.Cast<string>().ToList();
            if (!selectedRecipes.Any())
            {
                MessageBox.Show("Please select at least one recipe to reset.");
                return;
            }

            string resetRecipeDetails = "";

            foreach (var recipeName in selectedRecipes)
            {
                var recipe = recipes.FirstOrDefault(r => r.RecipeName == recipeName);
                if (recipe != null)
                {
                    recipe.ResetIngredients();
                    resetRecipeDetails += $"Recipe Name: {recipe.RecipeName} (Reset to Original Values)\n\nIngredients:\n";
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        resetRecipeDetails += $"{ingredient.Name}: {ingredient.Quantity} {ingredient.UnitOfMeasurement}, {ingredient.Calories} calories\n";
                    }

                    resetRecipeDetails += "\nSteps:\n";
                    foreach (var step in recipe.Steps)
                    {
                        resetRecipeDetails += $"{step.Description}\n";
                    }

                    resetRecipeDetails += "\n";
                }
            }

            TxtResetRecipes.Text = resetRecipeDetails;
        }

        // Event handler for Clear Recipes button
        public void ClearRecipes_Click(object sender, RoutedEventArgs e)
        {
            var selectedRecipes = LstRecipesToClear.SelectedItems.Cast<string>().ToList();
            if (!selectedRecipes.Any())
            {
                MessageBox.Show("Please select at least one recipe to clear.");
                return;
            }

            string clearedRecipeDetails = "";

            foreach (var recipeName in selectedRecipes)
            {
                var recipe = recipes.FirstOrDefault(r => r.RecipeName == recipeName);
                if (recipe != null)
                {
                    recipes.Remove(recipe);
                    clearedRecipeDetails += $"Recipe Name: {recipe.RecipeName} (Cleared)\n";
                }
            }

            UpdateRecipeLists();
            TxtClearedRecipes.Text = clearedRecipeDetails;
        }

        // Event handler for Apply Filter button
        public void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            string ingredientFilter = TxtIngredientFilter.Text.Trim();
            string foodGroupFilter = (CBFoodGroupFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            int maxCaloriesFilter = 0;

            // Parse the maximum calories filter value
            if (!string.IsNullOrWhiteSpace(TxtMaxCaloriesFilter.Text) && int.TryParse(TxtMaxCaloriesFilter.Text, out int maxCalories))
            {
                maxCaloriesFilter = maxCalories;
            }

            // Filter the recipes based on the user's input
            var filteredRecipes = recipes.Where(r =>
                (string.IsNullOrWhiteSpace(ingredientFilter) || r.Ingredients.Any(i => i.Name.ToLower().Contains(ingredientFilter.ToLower()))) &&
                (foodGroupFilter == "All" || string.IsNullOrWhiteSpace(foodGroupFilter) || r.FoodGroup.ToLower() == foodGroupFilter.ToLower()) &&
                (maxCaloriesFilter == 0 || r.CalorieCount <= maxCaloriesFilter)
            ).ToList();

            // Display the filtered recipes in the TextBlock
            TxtFilteredRecipes.Text = string.Empty;
            if (filteredRecipes.Any())
            {
                string filteredRecipeDetails = "Filtered Recipes:\n\n";
                foreach (var recipe in filteredRecipes)
                {
                    filteredRecipeDetails += $"Recipe Name: {recipe.RecipeName}\n";
                    filteredRecipeDetails += "Ingredients:\n";
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        filteredRecipeDetails += $"{ingredient.Name}: {ingredient.Quantity} {ingredient.UnitOfMeasurement}, {ingredient.Calories} calories\n";
                    }
                    filteredRecipeDetails += "Steps:\n";
                    foreach (var step in recipe.Steps)
                    {
                        filteredRecipeDetails += $"{step.Description}\n";
                    }
                    filteredRecipeDetails += "\n";
                }
                TxtFilteredRecipes.Text = filteredRecipeDetails;
            }
            else
            {
                TxtFilteredRecipes.Text = "No recipes match the filter criteria.";
            }
        }

        // Event handler for Reset Filter button
        public void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            TxtIngredientFilter.Clear();
            CBFoodGroupFilter.SelectedIndex = 0; // Select "All" in ComboBox
            TxtMaxCaloriesFilter.Clear();

            // Clear the filtered results display
            TxtFilteredRecipes.Text = string.Empty;

            // Display all recipes again
            UpdateRecipeLists();
        }

        // Event handler for Clear All Recipes button
        public void ClearAllRecipes_Click(object sender, RoutedEventArgs e)
        {
            recipes.Clear();
            UpdateRecipeLists();

            TxtClearedRecipes.Text = "All recipes cleared.";
        }

        // Method to handle calorie warning
        public void DisplayCalorieWarning(string message)
        {
            MessageBox.Show(message, "Calorie Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
