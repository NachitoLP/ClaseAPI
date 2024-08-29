using System.Data.SqlTypes;
using Microsoft.Data.SqlClient;
using ClaseAPI.Models;

public class Utils
{
    public Utils() { }
    public static List<Actions> getOrdersFromDB(string Status)
    {
        string sql_connection = DBHelper.GetConnectionString();

        List<Actions> actions = new List<Actions>();
        string query = "select TX_NUMBER, ORDER_DATE, ACTION, STATUS, SYMBOL, QUANTITY, PRICE from ORDERS_HISTORY WHERE STATUS = @Status";
        SqlConnection sqlConnection = new SqlConnection(sql_connection);
        sqlConnection.Open();

        SqlCommand cmd = new SqlCommand(query,sqlConnection);
        cmd.Parameters.AddWithValue("@Status", Status);

        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read()) {
            Actions action = new Actions();
            action.TxNumber = (int)reader["TX_NUMBER"];
            action.OrderDate = (DateTime)reader["ORDER_DATE"];
            action.Action = (string)reader["ACTION"];
            action.Status = (string)reader["STATUS"];
            action.Symbol = (string)reader["SYMBOL"];
            action.Quantity = (int)reader["QUANTITY"];
            action.Price = (decimal)reader["PRICE"];

            actions.Add(action);
        }

        sqlConnection.Close();

        return actions;
    }

}
