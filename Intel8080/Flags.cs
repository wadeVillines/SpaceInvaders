namespace Intel8080
{
    public class Flags
    {
        private byte _value;

        public bool Sign { get; set; }
        public bool Zero { get; set; }
        public bool AuxCarry { get; set; }
        public bool Parity { get; set; }
        public bool Carry { get; set; }

        public byte Value
        {
            get
            {
                _value = 0x2;

                if (Sign)
                    _value |= (byte)FlagBits.Sign;
                if (Zero)
                    _value |= (byte)FlagBits.Zero;
                if (AuxCarry)
                    _value |= (byte)FlagBits.AuxCarry;
                if (Parity)
                    _value |= (byte)FlagBits.Parity;
                if (Carry)
                    _value |= (byte)FlagBits.Carry;

                return _value;
            }

            set
            {
                Sign = (value & (byte)FlagBits.Sign) > 0;
                Zero = (value & (byte)FlagBits.Zero) > 0;
                AuxCarry = (value & (byte)FlagBits.AuxCarry) > 0;
                Parity = (value & (byte)FlagBits.Parity) > 0;
                Carry = (value & (byte)FlagBits.Carry) > 0;
            }
        }

        public enum FlagBits
        {
            Carry = 1,
            Parity = 4,
            AuxCarry = 16,
            Zero = 64,
            Sign = 128
        }
    }
}
