using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using myBacklog.Models;
using Plugin.CloudFirestore;

namespace myBacklog.Services
{
    public class FirebaseService : IFirebase
    {
        protected readonly ICloudFirestore Current = CrossCloudFirestore.Current;

        #region Category
        public async Task<bool> IsCategoryNameAwailableAsync(CategoryModel category)
        {
            var categories = await Current.Instance
                .GetCollection("Categories")
                .WhereEqualsTo("CategoryName", category.CategoryName)
                .GetDocumentsAsync();
            if(categories.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<int> GetCategoriesCountAsync()
        {
            return (await Current.Instance
                .GetCollection("Categories")
                .GetDocumentsAsync()).Count;
        }

        public async Task<string> InsertCategoryAsync(CategoryModel category)
        {
            await Current.Instance
                .GetCollection("Categories")
                .AddDocumentAsync(category);

            var result = (await Current.Instance
                .GetCollection("Categories")
                .WhereEqualsTo("CategoryName", category.CategoryName)
                .GetDocumentsAsync());
            var id = result.Documents.ElementAt(0).Id;

            category.CategoryID = id;
            category.ID = await GetCategoriesCountAsync();
            await UpdateCategoryAsync(category);

            return id;
        }

        public async Task<List<CategoryModel>> GetCategoriesAsync()
        {
            var result = await Current.Instance
                .GetCollection("Categories")
                .GetDocumentsAsync();
            return result.ToObjects<CategoryModel>()
                .OrderByDescending(x => x.ID)
                .ToList();
        }

        public async Task<CategoryModel> GetCategoryAsync(string categoryID)
        {
            var result = await Current.Instance
                .GetCollection("Categories")
                .WhereEqualsTo("CategoryID", categoryID)
                .GetDocumentsAsync();
            var list = result.ToObjects<CategoryModel>().ToList();
            return list.FirstOrDefault();
        }

        public async Task UpdateCategoryAsync(CategoryModel category)
        {
            await Current.Instance
                .GetCollection("Categories")
                .GetDocument(category.CategoryID)
                .UpdateDataAsync(category);
        }

        public async Task DeleteCategoryAsync(CategoryModel category)
        {
            await Current.Instance
                .GetCollection("Categories")
                .GetDocument(category.CategoryID)
                .DeleteDocumentAsync();

            var id = category.ID;
            var count = (await Current.Instance
                .GetCollection("Categories")
                .WhereGreaterThan("ID", category.ID)
                .GetDocumentsAsync())
                .Count;
            for(int i = id; i < id + count; i++)
            {
                var toUpdate = (await Current.Instance
                    .GetCollection("Categories")
                    .WhereEqualsTo("ID", i)
                    .GetDocumentsAsync())
                    .Documents
                    .ElementAt(0)
                    .ToObject<CategoryModel>();

                toUpdate.ID--;
                await UpdateCategoryAsync(toUpdate);
            }
        }
        #endregion

        #region States
        public async Task<bool> IsStateNameAwailableAsync(StateModel state)
        {
            var result = await Current.Instance
                .GetCollection("States")
                .WhereEqualsTo("CategoryID", state.CategoryID)
                .WhereEqualsTo("StateName", state.StateName)
                .GetDocumentsAsync();
            if(result.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<int> GetStatesCountAsync(string categoryID)
        {
            return (await Current.Instance
                .GetCollection("States")
                .GetDocumentsAsync()).Count;
        }

        public async Task InsertStateAsync(StateModel state)
        {
            await Current.Instance
                .GetCollection("States")
                .AddDocumentAsync(state);

            var result = await Current.Instance
                .GetCollection("States")
                .WhereEqualsTo("CategoryID", state.CategoryID)
                .WhereEqualsTo("StateName", state.StateName)
                .LimitTo(1)
                .GetDocumentsAsync();
            var id = result.Documents.FirstOrDefault().Id;

            state.StateID = id;
            state.ID = await GetStatesCountAsync(state.CategoryID);
            await UpdateStateAsync(state);
        }

        public async Task<List<StateModel>> GetStatesAsync(string categoryID)
        {
            var result = await Current.Instance
                .GetCollection("States")
                .WhereEqualsTo("CategoryID", categoryID)
                .GetDocumentsAsync();
            return result.ToObjects<StateModel>()
                .OrderByDescending(x => x.ID)
                .ToList();
        }

        public async Task<StateModel> GetStateAsync(string stateID)
        {
            var result = await Current.Instance
                .GetCollection("States")
                .GetDocument(stateID)
                .GetDocumentAsync();
            return result.ToObject<StateModel>();
        }

        public async Task UpdateStateAsync(StateModel state)
        {
            await Current.Instance
                .GetCollection("States")
                .GetDocument(state.StateID)
                .UpdateDataAsync(state);
        }

        public async Task DeleteStateAsync(StateModel state)
        {
            await Current.Instance
                .GetCollection("States")
                .GetDocument(state.StateID)
                .DeleteDocumentAsync();

            var id = state.ID;
            var count = (await Current.Instance
                .GetCollection("States")
                .WhereGreaterThan("ID", id)
                .GetDocumentsAsync())
                .Count;

            for(int i = id; i < id + count; i++)
            {
                var toUpdate = (await Current.Instance
                    .GetCollection("States")
                    .WhereEqualsTo("ID", i)
                    .GetDocumentsAsync())
                    .Documents
                    .ElementAt(0)
                    .ToObject<StateModel>();

                toUpdate.ID--;
                await UpdateStateAsync(toUpdate);
            }
        }
        #endregion

        #region Items
        public async Task<bool> IsItemNameAwailableAsync(ItemModel item)
        {
            var items = (await Current.Instance
                .GetCollection("Items")
                .WhereEqualsTo("CategoryID", item.CategoryID)
                .GetDocumentsAsync())
                .ToObjects<ItemModel>()
                .Where(x => x.ItemID != item.ItemID);
            if(items.Count() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<int> GetItemsCountAsync(CategoryModel category)
        {
            return (await Current.Instance
                .GetCollection("Items")
                .WhereEqualsTo("CategoryID", category.CategoryID)
                .GetDocumentsAsync()).Count;
        }

        public async Task<int> GetItemsCountAsync(StateModel state)
        {
            return (await Current.Instance
                .GetCollection("Items")
                .WhereEqualsTo("StateID", state.StateID)
                .GetDocumentsAsync()).Count;
        }

        public async Task<int> GetItemsCountAsync(ItemModel item)
        {
            return (await Current.Instance
                .GetCollection("Items")
                .WhereEqualsTo("CategoryID", item.CategoryID)
                .WhereEqualsTo("StateID", item.StateID)
                .WhereEqualsTo("ItemName", item.ItemName)
                .GetDocumentsAsync()).Count;
        }

        public async Task InsertItemAsync(ItemModel item)
        {
            await Current.Instance
                .GetCollection("Items")
                .AddDocumentAsync(item);

            var result = await Current.Instance
                .GetCollection("Items")
                .WhereEqualsTo("StateID", item.StateID)
                .WhereEqualsTo("ItemName", item.ItemName)
                .GetDocumentsAsync();
            var id = result.Documents.FirstOrDefault().Id;

            item.ItemID = id;
            item.ID = (await Current.Instance
                .GetCollection("Items")
                .GetDocumentsAsync())
                .Count;
            await UpdateItemAsync(item);
        }


        public async Task<List<ItemModel>> GetItemsAsync(CategoryModel category, int count, int? id)
        {
            if(id == null)
            {
                id = await GetItemsCountAsync(category);
            }

            var result = await Current.Instance
                .GetCollection("Items")
                .WhereEqualsTo("CategoryID", category.CategoryID)
                .WhereLessThan("ID", id)
                .LimitTo(count)
                .GetDocumentsAsync();
            return result.ToObjects<ItemModel>()
                .OrderByDescending(x => x.ID)
                .ToList();
        }

        public async Task<List<ItemModel>> GetItemsAsync(StateModel state, int count, int? id)
        {
            if (id == null)
            {
                id = await GetItemsCountAsync(state);
            }

            var result = await Current.Instance
                .GetCollection("Items")
                .WhereEqualsTo("StateID", state.StateID)
                .WhereLessThan("ID", id).LimitTo(count)
                .GetDocumentsAsync();
            return result.ToObjects<ItemModel>()
                .OrderByDescending(x => x.ID)
                .ToList();
        }

        public async Task<List<ItemModel>> GetItemsAsync(ItemModel item, int count, int? id)
        {
            if (id == null)
            {
                id = await GetItemsCountAsync(item);
            }

            var query = Current.Instance
                .GetCollection("Items")
                .WhereEqualsTo("CategoryID", item.CategoryID);

            if(item.StateID != null)
            {
                query = query.WhereEqualsTo("StateID", item.StateID);
            }
            if(item.ItemName != null)
            {
                query = query.WhereEqualsTo("ItemName", item.ItemName);
            }
            var result = await query.WhereLessThan("ID", id)
                .GetDocumentsAsync();

            return result.ToObjects<ItemModel>()
                .OrderByDescending(x => x.ID)
                .ToList();
        }

        public async Task<ItemModel> GetItemAsync(string itemID)
        {
            var result = await Current.Instance
                .GetCollection("Items")
                .GetDocument(itemID)
                .GetDocumentAsync();
            return result.ToObject<ItemModel>();
        }

        public async Task UpdateItemAsync(ItemModel item)
        {
            await Current.Instance
                .GetCollection("Items")
                .GetDocument(item.ItemID)
                .UpdateDataAsync(item);
        }

        public async Task DeleteItemAsync(ItemModel item)
        {
            await Current.Instance
                .GetCollection("Items")
                .GetDocument(item.ItemID)
                .DeleteDocumentAsync();

            var id = item.ID;
            var count = (await Current.Instance.GetCollection("Items")
                .WhereGreaterThan("ID", id)
                .GetDocumentsAsync())
                .Count;

            for(int i = id; i < id + count; i++)
            {
                var toUpdate = (await Current.Instance
                    .GetCollection("Items")
                    .WhereEqualsTo("ID", i)
                    .GetDocumentsAsync())
                    .Documents
                    .ElementAt(0)
                    .ToObject<ItemModel>();

                toUpdate.ID--;
                await UpdateItemAsync(toUpdate);
            }
        }
        #endregion

    }
}
