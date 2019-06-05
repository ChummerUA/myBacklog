using myBacklog.Models;
using myBacklog.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myBacklog.Data
{
    public class BacklogDatabase : IDatabase
    {
        readonly SQLiteAsyncConnection database;

        public BacklogDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);

            Task.WaitAll(database.CreateTableAsync<CategoryModel>(),
                database.CreateTableAsync<StateModel>(),
                database.CreateTableAsync<ItemModel>());
        }

        #region Categories
        public bool IsCategoryNameAwailable(CategoryModel category)
        {
            var result = database.Table<CategoryModel>()
                .FirstOrDefaultAsync(x => x.CategoryName == category.CategoryName &&
                x.CategoryID != category.CategoryID)
                .GetAwaiter()
                .GetResult();

            if(result == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<int> CreateCategoryAsync(CategoryModel category)
        {
            await database.InsertAsync(category);

            return await database.Table<CategoryModel>().CountAsync();
        }

        public async Task<List<CategoryModel>> GetCategoriesAsync()
        {
            return await database.Table<CategoryModel>().ToListAsync();
        }

        public async Task<CategoryModel> GetCategoryAsync(int id)
        {
            var category = await database.Table<CategoryModel>()
                .FirstOrDefaultAsync(x => x.CategoryID == id);

            return category;
        }

        public async Task UpdateCategoryAsync(CategoryModel category)
        {
            await database.UpdateAsync(category);
        }

        public async Task DeleteCategoryAsync(CategoryModel category)
        {
            await database.DeleteAsync(category);

            var states = await GetStatesAsync((int)category.CategoryID);
            foreach(var state in states)
            {
                await DeleteStateAsync(state);
            }
        }
        #endregion

        #region States
        public async Task CreateStateAsync(StateModel state)
        {
            await database.InsertAsync(state);
        }

        public async Task<List<StateModel>> GetStatesAsync(int categoryID)
        {
            return await database.Table<StateModel>()
                .Where(x => x.CategoryID == categoryID)
                .ToListAsync();
        }

        public async Task UpdateStateAsync(StateModel state)
        {
            await database.UpdateAsync(state);
        }

        public async Task DeleteStateAsync(StateModel state)
        {
            await database.DeleteAsync(state);
            var items = await database.Table<ItemModel>()
                .Where(x => x.StateID == state.StateID)
                .ToListAsync();

            foreach(var item in items)
            {
                await DeleteItemAsync(item);
            }
        }
        #endregion

        #region Items
        public bool IsItemNameAwailable(ItemModel item)
        {
            var check = database.Table<ItemModel>()
                .FirstOrDefaultAsync(x => x.CategoryID == item.CategoryID
                && x.ItemName == item.ItemName
                && x.ItemID != item.ItemID)
                .GetAwaiter()
                .GetResult();

            if(check != null)
            {
                return false;
            }

            return true;
        }

        public async Task CreateItemAsync(ItemModel item)
        {
            await database.InsertAsync(item);
        }

        public int GetCategoryItemsCount(int categoryID)
        {
            return database.Table<ItemModel>()
                .CountAsync(x => x.CategoryID == categoryID)
                .GetAwaiter()
                .GetResult();
        }

        public int GetStateItemsCount(int stateID)
        {
            return database.Table<ItemModel>()
                .CountAsync(x => x.StateID == stateID)
                .GetAwaiter()
                .GetResult();
        }

        public int GetSearchResultsCount(ItemModel item)
        {
            return database.Table<ItemModel>()
                .CountAsync(x => x.StateID == item.StateID &&
                x.ItemName.Contains(item.ItemName))
                .GetAwaiter()
                .GetResult();
        }

        public async Task<List<ItemModel>> GetCategoryItemsAsync(int categoryID)
        {
            return await database.Table<ItemModel>()
                .Where(x => x.CategoryID == categoryID)
                .OrderByDescending(x => x.ItemID)
                .Take(30)
                .ToListAsync();
        }

        public async Task<List<ItemModel>> GetCategoryItemsAsync(int categoryID, int itemID)
        {
            return await database.Table<ItemModel>()
                .Where(x => x.CategoryID == categoryID
                && x.ItemID < itemID)
                .OrderByDescending(x => x.ItemID)
                .Take(20)
                .ToListAsync();
        }

        public async Task<List<ItemModel>> GetStateItemsAsync(int stateID)
        {
            return await database.Table<ItemModel>()
                .Where(x => x.StateID == stateID)
                .OrderByDescending(x => x.ItemID)
                .Take(30)
                .ToListAsync();
        }

        public async Task<List<ItemModel>> GetStateItemsAsync(int stateID, int itemID)
        {
            return await database.Table<ItemModel>()
                .Where(x => x.StateID == stateID
                && x.ItemID < itemID)
                .OrderByDescending(x => x.ItemID)
                .Take(20)
                .ToListAsync();
        }

        public async Task<List<ItemModel>> GetSearchResultsAsync(ItemModel target)
        {
            if(target.State.StateName == "All")
            {
                return await database.Table<ItemModel>()
                    .Where(x => x.CategoryID == target.CategoryID &&
                    x.ItemName.Contains(target.ItemName))
                    .OrderByDescending(x => x.ItemID)
                    .Take(30)
                    .ToListAsync();
            }
            else
            {
                return await database.Table<ItemModel>()
                    .Where(x => x.StateID == target.StateID &&
                    x.ItemName == target.ItemName)
                    .OrderByDescending(x => x.ItemID)
                    .Take(30)
                    .ToListAsync();
            }
        }

        public async Task<List<ItemModel>> GetSearchResultsAsync(ItemModel target, int itemID)
        {
            if (target.State.StateName == "All")
            {
                return await database.Table<ItemModel>()
                    .Where(x => x.CategoryID == target.CategoryID &&
                    x.ItemName.Contains(target.ItemName) &&
                    x.ItemID < itemID)
                    .OrderByDescending(x => x.ItemID)
                    .Take(20)
                    .ToListAsync();
            }
            else
            {
                return await database.Table<ItemModel>()
                    .Where(x => x.StateID == target.StateID &&
                    x.ItemName.Contains(target.ItemName) &&
                    x.ItemID < itemID)
                    .OrderByDescending(x => x.ItemID)
                    .Take(20)
                    .ToListAsync();
            }
        }

        public async Task UpdateItemAsync(ItemModel item)
        {
            await database.UpdateAsync(item);
        }

        public async Task DeleteItemAsync(ItemModel item)
        {
            await database.DeleteAsync(item);
        }
        #endregion
    }
}
