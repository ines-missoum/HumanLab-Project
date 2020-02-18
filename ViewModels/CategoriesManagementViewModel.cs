using humanlab.DAL;
using humanlab.Helpers.Models;
using humanlab.Models;
using humanlab.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace humanlab.ViewModels
{
    class CategoriesManagementViewModel : BaseViewModel
    {
        /*** PUBLIC ATTRIBUTES ***/

        /// <summary>
        /// the list of categories used in the xaml listView
        /// </summary>
        private IEnumerable<IGrouping<string, CategoryOrdered>> categoriesForView;

        /// <summary>
        /// name field in the category creation form 
        /// </summary>
        private string newCategoryName;

        /// <summary>
        /// name field in the category update pop up form 
        /// </summary>
        private string updatedCategoryName;

        /// <summary>
        /// Delegate in charge of creating and saving the new category
        /// </summary>
        public DelegateCommand SaveNewCategoryDelegate { get; set; }

        /// <summary>
        /// Error message of the creation form
        /// </summary>
        private string errorNameMessageCreation;

        /// <summary>
        /// Error message of the creation  pop up form 
        /// </summary>
        private string errorNameMessageUpdate;

        /// <summary>
        /// the category selected in the listView
        /// </summary>
        private CategoryOrdered selectedCategory;

        /*** PRIVATE ATTRIBUTES ***/

        /// <summary>
        /// In charge of the interaction with the database
        /// </summary>
        private CategoryRepository repository;

        /// <summary>
        /// List of the categories of the db with their key added (key = first letter of the name to be able to display them in the alphabetical order)
        /// </summary>
        private List<CategoryOrdered> dbCategories;

        /// <summary>
        /// category update pop up form
        /// </summary>
        private ContentDialog updateCategoryDialog;

        /// <summary>
        /// indicates whether the list of categories is empty
        /// </summary>
        private bool isNoExistingCategory;
        /*** CONSTRUCTOR ***/

        public CategoriesManagementViewModel()
        {
            repository = new CategoryRepository();
            InitialiseCategories();
            newCategoryName = "";
            updatedCategoryName = "";
            SaveNewCategoryDelegate = new DelegateCommand(SaveNewCategoryAsync, CanSaveNewCategory);
            IsNoExistingCategory = dbCategories.Count() == 0;
        }

        /*** GETTERS AND SETTERS FOR PUBLIC ATTRIBUTES ***/

        public IEnumerable<IGrouping<string, CategoryOrdered>> Categories
        {
            get => categoriesForView;
            set => SetProperty(ref categoriesForView, value, "Categories");
        }

        public bool IsNoExistingCategory
        {
            get => isNoExistingCategory;
            set => SetProperty(ref isNoExistingCategory, value, "IsNoExistingCategory");
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

        public string UpdatedCategoryName
        {
            get => updatedCategoryName;
            set
            {
                SetProperty(ref updatedCategoryName, value, "UpdatedCategoryName");
                SaveNewCategoryDelegate.RaiseCanExecuteChanged();
            }
        }

        public string ErrorNameMessageCreation
        {
            get => errorNameMessageCreation;
            set => SetProperty(ref errorNameMessageCreation, value, "ErrorNameMessageCreation");
        }

        public string ErrorNameMessageUpdate
        {
            get => errorNameMessageUpdate;
            set => SetProperty(ref errorNameMessageUpdate, value, "ErrorNameMessageUpdate");
        }

        public CategoryOrdered SelectedCategory
        {
            get => selectedCategory;
            set => SetProperty(ref selectedCategory, value, "SelectedCategory");
        }

        /*** METHODS ***/

        /*** set up methods ***/

        /// <summary>
        /// Retrieves all the categories in the database and set the listView source
        /// </summary>
        private async void InitialiseCategories()
        {
            List<Category> categories = await repository.GetAllCategoriesAsync();
            dbCategories = new List<CategoryOrdered>();
            categories.ForEach(c => AddCategoryToDbCategories(c));
            UpdateCategoriesForView();
        }

        /*** helping methods ***/

        /// <summary>
        /// Add a category in the list of dabase categories
        /// </summary>
        /// <param name="c">category to add </param>
        private void AddCategoryToDbCategories(Category c)
        {
            dbCategories.Add(new CategoryOrdered(c, c.CategoryName.First().ToString().ToUpper()));
        }

        /// <summary>
        /// Rebuilds the listview source from the database categories list
        /// </summary>
        private void UpdateCategoriesForView()
        {
            dbCategories = dbCategories.OrderBy(c => c.Category.CategoryName).ToList();
            Categories = from category in dbCategories group category by category.Key;
        }

        /// <summary>
        /// Checks if a category already exists in database
        /// </summary>
        /// <param name="categoryName">category name to check</param>
        /// <returns>True if the category name already exist in dabase otherwise false</returns>
        private bool IsExistingCategory(string categoryName)
        {
            return dbCategories.Where(c => c.Category.CategoryName.ToLower().Equals(categoryName.ToLower()))
                                .Count() > 0;
        }

        /*** methods related to the category creation ***/

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

        /// <summary>
        /// Save the category saved in database and display a pop up info. 
        /// </summary>
        private async void SaveNewCategoryAsync()
        {
            //we save in db
            Category newCategory = new Category { CategoryName = newCategoryName };
            bool wellSaved = repository.SaveCategoryAsync(newCategory);

            //we display the message when process ends and update the view
            if (wellSaved)
            {
                AddCategoryToDbCategories(newCategory);
                UpdateCategoriesForView();
                DisplayMessagesService.showPersonalizedMessage("La catégorie " + NewCategoryName + " a été créée avec succès.");

                NewCategoryName = "";
                IsNoExistingCategory = dbCategories.Count() == 0;
            }
            else
                DisplayMessagesService.showPersonalizedMessage("Echec de la création de la catégorie " + NewCategoryName + ".");
        }

        /// <summary>
        /// Checks if the name of the category we want to save is not empty and do not already exist in db.
        /// </summary>
        /// <returns>True if we are allowed to save the category, otherwise false.</returns>
        private bool CanSaveNewCategory()
        {
            bool existingCategory = IsExistingCategory(NewCategoryName);
            if (existingCategory)
                ErrorNameMessageCreation = "Cette Catégorie existe déjà.";
            else
                ErrorNameMessageCreation = "";
            return !existingCategory && (NewCategoryName.Length > 0);
        }

        /*** methods related to the category update ***/

        /// <summary>
        /// Saves in memory the contentDialog
        /// </summary>
        /// <param name="sender">content dialog</param>
        /// <param name="e">arguments</param>
        public void categoryModificationContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            updateCategoryDialog = sender as ContentDialog;
        }

        /// <summary>
        /// Methods called when we cleck on a list view item. It shows the modification pop up form.
        /// </summary>
        /// <param name="sender">list view </param>
        /// <param name="e"></param>
        public async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            SelectedCategory = (CategoryOrdered)listView.SelectedItem;
            if (SelectedCategory != null)
                await updateCategoryDialog.ShowAsync();
        }

        /// <summary>
        /// Method called when the update name field of the pop up form changes. We save the content and check if it is valid (display the proper message to the user)
        /// </summary>
        /// <param name="sender">textbox</param>
        /// <param name="e"></param>
        public void updateCategory_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            UpdatedCategoryName = textBox.Text;

            bool existingCategory = IsExistingCategory(UpdatedCategoryName);
            if (existingCategory)
                ErrorNameMessageUpdate = "Cette Catégorie existe déjà.";
            else
                ErrorNameMessageUpdate = "";

            updateCategoryDialog.IsPrimaryButtonEnabled = !existingCategory && (UpdatedCategoryName.Length > 0);

        }

        /// <summary>
        /// Method called when we click on the close button of the pop up. It resets the name field to an empty value.
        /// </summary>
        /// <param name="sender">content dialog (modification pop up form)</param>
        /// <param name="args"></param>
        public void categoryModification_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            UpdatedCategoryName = "";
        }

        /// <summary>
        /// Update the category in database and display a pop up info. 
        /// </summary>
        /// <param name="sender">content dialog (modification pop up form)</param>
        /// <param name="args"></param>
        public async void categoryModification_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //we update the category
            Category newCategory = new Category { CategoryName = UpdatedCategoryName, CategoryId = SelectedCategory.Category.CategoryId };
            bool wellSaved = repository.SaveCategoryAsync(newCategory);

            //we display the message when process ends and update the view
            if (wellSaved)
            {
                DisplayMessagesService.showPersonalizedMessage("La catégorie " + SelectedCategory.Category.CategoryName + " a été renommée par " + UpdatedCategoryName + ".");
                SelectedCategory.Category.CategoryName = UpdatedCategoryName;
                UpdateCategoriesForView();
            }
            else
                DisplayMessagesService.showPersonalizedMessage("Echec de la modification de la catégorie " + SelectedCategory.Category.CategoryName + ".");
        }
    }
}
