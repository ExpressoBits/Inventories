using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ExpressoBits.Inventories
{
    [AddComponentMenu(menuName: "Expresso Bits/Inventories/Crafter")]
    [RequireComponent(typeof(Container))]
    public class Crafter : MonoBehaviour
    {
        /// <summary>
        /// List of possible recipes to craft
        /// </summary>
        public List<Recipe> Recipes => container.Database.Recipes;

        private List<Crafting> craftings = new List<Crafting>();
        private Container container;


        [SerializeField] private bool canCraft = true;

        /// <summary>
        /// Is the crafting list limited?
        /// </summary>
        [SerializeField] private bool isLimitCrafts = true;
        /// <summary>
        /// Maximum number of craftings if limit is used
        /// </summary>
        [SerializeField] private uint craftsLimit = 8;

        public Action<Recipe> OnCrafted;
        public Action<Recipe> OnRequestCraft;
        public Action<Crafting> OnAdd;
        public Action<int> OnRemoveAt;
        public Action<int> OnUpdate;

        public UnityEvent<int> OnUpdateUnityEvent;

        /// <summary>
        /// Is something currently being crafted?
        /// </summary>
        public bool IsCrafting => craftings.Count > 0f;
        public int CountOfCraftings => craftings.Count;
        public Container Container => container;
        public Database Database => container.Database;

        private void Awake()
        {
            container = GetComponent<Container>();
        }

        private void Update()
        {
            if (IsCrafting)
            {
                for (int i = 0; i < craftings.Count; i++)
                {
                    Crafting crafting = craftings[i];
                    crafting.AddTimeElapsed(Time.deltaTime);
                    if (crafting.IsFinished && canCraft)
                    {
                        Recipe recipe = Recipes[crafting.Index];
                        container.AddItem(recipe.Product);
                        OnCrafted?.Invoke(recipe);
                        //containerInteractor.AddOrDropItem(recipes.AllRecipes[crafting.Index].Product);
                        RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        craftings[i] = crafting;
                    }
                }
            }
        }

        public void SetCanCraft(bool canCraft)
        {
            this.canCraft = canCraft;
        }

        /// <summary>
        /// Check if it is possible to create this recipe
        /// It is checked if the crafts limit has been exceeded and then it is checked if the recipe items contain in the container
        /// </summary>
        /// <param name="recipe">Recipe to be checked</param>
        /// <returns>True if the recipe to be crafted is available</returns>
        public bool CanCraft(Recipe recipe)
        {
            if (isLimitCrafts && craftings.Count >= craftsLimit) return false;
            foreach (var items in recipe.RequiredItems)
            {
                if (!container.Has(items.Item, items.Amount)) return false;
            }
            return true;
        }

        private bool UseItems(Recipe recipe)
        {
            foreach (var items in recipe.RequiredItems)
            {
                if (container.RemoveItem(items.Item, items.Amount) > 0) return false;
            }
            return true;
        }

        public Crafting this[int index]
        {
            get { return craftings[index]; }
            set
            {
                craftings[index] = value;
                OnUpdate?.Invoke(index);
                OnUpdateUnityEvent?.Invoke(index);
            }
        }

        public void Add(Crafting crafting)
        {
            craftings.Add(crafting);
            OnAdd?.Invoke(crafting);
        }

        public void RemoveAt(int index)
        {
            craftings.RemoveAt(index);
            OnRemoveAt?.Invoke(index);
        }

        public void Craft(Recipe recipe)
        {
            if (CanCraft(recipe))
            {
                if (UseItems(recipe))
                {
                    Crafting crafting = new Crafting(Recipes.IndexOf(recipe), recipe.TimeForCraft);
                    Add(crafting);
                    OnRequestCraft?.Invoke(recipe);
                }
            }
        }

    }
}

