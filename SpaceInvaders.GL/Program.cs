
using Intel8080;
using Shared;
using System.IO;

internal class Program
{
    private static void Main(string[] args)
    {
        using var game = new SpaceInvaders.GL.Game1();
        game.Run();
    }
}