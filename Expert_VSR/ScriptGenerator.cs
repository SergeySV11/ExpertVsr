using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows;

namespace Expert_VSR
{
  public class ScriptGenerator
    {
        private string str;
        public ScriptGenerator(string connectionString)
        {
            str = connectionString;
        }
        public bool ExecSelect<T>(string commandString, out List<T> listOut)
            where T : DataVr, new()
        {
            listOut = new List<T>();
            StringBuilder errorMessages = new StringBuilder();

            using (SqlConnection connection = new SqlConnection(str))
            {
                try
                {
                    SqlCommand command = new SqlCommand(commandString, connection);
                    connection.Open();
                    command.CommandTimeout = 3600;
                    SqlDataReader reader = command.ExecuteReader();
                    do
                    {
                        while (reader.Read())
                        {
                            T t = new T();
                            t.Init(reader);
                            listOut.Add(t);
                        }
                    }
                    while (reader.NextResult());
                    reader.Close();
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append("Index #" + i + "\n" +
                            "Message: " + ex.Errors[i].Message + "\n" +
                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                            "Source: " + ex.Errors[i].Source + "\n" +
                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    MessageBox.Show(errorMessages.ToString());
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
            return true;
        }
        public bool ExecScript(string commandString)
        {
            StringBuilder errorMessages = new StringBuilder();

            using (SqlConnection connection = new SqlConnection(str))
            {
                try
                {
                    SqlCommand command = new SqlCommand(commandString, connection);
                    connection.Open();
                    command.CommandTimeout = 3600;
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append("Index #" + i + "\n" +
                            "Message: " + ex.Errors[i].Message + "\n" +
                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                            "Source: " + ex.Errors[i].Source + "\n" +
                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    MessageBox.Show(errorMessages.ToString());
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
            return true;
        }
        public void databaseFilePut(string varFilePath, string commandString)
        {
            byte[] file;
            using (var stream = new FileStream(varFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream))
                {
                    file = reader.ReadBytes((int)stream.Length);
                }
            }
            using (var varConnection = new SqlConnection(str))
            using (var sqlWrite = new SqlCommand(commandString, varConnection))
            {
                varConnection.Open();
                sqlWrite.CommandTimeout = 3600;
                sqlWrite.Parameters.Add("@File", SqlDbType.VarBinary, file.Length).Value = file;
                sqlWrite.ExecuteNonQuery();
                varConnection.Close();
            }
        }

        public void databaseFileRead(string varID, string varPathToNewLocation, string commandString)
        {
            using (var varConnection = new SqlConnection(str))
            using (var sqlQuery = new SqlCommand(commandString, varConnection))
            {
                sqlQuery.Parameters.AddWithValue("@varID", varID);
                varConnection.Open();
                sqlQuery.CommandTimeout = 3600;
                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                    if (sqlQueryResult != null)
                    {
                        sqlQueryResult.Read();
                        var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                        sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                        using (var fs = new FileStream(varPathToNewLocation, FileMode.Create, FileAccess.Write))
                            fs.Write(blob, 0, blob.Length);
                    }
                varConnection.Close();
            }
        }
    }
}
