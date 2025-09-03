using DataProvider;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CoffeeManagement.DAL
{
    public class BillInfoDAL : DBConnection
    {
        #region Singleton pattern
        private static BillInfoDAL instance;
        public static BillInfoDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new BillInfoDAL();
                return instance;
            }
            private set { instance = value; }
        }

        private BillInfoDAL() { SQLQuery.Instance.connectionString = connect; }
        #endregion

        public DataTable GetBillInfoByIdBill(int id)
        {
            return SQLQuery.Instance.ExecuteQuery("SP_GetBillInfoByIdBill @idBill", new object[] { id });
        }

        public int InsertBillInfo(int idBill, int idDrink, int amount)
        {
            return SQLQuery.Instance.ExecuteNonQuery("SP_InsertBillInfo @idBill, @idDrink, @amount", new object[] { idBill, idDrink, amount });
        }
        public int DeleteBillInfo(int id)
        {
            return SQLQuery.Instance.ExecuteNonQuery("SP_DeleteBillInfo @idBillInfo", new object[] { id });
        }
        public DataTable SP_GetListItemBySearch(string keyword)
        {
            return SQLQuery.Instance.ExecuteQuery("SP_GetListItemBySearch @keyword", new object[] { keyword });
        }
        public int UpdateDiscountBillInfo(int idBill, int discount)
        {
            return SQLQuery.Instance.ExecuteNonQuery("SP_UpdateDiscountBillInfo @IdBillInfo, @Discount", new object[] { idBill, discount });
        }
    }
}
