using System;
using System.Collections.Generic;
using System.Linq;

namespace POEPart3
{
    public class Recipe
    {
        public string RecipeName { get; set; }
        public string FoodGroup { get; set; }
        public int CalorieCount => Ingredients.Sum(i => i.Calories);

        public List<Ingredient> Ingredients { get; private set; } = new List<Ingredient>();
        public List<Ingredient> OriginalIngredients { get; private set; } = new List<Ingredient>();
        public List<Step> Steps { get; private set; } = new List<Step>();

        public event Action<string> CalorieNotification;

        public void AddIngredients(string name, int quantity, string unitOfMeasurement, int calories)
        {
            var ingredient = new Ingredient
            {
                Name = name,
                Quantity = quantity,
                UnitOfMeasurement = unitOfMeasurement,
                Calories = calories
            };

            Ingredients.Add(ingredient);

            // Store the original ingredient values
            OriginalIngredients.Add(new Ingredient
            {
                Name = name,
                Quantity = quantity,
                UnitOfMeasurement = unitOfMeasurement,
                Calories = calories
            });

            if (CalorieCount >= 300)
            {
                CalorieNotification?.Invoke("WARNING! \n Total calories exceed 300!");
            }
        }

        public void AddSteps(string description)
        {
            Steps.Add(new Step { Description = description });
        }

        public void SortIngredientsAlphabetically()
        {
            Ingredients = Ingredients.OrderBy(i => i.Name).ToList();
        }

        public void ResetIngredients()
        {
            Ingredients.Clear();
            foreach (var ingredient in OriginalIngredients)
            {
                Ingredients.Add(new Ingredient
                {
                    Name = ingredient.Name,
                    Quantity = ingredient.Quantity,
                    UnitOfMeasurement = ingredient.UnitOfMeasurement,
                    Calories = ingredient.Calories
                });
            }
        }
    }

    public class Ingredient
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string UnitOfMeasurement { get; set; }
        public int Calories { get; set; }
    }

    public class Step
    {
        public string Description { get; set; }
    }
}
