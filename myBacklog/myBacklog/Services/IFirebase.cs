using myBacklog.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace myBacklog.Services
{
    public interface IFirebase
    {
        #region Categories
        Task<bool> IsCategoryNameAwailableAsync(CategoryModel category);

        Task<int> GetCategoriesCountAsync();

        Task<string> InsertCategoryAsync(CategoryModel category);

        Task<List<CategoryModel>> GetCategoriesAsync(int count, int? id);

        Task<CategoryModel> GetCategoryAsync(string categoryID);

        Task UpdateCategoryAsync(CategoryModel category);

        Task DeleteCategoryAsync(CategoryModel category);
        #endregion

        #region States
        Task<int> GetStatesCountAsync(string categoryID);

        Task InsertStateAsync(StateModel state);

        Task<List<StateModel>> GetStatesAsync(string categoryID);

        Task<StateModel> GetStateAsync(string stateID);

        Task UpdateStateAsync(StateModel state);

        Task DeleteStateAsync(StateModel state);
        #endregion

        #region Items
        Task<bool> IsItemNameAwailableAsync(ItemModel item);

        Task InsertItemAsync(ItemModel item);

        Task<List<ItemModel>> GetItemsAsync(CategoryModel category, int count, int? id);

        Task<List<ItemModel>> GetItemsAsync(StateModel state, int count, int? id);

        Task<List<ItemModel>> GetItemsAsync(ItemModel item, int count, int? id);

        Task<ItemModel> GetItemAsync(string itemID);

        Task UpdateItemAsync(ItemModel item);

        Task DeleteItemAsync(ItemModel item);

        Task<int> GetItemsCountAsync(CategoryModel category);

        Task<int> GetItemsCountAsync(StateModel state);

        Task<int> GetItemsCountAsync(ItemModel item);
        #endregion
    }
}
