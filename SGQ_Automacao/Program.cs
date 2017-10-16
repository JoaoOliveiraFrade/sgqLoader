using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALM_Classes;
using Classes;

namespace SGQ_Automacao
{
    class Program
    {
        static void Main(string[] args)
        {
            ALMA_Projetos ALMA_Projetos = new ALMA_Projetos();

            ALMA_Projetos.Load_Table_ALMA_Projetos();
            ALMA_Projetos.Load_Data_From_ALM();

            ALMA_Projetos.Load_All_Data_From_ALM();

            //string x = "fim";
        }
    }
}
