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
    public class CustomerService : IServiceMapping<CustomerDTO>
    {
        #region Singleton pattern
        private static CustomerService instance;
        public static CustomerService Instance
        {
            get
            {
                if (instance == null)
                    instance = new CustomerService();
                return instance;
            }
            private set { instance = value; }
        }
        #endregion
        public bool InsertCustomer(string customerName, string street, DateTime birthday, string email, string mobiphone, string note)
        {
            return CustomerDAL.Instance.InsertCustomer(customerName, street, birthday, email, mobiphone, note) > 0;
        }

        public bool EditCustomer(int idCustomer, string customerName, string street, DateTime birthday, string email, string mobiphone, string note)
        {
            return CustomerDAL.Instance.EditCustomer(idCustomer, customerName, street, birthday, email, mobiphone, note) > 0;
        }
        public CustomerDTO MapToDTO(DataRow row)
        {
            return new CustomerDTO
            {
                IdCustomer = Convert.ToInt32(row["IdCustomer"]),
                CUSTOMER_NAME = row["CUSTOMER_NAME"].ToString(),
                //SEX = row["SEX"].ToString(),
                //STREET = row["STREET"].ToString(),
                //AREA_CODE = row["AREA_CODE"].ToString(),
                //CITY_ID = Convert.ToInt32(row["CITY_ID"]),
                //TELEPHONE = row["TELEPHONE"].ToString(),
                //FAX = row["FAX"].ToString(),
                //MOBIPHONE = row["MOBIPHONE"].ToString(),
                //BIRTHDAY = row["BIRTHDAY"] != DBNull.Value ? Convert.ToDateTime(row["BIRTHDAY"]) : (DateTime?)null,
                //EMAIL = row["EMAIL"].ToString(),
                //DISCOUNT = Convert.ToDecimal(row["DISCOUNT"]),
                //USER_ID = row["USER_ID"].ToString(),
                //NOTE = row["NOTE"].ToString()
                //ACTIVE = Convert.ToInt32(row["ACTIVE"]),
                //WORKSTATION_ID = Convert.ToInt32(row["WORKSTATION_ID"])
            };
        }
        public List<CustomerDTO> GetAllCustomers()
        {
            return MapToDTOList(CustomerDAL.Instance.GetCustomer());
        }
        
        public List<CustomerDTO> MapToDTOList(DataTable table)
        {
            var tables = new List<CustomerDTO>();

            foreach (DataRow row in table.Rows)
            {
                tables.Add(MapToDTO(row));
            }

            return tables;
        }
    }
}
