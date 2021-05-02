using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RISCV_Simulator.Controller
{
    class FileController
    {
        public static List<string> ReadFile(string filePath)
        {
            return File.ReadAllLines(filePath).ToList<string>();
        }
    }
}
