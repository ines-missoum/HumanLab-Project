using humanlab.DAL;
using humanlab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.ViewModels
{
    class CategoriesManagementViewModel : BaseViewModel
    {
        /*** PUBLIC ATTRIBUTES ***/

        /// <summary>
        /// the categories of the database
        /// </summary>
        private List<Category> categories;

        /*** PRIVATE ATTRIBUTES ***/

        /// <summary>
        /// In charge of the interaction  of the database
        /// </summary>
        private CategoryRepository repository;

        /*** CONSTRUCTOR ***/

        public CategoriesManagementViewModel()
        {
            repository = new CategoryRepository();
            InitialiseCategories();
        }

        /*** GETTERS AND SETTERS FOR PUBLIC ATTRIBUTES ***/

        public List<Category> Categories
        {
            get => categories;
            set => SetProperty(ref categories, value, "Categories");
        }

        /*** METHODS ***/
        private async void InitialiseCategories()
        {
            var categories = await repository.GetAllCategoriesAsync();
            Categories = new List<Category>();
            categories.ForEach(category => Categories.Add(category));
        }
    }
}
