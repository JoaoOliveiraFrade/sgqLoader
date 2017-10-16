using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sgq.bpt;
using sgq;

namespace Bpt
{
    class Program
    {
        static void Main(string[] args)
        {
            var almConnection = new BptConnection(Ambientes.HOMOLOGACAO);
            var sgqConnection = new Connection();

            var sqlMakerProjects = new SqlMakerProjects(almConnection, sgqConnection, TypeUpdate.Increment);

            var bptProjects = new BptProjects(sqlMakerProjects);
            var periodLoadOwnData = bptProjects.loadOwnData();
            var periodDeleteAllData = bptProjects.deleteAllData();
            var periodLoadFurtherData = bptProjects.loadFurtherData();

            Console.WriteLine("Tempo de carga");
            Console.WriteLine("==============================================");
            Console.WriteLine($"Carga de seus próprios dados        : {(int)periodLoadOwnData.End.Subtract(periodLoadOwnData.Start).Seconds} (s)");
            Console.WriteLine($"Exclusão de todos os dados          : {(int)periodDeleteAllData.End.Subtract(periodDeleteAllData.Start).Seconds} (s)");
            Console.WriteLine($"Carga de demais dados               : {(int)periodLoadFurtherData.End.Subtract(periodLoadFurtherData.Start).Seconds} (s)");
            Console.WriteLine("==============================================");

            Console.WriteLine("Fim");
            Console.ReadKey();
        }
    }
}
