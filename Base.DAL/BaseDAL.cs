using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;
using BS.DAL.AttributeExtend;
using System.Deployment.Internal;
using System.Data.SQLite;

namespace Base.DAL
{
    public class BaseDAL
    {
        public static string ConnectionStringCustomers= "Data Source=DataBase\\GJJ2.db";
        private static T Trans<T>(Type type, SQLiteDataReader reader)
        {
            object oObject = Activator.CreateInstance(type);
            foreach (var prop in type.GetProperties())
            {
                //prop.SetValue(oObject, readerprop.Name]]);
                //Eleven 可空类型，如果数据库存储的是null，直接SetValue会报错的
      
                  
                prop.SetValue(oObject, reader[prop.Name] is DBNull ? null : reader[prop.Name]);


                
            }
            return (T)oObject;
        }
        public static T Find<T>(string Condition = "")where T : class
        {
            try
            {
                Type type = typeof(T);
                string sql = $"Select {string.Join(",", type.GetProperties().Select(p => ($"{p.GetColumnName()} as {p.Name}")))} from  {type.Name} " + Condition;

                using (SQLiteConnection conn = new SQLiteConnection(ConnectionStringCustomers))
                {
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    T rst;
                    if (reader.Read())
                    {
                        rst= Trans<T>(type, reader);
                    }
                    else
                    {
                        rst= null;
                        //数据库没有，应该返回null，而不是一个默认对象
                    }
                    conn.Close();
                    return rst;

                }
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        public static List<T> FindAll<T>(string Condition="") where T:class
        {
 
            try
            {
                Type type = typeof(T);
                string sql = $"Select {string.Join(",", type.GetProperties().Select(p => ($"{p.GetColumnName()} as {p.Name}")))} from  {type.Name} " + Condition;
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionStringCustomers))
                {
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    List<T> tList = new List<T>();
                    while (reader.Read())
                    {

                        tList.Add(Trans<T>(type, reader));
                    }
                    return tList;
                }
            }
            catch (Exception)
            {

                throw;
            }
         
        }
        public static List<T> FindAll<T>(string Condition,out int Count) where T : class
        {
            Count = 0;
            try
            {
                Type type = typeof(T);
                string sql = $"Select {string.Join(",", type.GetProperties().Select(p => ($"{p.GetColumnName()} as {p.Name}")))} from  {type.Name} " + Condition;
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionStringCustomers))
                {
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    List<T> tList = new List<T>();
                    while (reader.Read())
                    {

                        tList.Add(Trans<T>(type, reader));
                    }
                    Count = tList.Count;
                    return tList;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public static int Add<T>(T t) where T : class
        {
            try
            {
                //id是自增的  所以不能新增

                Type type = t.GetType();
                string columnString = string.Join(",", type.GetProperties().Where(p => !p.Name.Equals("Id")).Select(p => $"{p.Name}"));
                string valueColumn = string.Join(",", type.GetProperties().Where(p => !p.Name.Equals("Id")).Select(p => $"@{p.Name}"));
                var parameterList = type.GetProperties().Where(p => !p.Name.Equals("Id")).Select(p => new SQLiteParameter($"@{p.Name}", p.GetValue(t) ?? DBNull.Value));//注意可空类型                                                               
                                                                                                                                                                       //假如都加引号，如果Name的值里面有个单引号，sql变成什么样的  Eleven's  sql会错
                                                                                                                                                                       //还有sql注入风险                                                                            
                                                                                                                                                                       //所以要参数化
                string sql = $"Insert INTO {type.Name} ({columnString}) values({valueColumn}); SELECT last_insert_rowid();";
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionStringCustomers))
                {
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    command.Parameters.AddRange(parameterList.ToArray());
                    conn.Open();

                    var result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                        // 可以在这里使用 lastInsertedId 来处理获取到的自增 Id
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public static int Delete<T>(T t, string Condition) where T : class
        {
            throw new NotImplementedException();
        }


        public static int Update<T>(T t, string Condition) where T : class
        {
            try
            {
                Type type = t.GetType();
                string columnString = string.Join(",", type.GetProperties().Where(p => !p.Name.Equals("Id")).Select(p => $"{p.Name}"));
                string valueColumn = string.Join(",", type.GetProperties().Where(p => !p.Name.Equals("Id")).Select(p => $"@{p.Name}"));
                var parameterList = type.GetProperties().Where(p => !p.Name.Equals("Id")).Select(p => new SQLiteParameter($"@{p.Name}", p.GetValue(t) ?? DBNull.Value));//注意可空类型                                                               
                string updateCloumn = string.Join(",", type.GetProperties().Where(p => !p.Name.Equals("Id")).Select(p => $"{p.Name}=@{p.Name}"));//注意可空类型                                                               

                string sql = $"update {type.Name} set  {updateCloumn} {Condition}";
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionStringCustomers))
                {
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    command.Parameters.AddRange(parameterList.ToArray());
                    conn.Open();
                    return command.ExecuteNonQuery();

                }
            }
            catch (Exception)
            {

                throw;
            }
          
        }

        public static int ExecuteSQL(string sql) 
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionStringCustomers))
                {
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    conn.Open();
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static void AddorUpdate<T>(T t, string Condition)where T : class
        {
            T t1=Find<T>(Condition);
            if(t1==null)
            {
                Add<T>(t);
            }
            else
            {
                Update<T>(t, Condition);
            }
        }
    }
}
