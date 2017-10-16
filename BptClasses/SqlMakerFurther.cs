using System.Text;
using Oracle.DataAccess.Client;
using System.Collections.Generic;
using sgq;
using System;

namespace sgq.bpt {
    public class SqlMakerFurther : SqlMaker {
        public BptProject BptProject { get; set; }

        public SqlMakerFurther(
            BptConnection bptConnection = null,
            Connection connection = null,
            TypeUpdate typeUpdate = TypeUpdate.Increment,
            BptProject bptProject = null) {

            if (bptConnection != null) {
                this.bptConnection = bptConnection;
            } else {
                throw new ArgumentNullException("bptConnection", "O parâmetro 'bptConnection' não pode ser null");
            }

            if (connection != null) {
                this.Connection = connection;
            } else {
                throw new ArgumentNullException("connection", "O parâmetro 'connection' não pode ser null");
            }

            if (bptProject != null) {
                this.BptProject = bptProject;
            } else {
                throw new ArgumentNullException("bptProject", "O parâmetro 'bptProject' não pode ser null");
            }
        }

        public override int GetCountRowsTarget {
            get {
                return int.Parse(this.Connection.Get_String($"select count(*) from {this.TargetTable} where Subprojeto='{this.BptProject.Subprojeto}' and Entrega='{this.BptProject.Entrega}'"));
            }
        }

        public override string LastInsertKey {
            get {
                var Result = this.Connection.Get_String($"select max(id) from {this.TargetTable} where Subprojeto='{this.BptProject.Subprojeto}' and Entrega='{this.BptProject.Entrega}'");

                if (string.IsNullOrEmpty(Result))
                    Result = "0";

                return Result;
            }
        }

        public override string LastUpdate {
            get {
                var Result = this.Connection.Get_String_Por_Id("BptProjets", "Componentes_Incremental_Inicio", this.BptProject.Id.ToString());

                if (string.IsNullOrEmpty(Result))
                    Result = this.Connection.Get_String_Por_Id("BptProjets", "Componentes_Completa_Inicio", this.BptProject.Id.ToString());

                if (string.IsNullOrEmpty(Result))
                    Result = "00-00-00 00:00:00";
                else
                    Result = Result.Substring(6, 2) + "-" + Result.Substring(3, 2) + "-" + Result.Substring(0, 2) + " " + Result.Substring(9, 8);

                return Result;
            }
        }

        public override string ConditionsDataSourceInsert {
            get {
                string result = "";

                if (this.typeUpdate == TypeUpdate.Increment) {
                    result = $"Subprojeto='{this.BptProject.Subprojeto}' and Entrega='{this.BptProject.Entrega}' and {dataSourceFieldId} > {this.LastInsertKey}";
                }

                return result;
            }
        }

        public override string ConditionsDataSourceUpdate {
            get {
                string result = "";

                if (this.typeUpdate == TypeUpdate.Increment && dataSourceFieldDateUpdade != "") {
                    result = $"Subprojeto='{this.BptProject.Subprojeto}' and Entrega='{this.BptProject.Entrega}' and substr({this.dataSourceFieldDateUpdade}, 3, 17) > '{this.LastUpdate}'";
                }

                return result;
            }
        }

        public override string getSqlDeleteKeys() {
            return $"delete {this.TargetTable}_Keys where Subprojeto='{this.BptProject.Subprojeto}' and Entrega='{this.BptProject.Entrega}'";
        }

    }
}
