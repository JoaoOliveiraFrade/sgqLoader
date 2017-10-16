using System.Text;
using Oracle.DataAccess.Client;
using System.Collections.Generic;
using sgq;
using System;

namespace sgq.bpt
{
    public class SqlMakerProjects : SqlMaker
    {
        public SqlMakerProjects(
            BptConnection bptConnection = null, 
            Connection connection = null, 
            TypeUpdate typeUpdate = TypeUpdate.Increment)
        {
            if (bptConnection != null)
                this.bptConnection = bptConnection;
            else
                throw new ArgumentNullException("bptConnection", "O parâmetro 'bptConnection' não pode ser null");

            if (connection != null)
                this.Connection = connection;
            else
                throw new ArgumentNullException("connection", "O parâmetro 'connection' não pode ser null");

            this.typeUpdate = typeUpdate;
        }

        public override int GetCountRowsTarget // DIFERENTE
        {
            get
            {
                return int.Parse(this.Connection.Get_String("select count(*) from " + this.TargetTable));
            }
        }

        public override string LastInsertKey // DIFERENTE
        {
            get
            {
                var Result = this.Connection.Get_String($"select max(id) from {this.TargetTable}");

                if (string.IsNullOrEmpty(Result))
                    Result = "0";

                return Result;
            }
        }

        public override string LastUpdate // DIFERENTE
        {
            get
            {
                return "";
            }
        }

        public override string ConditionsDataSourceInsert // DIFERENTE
        {
            get
            {
                string result = "";

                if (this.typeUpdate == TypeUpdate.Increment)
                {
                    result = dataSourceFieldId + " > " + this.LastInsertKey;
                }

                return result;
            }
        }

        public override string ConditionsDataSourceUpdate { get; set; } // DIFERENTE

        public override string getSqlDeleteKeys() // DIFERENTE
        {
            return $"truncate table {this.TargetTable}_Keys";
        }

    }
}
