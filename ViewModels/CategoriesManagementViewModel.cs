using humanlab.DAL;
using humanlab.Helpers.Models;
using humanlab.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private IEnumerable<IGrouping<string, CategoryOrdered>> categories;

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

        public IEnumerable<IGrouping<string, CategoryOrdered>> Categories
        {
            get => categories;
            set => SetProperty(ref categories, value, "Categories");
        }

        /*** METHODS ***/
        private async void InitialiseCategories()
        {
            List<Category> categories = await repository.GetAllCategoriesAsync();
            var dbCat = new List<CategoryOrdered>();
            categories.ForEach(c => dbCat.Add(new CategoryOrdered(c, c.CategoryName.First().ToString().ToUpper())));
            Categories = from t in dbCat group t by t.Key;
        }
    }
}
