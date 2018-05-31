using System;
using System.Collections.Generic;
using System.Text;
using Redsis.EVA.Client.Core;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace Redsis.EVA.Client.Core
{
    public class PruebaCrud
    {
        public void Connection()
        {
            string sqlOrderDetails = "SELECT * FROM articulo;";
            string sqlOrderDetail = "SELECT * FROM OrderDetails WHERE OrderDetailID = @OrderDetailID;";
            string sqlCustomerInsert = "INSERT INTO Customers (CustomerName) Values (@CustomerName);";
            
            using (var connection = new SqlConnection("Data Source=192.168.36.50;Initial Catalog=eva2;User ID=admindb;Password=R3ds1s2016"))
            {
                //var orderDetail = connection.QueryFirstOrDefault(sqlOrderDetails, new { OrderDetailID = 1 });
                var affectedRows = connection.Query(sqlOrderDetails).AsList();
            }

            Console.WriteLine("El coso si sirve");
            
            //SqlCommand com = new SqlCommand("INSERT INTO tblVALUES(@AssetID,@AssetName,@ConfigID)");
            string connectionString = @"Data Source=192.168.33.169;Initial Catalog=eva114;User ID=sa;Password=redsis123 ";
            string databaseTable = "articulo";
            string referenceAccountNumber = "0001134919";
            string selectQuery = string.Format("SELECT * FROM articulo ", databaseTable, referenceAccountNumber);
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                //open connection
                connection.Open();

                SqlCommand command = new SqlCommand(selectQuery, connection);

                Console.WriteLine("Prueba funciona");
                command.Connection = connection;
                command.CommandText = selectQuery;
                var result = command.ExecuteReader();
                //check if account exists
                var exists = result.HasRows;
            }
            catch (Exception exception)
            {
                #region connection error
                /*
                 * AlertDialog.Builder connectionException = new AlertDialog.Builder(this);
                connectionException.SetTitle("Connection Error");
                connectionException.SetMessage(exception.ToString());
                connectionException.SetNegativeButton("Return", delegate { });
                connectionException.Create();
                connectionException.Show();
                */
                #endregion
            }
        }
    }

    
}
