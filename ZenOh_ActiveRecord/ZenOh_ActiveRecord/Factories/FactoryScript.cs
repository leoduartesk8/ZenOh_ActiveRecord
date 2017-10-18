using System;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ZenOh_ActiveRecord.Factories
{
    public static class FactoryScript
    {

        #region Auxiliar

        private static bool IndetifierAutoIncrement(PropertyInfo property)
        {

            return ((FactoryScript.ContainAttribute(property, typeof(ColumnAttribute))) &&
                    (FactoryScript.ContainAttribute(property, typeof(KeyAttribute))) &&
                    (FactoryScript.ContainAttribute(property, typeof(DatabaseGeneratedAttribute))));
               
        }

        public static bool ContainAttribute(PropertyInfo Property, Type Attr)
        {
            IEnumerable<Attribute> Attributes = Property.GetCustomAttributes();

            foreach (var attr in Attributes)
            {
                if (attr.GetType().Equals(Attr))
                {
                    return true;
                }
            }

            return false;
        }

        private static string InsertFields(Type Type)
        {
            StringBuilder Query = new StringBuilder();

            PropertyInfo[] properties = Type.GetProperties();

            int count = 0;

            Query.Append("(");

            foreach (PropertyInfo property in properties)
            {

                if ((FactoryScript.ContainAttribute(property, typeof(ColumnAttribute))) &&
                    //(!FactoryScript.ContainAttribute(property, typeof(KeyAttribute))) &&
                    (!FactoryScript.ContainAttribute(property, typeof(DatabaseGeneratedAttribute)))
                   )
                {
                    if (count == 0)
                        Query.Append(property.Name);
                    else
                        Query.Append(", " + property.Name);

                    count++;
                }
            }

            Query.Append(") ");

            return Query.ToString();
        }

        private static string InsertParameters(Type Type)
        {
            StringBuilder Query = new StringBuilder();

            PropertyInfo[] properties = Type.GetProperties();

            int count = 0;

            Query.Append(" Values(");

            foreach (PropertyInfo property in properties)
            {

                if ((FactoryScript.ContainAttribute(property, typeof(ColumnAttribute))) &&
                    //(!FactoryScript.ContainAttribute(property, typeof(KeyAttribute))) &&
                    (!FactoryScript.ContainAttribute(property, typeof(DatabaseGeneratedAttribute)))
                   )
                {
                    if (count == 0)
                        Query.Append("@" + property.Name);
                    else
                        Query.Append(", @" + property.Name);

                    count++;
                }
            }

            Query.Append(") ");


            return Query.ToString();
        }

        public static string ReturnId(Type Type)
        {
            PropertyInfo[] properties = Type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (FactoryScript.ContainAttribute(property, typeof(KeyAttribute)))
                {
                    return "  RETURNING " + property.Name;
                }
            }

            return string.Empty;
        }

        #endregion

        #region Scripts
        public static string Insert(Type Type, string Schema)
        {
            StringBuilder Query = new StringBuilder();

            Query.Append("Insert into");
                    
            Query.Append("  " + Schema + "." + Type.Name);

            Query.Append(FactoryScript.InsertFields(Type));
            Query.Append(FactoryScript.InsertParameters(Type));

            return Query.ToString();
        }

        public static string Update(Type Type, string Schema)
        {
            StringBuilder Query = new StringBuilder();

            Query.Append("Update ");
            Query.Append("  " + Schema + "." + Type.Name);
            Query.Append(" set ");

            PropertyInfo[] properties = Type.GetProperties();
            String NameKey = "";

            #region 
            int Count = 0;
            foreach (PropertyInfo property in properties)
            {

                if (FactoryScript.ContainAttribute(property, typeof(KeyAttribute)))
                {
                    NameKey = property.Name;
                }
                else
                if (FactoryScript.ContainAttribute(property, typeof(ColumnAttribute)))
                {
                    if (Count == 0)
                        Query.Append(property.Name + " = @" + property.Name);
                    else
                        Query.Append(", " + property.Name + " = @" + property.Name);

                    Count++;
                }
            }
            #endregion

            #region condition
            Query.Append(" Where");
            Query.Append("  (@" + NameKey + " = " + NameKey + ")");
            #endregion

            return Query.ToString();
        }

        public static string Delete(Type Type, List<int> ids, string Schema)
        {
            StringBuilder Query = new StringBuilder();

            Query.Append(" Delete from ");
            Query.Append("  " + Schema + "." + Type.Name);
            Query.Append(" Where ");

            string idSQLIn = "";
            foreach (var i in idSQLIn)
            {
                if (idSQLIn == "")
                {
                    idSQLIn = i.ToString();
                }
                else
                    idSQLIn = idSQLIn + ", " + i;
            }

            PropertyInfo[] properties = Type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (FactoryScript.ContainAttribute(property, typeof(KeyAttribute)))
                {
                    Query.Append("  ( " + property.Name + " in (" + idSQLIn + "))");
                    break;
                }

            }

            return Query.ToString();
        }

        public static string Delete(Type Type, int id, string Schema)
        {
            StringBuilder Query = new StringBuilder();

            Query.Append(" Delete from ");
            Query.Append("  " + Schema + "." + Type.Name);
            Query.Append(" Where ");

            PropertyInfo[] properties = Type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (FactoryScript.ContainAttribute(property, typeof(KeyAttribute)))
                {
                    Query.Append("  ( " + property.Name + " = " + id + ")");
                    break;
                }

            }

            return Query.ToString();
        }

        public static string Delete(Type Type, string Schema)
        {
            StringBuilder Query = new StringBuilder();

            Query.Append(" Delete from ");
            Query.Append("  " + Schema + "." + Type.Name);
            Query.Append(" Where ");

            PropertyInfo[] properties = Type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (FactoryScript.ContainAttribute(property, typeof(KeyAttribute)))
                {
                    Query.Append("  ( " + property.Name + " = @" + property.Name + ")");
                    break;
                }

            }

            return Query.ToString();
        }

        public static string Select(Type Type, string Schema)
        {

            StringBuilder Query = new StringBuilder();

            Query.Append("Select ");
            Query.Append(" * ");
            Query.Append("From ");
            Query.Append("  " + Schema + "." + Type.Name);

            return Query.ToString();
        }

        public static string Select(Type Type, int Key, string Schema)
        {
            StringBuilder Query = new StringBuilder();

            Query.Append("Select ");
            Query.Append(" * ");
            Query.Append("From ");
            Query.Append("  " + Schema + "." + Type.Name);

            PropertyInfo[] properties = Type.GetProperties();

            foreach (var property in properties)
            {
                if (FactoryScript.ContainAttribute(property, typeof(KeyAttribute)))
                {
                    Query.Append(" Where (" + property.Name + " = " + Key + ")");
                    break;
                }
            }

            return Query.ToString();
        }

        public static string SelectValue(Type Type, string FieldName, object Value, string Schema)
        {
            StringBuilder Query = new StringBuilder();

            Query.Append("Select ");
            Query.Append("  * ");
            Query.Append("From ");
            Query.Append("  " + Schema + "." + Type.Name + " ");
            Query.Append("Where ");
            Query.Append("  (" + FieldName + " = " + Value + ")");

            return Query.ToString();
        }
        #endregion
    }
}

