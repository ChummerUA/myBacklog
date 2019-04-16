using myBacklog.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myBacklog.Data
{
    public class BacklogDatabase
    {
        readonly SQLiteAsyncConnection database;

        public BacklogDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);

            Task.WaitAll(database.CreateTableAsync<CategoryModel>(),
                database.CreateTableAsync<StateModel>(),
                database.CreateTableAsync<ItemModel>());
        }

        #region Category
        public Task<bool> IsCategoryExist(string name)
        {
            var category = database.Table<CategoryModel>().FirstOrDefaultAsync(x => x.CategoryName == name).Result;

            if(category != null)
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public async Task CreateCategoryAsync(CategoryModel category)
        {
            var categoryID = await database.InsertAsync(category);

            for (int i = 0; i < category.States.Count; i++)
            {
                category.States[i].CategoryID = categoryID;
            }

            await database.InsertAllAsync(category.States.ToList());
        }

        public async Task<List<CategoryModel>> GetCategoriesAsync()
        {
            return await database.Table<CategoryModel>().ToListAsync();
        }

        public async Task<CategoryModel> GetCategoryAsync(int id)
        {
            return await database.Table<CategoryModel>().ElementAtAsync(id);
        }

        public async Task UpdateCategoryAsync(CategoryModel category)
        {
            await database.UpdateAsync(category);
        }

        public async Task DeleteCategoryAsync(CategoryModel category)
        {
            await database.DeleteAsync(category);
        }
        #endregion

        #region States
        public async Task CreateStateAsync(StateModel state)
        {
            await database.InsertAsync(state);
        }

        public async Task CreateStatesAsync(List<StateModel> states)
        {
            await database.InsertAllAsync(states);
        }

        public async Task<List<StateModel>> GetStatesAsync(int categoryID)
        {
            var states = await database.Table<StateModel>().Where(x => x.CategoryID == categoryID).ToListAsync();

            for(int i = 0; i < states.Count; i++)
            {
                string colorName = states[i].ColorName;

                states[i].Color = System.Drawing.Color.FromName(colorName);
            }

            return states;
        }
        
        public async Task UpdateStateAsync(StateModel state)
        {
            await database.UpdateAsync(state);
        }

        public async Task DeleteStateAsync(StateModel state)
        {
            await database.DeleteAsync(state);
        }
        #endregion

        #region Items
        public async Task CreateItemAsync(ItemModel item)
        {
            await database.InsertAsync(item);
        }

        public async Task<List<ItemModel>> GetItemsAsync(int categoryID)
        {
            return await database.Table<ItemModel>().Where(x => x.CategoryID == categoryID).ToListAsync();
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
