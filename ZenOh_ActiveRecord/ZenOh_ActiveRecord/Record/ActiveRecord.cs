using Dapper;
using Npgsql;
using ZenOh_ActiveRecord.Factories;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace ZenOh_ActiveRecord.Record
{

    public class ActiveRecord<T> where T : ActiveRecord<T>
    {

        protected string _schemaName { get; set; }

        private NpgsqlConnection _connection;

        public void SetConnection(NpgsqlConnection connection)
        {
            _connection = connection;
        }       
        
        private  NpgsqlConnection GetConnection()
        {
            if (_connection == null)
            {
                return FactoryConnection.GetConnection();
            }
            else
                return _connection;
        }
        
        public ActiveRecord()
        {

        }

        public ActiveRecord(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public ActiveRecord(NpgsqlConnection connection, string schemaName)
        {
            _connection = connection;
            _schemaName = schemaName;
        }

        private void SaveLinks(int idEntityMaster)
        {

        }

        private int ReturnId()
        {
           
            PropertyInfo[] properties =  this.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (FactoryScript.ContainAttribute(property, typeof(KeyAttribute)))
                {
                    return (int) property.GetValue(this);
                }
            }

            return 0;
        }

        public void SaveRecord()
        {
            //TODO: Chamar metodo SaveRecord de todas as propriedades linkadas
            
            int idRecord = ReturnId();

            if (FindById(idRecord) == null)
            {
                string scriptInsert =
                FactoryScript.Insert(this.GetType(), this._schemaName) + FactoryScript.ReturnId(this.GetType());

                idRecord = _connection.Query<int>(scriptInsert, this).FirstOrDefault<int>();

                PropertyInfo[] properties = this.GetType().GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    if (FactoryScript.ContainAttribute(property, typeof(KeyAttribute)))
                    {
                        property.SetValue(this, idRecord);
                        break;
                    }
                }
                                
            }
            else
            {
                string scriptUpdate = FactoryScript.Update(this.GetType(), this._schemaName);

                GetConnection().Execute(scriptUpdate, this);
            }

            SaveLinks(idRecord);

        }

        public bool Delete()
        {
            try
            {
                string scriptDelete = FactoryScript.Delete(typeof(T), "public");

                GetConnection().Execute(scriptDelete, this);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public NpgsqlTransaction BeginTransaction()
        {
            return GetConnection().BeginTransaction();
        }

        public void DisposeConnection()
        {
            GetConnection().Dispose();
        }

        #region
        public static List<T> FindAll()
        {

            using (var connection = FactoryConnection.GetConnection())
            {
                string scriptSelect = FactoryScript.Select(typeof(T), "public");

                var Result = connection.Query<T>(scriptSelect).ToList<T>();
                
                return Result;
            }
        }

        public static T FindById(int Id)
        {
            using (var connection = FactoryConnection.GetConnection())
            {
                string scriptSelect = FactoryScript.Select(typeof(T), Id, "public");

                var Result = connection.Query<T>(scriptSelect).FirstOrDefault<T>();

                return Result;
            }
        }

        public static List<T> FindCustom(SQLBuilder sqlBuilder)
        {
            using (var connection  = FactoryConnection.GetConnection())
            {
                var result = connection.Query<T>(sqlBuilder.ToString()).ToList<T>();
                return result;
            }          
            
        }
        
        #endregion
    }
}
