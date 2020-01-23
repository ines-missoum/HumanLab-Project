using humanlab.DAL;
using humanlab.Helpers.Models;
using humanlab.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace humanlab.ViewModels
{
    class CategoriesManagementViewModel : BaseViewModel
    {
        /*** PUBLIC ATTRIBUTES ***/

        private IEnumerable<IGrouping<string, CategoryOrdered>> categoriesForView;

        private string newCategoryName;

        public DelegateCommand SaveNewCategoryDelegate { get; set; }

        private string errorNameMessage;

        /*** PRIVATE ATTRIBUTES ***/

        /// <summary>
        /// In charge of the interaction  of the database
        /// </summary>
        private CategoryRepository repository;

        private List<CategoryOrdered> dbCategories;

        /*** CONSTRUCTOR ***/

        public CategoriesManagementViewModel()
        {
            repository = new CategoryRepository();
            InitialiseCategories();
            newCategoryName = "";
            SaveNewCategoryDelegate = new DelegateCommand(SaveNewCategoryAsync, CanSaveNewCategory);
        }

        /*** GETTERS AND SETTERS FOR PUBLIC ATTRIBUTES ***/

        public IEnumerable<IGrouping<string, CategoryOrdered>> Categories
        {
            get => categoriesForView;
            set => SetProperty(ref categoriesForView, value, "Categories");
        }

        public string NewCategoryName
        {
            get => newCategoryName;
            set
            {
                SetProperty(ref newCategoryName, value, "NewCategoryName");
                SaveNewCategoryDelegate.RaiseCanExecuteChanged();
            }
        }

        public string ErrorNameMessage
        {
            get => errorNameMessage;
            set => SetProperty(ref errorNameMessage, value, "ErrorNameMessage");
        }

        /*** METHODS ***/

        private void AddCategoryToDbCategories(Category c)
        {
            dbCategories.Add(new CategoryOrdered(c, c.CategoryName.First().ToString().ToUpper()));
        }

        private void UpdateCategoriesForView()
        {
            dbCategories = dbCategories.OrderBy(c => c.Category.CategoryName).ToList();
            Categories = from category in dbCategories group category by category.Key;
        }

        private bool IsExistingCategory(string categoryName)
        {
            return dbCategories.Where(c => c.Category.CategoryName.ToLower().Equals(categoryName.ToLower()))
                                .Count() > 0;
        }
        private async void InitialiseCategories()
        {
            List<Category> categories = await repository.GetAllCategoriesAsync();
            dbCategories = new List<CategoryOrdered>();
            categories.ForEach(c => AddCategoryToDbCategories(c));
            UpdateCategoriesForView();
        }

        /// <summary>
        /// Method called each time the new category name field in the form is changed. We save it here.
        /// </summary>
        /// <param name="sender">the xaml textbox</param>
        /// <param name="args">arguments</param>
        public void TextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            TextBox box = sender as TextBox;
            NewCategoryName = box.Text;
        }

        private async void SaveNewCategoryAsync()
        {
            Category newCategory = new Category { CategoryName = newCategoryName };
            bool wellSaved = repository.SaveCategoryAsync(newCategory);

            //we display the message when process ends and update the view
            MessageDialog messageDialog;
            if (wellSaved)
            {
                AddCategoryToDbCategories(newCategory);
                UpdateCategoriesForView();
                messageDialog = new MessageDialog("La catégorie " + NewCategoryName + " a été créée avec succès.");
            }
            else
                messageDialog = new MessageDialog("Echec de la création de la catégorie " + NewCategoryName + ".");

            await messageDialog.ShowAsync();

        }

        private bool CanSaveNewCategory()
        {
            bool existingCategory = IsExistingCategory(NewCategoryName);
            if (existingCategory)
                ErrorNameMessage = "Cette Catégorie existe déjà.";
            else
                ErrorNameMessage = "";
            return !existingCategory && (NewCategoryName.Length > 0);
        }
    }
}
