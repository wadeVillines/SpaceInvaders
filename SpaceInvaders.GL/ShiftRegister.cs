using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders.GL
{
    public class ShiftRegister
    {
        public byte ShiftOffset { get; set; }
        public int ShiftRegisterHi { get; set; }
        public int ShiftRegisterLo { get; set; }

        public byte Read()
        {
            var register = (ShiftRegisterHi << 8) | ShiftRegisterLo;
            byte result = (byte)((register >> (8 - ShiftOffset)) & 0xff);
            return result;
        }

        public void RightShift(byte data)
        {
            ShiftRegisterLo = ShiftRegisterHi;
            ShiftRegisterHi = data;
        }

        public void CopyFrom(ShiftRegister copy)
        {
            ShiftOffset = copy.ShiftOffset;
            ShiftRegisterLo = copy.ShiftRegisterLo;
            ShiftRegisterHi = copy.ShiftRegisterHi;
        }
    }
}
