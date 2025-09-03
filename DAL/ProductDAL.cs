using DataProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace CoffeeManagement.DAL
{
    public class ProductDAL : DBConnection
    {
        #region Singleton pattern
        private static ProductDAL instance;
        public static ProductDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new ProductDAL();
                return instance;
            }
            private set { instance = value; }
        }

        private ProductDAL() { SQLQuery.Instance.connectionString = connect; }
        #endregion
        public DataTable GetTable()
        {
            return SQLQuery.Instance.ExecuteQuery("SP_GetInProduct", new object[] { });
        }
        public int InsertProduct(int idDrink, int quantity, string dateFrom, string dateTo)
        {
            return SQLQuery.Instance.ExecuteNonQuery("SP_InsertProduct @IdDrink, @Quantity, @DateFrom, @DateTo", new object[] { idDrink, quantity, dateFrom, dateTo });
        }
    }
}
