using System.Collections.Generic;

namespace sgq
{
    public class SqlMaker2Param
    {
        public List<Field> fields { get; set; }

        public List<Field> keys {
            get {
                var keys = new List<Field>();
                foreach (var field in this.fields) {
                    if (field.key) {
                        keys.Add(field);
                    }
                }
                return keys;
            }
        }


        public string typeDB { get; set; }


        public string dataSource { get; set; }

        public string dataSourceFilterCondition { get; set; }

        public string dataSourceFilterConditionInsert { get; set; }

        public string dataSourceFilterConditionUpdate { get; set; }


        public string targetTable { get; set; }

        public string targetSqlLastIdInserted { get; set; }

        public string targetLastIdInserted {
            get {
                Connection conn = new Connection();
                string result = conn.Get_String(this.targetSqlLastIdInserted);
                if (result == "" || result == null) {
                    result = "0";
                }

                return result;
            }
        }


        public string targetSqlLastDateUpdate { get; set; }

        public string targetLastDateUpdate {
            get {
                Connection conn = new Connection();
                string result = conn.Get_String(this.targetSqlLastDateUpdate);
                if (result == "" || result == null) {
                    result = "00-00-00 00:00:00";
                }  else {
                    result = result.Substring(6, 2) + "-" + result.Substring(3, 2) + "-" + result.Substring(0, 2) + " " + result.Substring(9, 8);
                }

                return result;
            }
        }

        public string Ultima_Atualizacao { get; set; }
    }
}
