using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public interface IProductRepository
    {
        IEnumerable<Product> ActiveProducts();
        IEnumerable<Product> SearchProduct(string productName, decimal? minPrice, decimal? maxPrice, DateTime? startDate, DateTime? endDate);
        bool CreateProduct(string productName, decimal price);
        bool UpdateProduct(int productId, string productName, decimal price);
        bool DeleteProduct(int productId);
        IEnumerable<ApprovalQueueItem> GetApprovalQueue();
        bool CreateApprovalProduct(string productName, decimal price);
        bool DeleteApprovalProduct(Product product);
        bool UpdateApprovalProduct(int productId, string productName, decimal price);
        IEnumerable<Product> ActiveProductsEdit(int productId);
    }
}
