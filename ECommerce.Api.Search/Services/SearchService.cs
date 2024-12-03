using ECommerce.Api.Search.Interface;
using ECommerce.Api.Search.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        
        private readonly IOrdersService ordersService;
        private readonly IProductsService productsService;

        public SearchService(IOrdersService ordersService, IProductsService productsService)
        {
            this.ordersService = ordersService;
            this.productsService = productsService;
        }
        public async Task<(bool IsSuccess, dynamic SearchResults)> SearchAsync(int customerId)
        {
            var ordersResults = await ordersService.GetOrdersAsync(customerId);

            var productResults = await productsService.GetProductsAsync();

            if(ordersResults.IsSuccess)
            {
                foreach (var order in ordersResults.Orders)
                {
                    foreach(var item in order.Items)
                    {
                        item.ProductName = productResults.IsSuccess ?
                            productResults.Products.FirstOrDefault(p => p.Id == item.ProductId)?.Name :
                            "Product information is not available";
                    }
                }
                var result = new
                {
                    Orders = ordersResults.Orders
                };

                return(true, result);
            }

            return(false, null);
        }
    }
}