using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace VPSBot
{
    public class SaveOrder
    {
        static string connectionString = "Data Source=A2MD19251;Initial Catalog=OrderList;Integrated Security=True";

        public static bool CreateOrder(OrderEntity order)
        {
            try
            {
                bool isSaved = false;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(connectionString))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "spSaveOrder ";
                        command.Connection = connection;

                        command.Parameters.AddWithValue("@type", order.type);

                        command.Parameters.AddWithValue("@os", order.os);

                        command.Parameters.AddWithValue("@processor", order.processor);

                        command.Parameters.AddWithValue("@ram", order.ram);

                        int nCountRow = command.ExecuteNonQuery();
                        if (nCountRow > 0)
                            isSaved = true;
                        else
                            throw new Exception("Incomplete data.");
                        return isSaved;
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}