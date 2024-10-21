using System.Data.SqlTypes;
using Microsoft.Data.SqlClient;
using ClaseAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Options;
using static System.Collections.Specialized.BitVector32;

public class Utils
{
    public Utils() { }

    public static List<Actions> getAllOrders()
    {
        string sql_connection = DBHelper.GetConnectionString();
        List<Actions> actions = new List<Actions>();

        string query = "SELECT * FROM ORDERS";

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
            string query = "SELECT TX_NUMBER, ORDER_DATE, ACTION, STATUS, SYMBOL, QUANTITY, PRICE FROM ORDERS WHERE 1 = 1";
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

    public static Actions getActionById(int txnumber)
    {
        try
        {
            Actions action = null;

            string sql_connection = DBHelper.GetConnectionString();
            string query = @"SELECT * FROM ORDERS WHERE TX_NUMBER = @TX_NUMBER";

            using (SqlConnection connection = new SqlConnection(sql_connection))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TX_NUMBER", txnumber);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            action = new Actions
                            {
                                TxNumber = (int)reader["TX_NUMBER"],
                                OrderDate = (DateTime)reader["ORDER_DATE"],
                                Action = (string)reader["ACTION"],
                                Status = (string)reader["STATUS"],
                                Symbol = (string)reader["SYMBOL"],
                                Quantity = (int)reader["QUANTITY"],
                                Price = (decimal)reader["PRICE"]
                            };
                        }
                    }
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
    public static StockMarket getActionBySymbol(string Symbol)
    {
        try
        {
            StockMarket action = null;

            string sql_connection = DBHelper.GetConnectionString();
            string query = @"SELECT * FROM stock_market_shares WHERE SYMBOL LIKE @SYMBOL";

            using (SqlConnection connection = new SqlConnection(sql_connection))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SYMBOL", "%" + Symbol + "%");

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            action = new StockMarket
                            {
                                StockId = (int)reader["ID"],
                                StockSymbol = (string)reader["SYMBOL"],
                                StockUnitPrice = (decimal)reader["UNIT_PRICE"]
                            };
                        }
                    }
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

    private static void LogOrderHistory(Actions action, string operation, decimal actionPrice)
    {
        string sql_connection = DBHelper.GetConnectionString();

        string historyQuery = @"
            INSERT INTO ORDERS_HISTORY (ACTION, STATUS, SYMBOL, QUANTITY, PRICE, OPERATION_DATE, OPERATION)
            VALUES (@Action, @Status, @Symbol, @Quantity, @Price, @OperationDate, @Operation);
        ";

        using (SqlConnection connection = new SqlConnection(sql_connection))
        {
            using (SqlCommand command = new SqlCommand(historyQuery, connection))
            {
                command.Parameters.AddWithValue("@Action", action.Action);
                command.Parameters.AddWithValue("@Status", action.Status);
                command.Parameters.AddWithValue("@Symbol", action.Symbol);
                command.Parameters.AddWithValue("@Quantity", action.Quantity);
                command.Parameters.AddWithValue("@Price", actionPrice);
                command.Parameters.AddWithValue("@OperationDate", DateTime.Now);
                command.Parameters.AddWithValue("@Operation", operation);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
    public static Actions PostOrder (Actions action)
    {
        try
        {
            string sql_connection = DBHelper.GetConnectionString();
            StockMarket stockAction = getActionBySymbol(action.Symbol);

            decimal unitPrice = stockAction?.StockUnitPrice ?? 500.00m;
            decimal actionPrice = action.Quantity * unitPrice;

            if (actionPrice > 9999999999.99m)
            {
                throw new ArgumentOutOfRangeException("El precio de la acción excede el límite permitido.");
            }

            string query = @"
                INSERT INTO ORDERS (ORDER_DATE, ACTION, STATUS, SYMBOL, QUANTITY, PRICE)
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
                    command.Parameters.AddWithValue("@Price", actionPrice);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            LogOrderHistory(action, "INSERT", actionPrice);

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

    public static Actions ChangeStatus(int txnumber, string status)
    {
        try
        {
            string sql_connection = DBHelper.GetConnectionString();

            Actions action = getActionById(txnumber);


            string query = @"UPDATE ORDERS SET STATUS = @Status WHERE TX_NUMBER = @TxNumber";

            using (SqlConnection connection = new SqlConnection(sql_connection))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TxNumber", action.TxNumber);
                    command.Parameters.AddWithValue("@Status", status);
                    
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            LogOrderHistory(action, "UPDATE", action.Price);

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
