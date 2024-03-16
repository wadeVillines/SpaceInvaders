using Intel8080;
using Shared;
using System.Diagnostics;
using Memory = Shared.Memory<byte>;

internal class Program
{
    private static Memory _memory = new Memory(0xffff);
    private static Cpu _cpu = new Cpu(_memory);

    private static void Main(string[] args)
    {
        // fetch program bytes
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "cpudiag.bin");
        var progBytes = File.ReadAllBytes(filePath);

        // load program into guest memory
        _memory.Load(progBytes, 0x0100);
        _cpu.PC = 0x0100;

        // move stack pointer
        _memory[0x0170] = 0x07;

        // skip DAA test by jumping
        //_memory[0x059c] = 0xc3;
        //_memory[0x059d] = 0xc2;
        //_memory[0x059e] = 0x05;

        while (_cpu.PC < _memory.Count)
        {
            if (IsCalling()) // if next instruction is CALL
            {
                if (IsPrinting())
                {
                    if (_cpu.C == 0x9) // if C = 0x09
                    {
                        var offset = ((_cpu.D << 8) | (_cpu.E)) + 3;
                        string str = string.Empty;
                        while (_memory[offset] != '$')
                        {
                            str += (char)_memory[offset];
                            offset++;
                        }

                        Console.Write(str);
                        Console.Write("\n");
                    }
                    else if (_cpu.C == 0x2)
                    {
                        Console.Write("print char routine called\n");
                    }

                    // skip the CALL
                    _cpu.PC += 3;
                    continue;
                }
                else if (IsExiting())
                {
                    break;
                }
            }

            _cpu.Step();
        }

        Console.WriteLine("Done");
        Console.ReadLine();
    }

    private static bool IsCalling()
    {
        return _memory[_cpu.PC] == 0xcd;
    }

    private static bool IsExiting()
    {
        return
            IsCalling() &&
            _memory[_cpu.PC + 1] == 0x00 &&
            _memory[_cpu.PC + 2] == 0x00;
    }

    private static bool IsPrinting()
    {
        try
        {
            // is next instruction CALL $0005?
            return
                IsCalling() &&
                _memory[_cpu.PC + 1] == 0x05 &&
                _memory[_cpu.PC + 2] == 0x00;
        }
        catch
        {
        }

        return false;
    }
}