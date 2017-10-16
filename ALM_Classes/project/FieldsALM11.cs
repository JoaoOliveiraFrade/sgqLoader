using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.alm {
    public class FieldsALM11 : Fields  {
        public FieldsALM11() {
            this.list.Add(new Field() { target = "Origem", source = "upper(rtrim(CREATE_from_PROJECT))" });
            this.list.Add(new Field() { target = "Ativo", source = "upper(rtrim(PR_IS_ACTIVE))" });
            // this.list.Add(new Field() { target = "Template_Nome", source = "(select rtrim(p2.project_name) from qcsiteadmin_db2.PROJECTS p2 where p2.project_uid = (select PRL_from_PROJECT_UID from qcsiteadmin_db2.PROJECT_LINKS where PRL_TO_PROJECT_UID = project_uid))" });
            // this.list.Add(new Field() { target = "Template", source = "5" });
            this.list.Add(new Field() { target = "Template_Nome", source = "'TD05'" });
            this.list.Add(new Field() { target = "Template", source = "5" });
            this.list.Add(new Field() { target = "Dt_Criacao", source = "substr(dbms_lob.substr(description, 30, 1),20,2) || '-' || substr(dbms_lob.substr(description, 30, 1),17,2) || '-' || substr(dbms_lob.substr(description, 30, 1),14,2) || ' ' || substr(dbms_lob.substr(description, 30, 1),23,8)"});
        }
    }
}

