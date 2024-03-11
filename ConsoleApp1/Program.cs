// See https://aka.ms/new-console-template for more information
using Intel8080;
using SDL2;
using Memory = Shared.Memory<byte>;

internal class Program
{
    private static void Main(string[] args)
    {
        Memory memory = new Memory(0xffff);
        Cpu cpu = new Cpu(memory);

        // get program bytes
        string path = "G:\\My Drive\\EmuDev\\Programs\\cpudiag.bin";
        byte[] bytes = File.ReadAllBytes(path);

        var initResult = SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

        // create game window
        var window = SDL.SDL_CreateWindow(
            "Space Invaders",
            SDL.SDL_WINDOWPOS_CENTERED,
            SDL.SDL_WINDOWPOS_CENTERED,
            224,
            256,
            SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

        // create renderer
        var renderer = SDL.SDL_CreateRenderer(
            window,
            -1,
            SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

        // scale the display
        SDL.SDL_RenderSetScale(renderer, 1, 1);

        //cpu.Start();

        Console.ReadLine();
    }
}