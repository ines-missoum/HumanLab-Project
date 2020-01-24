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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace humanlab.ViewModels
{
    class CategoriesManagementViewModel : BaseViewModel
    {
        /*** PUBLIC ATTRIBUTES ***/

        private IEnumerable<IGrouping<string, CategoryOrdered>> categoriesForView;

        private string newCategoryName;

        private string updatedCategoryName;

        public DelegateCommand SaveNewCategoryDelegate { get; set; }

        private string errorNameMessageCreation;

        private string errorNameMessageUpdate;

        private CategoryOrdered selectedCategory;

        /*** PRIVATE ATTRIBUTES ***/

        /// <summary>
        /// In charge of the interaction  of the database
        /// </summary>
        private CategoryRepository repository;

        private List<CategoryOrdered> dbCategories;

        private ContentDialog updateCategoryDialog;

        /*** CONSTRUCTOR ***/

        public CategoriesManagementViewModel()
        {
            repository = new CategoryRepository();
            InitialiseCategories();
            newCategoryName = "";
            updatedCategoryName = "";
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
                NewCategoryName = "";
            }
            else
                messageDialog = new MessageDialog("Echec de la création de la catégorie " + NewCategoryName + ".");

            await messageDialog.ShowAsync();

        }

        private bool CanSaveNewCategory()
        {
            bool existingCategory = IsExistingCategory(NewCategoryName);
            if (existingCategory)
                ErrorNameMessageCreation = "Cette Catégorie existe déjà.";
            else
                ErrorNameMessageCreation = "";
            return !existingCategory && (NewCategoryName.Length > 0);
        }

        //Modification of category methods

        public async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            SelectedCategory = (CategoryOrdered) listView.SelectedItem;
            Debug.WriteLine(SelectedCategory.Category.CategoryName);
            await updateCategoryDialog.ShowAsync();
        }

        public void categoryModificationContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            updateCategoryDialog = sender as ContentDialog;
        }


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

        public void categoryModification_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            UpdatedCategoryName = "";
        }

        public void categoryModification_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }
    }
}
