using Microsoft.Win32;
using System.Diagnostics;
using System.Text;
using Memory = Shared.Memory<byte>;

namespace Intel8080
{
    public class Cpu
    {
        private InstructionSet instructions = new InstructionSet();
        private Instruction? _instruction;
        private StringBuilder str = new StringBuilder();

        public Cpu()
        {
            PC = 0;
            Flags = new Flags();
            Memory = new Memory(0xffff);
        }

        public Cpu(Memory memory) : this()
        {
            Memory = memory;
        }

        public bool INTE { get; set; }
        public byte? InterruptRequest { get; set; }
        public Flags Flags { get; set; }
        public Memory Memory { get; set; }
        public byte A { get; set; }
        public byte B { get; set; }
        public byte C { get; set; }
        public byte D { get; set; }
        public byte E { get; set; }
        public byte H { get; set; }
        public byte L { get; set; }
        public byte W { get; set; }
        public byte Z { get; set; }
        public ushort PC { get; set; }
        public ushort SP { get; set; }
        public byte IR { get; set; }

        public ushort BC
        {
            get => (ushort)((B << 8) | C);
            set
            {
                B = (byte)(value >> 8);
                C = (byte)value;
            }
        }

        public ushort DE
        {
            get => (ushort)((D << 8) | E);
            set
            {
                D = (byte)(value >> 8);
                E = (byte)value;
            }
        }

        public ushort HL
        {
            get => (ushort)((H << 8) | L);
            set
            {
                H = (byte)(value >> 8);
                L = (byte)value;
            }
        }

        public ushort WZ
        {
            get => (ushort)((W << 8) | Z);
            set
            {
                W = (byte)(value >> 8);
                Z = (byte)value;
            }
        }

        public delegate void WritePortEvent(byte port, byte data);
        public event WritePortEvent? OnPortWrite;

        public delegate byte ReadPortEvent(byte port);
        public event ReadPortEvent? OnPortRead;

        private void SetFlags(
            byte? result = null, 
            bool? carry = null, 
            bool? auxCarry = null)
        {
            if (result.HasValue)
            {
                Flags.Zero = result == 0;
                Flags.Sign = (result & 0x80) == 0x80;

                // calculate parity flag by counting number of set bits
                Flags.Parity = true;
                for (int i = 0; i < 8; i++)
                {
                    if ((result & 0x1) == 1)
                        Flags.Parity = !Flags.Parity;
                
                    result = (byte)(result >> 1);
                }
            }

            if (carry.HasValue)
                Flags.Carry = carry.Value;

            if (auxCarry.HasValue)
                Flags.AuxCarry = auxCarry.Value;
        }

        public void Interrupt(byte request)
        {
            if (INTE)
                InterruptRequest = request;
        }

        public int Step()
        {
            // interrupts are handled on the machine cycle after they are received
            if (InterruptRequest.HasValue)
            {
                //INTE = false;
                IR = InterruptRequest.Value;
                InterruptRequest = null;
            }
            else
            {
                // fetch next instruction
                IR = Memory[PC];
                _instruction = instructions[IR];
                if (_instruction.Size > 1)
                    Z = Memory[PC + 1];
                if (_instruction.Size > 2)
                    W = Memory[PC + 2];

                PC = (ushort)(PC + _instruction.Size);
            }

            var states = Execute();
            return states;
        }

        public double Run(double remainingStates)
        {
            int states = 0;
            while (remainingStates > 0)
            {
                remainingStates -= states;
            }

            return remainingStates;
        }

        public int Execute()
        {
            int? states = null;

            switch (IR)
            {
                case OpCodes.NOP:
                    states = 4;
                    break;

                case OpCodes.LXI_B:
                    BC = WZ;
                    states = 10;
                    break;

                case OpCodes.STAX_B:
                    Memory[BC] = A;
                    states = 7;
                    break;

                case OpCodes.INX_B:
                    BC++;
                    states = 5;
                    break;

                case OpCodes.INR_B:
                    B++;
                    SetFlags(B);
                    states = 5;
                    break;

                case OpCodes.DCR_B:
                    B--;
                    SetFlags(B);
                    states = 5;
                    break;

                case OpCodes.MVI_B:
                    B = Z;
                    states = 7;
                    break;

                case OpCodes.RLC:
                    ROTATE_L();
                    states = 4;
                    break;

                case 0x08: throw new NotImplementedException();

                case OpCodes.DAD_B:
                    DAD(BC);
                    states = 10;
                    break;

                case OpCodes.LDAX_B:
                    A = Memory[BC];
                    states = 7;
                    break;

                case OpCodes.DCX_B:
                    BC--;
                    states = 5;
                    break;

                case OpCodes.INR_C:
                    C++;
                    SetFlags(C);
                    states = 5;
                    break;

                case OpCodes.DCR_C:
                    C--;
                    SetFlags(C);
                    states = 5;
                    break;

                case OpCodes.MVI_C:
                    C = Z;
                    states = 7;
                    break;

                case OpCodes.RRC:
                    ROTATE_R();
                    states = 4;
                    break;

                case 0x10: throw new NotImplementedException();

                case OpCodes.LXI_D:
                    DE = WZ;
                    states = 10;
                    break;

                case OpCodes.STAX_D:
                    Memory[DE] = A;
                    states = 7;
                    break;

                case OpCodes.INX_D:
                    DE++;
                    states = 5;
                    break;
                    throw new NotImplementedException();

                case OpCodes.INR_D:
                    D++;
                    SetFlags(D);
                    states = 5;
                    break;

                case OpCodes.DCR_D:
                    D--;
                    SetFlags(D);
                    states = 5;
                    break;

                case OpCodes.MVI_D:
                    D = Z;
                    states = 7;
                    break;

                case OpCodes.RAL:
                    ROTATE_L(rotateCarry: true);
                    states = 4;
                    break;

                case 0x18: throw new NotImplementedException();

                case OpCodes.DAD_D:
                    DAD(DE);
                    states = 10;
                    break;

                case OpCodes.LDAX_D:
                    A = Memory[DE];
                    states = 7;
                    break;

                case OpCodes.DCX_D:
                    DE--;
                    states = 5;
                    break;

                case OpCodes.INR_E:
                    E++;
                    SetFlags(E);
                    states = 5;
                    break;

                case OpCodes.DCR_E:
                    E--;
                    SetFlags(E);
                    states = 5;
                    break;

                case OpCodes.MVI_E:
                    E = Z;
                    states = 7;
                    break;

                case OpCodes.RAR:
                    ROTATE_R(rotateCarry: true);
                    states = 4;
                    break;

                case 0x20: throw new NotImplementedException();

                case OpCodes.LXI_H:
                    HL = WZ;
                    states = 10;
                    break;

                case OpCodes.SHLD:
                    Memory[WZ] = L;
                    Memory[WZ + 1] = H;
                    states = 16;
                    break;

                case OpCodes.INX_H:
                    HL++;
                    states = 5;
                    break;

                case OpCodes.INR_H:
                    H++;
                    SetFlags(H);
                    states = 5;
                    break;

                case OpCodes.DCR_H:
                    H--;
                    SetFlags(H);
                    states = 5;
                    break;

                case OpCodes.MVI_H:
                    H = Z;
                    states = 7;
                    break;

                case OpCodes.DAA:
                    DAA();
                    states = 4;
                    break;

                case 0x28: throw new NotImplementedException();

                case OpCodes.DAD_H:
                    DAD(HL);
                    states = 10;
                    break;

                case OpCodes.LHLD:
                    L = Memory[WZ];
                    H = Memory[WZ + 1];
                    states = 16;
                    break;

                case OpCodes.DCX_H:
                    HL--;
                    states = 5;
                    break;

                case OpCodes.INR_L:
                    L++;
                    SetFlags(L);
                    states = 5;
                    break;

                case OpCodes.DCR_L:
                    L--;
                    SetFlags(L);
                    states = 5;
                    break;

                case OpCodes.MVI_L:
                    L = Z;
                    states = 7;
                    break;

                case OpCodes.CMA:
                    A = (byte)~A;
                    states = 4;
                    break;

                case 0x30: throw new NotImplementedException();

                case OpCodes.LXI_SP:
                    SP = WZ;
                    states = 10;
                    break;

                case OpCodes.STA:
                    Memory[WZ] = A;
                    states = 13;
                    break;

                case OpCodes.INX_SP:
                    SP++;
                    states = 5;
                    break;

                case OpCodes.INR_M:
                    Memory[HL] = (byte)(Memory[HL] + 1);
                    SetFlags(Memory[HL]);
                    states = 10;
                    break;

                case OpCodes.DCR_M:
                    Memory[HL] = (byte)(Memory[HL] - 1);
                    SetFlags(Memory[HL]);
                    states = 10;
                    break;

                case OpCodes.MVI_M:
                    Memory[HL] = Z;
                    states = 10;
                    break;

                case OpCodes.STC:
                    Flags.Carry = true;
                    states = 4;
                    break;

                case 0x38: throw new NotImplementedException();

                case OpCodes.DAD_SP:
                    DAD(SP);
                    states = 10;
                    break;

                case OpCodes.LDA:
                    A = Memory[WZ];
                    states = 13;
                    break;

                case OpCodes.DCX_SP:
                    SP--;
                    states = 5;
                    break;

                case OpCodes.INR_A:
                    A++;
                    SetFlags(A);
                    states = 5;
                    break;

                case OpCodes.DCR_A:
                    A--;
                    SetFlags(A);
                    states = 5;
                    break;

                case OpCodes.MVI_A:
                    A = Z;
                    states = 7;
                    break;

                case OpCodes.CMC:
                    Flags.Carry = !Flags.Carry;
                    states = 4;
                    break;

                case OpCodes.MOV_B_B:
                    states = 5;
                    break;

                case OpCodes.MOV_B_C:
                    B = C;
                    states = 5;
                    break;

                case OpCodes.MOV_B_D:
                    B = D;
                    states = 5;
                    break;

                case OpCodes.MOV_B_E:
                    B = E;
                    states = 5;
                    break;

                case OpCodes.MOV_B_H:
                    B = H;
                    states = 5;
                    break;

                case OpCodes.MOV_B_L:
                    B = L;
                    states = 5;
                    break;

                case OpCodes.MOV_B_M:
                    B = Memory[HL];
                    states = 7;
                    break;

                case OpCodes.MOV_B_A:
                    B = A;
                    states = 5;
                    break;

                case OpCodes.MOV_C_B:
                    C = B;
                    states = 5;
                    break;

                case OpCodes.MOV_C_C:
                    states = 5;
                    break;

                case OpCodes.MOV_C_D:
                    C = D;
                    states = 5;
                    break;

                case OpCodes.MOV_C_E:
                    C = E;
                    states = 5;
                    break;

                case OpCodes.MOV_C_H:
                    C = H;
                    states = 5;
                    break;

                case OpCodes.MOV_C_L:
                    C = L;
                    states = 5;
                    break;

                case OpCodes.MOV_C_M:
                    C = Memory[HL];
                    states = 7;
                    break;

                case OpCodes.MOV_C_A:
                    C = A;
                    states = 5;
                    break;

                case OpCodes.MOV_D_B:
                    D = B;
                    states = 5;
                    break;

                case OpCodes.MOV_D_C:
                    D = C;
                    states = 5;
                    break;

                case OpCodes.MOV_D_D:
                    states = 5;
                    break;

                case OpCodes.MOV_D_E:
                    D = E;
                    states = 5;
                    break;

                case OpCodes.MOV_D_H:
                    D = H;
                    states = 5;
                    break;

                case OpCodes.MOV_D_L:
                    D = L;
                    states = 5;
                    break;

                case OpCodes.MOV_D_M:
                    D = Memory[HL];
                    states = 7;
                    break;

                case OpCodes.MOV_D_A:
                    D = A;
                    states = 5;
                    break;

                case OpCodes.MOV_E_B:
                    E = B;
                    states = 5;
                    break;

                case OpCodes.MOV_E_C:
                    E = C;
                    states = 5;
                    break;

                case OpCodes.MOV_E_D:
                    E = D;
                    states = 5;
                    break;

                case OpCodes.MOV_E_E:
                    states = 5;
                    break;

                case OpCodes.MOV_E_H:
                    E = H;
                    states = 5;
                    break;

                case OpCodes.MOV_E_L:
                    E = L;
                    states = 5;
                    break;

                case OpCodes.MOV_E_M:
                    E = Memory[HL];
                    states = 7;
                    break;

                case OpCodes.MOV_E_A:
                    E = A;
                    states = 5;
                    break;

                case OpCodes.MOV_H_B:
                    H = B;
                    states = 5;
                    break;

                case OpCodes.MOV_H_C:
                    H = C;
                    states = 5;
                    break;

                case OpCodes.MOV_H_D:
                    H = D;
                    states = 5;
                    break;

                case OpCodes.MOV_H_E:
                    H = E;
                    states = 5;
                    break;

                case OpCodes.MOV_H_H:
                    states = 5;
                    break;

                case OpCodes.MOV_H_L:
                    H = L;
                    states = 5;
                    break;

                case OpCodes.MOV_H_M:
                    H = Memory[HL];
                    states = 7;
                    break;

                case OpCodes.MOV_H_A:
                    H = A;
                    states = 5;
                    break;

                case OpCodes.MOV_L_B:
                    L = B;
                    states = 5;
                    break;

                case OpCodes.MOV_L_C:
                    L = C;
                    states = 5;
                    break;

                case OpCodes.MOV_L_D:
                    L = D;
                    states = 5;
                    break;

                case OpCodes.MOV_L_E:
                    L = E;
                    states = 5;
                    break;

                case OpCodes.MOV_L_H:
                    L = H;
                    states = 5;
                    break;

                case OpCodes.MOV_L_L:
                    states = 5;
                    break;

                case OpCodes.MOV_L_M:
                    L = Memory[HL];
                    states = 7;
                    break;

                case OpCodes.MOV_L_A:
                    L = A;
                    states = 5;
                    break;

                case OpCodes.MOV_M_B:
                    Memory[HL] = B;
                    states = 7;
                    break;

                case OpCodes.MOV_M_C:
                    Memory[HL] = C;
                    states = 7;
                    break;

                case OpCodes.MOV_M_D:
                    Memory[HL] = D;
                    states = 7;
                    break;

                case OpCodes.MOV_M_E:
                    Memory[HL] = E;
                    states = 7;
                    break;

                case OpCodes.MOV_M_H:
                    Memory[HL] = H;
                    states = 7;
                    break;

                case OpCodes.MOV_M_L:
                    Memory[HL] = L;
                    states = 7;
                    break;

                case OpCodes.HLT: 
                    throw new NotImplementedException();

                case OpCodes.MOV_M_A:
                    Memory[HL] = A;
                    states = 7;
                    break;

                case OpCodes.MOV_A_B:
                    A = B;
                    states = 5;
                    break;

                case OpCodes.MOV_A_C:
                    A = C;
                    states = 5;
                    break;

                case OpCodes.MOV_A_D:
                    A = D;
                    states = 5;
                    break;

                case OpCodes.MOV_A_E:
                    A = E;
                    states = 5;
                    break;

                case OpCodes.MOV_A_H:
                    A = H;
                    states = 5;
                    break;

                case OpCodes.MOV_A_L:
                    A = L;
                    states = 5;
                    break;

                case OpCodes.MOV_A_M:
                    A = Memory[HL];
                    states = 7;
                    break;

                case OpCodes.MOV_A_A:
                    states = 5;
                    break;

                case OpCodes.ADD_B:
                    ADD(B);
                    states = 4;
                    break;

                case OpCodes.ADD_C:
                    ADD(C);
                    states = 4;
                    break;

                case OpCodes.ADD_D:
                    ADD(D);
                    states = 4;
                    break;

                case OpCodes.ADD_E:
                    ADD(E);
                    states = 4;
                    break;

                case OpCodes.ADD_H:
                    ADD(H);
                    states = 4;
                    break;

                case OpCodes.ADD_L:
                    ADD(L);
                    states = 4;
                    break;

                case OpCodes.ADD_M:
                    ADD(Memory[HL]);
                    states = 7;
                    break;

                case OpCodes.ADD_A:
                    ADD(A);
                    states = 4;
                    break;

                case OpCodes.ADC_B:
                    ADD(B, withCarry: true);
                    states = 4;
                    break;

                case OpCodes.ADC_C:
                    ADD(C, withCarry: true);
                    states = 4;
                    break;

                case OpCodes.ADC_D:
                    ADD(D, withCarry: true);
                    states = 4;
                    break;

                case OpCodes.ADC_E:
                    ADD(E, withCarry: true);
                    states = 4;
                    break;

                case OpCodes.ADC_H:
                    ADD(H, withCarry: true);
                    states = 4;
                    break;

                case OpCodes.ADC_L:
                    ADD(L, withCarry: true);
                    states = 4;
                    break;

                case OpCodes.ADC_M:
                    ADD(Memory[HL], withCarry: true);
                    states = 7;
                    break;

                case OpCodes.ADC_A:
                    ADD(A, withCarry: true);
                    states = 4;
                    break;

                case OpCodes.SUB_B:
                    SUB(B);
                    states = 4;
                    break;

                case OpCodes.SUB_C:
                    SUB(C);
                    states = 4;
                    break;

                case OpCodes.SUB_D:
                    SUB(D);
                    states = 4;
                    break;

                case OpCodes.SUB_E:
                    SUB(E);
                    states = 4;
                    break;

                case OpCodes.SUB_H:
                    SUB(H);
                    states = 4;
                    break;

                case OpCodes.SUB_L:
                    SUB(L);
                    states = 4;
                    break;

                case OpCodes.SUB_M:
                    SUB(Memory[HL]);
                    states = 7;
                    break;

                case OpCodes.SUB_A:
                    SUB(A);
                    states = 4;
                    break;

                case OpCodes.SBB_B:
                    SUB(B, withBorrow: true);
                    states = 4;
                    break;

                case OpCodes.SBB_C:
                    SUB(C, withBorrow: true);
                    states = 4;
                    break;

                case OpCodes.SBB_D:
                    SUB(D, withBorrow: true);
                    states = 4;
                    break;

                case OpCodes.SBB_E:
                    SUB(E, withBorrow: true);
                    states = 4;
                    break;

                case OpCodes.SBB_H:
                    SUB(H, withBorrow: true);
                    states = 4;
                    break;

                case OpCodes.SBB_L:
                    SUB(L, withBorrow: true);
                    states = 4;
                    break;

                case OpCodes.SBB_M:
                    SUB(Memory[HL], withBorrow: true);
                    states = 7;
                    break;

                case OpCodes.SBB_A:
                    SUB(A, withBorrow: true);
                    states = 4;
                    break;

                case OpCodes.ANA_B:
                    AND(B);
                    states = 4;
                    break;

                case OpCodes.ANA_C:
                    AND(C);
                    states = 4;
                    break;

                case OpCodes.ANA_D:
                    AND(D);
                    states = 4;
                    break;

                case OpCodes.ANA_E:
                    AND(E);
                    states = 4;
                    break;

                case OpCodes.ANA_H:
                    AND(H);
                    states = 4;
                    break;

                case OpCodes.ANA_L:
                    AND(L);
                    states = 4;
                    break;

                case OpCodes.ANA_M:
                    AND(Memory[HL]);
                    states = 7;
                    break;

                case OpCodes.ANA_A:
                    AND(A);
                    states = 4;
                    break;

                case OpCodes.XRA_B:
                    XOR(B);
                    states = 4;
                    break;

                case OpCodes.XRA_C:
                    XOR(C);
                    states = 4;
                    break;

                case OpCodes.XRA_D:
                    XOR(D);
                    states = 4;
                    break;

                case OpCodes.XRA_E:
                    XOR(E);
                    states = 4;
                    break;

                case OpCodes.XRA_H:
                    XOR(H);
                    states = 4;
                    break;

                case OpCodes.XRA_L:
                    XOR(L);
                    states = 4;
                    break;

                case OpCodes.XRA_M:
                    XOR(Memory[HL]);
                    states = 7;
                    break;

                case OpCodes.XRA_A:
                    XOR(A);
                    states = 4;
                    break;

                case OpCodes.ORA_B:
                    OR(B);
                    states = 4;
                    break;

                case OpCodes.ORA_C:
                    OR(C);
                    states = 4;
                    break;

                case OpCodes.ORA_D:
                    OR(D);
                    states = 4;
                    break;

                case OpCodes.ORA_E:
                    OR(E);
                    states = 4;
                    break;

                case OpCodes.ORA_H:
                    OR(H);
                    states = 4;
                    break;

                case OpCodes.ORA_L:
                    OR(L);
                    states = 4;
                    break;

                case OpCodes.ORA_M:
                    OR(Memory[HL]);
                    states = 7;
                    break;

                case OpCodes.ORA_A:
                    OR(A);
                    states = 4;
                    break;

                case OpCodes.CMP_B:
                    COMPARE(B);
                    states = 4;
                    break;

                case OpCodes.CMP_C:
                    COMPARE(C);
                    states = 4;
                    break;

                case OpCodes.CMP_D:
                    COMPARE(D);
                    states = 4;
                    break;

                case OpCodes.CMP_E:
                    COMPARE(E);
                    states = 4;
                    break;

                case OpCodes.CMP_H:
                    COMPARE(H);
                    states = 4;
                    break;

                case OpCodes.CMP_L:
                    COMPARE(L);
                    states = 4;
                    break;

                case OpCodes.CMP_M:
                    COMPARE(Memory[HL]);
                    states = 7;
                    break;

                case OpCodes.CMP_A:
                    COMPARE(A);
                    states = 4;
                    break;

                case OpCodes.RNZ:
                    states = 5;
                    if (!Flags.Zero)
                    {
                        RET();
                        states = 11;
                    }
                    break;

                case OpCodes.POP_B:
                    C = POP();
                    B = POP();
                    states = 10;
                    break;

                case OpCodes.JNZ:
                    if (!Flags.Zero)
                        PC = WZ;
                    states = 10;
                    break;

                case OpCodes.JMP:
                    PC = WZ;
                    states = 10;
                    break;

                case OpCodes.CNZ:
                    states = 11;
                    if (!Flags.Zero)
                    {
                        CALL(WZ);
                        states = 17;
                    }
                    break;

                case OpCodes.PUSH_B:
                    PUSH(B);
                    PUSH(C);
                    states = 11;
                    break;

                case OpCodes.ADI:
                    ADD(Z);
                    states = 7;
                    break;

                case OpCodes.RST_0:
                    CALL(0x0000);
                    states = 11;
                    break;

                case OpCodes.RZ:
                    states = 5;
                    if (Flags.Zero)
                    {
                        RET();
                        states = 11;
                    }
                    break;

                case OpCodes.RET:
                    RET();
                    states = 10;
                    break;

                case OpCodes.JZ:
                    if (Flags.Zero)
                        PC = WZ;
                    states = 10;
                    break;

                case 0xcb: throw new NotImplementedException();

                case OpCodes.CZ:
                    states = 11;
                    if (Flags.Zero)
                    {
                        CALL(WZ);
                        states = 17;
                    }
                    break;

                case OpCodes.CALL:
                    CALL(WZ);
                    states = 17;
                    break;

                case OpCodes.ACI:
                    ADD(Z, withCarry: true);
                    states = 7;
                    break;

                case OpCodes.RST_1:
                    CALL(0x0008);
                    states = 11;
                    break;

                case OpCodes.RNC:
                    states = 5;
                    if (!Flags.Carry)
                    {
                        RET();
                        states = 11;
                    }
                    break;

                case OpCodes.POP_D:
                    E = POP();
                    D = POP();
                    states = 10;
                    break;

                case OpCodes.JNC:
                    if (!Flags.Carry)
                        PC = WZ;
                    states = 10;
                    break;

                case OpCodes.OUT:
                    OnPortWrite!(Z, A);
                    states = 10;
                    break;

                case OpCodes.CNC:
                    states = 11;
                    if (!Flags.Carry)
                    {
                        CALL(WZ);
                        states = 17;
                    }
                    break;

                case OpCodes.PUSH_D:
                    PUSH(D);
                    PUSH(E);
                    states = 11;
                    break;

                case OpCodes.SUI:
                    SUB(Z);
                    states = 7;
                    break;

                case OpCodes.RST_2:
                    CALL(0x0010);
                    states = 11;
                    break;

                case OpCodes.RC:
                    states = 5;
                    if (Flags.Carry)
                    {
                        RET();
                        states = 11;
                    }
                    break;

                case 0xd9: throw new NotImplementedException();

                case OpCodes.JC:
                    if (Flags.Carry)
                        PC = WZ;
                    states = 10;
                    break;

                case OpCodes.IN:
                    A = OnPortRead!(Z);
                    states = 10;
                    break;

                case OpCodes.CC:
                    states = 11;
                    if (Flags.Carry)
                    {
                        CALL(WZ);
                        states = 17;
                    }
                    break;

                case 0xdd: throw new NotImplementedException();

                case OpCodes.SBI:
                    SUB(Z, withBorrow: true);
                    states = 7;
                    break;

                case OpCodes.RST_3:
                    CALL(0x0018);
                    states = 11;
                    break;

                case OpCodes.RPO:
                    states = 5;
                    if (!Flags.Parity)
                    {
                        RET();
                        states = 11;
                    }
                    break;

                case OpCodes.POP_H:
                    L = POP();
                    H = POP();
                    states = 10;
                    break;

                case OpCodes.JPO:
                    if (!Flags.Parity)
                        PC = WZ;
                    states = 10;
                    break;

                case OpCodes.XTHL:
                    WZ = HL;
                    L = Memory[SP];
                    H = Memory[SP + 1];
                    Memory[SP] = Z;
                    Memory[SP + 1] = W;
                    states = 18;
                    break;

                case OpCodes.CPO:
                    states = 11;
                    if (!Flags.Parity)
                    {
                        CALL(WZ);
                        states = 17;
                    }
                    break;

                case OpCodes.PUSH_H:
                    PUSH(H);
                    PUSH(L);
                    states = 11;
                    break;

                case OpCodes.ANI:
                    AND(Z);
                    states = 7;
                    break;

                case OpCodes.RST_4:
                    CALL(0x0020);
                    states = 11;
                    break;

                case OpCodes.RPE:
                    states = 5;
                    if (Flags.Parity)
                    {
                        RET();
                        states = 11;
                    }
                    break;

                case OpCodes.PCHL:
                    PC = HL;
                    states = 5;
                    break;

                case OpCodes.JPE:
                    if (Flags.Parity)
                        PC = WZ;
                    states = 10;
                    break;

                case OpCodes.XCHG:
                    WZ = HL;
                    HL = DE;
                    DE = WZ;
                    states = 4;
                    break;

                case OpCodes.CPE:
                    states = 11;
                    if (Flags.Parity)
                    {
                        CALL(WZ);
                        states = 17;
                    }
                    break;

                case 0xed: throw new NotImplementedException();

                case OpCodes.XRI:
                    XOR(Z);
                    states = 7;
                    break;

                case OpCodes.RST_5:
                    CALL(0x0028);
                    states = 11;
                    break;

                case OpCodes.RP:
                    states = 5;
                    if (!Flags.Sign)
                    {
                        RET();
                        states = 11;
                    }
                    break;

                case OpCodes.POP_PSW:
                    Flags.Value = POP();
                    A = POP();
                    states = 10;
                    break;

                case OpCodes.JP:
                    if (!Flags.Sign)
                        PC = WZ;
                    states = 10;
                    break;

                case OpCodes.DI:
                    INTE = false;
                    states = 4;
                    break;

                case OpCodes.CP:
                    states = 11;
                    if (!Flags.Sign)
                    {
                        CALL(WZ);
                        states = 17;
                    }
                    break;

                case OpCodes.PUSH_PSW:
                    PUSH(A);
                    PUSH(Flags.Value);
                    states = 11;
                    break;

                case OpCodes.ORI:
                    OR(Z);
                    states = 7;
                    break;

                case OpCodes.RST_6:
                    CALL(0x0030);
                    states = 11;
                    break;

                case OpCodes.RM:
                    states = 5;
                    if (Flags.Sign)
                    {
                        RET();
                        states = 11;
                    }
                    break;

                case OpCodes.SPHL:
                    SP = HL;
                    states = 5;
                    break;

                case OpCodes.JM:
                    if (Flags.Sign)
                        PC = WZ;
                    states = 10;
                    break;

                case OpCodes.EI:
                    INTE = true;
                    states = 4;
                    break;

                case OpCodes.CM:
                    states = 11;
                    if (Flags.Sign)
                    {
                        CALL(WZ);
                        states = 17;
                    }
                    break;

                case 0xfd: throw new NotImplementedException();

                case OpCodes.CPI:
                    COMPARE(Z);
                    states = 7;
                    break;

                case OpCodes.RST_7:
                    CALL(0x0038);
                    states = 11;
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (!states.HasValue)
                throw new NotImplementedException();

            // Console.WriteLine(_instruction!.ASM);

            return states.Value;
        }

        private void CALL(ushort address)
        {
            PUSH((byte)(PC >> 8));
            PUSH((byte)PC);
            PC = address;
        }

        private void RET()
        {
            PC = POP();
            PC |= (ushort)(POP() << 8);
        }

        private void ADD(byte operand, bool withCarry = false)
        {
            if (withCarry && Flags.Carry)
                operand += 1;

            var result = A + operand;

            // an aux carry occurs when the sum of lower nybbles is greater than 0x0f
            bool auxCarry =
                (A & 0xf) >= 0x08 &&
                (operand & 0xf) >= 0x08;

            A = (byte)result;

            SetFlags(A, 
                carry: result > 0xff,
                auxCarry: auxCarry);
        }

        private void DAD(ushort operand)
        {
            var result = HL + operand;
            Flags.Carry = result > 0xffff;
            HL = (ushort)result;
        }

        private void SUB(byte operand, bool withBorrow = false)
        {
            if (withBorrow && Flags.Carry)
                operand -= 1;

            var result = A - operand;

            // an aux borrow occurs when the subtrahend's lower nybble is greater than the minuend's lower nybble
            // result = minuend - subtrahend
            bool auxBorrow = (A & 0x0f) < (operand & 0x0f);

            A = (byte)result;

            SetFlags(A,
                carry: result < 0,
                auxCarry: auxBorrow);
        }

        private void DAA()
        {
            // convert Accumulator contents to Binary Coded Decimal format
            bool auxCarry = false;
            bool carry = false;

            // 1) if the lo nybble is greater than 9, or the AC flag is set, add 6 to Accumulator
            // does this addition apperation set any flags?
            byte nybble = (byte)(A & 0x0f);
            if (nybble > 9 || Flags.AuxCarry)
            {
                A += 6;
                auxCarry = nybble >= 9;
            }

            // 2) if the hi nybble is now greater than 9, or the CY flag is set, add 6 to the hi nybble
            // does this addition operation set any flags?
            nybble = (byte)(A >> 4);
            if (nybble > 9 || Flags.Carry)
            {
                carry = nybble >= 9;
                nybble += 6;
                A = (byte)((nybble << 4) | (A & 0x0f));
            }

            SetFlags(A,
                carry: carry,
                auxCarry: auxCarry);
        }

        private void COMPARE(byte operand)
        {
            var result = (byte)(A - operand);

            bool auxBorrow = (A & 0x0f) < (operand & 0x0f);

            SetFlags(result, 
                carry: operand > A,
                auxCarry: auxBorrow);
        }

        private void AND(byte operand)
        {
            A = (byte)(A & operand);
            SetFlags(A, false, false);
        }

        private void OR(byte operand)
        {
            A = (byte)(A | operand);
            SetFlags(A, false, false);
        }

        private void XOR(byte operand)
        {
            A = (byte)(A ^ operand);
            SetFlags(A, false, false);
        }

        private void ROTATE_L(bool rotateCarry = false)
        {
            var result = A << 1;

            if (rotateCarry)
            {
                if (Flags.Carry)
                    result |= 0x1;
            }
            else
            {
                result |= (A >> 7);
            }

            Flags.Carry = (A & 0x80) > 0;
            A = (byte)result;
        }

        private void ROTATE_R(bool rotateCarry = false)
        {
            var result = A >> 1;

            if (rotateCarry)
            {
                if (Flags.Carry)
                    result |= 0x80;
            }
            else
            {
                result |= (A << 7);
            }

            Flags.Carry = (A & 0x01) > 0;
            A = (byte)result;
        }

        private void PUSH(byte value)
        {
            Memory[--SP] = value;
        }

        private byte POP()
        {
            return Memory[SP++];
        }

        public override string ToString()
        {
            str.Clear();
            str.AppendLine($"AF: {A:x2}{Flags.Value & 0xfd:x2}");
            str.AppendLine($"BC: {B:x2}{C:x2}");
            str.AppendLine($"DE: {D:x2}{E:x2}");
            str.AppendLine($"HL: {H:x2}{L:x2}");
            str.AppendLine($"PC: {PC:x4}");
            str.AppendLine($"SP: {SP:x4}");

            return str.ToString();
        }
    }
}