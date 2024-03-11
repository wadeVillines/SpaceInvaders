using Intel8080;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shared;
using System;
using Memory = Shared.Memory<byte>;
using System.IO;
using System.Data;
using System.Diagnostics;

namespace SpaceInvaders.GL
{
    public class Game1 : Game
    {
        private const int ScreenWidth = 224;
        private const int ScreenHeight = 256;
        private const int Scale = 4;

        // microseconds
        private const double CpuClockPeriod = 0.5d;
        private const double FrameTime = 16666.67;
        private const double InterruptInterval = FrameTime / 2;

        private double RemainingStates = 0;
        private double TimeUntilNextInterrupt = InterruptInterval;
        private bool IsVBlank = true;

        private Memory Memory;
        private Cpu Cpu;
        private ShiftRegister ShiftRegister;

        Texture2D Screen;
        Color[] Pixels;
        Color[] PixelBuffer;

        KeyboardState KeyboardState;
        int ControlStateP1;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // set up guest hardware
            ShiftRegister = new ShiftRegister();
            Memory = new Memory(0xffff);
            Cpu = new Cpu(Memory);
            Cpu.OnPortRead += Cpu_OnPortRead;
            Cpu.OnPortWrite += Cpu_OnPortWrite;

            // fetch program bytes
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "SpaceInvaders");
            var progBytes = File.ReadAllBytes(filePath);

            // load program into guest memory
            Memory.Load(progBytes, 0x0);

            // set up graphics
            Window.Title = "Space Invaders";
            _graphics.PreferredBackBufferWidth = ScreenWidth * Scale;
            _graphics.PreferredBackBufferHeight = ScreenHeight * Scale;
            _graphics.ApplyChanges();

            Pixels = new Color[ScreenWidth * ScreenHeight];
            PixelBuffer = new Color[ScreenWidth * ScreenHeight];
            Screen = new Texture2D(_graphics.GraphicsDevice, ScreenWidth, ScreenHeight);

            base.Initialize();
        }

        private void Cpu_OnPortWrite(byte port, byte data)
        {
            switch (port)
            {
                case 0x2: // shift register offset (only bits 2..0 are writable)
                    ShiftRegister.ShiftOffset = (byte)(data & 0x7);
                    break;
                case 0x3:
                    break;
                case 0x4: // bit shift the shift register to the right by 1 byte
                    ShiftRegister.RightShift(data);
                    break;
                case 0x5:
                    break;
                case 0x6:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private byte Cpu_OnPortRead(byte port)
        {
            byte result;

            switch (port)
            {
                case 0x00:
                    return 0x0e;
                case 0x1:
                    result = (byte)(ControlStateP1 | 0b00001000);
                    break;
                case 0x2:
                    result = 0x80;
                    break;
                case 0x3:
                    result = ShiftRegister.Read();
                    break;
                default:
                    throw new NotImplementedException();
            }

            return result;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Screen = new Texture2D(GraphicsDevice, ScreenWidth, ScreenHeight);
            Pixels = new Color[ScreenWidth * ScreenHeight];
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // how much time has passed since last update?
            double deltaTime = gameTime.ElapsedGameTime.TotalMicroseconds;

            // how many cpu states can we run in that time?
            RemainingStates += (deltaTime / CpuClockPeriod);

            // run cpu for that many states
            while (RemainingStates > 0)
            {
                // is it time for the next interrupt?
                if (TimeUntilNextInterrupt < 0)
                {
                    IsVBlank = !IsVBlank;

                    if (IsVBlank)
                        Cpu.Interrupt(OpCodes.RST_2);
                    else
                        Cpu.Interrupt(OpCodes.RST_1);

                    TimeUntilNextInterrupt += InterruptInterval;
                }

                var states = Cpu.Step();
                TimeUntilNextInterrupt -= states * CpuClockPeriod;
                RemainingStates -= states;
            }

            // TODO: Add your update logic here
            DrawScreen();

            KeyboardState = Keyboard.GetState();
            ReadP1Controls();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            Screen.SetData(Pixels, 0, Pixels.Length);
            _spriteBatch.Draw(Screen, new Rectangle(0, 0, ScreenWidth * Scale, ScreenHeight * Scale), Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawScreen()
        {
            // this code rotates the game screen 90 degrees counter clockwise
            // in an actual arcade cabinet, the monitor is rotated in this way
            int row = ScreenHeight;
            int col = 0;

            for (int address = 0x2400; address < 0x4000; address++)
            {
                int data = Memory[address];

                if (row == 0)
                {
                    row = ScreenHeight;
                    col++;
                }

                for (int j = 0; j < 8; j++)
                {
                    Color pxColor = ((data >> j) & 0x01) == 0x01
                        ? Color.White
                        : Color.Black;
                    row--;
                    Pixels[row * ScreenWidth + col] = pxColor;
                }
            }
        }

        private void ReadP1Controls()
        {
            if (KeyboardState.IsKeyDown(Keys.Enter))
                ControlStateP1 |= (int)Port1Flags.P1Start;
            else
                ControlStateP1 &= ~(int)Port1Flags.P1Start;

            if (KeyboardState.IsKeyDown(Keys.Left))
                ControlStateP1 |= (int)Port1Flags.MoveLeft;
            else
                ControlStateP1 &= ~(int)Port1Flags.MoveLeft;

            if (KeyboardState.IsKeyDown(Keys.Right))
                ControlStateP1 |= (int)Port1Flags.MoveRight;
            else
                ControlStateP1 &= ~(int)Port1Flags.MoveRight;

            if (KeyboardState.IsKeyDown(Keys.Space))
                ControlStateP1 |= (int)Port1Flags.Shoot;
            else
                ControlStateP1 &= ~(int)Port1Flags.Shoot;

            if (KeyboardState.IsKeyDown(Keys.C))
                ControlStateP1 |= (int)Port1Flags.CoinInsert;
            else
                ControlStateP1 &= ~(int)Port1Flags.CoinInsert;
        }
    }

    public enum Port1Flags
    {
        CoinInsert = 0x01,
        P2Start = 0x02,
        P1Start = 0x04,
        Shoot = 0x10,
        MoveLeft = 0x20,
        MoveRight = 0x40
    }
}