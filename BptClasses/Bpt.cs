using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class Bpt
    {
        public SqlMakerFurther SqlMaker { get; set; }

        public Period Insert()
        {
            var period = new Period();

            string sqlInsert = this.SqlMaker.GetSqlInsert().Replace("{SqlMaker.BptProject.Esquema}", SqlMaker.BptProject.Esquema).Replace("{Subprojeto}", SqlMaker.BptProject.Subprojeto).Replace("{Entrega}", SqlMaker.BptProject.Entrega);

            OracleDataReader OracleDataReaderInsert = this.SqlMaker.bptConnection.Get_DataReader(sqlInsert);
            if (OracleDataReaderInsert != null && OracleDataReaderInsert.HasRows == true)
            {
                this.SqlMaker.Connection.Executar(ref OracleDataReaderInsert, 1);
            }

            period.End = DateTime.Now;

            return period;
        }

        public Period Update()
        {
            var period = new Period();

            string SqlUpdate = this.SqlMaker.GetSqlUpdate().Replace("{SqlMaker.BptProject.Esquema}", SqlMaker.BptProject.Esquema).Replace("{Subprojeto}", SqlMaker.BptProject.Subprojeto).Replace("{Entrega}", SqlMaker.BptProject.Entrega);
            OracleDataReader OracleDataReaderUpdate = this.SqlMaker.bptConnection.Get_DataReader(SqlUpdate);
            if (OracleDataReaderUpdate != null && OracleDataReaderUpdate.HasRows == true)
            {
                this.SqlMaker.Connection.Executar(ref OracleDataReaderUpdate, 1);
            }

            period.End = DateTime.Now;

            return period;
        }

        public Period LoadValidkeys()
        {
            var period = new Period();

            this.SqlMaker.Connection.Executar(this.SqlMaker.getSqlDeleteKeys());
            this.SqlMaker.Connection.Executar(this.SqlMaker.getSqlInsertKeys());

            period.End = DateTime.Now;

            return period;
        }

        public Period Delete()
        {
            var period = new Period();

            string SqlDelete = this.SqlMaker.getSqlDelete();
            this.SqlMaker.Connection.Executar(SqlDelete);

            period.End = DateTime.Now;

            return period;
        }

        public Period LoadData()
        {
            var period = new Period();

            if (this.SqlMaker.GetCountRowsTarget > 0)
            {
                this.LoadValidkeys();
                this.Delete();
                this.Update();
                this.Insert();
            }
            else
                this.Insert();

            period.End = DateTime.Now;

            return period;
        }
    }
}

