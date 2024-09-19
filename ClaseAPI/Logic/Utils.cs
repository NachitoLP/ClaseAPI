using System.Data.SqlTypes;
using Microsoft.Data.SqlClient;
using ClaseAPI.Models;
using Microsoft.AspNetCore.Mvc;

public class Utils
{
    public Utils() { }

    public static List<Actions> getAllOrders()
    {
        string sql_connection = DBHelper.GetConnectionString();
        List<Actions> actions = new List<Actions>();

        string query = "SELECT * FROM ORDERS_HISTORY";

        try
        {
            using (SqlConnection sqlConnection = new SqlConnection(sql_connection))
            {
                sqlConnection.Open();
                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Actions action = new Actions
                            {
                                TxNumber = (int)reader["TX_NUMBER"],
                                OrderDate = (DateTime)reader["ORDER_DATE"],
                                Action = (string)reader["ACTION"],
                                Status = (string)reader["STATUS"],
                                Symbol = (string)reader["SYMBOL"],
                                Quantity = (int)reader["QUANTITY"],
                                Price = (decimal)reader["PRICE"]
                            };
                            actions.Add(action);
                        }
                    }
                }
            }

            return actions;
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

    public static List<Actions> getOrdersByParameter(string? Status, int? year)
    {
        string sql_connection = DBHelper.GetConnectionString();
        List<Actions> actions = new List<Actions>();

        try
        {
            string query = "SELECT TX_NUMBER, ORDER_DATE, ACTION, STATUS, SYMBOL, QUANTITY, PRICE FROM ORDERS_HISTORY WHERE 1 = 1";
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(Status))
            {
                query += " AND STATUS = @Status";
                parameters.Add(new SqlParameter("@Status", Status));
            }

            if (year.HasValue)
            {
                query += " AND YEAR(ORDER_DATE) = @OrderDate";
                parameters.Add(new SqlParameter("@OrderDate", year));
            }

            using (SqlConnection sqlConnection = new SqlConnection(sql_connection))
            {
                sqlConnection.Open();
                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Actions action = new Actions
                            {
                                TxNumber = (int)reader["TX_NUMBER"],
                                OrderDate = (DateTime)reader["ORDER_DATE"],
                                Action = (string)reader["ACTION"],
                                Status = (string)reader["STATUS"],
                                Symbol = (string)reader["SYMBOL"],
                                Quantity = (int)reader["QUANTITY"],
                                Price = (decimal)reader["PRICE"]
                            };
                            actions.Add(action);
                        }
                    }
                }
            }

            return actions;
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }


    public static Actions PostOrder (Actions action)
    {
        try
        {
            string sql_connection = DBHelper.GetConnectionString();
            string query = @"
                INSERT INTO ORDERS_HISTORY (ORDER_DATE, ACTION, STATUS, SYMBOL, QUANTITY, PRICE)
                VALUES (@OrderDate, @Action, @Status, @Symbol, @Quantity, @Price);
            ";

            using (SqlConnection connection = new SqlConnection(sql_connection))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderDate", action.OrderDate);
                    command.Parameters.AddWithValue("@Action", action.Action);
                    command.Parameters.AddWithValue("@Status", action.Status);
                    command.Parameters.AddWithValue("@Symbol", action.Symbol);
                    command.Parameters.AddWithValue("@Quantity", action.Quantity);
                    command.Parameters.AddWithValue("@Price", action.Price);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return action;
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }
}
