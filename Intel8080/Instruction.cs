using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Intel8080
{
    public class Instruction
    {
        public Instruction(byte code, int size, string? asm = null)
        {
            Code = code;
            Size = size;
            ASM = asm;
        }

        public byte Code { get; set; }
        public int Size { get; private set; }
        public string? ASM { get; set; }
    }
}