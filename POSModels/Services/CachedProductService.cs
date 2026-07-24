using Microsoft.Extensions.Caching.Memory;
using POSModels.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POSModels.Services
{
    public interface ICachedProductService
    {
        Task<List<dynamic>> GetCachedStockListAsync();
        void ClearStockCache();
    }

    public class CachedProductService : ICachedProductService
    {
        private readonly IBillingService _billingService;
        private readonly IMemoryCache _cache;
        private const string STOCK_CACHE_KEY = "POS_STOCK_LIST_CACHE";

        public CachedProductService(IBillingService billingService, IMemoryCache cache)
        {
            _billingService = billingService;
            _cache = cache;
        }

        public async Task<List<dynamic>> GetCachedStockListAsync()
        {
            if (!_cache.TryGetValue(STOCK_CACHE_KEY, out List<dynamic>? stockList) || stockList == null)
            {
                stockList = await _billingService.getAllStockList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                _cache.Set(STOCK_CACHE_KEY, stockList, cacheEntryOptions);
            }

            return stockList;
        }

        public void ClearStockCache()
        {
            _cache.Remove(STOCK_CACHE_KEY);
        }
    }
}
