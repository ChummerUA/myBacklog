using myBacklog.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace myBacklog.Services
{
    public interface IDatabase
    {
        #region Category
        bool IsCategoryNameAwailable(CategoryModel category);

        Task<int> CreateCategoryAsync(CategoryModel category);

        Task<List<CategoryModel>> GetCategoriesAsync();

        Task<CategoryModel> GetCategoryAsync(int id);

        Task UpdateCategoryAsync(CategoryModel category);

        Task DeleteCategoryAsync(CategoryModel category);
        #endregion

        #region States
        Task CreateStateAsync(StateModel state);

        Task<List<StateModel>> GetStatesAsync(int categoryID);

        Task UpdateStateAsync(StateModel state);

        Task DeleteStateAsync(StateModel state);
        #endregion

        #region Items
        bool IsItemNameAwailable(ItemModel item);

        Task CreateItemAsync(ItemModel item);

        int GetCategoryItemsCount(int categoryID);

        int GetStateItemsCount(int stateID);

        int GetSearchResultsCount(ItemModel item);

        Task<List<ItemModel>> GetCategoryItemsAsync(int categoryID);

        Task<List<ItemModel>> GetCategoryItemsAsync(int categoryID, int itemID);

        Task<List<ItemModel>> GetStateItemsAsync(int stateID);

        Task<List<ItemModel>> GetSearchResultsAsync(ItemModel target);

        Task<List<ItemModel>> GetSearchResultsAsync(ItemModel target, int itemID);

        Task UpdateItemAsync(ItemModel item);

        Task DeleteItemAsync(ItemModel item);
        #endregion
    }
}
