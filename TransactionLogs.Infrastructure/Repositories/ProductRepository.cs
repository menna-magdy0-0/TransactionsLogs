using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionLogs.Domain.Entities;
using TransactionLogs.Domain.Interfaces;
using TransactionLogs.Infrastructure.Data;

namespace TransactionLogs.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly TransactionContext context;

        public ProductRepository(TransactionContext context)
        {
            this.context = context;
        }

        public async Task AddProductAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product cannot be null.");
            }

            await context.Products.AddAsync(product); // Use AddAsync to ensure asynchronous behavior
            await context.SaveChangesAsync(); // Await SaveChangesAsync to persist changes
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await GetProductByIdAsync(id); // Await the Task to get the actual Product object  
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            context.Products.Remove(product); // Pass the Product object directly  
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var products=await context.Products.ToListAsync();
            if (products == null || !products.Any())
            {
                throw new KeyNotFoundException("No products found.");
            }
            return products;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            return product;
        }
   
        

        public async Task UpdateProductAsync(Product product)
        {
            var existingProduct = await context.Products.FindAsync(product.Id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {product.Id} not found.");
            }

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;

            context.Products.Update(existingProduct);
            await context.SaveChangesAsync();
        }
    }
}
