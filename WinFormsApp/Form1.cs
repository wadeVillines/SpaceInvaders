using Intel8080;
using Memory = Intel8080.Memory<byte>;

namespace WinFormsApp
{
    public partial class Form1 : Form
    {
        private static Memory memory = new Memory(0xffff);
        private static Cpu cpu = new Cpu(memory);
        private static Machine machine = new Machine();

        SolidBrush PixelOn = new SolidBrush(Color.White);
        SolidBrush PixelOff = new SolidBrush(Color.Black);

        public Form1()
        {
            InitializeComponent();

            while (true)
            {
                ExecuteInstruction();
            }
        }

        private static void ExecuteInstruction()
        {
            cpu.Instruction = memory[cpu.PC++];
            string? asm = null;

            switch (cpu.Instruction)
            {
                case 0x00: // NOP
                    asm = "NOP";
                    break;

                case 0x01: // LXI B, d16
                    cpu.C = memory[cpu.PC++];
                    cpu.B = memory[cpu.PC++];
                    asm = $"LXI B, ${cpu.BC:x4}";
                    break;

                case 0x05: // DCR B
                    cpu.B--;

                    cpu.Flags.Zero = cpu.B == 0;
                    cpu.Flags.Sign = cpu.B >= 0x80;
                    cpu.Flags.Parity = (cpu.B & 0x01) != 0x01;
                    cpu.Flags.AuxCarry = (cpu.B & 0xf) == 0xf;
                    asm = "DCR B";
                    break;

                case 0x06: // MVI B, d8
                    cpu.B = memory[cpu.PC++];
                    asm = $"MVI B, ${cpu.B:x2}";
                    break;

                case 0x09: // DAD B
                    cpu.Flags.Carry = (cpu.HL + cpu.BC) > 0xffff;
                    cpu.HL += cpu.BC;
                    asm = "DAD B";
                    break;

                case 0x0d: // DCR C
                    cpu.C--;

                    cpu.Flags.Zero = cpu.C == 0;
                    cpu.Flags.Sign = cpu.C >= 0x80;
                    cpu.Flags.Parity = (cpu.C & 0x01) != 0x01;
                    cpu.Flags.AuxCarry = (cpu.C & 0xf) == 0xf;
                    asm = "DCR C";
                    break;

                case 0x0e: // MVI C, d8
                    cpu.C = memory[cpu.PC++];
                    asm = $"MVI C, ${cpu.C:x2}";
                    break;

                case 0x0f: // RRC
                    cpu.Z = (byte)(cpu.A >> 1);
                    cpu.A = (byte)(cpu.Z | ((cpu.A & 0x01) << 7));
                    cpu.Flags.Carry = cpu.A >= 0x80;
                    asm = "RRC";
                    break;

                case 0x11: // LXI D, d16
                    cpu.E = memory[cpu.PC++];
                    cpu.D = memory[cpu.PC++];
                    asm = $"LXI D, ${cpu.DE:x4}";
                    break;

                case 0x13: // INX D
                    cpu.DE++;
                    asm = $"INX D";
                    break;

                case 0x19: // DAD D
                    cpu.Flags.Carry = cpu.HL + cpu.DE > 0xffff;
                    cpu.HL += cpu.DE;
                    asm = "DAD D";
                    break;

                case 0x1a: // LDAX D
                    cpu.A = memory[cpu.DE];
                    asm = $"LDAX D";
                    break;

                case 0x21: // LXI H, d16
                    cpu.L = memory[cpu.PC++];
                    cpu.H = memory[cpu.PC++];
                    asm = $"LXI H, ${cpu.HL:x4}";
                    break;

                case 0x23: // INX H
                    cpu.HL++;
                    asm = $"INX H";
                    break;

                case 0x26: // MVI H, d8
                    cpu.H = memory[cpu.PC++];
                    asm = $"MVI ${cpu.H:x2}";
                    break;

                case 0x29: // DAD H
                    cpu.Flags.Carry = (cpu.HL + cpu.HL) > 0xffff;
                    cpu.HL += cpu.HL;
                    asm = "DAD H";
                    break;

                case 0x31: // LXI SP, d16
                    cpu.Z = memory[cpu.PC++];
                    cpu.W = memory[cpu.PC++];
                    cpu.SP = cpu.WZ;
                    asm = $"LXI SP, ${cpu.WZ:x4}";
                    break;

                case 0x32: // STA addr
                    cpu.Z = memory[cpu.PC++];
                    cpu.W = memory[cpu.PC++];
                    memory[cpu.WZ] = cpu.A;
                    asm = $"STA ${cpu.WZ:x4}";
                    break;

                case 0x36: // MVI M, d8
                    cpu.Z = memory[cpu.PC++];
                    memory[cpu.HL] = cpu.Z;
                    asm = $"MVI M, ${cpu.Z:x2}";
                    break;

                case 0x3a: // LDA addr
                    cpu.Z = memory[cpu.PC++];
                    cpu.W = memory[cpu.PC++];
                    cpu.A = memory[cpu.WZ];
                    asm = $"LDA ${cpu.WZ:x4}";
                    break;

                case 0x3e: // MVI A, d8
                    cpu.A = memory[cpu.PC++];
                    asm = $"MVI A, ${cpu.A:x2}";
                    break;

                case 0x56: // MOV D, M
                    cpu.D = memory[cpu.HL];
                    asm = "MOV D, M";
                    break;

                case 0x5e: // MOV E, M
                    cpu.E = memory[cpu.HL];
                    asm = "MOV E, M";
                    break;

                case 0x66: // MOV H, M
                    cpu.H = memory[cpu.HL];
                    asm = "MOV H, M";
                    break;

                case 0x6f: // MOV L, A
                    cpu.L = cpu.A;
                    asm = "MOV L, A";
                    break;

                case 0x77: // MOV M, A
                    memory[cpu.HL] = cpu.A;
                    asm = $"MOV M, A";
                    break;

                case 0x7a: // MOV A, D
                    cpu.A = cpu.D;
                    asm = "MOV A, D";
                    break;

                case 0x7b: // MOV A, E
                    cpu.A = cpu.E;
                    asm = "MOV A, E";
                    break;

                case 0x7c: // MOV A, H
                    cpu.A = cpu.H;
                    asm = "MOV A, H";
                    break;

                case 0x7e: // MOV A, M
                    cpu.A = memory[cpu.HL];
                    asm = "MOV A, M";
                    break;

                case 0xa7: // ANA A
                    cpu.Z = (byte)(cpu.A & cpu.A);

                    cpu.Flags.Zero = cpu.Z == 0;
                    cpu.Flags.Sign = cpu.Z >= 0x80;
                    cpu.Flags.Parity = (cpu.Z & 0x01) != 0x01;
                    cpu.Flags.Carry = false;
                    cpu.Flags.AuxCarry =
                        (cpu.A & 0x8) == 0x8;

                    cpu.A = cpu.Z;
                    asm = "ANA A";
                    break;

                case 0xaf: // XRA A
                    cpu.Z = (byte)(cpu.A ^ cpu.A);

                    cpu.Flags.Zero = cpu.Z == 0;
                    cpu.Flags.Sign = cpu.Z >= 0x80;
                    cpu.Flags.Parity = (cpu.Z & 0x01) != 0x01;
                    cpu.Flags.Carry = false;
                    cpu.Flags.AuxCarry =
                        (cpu.A & 0x8) == 0x8;

                    cpu.A = cpu.Z;
                    asm = "XRA A";
                    break;

                case 0xc1: // POP B
                    cpu.C = memory[cpu.SP++];
                    cpu.B = memory[cpu.SP++];
                    asm = "POP B";
                    break;

                case 0xc2: // JNZ addr
                    cpu.Z = memory[cpu.PC++];
                    cpu.W = memory[cpu.PC++];

                    if (!cpu.Flags.Zero)
                        cpu.PC = cpu.WZ;

                    asm = $"JNZ ${cpu.WZ:x4}";
                    break;

                case 0xc3: // JMP addr
                    cpu.Z = memory[cpu.PC++];
                    cpu.W = memory[cpu.PC++];
                    cpu.PC = cpu.WZ;
                    asm = $"JMP ${cpu.WZ:x4}";
                    break;

                case 0xc5: // PUSH B
                    memory[--cpu.SP] = cpu.B;
                    memory[--cpu.SP] = cpu.C;
                    asm = "PUSH B";
                    break;

                case 0xc6: // ADI d8
                    cpu.Z = memory[cpu.PC++];
                    cpu.W = (byte)(cpu.A + cpu.Z);

                    cpu.Flags.Zero = cpu.W == 0;
                    cpu.Flags.Sign = cpu.W >= 0x80;
                    cpu.Flags.Parity = (cpu.W & 0x1) != 0x1;
                    cpu.Flags.Carry = (cpu.Z + cpu.A) > 0xff;
                    cpu.Flags.AuxCarry = (cpu.W & 0xf) == 0x0;

                    cpu.A = cpu.W;
                    asm = $"ADI ${cpu.Z:x2}";
                    break;

                case 0xc9: // RET
                    cpu.Z = memory[cpu.SP++];
                    cpu.W = memory[cpu.SP++];
                    cpu.PC = cpu.WZ;
                    asm = "RET";
                    break;

                case 0xcd: // CALL addr
                    cpu.Z = memory[cpu.PC++];
                    cpu.W = memory[cpu.PC++];
                    memory[--cpu.SP] = (byte)(cpu.PC >> 8);
                    memory[--cpu.SP] = (byte)cpu.PC;
                    cpu.PC = cpu.WZ;
                    asm = $"CALL ${cpu.WZ:x4}";
                    break;

                case 0xd1: // POP D
                    cpu.E = memory[cpu.SP++];
                    cpu.D = memory[cpu.SP++];
                    asm = "POP D";
                    break;

                case 0xd3: // OUT d8
                           // TODO: implement this instruction completely
                    cpu.Z = memory[cpu.PC++];
                    machine.WritePort(cpu.Z, cpu.A);
                    asm = $"OUT ${cpu.Z:x2}";
                    break;

                case 0xd5: // PUSH D
                    memory[--cpu.SP] = cpu.D;
                    memory[--cpu.SP] = cpu.E;
                    asm = "PUSH D";
                    break;

                case 0xe1: // POP H
                    cpu.L = memory[cpu.SP++];
                    cpu.H = memory[cpu.SP++];
                    asm = "POP H";
                    break;

                case 0xe5: // PUSH H
                    memory[--cpu.SP] = cpu.H;
                    memory[--cpu.SP] = cpu.L;
                    asm = "PUSH H";
                    break;

                case 0xe6: // ANI d8
                    cpu.Z = memory[cpu.PC++];
                    cpu.Z = (byte)(cpu.A & cpu.Z);

                    cpu.Flags.Zero = cpu.Z == 0;
                    cpu.Flags.Sign = cpu.Z >= 0x80;
                    cpu.Flags.Parity = (cpu.Z & 0x01) != 0x01;
                    cpu.Flags.Carry = false;
                    cpu.Flags.AuxCarry =
                        ((cpu.A | cpu.Z) & 0x8) == 0x8;

                    cpu.A = cpu.Z;
                    asm = $"ANI ${cpu.Z}";
                    break;

                case 0xeb: // XCHG
                    cpu.W = cpu.H; // swap H and D
                    cpu.H = cpu.D;
                    cpu.D = cpu.W;

                    cpu.W = cpu.L; // swap L and E
                    cpu.L = cpu.E;
                    cpu.E = cpu.W;
                    asm = "XCHG";
                    break;

                case 0xf1: // POP PSW
                    cpu.Flags.Value = memory[cpu.SP++];
                    cpu.A = memory[cpu.SP++];
                    asm = "POP PSW";
                    break;

                case 0xf5: // PUSH PSW
                    memory[--cpu.SP] = cpu.A;
                    memory[--cpu.SP] = cpu.Flags.Value;
                    asm = "PUSH PSW";
                    break;

                case 0xfb: // EI
                    cpu.INTE = true;
                    asm = "EI";
                    break;

                case 0xfe: // CPI d8
                    cpu.Z = memory[cpu.PC++];
                    cpu.W = (byte)(cpu.A - cpu.Z);

                    cpu.Flags.Zero = cpu.W == 0;
                    cpu.Flags.Sign = cpu.W >= 0x80;
                    cpu.Flags.Parity = (cpu.W & 0x1) != 0x1;
                    cpu.Flags.Carry = cpu.W > cpu.A;
                    cpu.Flags.AuxCarry = (cpu.W & 0xf) == 0xf;
                    asm = $"CPI ${cpu.Z:x2}";
                    break;

                default:
                    throw new NotImplementedException();
            }

            Console.WriteLine(asm);
        }


        private void SetPixel(Graphics g, int x, int y)
        {
            g.FillRectangle(PixelOn, x, y, 1, 1);
        }

        private void ClearPixel(Graphics g, int x, int y)
        {
            g.FillRectangle(PixelOff, x, y, 1, 1);
        }
    }
}
