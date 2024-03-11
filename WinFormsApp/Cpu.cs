using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Intel8080
{
    public class Cpu
    {
        StringBuilder str = new StringBuilder();

        public Cpu()
        {
            PC = 0;
            Flags = new Flags();
            Memory = new Memory<byte>();
        }

        public Cpu(Memory<byte> memory)
            : this()
        {
            Memory = memory;
        }

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
        public bool INTE { get; set; }
        public byte Instruction { get; set; }
        public Flags Flags { get; set; }
        public Memory<byte> Memory { get; set; }

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

        public byte F => (byte)(Flags.Value & 0xfd);

        public override string ToString()
        {
            str.Clear();
            str.AppendLine($"AF: {A:x2}{F:x2}");
            str.AppendLine($"BC: {B:x2}{C:x2}");
            str.AppendLine($"DE: {D:x2}{E:x2}");
            str.AppendLine($"HL: {H:x2}{L:x2}");
            str.AppendLine($"PC: {PC:x4}");
            str.AppendLine($"SP: {SP:x4}");
            str.AppendLine(string.Format("Flags: {0}{1}{2}{3}{4}",
                Flags.Zero ? "z" : ".",
                Flags.Sign ? "s" : ".",
                Flags.Parity ? "p" : ".",
                Flags.AuxCarry ? "i" : ".",
                Flags.Carry ? "c" : "."));

            return str.ToString();
        }
    }

    public class Flags
    {
        public bool Sign { get; set; }
        public bool Zero { get; set; }
        public bool AuxCarry { get; set; }
        public bool Parity { get; set; }
        public bool Carry { get; set; }

        public byte Value
        {
            get
            {
                byte result = 0x2;

                if (Sign)
                    result |= (byte)CpuFlags.Sign;
                if (Zero)
                    result |= (byte)CpuFlags.Zero;
                if (AuxCarry)
                    result |= (byte)CpuFlags.AuxCarry;
                if (Parity)
                    result |= (byte)CpuFlags.Parity;
                if (Carry)
                    result |= (byte)CpuFlags.Carry;

                return result;
            }

            set
            {
                Sign = (value & (byte)CpuFlags.Sign) > 0;
                Zero = (value & (byte)CpuFlags.Zero) > 0;
                AuxCarry = (value & (byte)CpuFlags.AuxCarry) > 0;
                Parity = (value & (byte)CpuFlags.Parity) > 0;
                Carry = (value & (byte)CpuFlags.Carry) > 0;
            }
        }
    }

    public enum CpuFlags
    {
        Carry = 1,
        Parity = 4,
        AuxCarry = 16,
        Zero = 64,
        Sign = 128
    }
}
