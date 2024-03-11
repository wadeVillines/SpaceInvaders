using System.Reflection;

namespace Intel8080
{
    public class InstructionSet
    {
        private static List<Instruction> Instructions = new List<Instruction>(0x100);

        public Instruction this[byte OpCode]
        {
            get => Instructions[OpCode];
        }

        public InstructionSet()
        {
            for (int i = 0; i < 0x100; i++)
                Instructions.Add(NOP);

            var instructions = typeof(InstructionSet).GetFields()
                .Where(x => x.FieldType == typeof(Instruction))
                .ToList();

            foreach (var field in instructions)
            {
                var instr = field.GetValue(field) as Instruction;
                Instructions[instr!.Code] = instr;
            }
        }

        public static Instruction NOP = new(OpCodes.NOP, 1, "NOP");
        public static Instruction LXI_B = new(OpCodes.LXI_B, 3, "LXI B, {d16}");
        public static Instruction STAX_B = new(OpCodes.STAX_B, 1, "STAX B");
        public static Instruction INX_B = new(OpCodes.INX_B, 1, "INX B");
        public static Instruction INR_B = new(OpCodes.INR_B, 1, "INR B");
        public static Instruction DCR_B = new(OpCodes.DCR_B, 1, "DCR B");
        public static Instruction MVI_B = new(OpCodes.MVI_B, 2, "MVI B, {d8}");
        public static Instruction RLC = new(OpCodes.RLC, 1, "RLC");
        //public static Instruction = 0x08;
        public static Instruction DAD_B = new(OpCodes.DAD_B, 1, "DAD B");
        public static Instruction LDAX_B = new(OpCodes.LDAX_B, 1, "LDAX B");
        public static Instruction DCX_B = new(OpCodes.DCX_B, 1, "DCX B");
        public static Instruction INR_C = new(OpCodes.INR_C, 1, "INR C");
        public static Instruction DCR_C = new(OpCodes.DCR_C, 1, "DCR C");
        public static Instruction MVI_C = new(OpCodes.MVI_C, 2, "MVI C, {d8}");
        public static Instruction RRC = new(OpCodes.RRC, 1, "RRC");

        //public static Instruction = 0x10;
        public static Instruction LXI_D = new(OpCodes.LXI_D, 3, "LXI D, {d16}");
        public static Instruction STAX_D = new(OpCodes.STAX_D, 1, "STAX D");
        public static Instruction INX_D = new(OpCodes.INX_D, 1, "INX D");
        public static Instruction INR_D = new(OpCodes.INR_D, 1, "INR D");
        public static Instruction DCR_D = new(OpCodes.DCR_D, 1, "DCR D");
        public static Instruction MVI_D = new(OpCodes.MVI_D, 2, "MVI D, {d8}");
        public static Instruction RAL = new(OpCodes.RAL, 1, "RAL");
        //public static Instruction = 0x18;
        public static Instruction DAD_D = new(OpCodes.DAD_D, 1, "DAD D");
        public static Instruction LDAX_D = new(OpCodes.LDAX_D, 1, "LDAX D");
        public static Instruction DCX_D = new(OpCodes.DCX_D, 1, "DCX D");
        public static Instruction INR_E = new(OpCodes.INR_E, 1, "INR E");
        public static Instruction DCR_E = new(OpCodes.DCR_E, 1, "DCR E");
        public static Instruction MVI_E = new(OpCodes.MVI_E, 2, "MVI E, {d8}");
        public static Instruction RAR = new(OpCodes.RAR, 1, "RAR");

        //public static Instruction = 0x20;
        public static Instruction LXI_H = new(OpCodes.LXI_H, 3, "LXI H, {d16}");
        public static Instruction SHLD = new(OpCodes.SHLD, 3, "SHLD {addr}");
        public static Instruction INX_H = new(OpCodes.INX_H, 1, "INX H");
        public static Instruction INR_H = new(OpCodes.INR_H, 1, "INR H");
        public static Instruction DCR_H = new(OpCodes.DCR_H, 1, "DCR H");
        public static Instruction MVI_H = new(OpCodes.MVI_H, 2, "MVI H, {d8}");
        public static Instruction DAA = new(OpCodes.DAA, 1, "DAA");
        //public static Instruction = 0x28;
        public static Instruction DAD_H = new(OpCodes.DAD_H, 1, "DAD H");
        public static Instruction LHLD = new(OpCodes.LHLD, 3, "LHLD {addr}");
        public static Instruction DCX_H = new(OpCodes.DCX_H, 1, "DCX H");
        public static Instruction INR_L = new(OpCodes.INR_L, 1, "INR L");
        public static Instruction DCR_L = new(OpCodes.DCR_L, 1, "DCR L");
        public static Instruction MVI_L = new(OpCodes.MVI_L, 2, "MVI L, {d8}");
        public static Instruction CMA = new(OpCodes.CMA, 1, "CMA");

        //public static Instruction = 0x30;
        public static Instruction LXI_SP = new(OpCodes.LXI_SP, 3, "LXI SP, {d16}");
        public static Instruction STA = new(OpCodes.STA, 3, "STA {addr}");
        public static Instruction INX_SP = new(OpCodes.INX_SP, 1, "INX SP");
        public static Instruction INR_M = new(OpCodes.INR_M, 1, "INR M");
        public static Instruction DCR_M = new(OpCodes.DCR_M, 1, "DCR M");
        public static Instruction MVI_M = new(OpCodes.MVI_M, 2, "MVI M, {d8}");
        public static Instruction STC = new(OpCodes.STC, 1, "STC");
        //public static Instruction = 0x38;
        public static Instruction DAD_SP = new(OpCodes.DAD_SP, 1, "DAD SP");
        public static Instruction LDA = new(OpCodes.LDA, 3, "LDA {addr}");
        public static Instruction DCX_SP = new(OpCodes.DCX_SP, 1, "DCX SP");
        public static Instruction INR_A = new(OpCodes.INR_A, 1, "INR A");
        public static Instruction DCR_A = new(OpCodes.DCR_A, 1, "DCR A");
        public static Instruction MVI_A = new(OpCodes.MVI_A, 2, "MVI A, {d8}");
        public static Instruction CMC = new(OpCodes.CMC, 1, "CMC");

        public static Instruction MOV_B_B = new(OpCodes.MOV_B_B, 1, "MOV B, B");
        public static Instruction MOV_B_C = new(OpCodes.MOV_B_C, 1, "MOV B, C");
        public static Instruction MOV_B_D = new(OpCodes.MOV_B_D, 1, "MOV B, D");
        public static Instruction MOV_B_E = new(OpCodes.MOV_B_E, 1, "MOV B, E");
        public static Instruction MOV_B_H = new(OpCodes.MOV_B_H, 1, "MOV B, H");
        public static Instruction MOV_B_L = new(OpCodes.MOV_B_L, 1, "MOV B, L");
        public static Instruction MOV_B_M = new(OpCodes.MOV_B_M, 1, "MOV B, M");
        public static Instruction MOV_B_A = new(OpCodes.MOV_B_A, 1, "MOV B, A");
        public static Instruction MOV_C_B = new(OpCodes.MOV_C_B, 1, "MOV C, B");
        public static Instruction MOV_C_C = new(OpCodes.MOV_C_C, 1, "MOV C, C");
        public static Instruction MOV_C_D = new(OpCodes.MOV_C_D, 1, "MOV C, D");
        public static Instruction MOV_C_E = new(OpCodes.MOV_C_E, 1, "MOV C, E");
        public static Instruction MOV_C_H = new(OpCodes.MOV_C_H, 1, "MOV C, H");
        public static Instruction MOV_C_L = new(OpCodes.MOV_C_L, 1, "MOV C, L");
        public static Instruction MOV_C_M = new(OpCodes.MOV_C_M, 1, "MOV C, M");
        public static Instruction MOV_C_A = new(OpCodes.MOV_C_A, 1, "MOV C, A");

        public static Instruction MOV_D_B = new(OpCodes.MOV_D_B, 1, "MOV D, B");
        public static Instruction MOV_D_C = new(OpCodes.MOV_D_C, 1, "MOV D, C");
        public static Instruction MOV_D_D = new(OpCodes.MOV_D_D, 1, "MOV D, D");
        public static Instruction MOV_D_E = new(OpCodes.MOV_D_E, 1, "MOV D, E");
        public static Instruction MOV_D_H = new(OpCodes.MOV_D_H, 1, "MOV D, H");
        public static Instruction MOV_D_L = new(OpCodes.MOV_D_L, 1, "MOV D, L");
        public static Instruction MOV_D_M = new(OpCodes.MOV_D_M, 1, "MOV D, M");
        public static Instruction MOV_D_A = new(OpCodes.MOV_D_A, 1, "MOV D, A");
        public static Instruction MOV_E_B = new(OpCodes.MOV_E_B, 1, "MOV E, B");
        public static Instruction MOV_E_C = new(OpCodes.MOV_E_C, 1, "MOV E, C");
        public static Instruction MOV_E_D = new(OpCodes.MOV_E_D, 1, "MOV E, D");
        public static Instruction MOV_E_E = new(OpCodes.MOV_E_E, 1, "MOV E, E");
        public static Instruction MOV_E_H = new(OpCodes.MOV_E_H, 1, "MOV E, H");
        public static Instruction MOV_E_L = new(OpCodes.MOV_E_L, 1, "MOV E, L");
        public static Instruction MOV_E_M = new(OpCodes.MOV_E_M, 1, "MOV E, M");
        public static Instruction MOV_E_A = new(OpCodes.MOV_E_A, 1, "MOV E, A");

        public static Instruction MOV_H_B = new(OpCodes.MOV_H_B, 1, "MOV H, B");
        public static Instruction MOV_H_C = new(OpCodes.MOV_H_C, 1, "MOV H, C");
        public static Instruction MOV_H_D = new(OpCodes.MOV_H_D, 1, "MOV H, D");
        public static Instruction MOV_H_E = new(OpCodes.MOV_H_E, 1, "MOV H, E");
        public static Instruction MOV_H_H = new(OpCodes.MOV_H_H, 1, "MOV H, H");
        public static Instruction MOV_H_L = new(OpCodes.MOV_H_L, 1, "MOV H, L");
        public static Instruction MOV_H_M = new(OpCodes.MOV_H_M, 1, "MOV H, M");
        public static Instruction MOV_H_A = new(OpCodes.MOV_H_A, 1, "MOV H, A");
        public static Instruction MOV_L_B = new(OpCodes.MOV_L_B, 1, "MOV L, B");
        public static Instruction MOV_L_C = new(OpCodes.MOV_L_C, 1, "MOV L, C");
        public static Instruction MOV_L_D = new(OpCodes.MOV_L_D, 1, "MOV L, D");
        public static Instruction MOV_L_E = new(OpCodes.MOV_L_E, 1, "MOV L, E");
        public static Instruction MOV_L_H = new(OpCodes.MOV_L_H, 1, "MOV L, H");
        public static Instruction MOV_L_L = new(OpCodes.MOV_L_L, 1, "MOV L, L");
        public static Instruction MOV_L_M = new(OpCodes.MOV_L_M, 1, "MOV L, M");
        public static Instruction MOV_L_A = new(OpCodes.MOV_L_A, 1, "MOV L, A");

        public static Instruction MOV_M_B = new(OpCodes.MOV_M_B, 1, "MOV M, B");
        public static Instruction MOV_M_C = new(OpCodes.MOV_M_C, 1, "MOV M, C");
        public static Instruction MOV_M_D = new(OpCodes.MOV_M_D, 1, "MOV M, D");
        public static Instruction MOV_M_E = new(OpCodes.MOV_M_E, 1, "MOV M, E");
        public static Instruction MOV_M_H = new(OpCodes.MOV_M_H, 1, "MOV M, H");
        public static Instruction MOV_M_L = new(OpCodes.MOV_M_L, 1, "MOV M, L");
        public static Instruction HLT = new(OpCodes.HLT, 1, "HLT");
        public static Instruction MOV_M_A = new(OpCodes.MOV_M_A, 1, "MOV M, A");
        public static Instruction MOV_A_B = new(OpCodes.MOV_A_B, 1, "MOV A, B");
        public static Instruction MOV_A_C = new(OpCodes.MOV_A_C, 1, "MOV A, C");
        public static Instruction MOV_A_D = new(OpCodes.MOV_A_D, 1, "MOV A, D");
        public static Instruction MOV_A_E = new(OpCodes.MOV_A_E, 1, "MOV A, E");
        public static Instruction MOV_A_H = new(OpCodes.MOV_A_H, 1, "MOV A, H");
        public static Instruction MOV_A_L = new(OpCodes.MOV_A_L, 1, "MOV A, L");
        public static Instruction MOV_A_M = new(OpCodes.MOV_A_M, 1, "MOV A, M");
        public static Instruction MOV_A_A = new(OpCodes.MOV_A_A, 1, "MOV A, A");

        public static Instruction ADD_B = new(OpCodes.ADD_B, 1, "ADD B");
        public static Instruction ADD_C = new(OpCodes.ADD_C, 1, "ADD C");
        public static Instruction ADD_D = new(OpCodes.ADD_D, 1, "ADD D");
        public static Instruction ADD_E = new(OpCodes.ADD_E, 1, "ADD E");
        public static Instruction ADD_H = new(OpCodes.ADD_H, 1, "ADD H");
        public static Instruction ADD_L = new(OpCodes.ADD_L, 1, "ADD L");
        public static Instruction ADD_M = new(OpCodes.ADD_M, 1, "ADD M");
        public static Instruction ADD_A = new(OpCodes.ADD_A, 1, "ADD A");
        public static Instruction ADC_B = new(OpCodes.ADC_B, 1, "ADC B");
        public static Instruction ADC_C = new(OpCodes.ADC_C, 1, "ADC C");
        public static Instruction ADC_D = new(OpCodes.ADC_D, 1, "ADC D");
        public static Instruction ADC_E = new(OpCodes.ADC_E, 1, "ADC E");
        public static Instruction ADC_H = new(OpCodes.ADC_H, 1, "ADC H");
        public static Instruction ADC_L = new(OpCodes.ADC_L, 1, "ADC L");
        public static Instruction ADC_M = new(OpCodes.ADC_M, 1, "ADC M");
        public static Instruction ADC_A = new(OpCodes.ADC_A, 1, "ADC A");

        public static Instruction SUB_B = new(OpCodes.SUB_B, 1, "SUB B");
        public static Instruction SUB_C = new(OpCodes.SUB_C, 1, "SUB C");
        public static Instruction SUB_D = new(OpCodes.SUB_D, 1, "SUB D");
        public static Instruction SUB_E = new(OpCodes.SUB_E, 1, "SUB E");
        public static Instruction SUB_H = new(OpCodes.SUB_H, 1, "SUB H");
        public static Instruction SUB_L = new(OpCodes.SUB_L, 1, "SUB L");
        public static Instruction SUB_M = new(OpCodes.SUB_M, 1, "SUB M");
        public static Instruction SUB_A = new(OpCodes.SUB_A, 1, "SUB A");
        public static Instruction SBB_B = new(OpCodes.SBB_B, 1, "SBB B");
        public static Instruction SBB_C = new(OpCodes.SBB_C, 1, "SBB C");
        public static Instruction SBB_D = new(OpCodes.SBB_D, 1, "SBB D");
        public static Instruction SBB_E = new(OpCodes.SBB_E, 1, "SBB E");
        public static Instruction SBB_H = new(OpCodes.SBB_H, 1, "SBB H");
        public static Instruction SBB_L = new(OpCodes.SBB_L, 1, "SBB L");
        public static Instruction SBB_M = new(OpCodes.SBB_M, 1, "SBB M");
        public static Instruction SBB_A = new(OpCodes.SBB_A, 1, "SBB A");

        public static Instruction ANA_B = new(OpCodes.ANA_B, 1, "ANA B");
        public static Instruction ANA_C = new(OpCodes.ANA_C, 1, "ANA C");
        public static Instruction ANA_D = new(OpCodes.ANA_D, 1, "ANA D");
        public static Instruction ANA_E = new(OpCodes.ANA_E, 1, "ANA E");
        public static Instruction ANA_H = new(OpCodes.ANA_H, 1, "ANA H");
        public static Instruction ANA_L = new(OpCodes.ANA_L, 1, "ANA L");
        public static Instruction ANA_M = new(OpCodes.ANA_M, 1, "ANA M");
        public static Instruction ANA_A = new(OpCodes.ANA_A, 1, "ANA A");
        public static Instruction XRA_B = new(OpCodes.XRA_B, 1, "XRA B");
        public static Instruction XRA_C = new(OpCodes.XRA_C, 1, "XRA C");
        public static Instruction XRA_D = new(OpCodes.XRA_D, 1, "XRA D");
        public static Instruction XRA_E = new(OpCodes.XRA_E, 1, "XRA E");
        public static Instruction XRA_H = new(OpCodes.XRA_H, 1, "XRA H");
        public static Instruction XRA_L = new(OpCodes.XRA_L, 1, "XRA L");
        public static Instruction XRA_M = new(OpCodes.XRA_M, 1, "XRA M");
        public static Instruction XRA_A = new(OpCodes.XRA_A, 1, "XRA A");

        public static Instruction ORA_B = new(OpCodes.ORA_B, 1, "ORA B");
        public static Instruction ORA_C = new(OpCodes.ORA_C, 1, "ORA C");
        public static Instruction ORA_D = new(OpCodes.ORA_D, 1, "ORA D");
        public static Instruction ORA_E = new(OpCodes.ORA_E, 1, "ORA E");
        public static Instruction ORA_H = new(OpCodes.ORA_H, 1, "ORA H");
        public static Instruction ORA_L = new(OpCodes.ORA_L, 1, "ORA L");
        public static Instruction ORA_M = new(OpCodes.ORA_M, 1, "ORA M");
        public static Instruction ORA_A = new(OpCodes.ORA_A, 1, "ORA A");
        public static Instruction CMP_B = new(OpCodes.CMP_B, 1, "CMP B");
        public static Instruction CMP_C = new(OpCodes.CMP_C, 1, "CMP C");
        public static Instruction CMP_D = new(OpCodes.CMP_D, 1, "CMP D");
        public static Instruction CMP_E = new(OpCodes.CMP_E, 1, "CMP E");
        public static Instruction CMP_H = new(OpCodes.CMP_H, 1, "CMP H");
        public static Instruction CMP_L = new(OpCodes.CMP_L, 1, "CMP L");
        public static Instruction CMP_M = new(OpCodes.CMP_M, 1, "CMP M");
        public static Instruction CMP_A = new(OpCodes.CMP_A, 1, "CMP A");

        public static Instruction RNZ = new(OpCodes.RNZ, 1, "RNZ");
        public static Instruction POP_B = new(OpCodes.POP_B, 1, "POP B");
        public static Instruction JNZ = new(OpCodes.JNZ, 3, "JNZ {addr}");
        public static Instruction JMP = new(OpCodes.JMP, 3, "JMP {addr}");
        public static Instruction CNZ = new(OpCodes.CNZ, 3, "CNZ {addr}");
        public static Instruction PUSH_B = new(OpCodes.PUSH_B, 1, "PUSH B");
        public static Instruction ADI = new(OpCodes.ADI, 2, "ADI {d8}");
        public static Instruction RST_0 = new(OpCodes.RST_0, 1, "RST 0");
        public static Instruction RZ = new(OpCodes.RZ, 1, "RZ");
        public static Instruction RET = new(OpCodes.RET, 1, "RET");
        public static Instruction JZ = new(OpCodes.JZ, 3, "JZ {addr}");
        //public static Instruction = 0xcb;
        public static Instruction CZ = new(OpCodes.CZ, 3, "CZ {addr}");
        public static Instruction CALL = new(OpCodes.CALL, 3, "CALL {addr}");
        public static Instruction ACI = new(OpCodes.ACI, 2, "ACI {d8}");
        public static Instruction RST_1 = new(OpCodes.RST_1, 1, "RST 1");

        public static Instruction RNC = new(OpCodes.RNC, 1, "RNC");
        public static Instruction POP_D = new(OpCodes.POP_D, 1, "POP D");
        public static Instruction JNC = new(OpCodes.JNC, 3, "JNC {addr}");
        public static Instruction OUT = new(OpCodes.OUT, 2, "OUT {d8}");
        public static Instruction CNC = new(OpCodes.CNC, 3, "CNC {addr}");
        public static Instruction PUSH_D = new(OpCodes.PUSH_D, 1, "PUSH D");
        public static Instruction SUI = new(OpCodes.SUI, 2, "SUI {d8}");
        public static Instruction RST_2 = new(OpCodes.RST_2, 1, "RST 2");
        public static Instruction RC = new(OpCodes.RC, 1, "RC");
        //public static Instruction = 0xd9;
        public static Instruction JC = new(OpCodes.JC, 3, "JC {addr}");
        public static Instruction IN = new(OpCodes.IN, 2, "IN {d8}");
        public static Instruction CC = new(OpCodes.CC, 3, "CC {addr}");
        //public static Instruction = 0xdd;
        public static Instruction SBI = new(OpCodes.SBI, 2, "SBI {d8}");
        public static Instruction RST_3 = new(OpCodes.RST_3, 1, "RST 3");

        public static Instruction RPO = new(OpCodes.RPO, 1, "RPO");
        public static Instruction POP_H = new(OpCodes.POP_H, 1, "POP H");
        public static Instruction JPO = new(OpCodes.JPO, 3, "JPO {addr}");
        public static Instruction XTHL = new(OpCodes.XTHL, 1, "XTHL");
        public static Instruction CPO = new(OpCodes.CPO, 3, "CPO {addr}");
        public static Instruction PUSH_H = new(OpCodes.PUSH_H, 1, "PUSH H");
        public static Instruction ANI = new(OpCodes.ANI, 2, "ANI {d8}");
        public static Instruction RST_4 = new(OpCodes.RST_4, 1, "RST 4");
        public static Instruction RPE = new(OpCodes.RPE, 1, "RPE");
        public static Instruction PCHL = new(OpCodes.PCHL, 1, "PCHL");
        public static Instruction JPE = new(OpCodes.JPE, 3, "JPE {addr}");
        public static Instruction XCHG = new(OpCodes.XCHG, 1, "XCHG");
        public static Instruction CPE = new(OpCodes.CPE, 3, "CPE {addr}");
        //public static Instruction = 0xed;
        public static Instruction XRI = new(OpCodes.XRI, 2, "XRI {d8}");
        public static Instruction RST_5 = new(OpCodes.RST_5, 1, "RST 5");

        public static Instruction RP = new(OpCodes.RP, 1, "RP");
        public static Instruction POP_PSW = new(OpCodes.POP_PSW, 1, "POP PSW");
        public static Instruction JP = new(OpCodes.JP, 3, "JP {addr}");
        public static Instruction DI = new(OpCodes.DI, 1, "DI");
        public static Instruction CP = new(OpCodes.CP, 3, "CP {addr}");
        public static Instruction PUSH_PSW = new(OpCodes.PUSH_PSW, 1, "PUSH PSW");
        public static Instruction ORI = new(OpCodes.ORI, 2, "ORI {d8}");
        public static Instruction RST_6 = new(OpCodes.RST_6, 1, "RST 6");
        public static Instruction RM = new(OpCodes.RM, 1, "RM");
        public static Instruction SPHL = new(OpCodes.SPHL, 1, "SPHL");
        public static Instruction JM = new(OpCodes.JM, 3, "JM {addr}");
        public static Instruction EI = new(OpCodes.EI, 1, "EI");
        public static Instruction CM = new(OpCodes.CM, 3, "CM {addr}");
        //public static Instruction = 0xfd;
        public static Instruction CPI = new(OpCodes.CPI, 2, "CPI {d8}");
        public static Instruction RST_7 = new(OpCodes.RST_7, 1, "RST 7");
    }
}