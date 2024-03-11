namespace Intel8080
{
    public class Machine
    {
        // 16 bit shift register
        public byte ShiftRegisterHi { get; set; }
        public byte ShiftRegisterLo { get; set; }
        private byte ShiftOffset { get; set; }

        public byte ReadPort(byte port)
        {
            byte result = 0;

            switch (port)
            {
                case 3: // read shift register
                    var shiftRegister = (ShiftRegisterHi << 8) | ShiftRegisterLo;
                    result = (byte)(shiftRegister >> (8 - ShiftOffset));
                    break;
            }

            return result;
        }

        public void WritePort(byte port, byte value)
        {
            switch (port)
            {
                case 2: // shift register result offset (bits 0,1,2)
                    ShiftOffset = (byte)(value & 0x7);
                    break;

                case 4: // fill the shift register
                    ShiftRegisterLo = ShiftRegisterHi;
                    ShiftRegisterHi = value;
                    break;
            }
        }
    }
}