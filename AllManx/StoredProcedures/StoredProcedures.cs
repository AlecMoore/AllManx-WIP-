using AllManx.Models;
using System.Data;
using System.Data.SqlClient;

namespace OrganisedMe.StoredProcedures
{
    public static class StoredProcedures
    {

        public static string SqlconString = "Data Source=DESKTOP-J6APBTK;Initial Catalog=AllManx;Integrated Security=True";
        public static bool CreateUser(User user)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(SqlconString))
                {
                    sqlCon.Open();
                    SqlCommand sql_cmnd = new SqlCommand("dbo.CreateUser", sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    sql_cmnd.Parameters.AddWithValue("@Email", user.Email);
                    sql_cmnd.Parameters.AddWithValue("@Password", user.Password);
                    sql_cmnd.ExecuteNonQuery();
                    sqlCon.Close();

                    return true;
                }
            } catch (Exception ex)
            {
                return false;
            }
        }

        public static bool DeleteUser(int Id)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(SqlconString))
                {
                    sqlCon.Open();
                    SqlCommand sql_cmnd = new SqlCommand("dbo.DeleteUser", sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    sql_cmnd.Parameters.AddWithValue("@Id", Id);
                    sql_cmnd.ExecuteNonQuery();
                    sqlCon.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static List<User> Getusers()
        {
            try
            {
                DataTable dataTable = new DataTable();

                using (SqlConnection sqlCon = new SqlConnection(SqlconString))
                {
                    sqlCon.Open();
                    SqlCommand sql_cmnd = new SqlCommand("dbo.GetUsers", sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    var dataReader = sql_cmnd.ExecuteReader();
                    //sqlCon.Close();

                    List<User> users = new List<User>();
                    while(dataReader.Read())
                    {
                        User t = new User();
                        foreach (var prop in t.GetType().GetProperties())
                        {
                            var propType = prop.PropertyType;
                            prop.SetValue(t, Convert.ChangeType(dataReader[prop.Name].ToString(), propType));
                        }
                        users.Add(t);
                    }

                    return users;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
