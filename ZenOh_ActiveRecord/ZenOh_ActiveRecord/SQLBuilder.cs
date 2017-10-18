using System;
using System.Collections.Generic;

namespace ZenOh_ActiveRecord
{

    public class SQLBuilder
    {
        private string Condition;
        private Type Entity;
        private string Schema;
        private string Script;

        public SQLBuilder(Type Entity, string Schema)
        {
            this.Entity = Entity;
            this.Schema = Schema;
            this.Condition = "";
        }
        
        public SQLBuilder Select()
        {
            this.Script = "Select * from " + this.Schema + "." + Entity.Name;
            return this;
        }

        public SQLBuilder Delete()
        {
            this.Script = "Delete from " + this.Schema + "." + Entity.Name;
            return this;
        }

        public override string ToString()
        {
            return this.Script + " Where " + this.Condition;
        }

        public SQLBuilder Eq(string Field, object value)
        {
            if (this.Condition == string.Empty)
                this.Condition = "( " + Field + " = '" + value + "')";
            else
                this.Condition = this.Condition + " and ( " + Field + " = '" + value + "')";

            return this;
        }

        public SQLBuilder Not(string Field, object value)
        {
            if (this.Condition == string.Empty)
                this.Condition = "( " + Field + " <> '" + value + "')";
            else
                this.Condition = this.Condition + " and ( " + Field + " <> '" + value + "')";

            return this;
        }

        public SQLBuilder In(string Field, List<Object> Values)
        {
            string ConditionIn = "(";

            foreach (var i in Values)
            {
                if (ConditionIn == string.Empty)
                    ConditionIn = "'" +  i.ToString() + "'";
                else
                    ConditionIn = ConditionIn + ", " + "'" + i.ToString() + "'";

            }

            if (this.Condition == string.Empty)
                this.Condition = "( " + Field + " in " + ConditionIn + ")";
            else
                this.Condition = this.Condition + " and ( " + Field + " in " + ConditionIn + ")";

            return this;
        }

        public SQLBuilder NotIn(string Field, List<object> Values)
        {
            string ConditionIn = "(";

            foreach (var i in Values)
            {
                if (ConditionIn == string.Empty)
                    ConditionIn = i.ToString();
                else
                    ConditionIn = ConditionIn + ", " + i.ToString();

            }

            if (this.Condition == string.Empty)
                this.Condition = "( " + Field + " not in " + ConditionIn + ")";
            else
                this.Condition = this.Condition + " and ( " + Field + " not in " + ConditionIn + ")";

            return this;
        }

        public SQLBuilder Betwenn(string Field, object Start, object End)
        {
            if (Condition == string.Empty)
                Condition = " ( " + Field + " between '" + Start + "' and '" + End + "')";
            else
                Condition = Condition + " and ( " + Field + " between '" + Start + "' and '" + End + "')";

            return this;
        }
    }
}