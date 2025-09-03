using CoffeeManagement.DAL;
using CoffeeManagement.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeManagement.BLL
{
    public class ProductService : IServiceMapping<ProductDTO>
    {        
        #region Singleton pattern
        private static ProductService instance;
        public static ProductService Instance
        {
            get
            {
                if (instance == null)
                    instance = new ProductService();
                return instance;
            }
            private set { instance = value; }
        }
        #endregion
        public bool InsertProduct(int idDrink, int quantity, DateTime dateFrom, DateTime dateTo)
        {
            return ProductDAL.Instance.InsertProduct(idDrink, quantity, dateFrom.ToString(), dateTo.ToString()) > 0;
        }

        public ProductDTO MapToDTO(DataRow row)
        {
            return new ProductDTO
            {
                IdProduct = Convert.ToInt32(row["IdProduct"]),
                IdDrink = Convert.ToInt32(row["IdDrink"]),
                Quantity = Convert.ToInt32(row["Quantity"])
            };
        }

        public List<ProductDTO> MapToDTOList(DataTable table)
        {
            var products = new List<ProductDTO>();

            foreach (DataRow row in table.Rows)
            {
                products.Add(MapToDTO(row));
            }

            return products;
        }
    }
}
