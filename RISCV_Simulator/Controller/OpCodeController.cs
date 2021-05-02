using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RISCV_Simulator.Controller
{
    enum OP_TYPE
    {
        Type1,
        Type2,
        Type3
    }

    public static class OpCodeController
    {
        /*
        // THREE PARAMS
        private static string LB = "imm12 rst5 000 rd5 0000011";
        private static string LH = "imm12 rst5 001 rd5 0000011";
        private static string LW = "imm12 rst5 010 rd5 0000011";
        private static string ADDI = "imm12 rst5 000 rd5 0010011";
        private static string SLTI = "imm12 rst5 010 rd5 0010011";

        // THREE PARAMS, NO IMM
        private static string ADD = "0000000 rst5 rsf5 000 rd5 0110011";
        private static string SLT = "0000000 rst5 rsf5 010 rd5 0110011";

        // FOUR PARAMS
        private static string SB = "imm7 rst5 rsf5 000 imm5 0100011";
        private static string SH = "imm7 rst5 rsf5 001 imm5 0100011";
        private static string SW = "imm7 rst5 rsf5 010 imm5 0100011";
        private static string BEQ = "imm7 rst5 rsf5 000 imm5 1100011";
        private static string BNE = "imm7 rst5 rsf5 001 imm5 1100011";
        private static string BLT = "imm7 rst5 rsf5 100 imm5 1100011";
        private static string BGE = "imm7 rst5 rsf5 101 imm5 1100011";
        */


        // THREE PARAMS
        private static string LB = "{2} {1} 000 {0} 0000011";
        private static string LH = "{2} {1} 001 {0} 0000011";
        private static string LW = "{2} {1} 010 {0} 0000011";
        private static string ADDI = "{2} {1} 000 {0} 0010011";
        private static string SLTI = "{2} {1}  010 {0} 0010011";

        // THREE PARAMS, NO IMM
        private static string ADD = "0000000 {2} {1} 000 {0} 0110011";
        private static string SLT = "0000000 {2} {1} 010 {0} 0110011";

        // FOUR PARAMS
        private static string SB = "{3} {2} {1} 000 {0} 0100011";
        private static string SH = "{3} {2} {1} 001 {0} 0100011";
        private static string SW = "{3} {2} {1} 010 {0} 0100011";
        private static string BEQ = "{3} {2} {1} 000 {0} 1100011";
        private static string BNE = "{3} {2} {1} 001 {0} 1100011";
        private static string BLT = "{3} {2} {1} 100 {0} 1100011";
        private static string BGE = "{3} {2} {1} 101 {0} 1100011";

        public static List<string> GetOpCode(List<string> text)
        {
            List<string> opcLst = new List<string>();
            foreach (string t in text)
            {
                string[] arr = t.Split(',');
                string[] arr2 = arr[0].Trim().Split(' ');
                string[] res = arr2.Concat(arr.Skip(1).ToArray()).ToArray();
                string opcode = ConvertBinToHEX(GenerateOpCode(res));
                opcLst.Add(opcode);
            }
            return opcLst;
        }

        private static string ConvertBinToHEX(string binvalue)
        {
            int pos = 0;
            string opcode = "0x";
            for (int i = 0; i < 8; i++)
            {
                string hx = binvalue.Substring(pos, 4);
                pos = pos + 4;
                opcode = opcode + Convert.ToString(Convert.ToInt64(hx, 2), 16); 
            }
            return opcode;
        }

        private static string GenerateOpCode(string[] arr)
        {
            string format = string.Empty;
            OP_TYPE type = OP_TYPE.Type1;
            switch (arr[0].ToUpper())
            {
                case "LB": format = LB; type = OP_TYPE.Type1; break;
                case "LH": format = LH; type = OP_TYPE.Type1; break;
                case "LW": format = LW; type = OP_TYPE.Type1; break;
                case "SB": format = SB; type = OP_TYPE.Type3; break;
                case "SH": format = SH; type = OP_TYPE.Type3; break;
                case "SW": format = SW; type = OP_TYPE.Type3; break;
                case "ADD": format = ADD; type = OP_TYPE.Type2; break;
                case "ADDI": format = ADDI; type = OP_TYPE.Type1; break;
                case "SLT": format = SLT; type = OP_TYPE.Type2; break;
                case "SLTI": format = SLTI; type = OP_TYPE.Type1; break;
                case "BEQ": format = BEQ; type = OP_TYPE.Type3; break;
                case "BNE": format = BNE; type = OP_TYPE.Type3; break;
                case "BLT": format = BLT; type = OP_TYPE.Type3; break;
                case "BGE": format = BGE; type = OP_TYPE.Type3; break;
            }
            return CreateOpCode(arr, format, type); 
        }

        private static string CreateOpCode(string[] arr, string format, OP_TYPE type)
        {
            string newCode = string.Empty;
            if ((type == OP_TYPE.Type1) || (type == OP_TYPE.Type2))
            {
                string rd = ConstructCode(Convert.ToString(Int32.Parse(Regex.Match(arr[1], @"\d+").Value), 2), 5);
                string rs = ConstructCode(Convert.ToString(Int32.Parse(Regex.Match(GetNumber(arr[2]), @"\d+").Value), 2), 5);
                string third_term = string.Empty;
                
                if (type == OP_TYPE.Type1)
                {
                    third_term = ConstructCode(string.Empty, 12);
                    if (arr.Length > 3)
                    {
                        third_term = ConstructCode(Convert.ToString(Int32.Parse(Regex.Match(arr[3], @"\d+").Value), 2), 12);
                    }
                }
                else
                {
                    third_term = ConstructCode(Convert.ToString(Int32.Parse(Regex.Match(arr[3], @"\d+").Value), 2), 5);
                }

                newCode = String.Format(format, rd, rs, third_term).Replace(" ", "");
            } else
            {
                string rd = ConstructCode(Convert.ToString(Int32.Parse(Regex.Match(arr[1], @"\d+").Value), 2), 5);
                string rs = ConstructCode(Convert.ToString(Int32.Parse(Regex.Match(arr[2], @"\d+").Value), 2), 5);
                newCode = String.Format(format, "00000", rd, rs, "0000000").Replace(" ", "");
            }
            return newCode;
        }

        private static string ConstructCode(string code, int count)
        {
            if (code.Length != count)
            {
                int sub = count - code.Length;
                return String.Empty.PadRight(sub, '0') + code;
            }
            return code;
        }

        private static string GetNumber(string str)
        {
            bool IsX = false;
            string tmp = string.Empty;
            foreach(var c in str)
            {
                if ((IsX) && (Char.IsDigit(c)))
                    tmp = tmp + c;
                if (c == 'x')
                    IsX = true;
            }
            return tmp;
        }
    }
}
