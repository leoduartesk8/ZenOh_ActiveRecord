using Npgsql;
using System.Collections.Generic;

namespace ZenOh_ActiveRecord.Record
{
    public interface IActiveRecord<T>
    {
        void Add(T entity);
        void Add(List<T> entities);
        void Update(T entity);
        void Update(List<T> entities);
        bool Delete(int id);
        bool Delete(List<int> id);
        T FindById(int id);
        List<T> FindAll();
        List<T> FindCustom(SQLBuilder sqlBuilder);

    }
}
