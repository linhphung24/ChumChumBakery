using DataProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeManagement.DAL
{
    public class CustomerDAL : DBConnection
    {
        #region Singleton pattern
        private static CustomerDAL instance;
        public static CustomerDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new CustomerDAL();
                return instance;
            }
            private set { instance = value; }
        }

        private CustomerDAL() { SQLQuery.Instance.connectionString = connect; }
        #endregion
        public DataTable GetCustomer()
        {
            return SQLQuery.Instance.ExecuteQuery("SP_GetCustomer");
        }
        public DataTable SearchCustomer(string keyword)
        {
            return SQLQuery.Instance.ExecuteQuery("SP_SearchCustomer @keyword", new object[] { keyword });
        }
        public DataTable GetCustomerByID(int IdCustomer)
        {
            return SQLQuery.Instance.ExecuteQuery("SP_GetCustomerByID @IdCustomer", new object[] { IdCustomer });
        }
        public int InsertCustomer(string customerName, string street, DateTime birthday, string email, string mobiphone, string note)
        {
            return SQLQuery.Instance.ExecuteNonQuery("SP_InsertCustomer @CUSTOMER_NAME, @STREET, @BIRTHDAY, @EMAIL, @MOBIPHONE, @NOTE", new object[] { customerName, street, birthday, email, mobiphone, note });
        }

        public int EditCustomer(int idCustomer, string customerName, string street, DateTime birthday, string email, string mobiphone, string note)
        {
            return SQLQuery.Instance.ExecuteNonQuery("SP_EditCustomer @IdCustomer, @CUSTOMER_NAME, @STREET, @BIRTHDAY, @EMAIL, @MOBIPHONE, @NOTE", new object[] { idCustomer, customerName, street, birthday, email, mobiphone, note });
        }

        public bool DeleteCustomer(int idCustomer)
        {
            return SQLQuery.Instance.ExecuteNonQuery("SP_DeleteCustomer @IdCustomer", new object[] { idCustomer }) > 0;
        }

    }
}
