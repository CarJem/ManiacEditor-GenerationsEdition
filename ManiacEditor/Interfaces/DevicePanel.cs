﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
//using Microsoft.DirectX;
using SharpDX;
using SharpDX.Direct3D9;
//using Microsoft.DirectX.Direct3D;
using SharpDX.Windows;

using Font = SharpDX.Direct3D9.Font;
using Rectangle = System.Drawing.Rectangle;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Bitmap = System.Drawing.Bitmap;
/*
using ResultCode = SharpDX.Direct3D9.ResultCode;
using Vector3 = SharpDX.Vector3;
using Matrix = SharpDX.Matrix;
using DeviceType = SharpDX.Direct3D9.DeviceType;
using DeviceCaps = SharpDX.Direct3D9.DeviceCaps;
using Device = SharpDX.Direct3D9.Device;
using CreateFlags = SharpDX.Direct3D9.CreateFlags;
using Texture = SharpDX.Direct3D9.Texture;
using TextureFilter = SharpDX.Direct3D9.TextureFilter;
using SwapEffect = SharpDX.Direct3D9.SwapEffect;
using PresentParameters = SharpDX.Direct3D9.PresentParameters;
using ClearFlags = SharpDX.Direct3D9.ClearFlags;
using Surface = Microsoft.DirectX.Direct3D.Surface;
using FontWeight = SharpDX.Direct3D9.FontWeight;
using FontQuality = SharpDX.Direct3D9.FontQuality;
using Sprite = SharpDX.Direct3D9.Sprite;
using FontDescription = SharpDX.Direct3D9.FontDescription;
using SpriteFlags = SharpDX.Direct3D9.SpriteFlags;
using ImageFileFormat = SharpDX.Direct3D9.ImageFileFormat;*/


/*
 * This file is a ported old rendering code
 * It will be replaced with OpenGL soon (Update: Or never if this pace keeps up - CarJem Generations)
 */


namespace ManiacEditor
{
    public partial class DevicePanel : UserControl
    {
        public Editor EditorInstance;

        #region Members

        public bool mouseMoved = false;
        public bool renderInProgress = false;
        DialogResult deviceExceptionResult;

        public int DrawWidth;
        public int DrawHeight;

        public int ScreenPosWidth;
        public int ScreenPosHeight;

        private Sprite sprite;
        private Sprite sprite2;
        private Sprite HUD;
        Texture tx;
        Bitmap txb;
        Texture tcircle;
        Texture tecircle;
        Texture hvcursor;
        Bitmap hvcursorb;
        Texture vcursor;
        Bitmap vcursorb;
        Texture hcursor;
        Bitmap hcursorb;
        Bitmap tcircleb;
        Bitmap tecircleb;

        public bool bRender = true;

        // The DirectX device
        public Device _device = null;
        public bool deviceLost;
        private Direct3D direct3d = new Direct3D();
        private Font font;
        private Font fontBold;
        public VertexBuffer vb;
        // The Form to place the DevicePanel onto
        public IDrawArea _parent = null;
        private PresentParameters presentParams;
        // On this event we can start to render our scene
        public event RenderEventHandler OnRender;

        // Now we know that the device is created
        public event CreateDeviceEventHandler OnCreateDevice;

        public MouseEventArgs lastEvent;

        

        #endregion

        #region ctor

        public DevicePanel(Editor instance = null)
        {
            InitializeComponent();
            EditorInstance = instance;
        }

        #endregion

        #region Init DX

        /// <summary>
        /// Init the DirectX-Stuff here
        /// </summary>
        /// <param name="parent">parent of the DevicePanel</param>
        /// 

        public void Init(IDrawArea parent)
        {
            try
            {
                _parent = parent;

                // Setup our D3D stuff
                presentParams = new PresentParameters();
                presentParams.Windowed = true;
                presentParams.SwapEffect = SwapEffect.Discard;


                Capabilities caps = direct3d.Adapters.First().GetCaps(DeviceType.Hardware);

                CreateFlags createFlags;

                if ((caps.DeviceCaps & DeviceCaps.HWTransformAndLight) != 0)
                {
                    createFlags = CreateFlags.HardwareVertexProcessing;
                }
                else
                {
                    createFlags = CreateFlags.SoftwareVertexProcessing;
                }

                if ((caps.DeviceCaps & DeviceCaps.PureDevice) != 0 && createFlags ==
                    CreateFlags.HardwareVertexProcessing)
                {
                    createFlags |= CreateFlags.PureDevice;
                }


                _device = new Device(direct3d, 0, DeviceType.Hardware, this.Handle, createFlags, presentParams);
                _device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
                _device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.None);
                _device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.None);
                //_device.SetRenderState(RenderState.ZWriteEnable, false);
                //_device.SetRenderState(RenderState.ZEnable, false);

                if (OnCreateDevice != null)
                {
                    OnCreateDevice(this, new DeviceEventArgs(_device));
                }


                //vb = new VertexBuffer(typeof(CustomVertex.PositionTextured),
                //vb = new VertexBuffer(new IntPtr(), 4, _device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);
                //vb = new VertexBuffer(_device, 4, Usage.Dynamic | Usage.WriteOnly, VertexFormat.Position, Pool.Default);

                //_device.SetStreamSource(0, vb, 0, 0);


            txb = new Bitmap(1, 1);
                using (Graphics g = Graphics.FromImage(txb))
                    g.Clear(Color.White);

                tcircleb = new Bitmap(9, 9);
                using (Graphics g = Graphics.FromImage(tcircleb))
                    g.FillEllipse(Brushes.White, new Rectangle(0, 0, 9, 9));
            
                tecircleb = new Bitmap(14, 14);
                using (Graphics g = Graphics.FromImage(tecircleb))
                    g.DrawEllipse(new Pen(Brushes.White), new Rectangle(0, 0, 13, 13));
                
                hcursorb = new Bitmap(32, 32);
                using (Graphics g = Graphics.FromImage(hcursorb))
                    Cursors.NoMoveHoriz.Draw(g, new Rectangle(0, 0, 32, 32));
                MakeGray(hcursorb);
                
                vcursorb = new Bitmap(32, 32);
                using (Graphics g = Graphics.FromImage(vcursorb))
                    Cursors.NoMoveVert.Draw(g, new Rectangle(0, 0, 32, 32));
                MakeGray(vcursorb);
                
                hvcursorb = new Bitmap(32, 32);
                using (Graphics g = Graphics.FromImage(hvcursorb))
                    Cursors.NoMove2D.Draw(g, new Rectangle(0, 0, 32, 32));
                MakeGray(hvcursorb);

                if (parent != null)
                {
                    InitDeviceResources();
                }

            }
            catch (SharpDXException ex)
            {
                try
                {
                    _device.Reset();
                }
                catch
                {
                    throw new ArgumentException("Error initializing DirectX", ex);
                }

            }
            catch (DllNotFoundException)
            {
                throw new Exception("Please install DirectX Redistributable June 2010");
            }
        }

        public bool getDeviceLostState()
        {
            if (deviceLost)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Run()
        {

            RenderLoop.Run(this, () =>
            {
                // Another option is not use RenderLoop at all and call Render when needed, and call here every tick for animations
                    if (bRender && !deviceLost) Render();
                    if (mouseMoved && bRender)
                    {
                        OnMouseMove(lastEvent);
                        mouseMoved = false;
                    }


            });
            //Application.DoEvents();

        }

        public void InitDeviceResources()
        {
            sprite = new Sprite(_device);
            sprite2 = new Sprite(_device);
            HUD = new Sprite(_device);

            tx = TextureCreator.FromBitmap(_device, txb);
            tcircle = TextureCreator.FromBitmap(_device, tcircleb);
            tecircle = TextureCreator.FromBitmap(_device, tecircleb);
            hcursor = TextureCreator.FromBitmap(_device, hcursorb);
            vcursor = TextureCreator.FromBitmap(_device, vcursorb);
            hvcursor = TextureCreator.FromBitmap(_device, hvcursorb);

            FontDescription fontDescription = new FontDescription()
            {
                Height = 18,
                Italic = false,
                CharacterSet = FontCharacterSet.Ansi,
                FaceName = "Microsoft Sans Serif",
                MipLevels = 0,
                OutputPrecision = FontPrecision.TrueType,
                PitchAndFamily = FontPitchAndFamily.Default,
                Quality = FontQuality.Antialiased,
                Weight = FontWeight.Regular
            };

            FontDescription fontDescriptionBold = new FontDescription()
            {
                Height = 18,
                Italic = false,
                CharacterSet = FontCharacterSet.Ansi,
                FaceName = "Microsoft Sans Serif",
                MipLevels = 0,
                OutputPrecision = FontPrecision.TrueType,
                PitchAndFamily = FontPitchAndFamily.Default,
                Quality = FontQuality.Antialiased,
                Weight = FontWeight.Bold
            };

            font = new Font(_device, fontDescription);
            fontBold = new Font(_device, fontDescriptionBold);
        }
        /// <summary>
        /// Attempt to recover the device if it is lost.
        /// </summary>
        public void AttemptRecovery(SharpDXException ex)
        {
            Process proc = Process.GetCurrentProcess();
            long memory = proc.PrivateMemorySize64;

            if (!Environment.Is64BitProcess && memory >= 1500000000)
            {
                Debug.Print("Out of Video Memory!");
                DeviceExceptionDialog(1, ex, null);
            }
            else
            {
                Result result = _device.TestCooperativeLevel();
                if (result == ResultCode.DeviceLost)
                {
                    try
                    {
                        Debug.Print("Device Lost! Fixing....");
                        DisposeDeviceResources();
                        InitDeviceResources();
                        deviceLost = false;
                    }
                    catch (SharpDXException ex2)
                    {
                        DeviceExceptionDialog(0, ex, ex2);
                    }
                }
                else if (result == ResultCode.DeviceRemoved)
                {
                    try
                    {
                        Debug.Print("Device Removed! Fixing....");
                        ResetDevice();
                        deviceLost = false;
                    }
                    catch (SharpDXException ex2)
                    {
                        DeviceExceptionDialog(0, ex, ex2);
                    }
                }
                else if (result == ResultCode.OutOfVideoMemory)
                {
                    Debug.Print("Out of Video Memory!");
                    DeviceExceptionDialog(1, ex, null);
                }
                else if (result == ResultCode.DeviceNotReset)
                {
                    try
                    {
                        deviceLost = true;
                        Debug.Print("Device Not Reset! Fixing....");
                        DisposeDeviceResources();
                        InitDeviceResources();
                        Init(EditorInstance);
                        EditorInstance.DisposeTextures();
                        deviceLost = false;
                    }
                    catch (SharpDXException ex2)
                    {
                        DeviceExceptionDialog(0, ex, ex2);
                    }
                }
                else
                {
                    DeviceExceptionDialog(0, ex, null);
                }
            }



        }




        public void ResetDevice()
        {
                DisposeDeviceResources();
                _parent.DisposeTextures();
                _device.Reset(presentParams);
                deviceLost = false;
                InitDeviceResources();

        }

        private void MakeGray(Bitmap image)
        {
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    if (image.GetPixel(x, y).Name == "ffffffff")
                        image.SetPixel(x, y, Color.Transparent);
                    else if (image.GetPixel(x, y).Name == "ff000000")
                        image.SetPixel(x, y, Color.Gray);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Extend this list of properties if you like
        /// </summary>
        private Color _deviceBackColor = Color.Black;

        public Color DeviceBackColor
        {
            get { return _deviceBackColor; }
            set { _deviceBackColor = value; }
        }


        #endregion

        #region Rendering

        public void DeviceExceptionDialog(int state, SharpDXException ex, SharpDXException ex2, AccessViolationException ex3 = null)
        {
            String error = "No Error. OH-NO!";
            String error2 = "";
            String error3 = "";
            String errorCode = "404 (No Error Code Found)";
            String errorCode2 = "404 (No Secondary Error Code Found)";
            String errorCode3 = "404 (No Trimary Error Code Found)";

            if (ex != null)
            {
                 error = ex.ToString();
                 errorCode = ex.ResultCode.ToString();
            }
            if (ex3 != null)
            {
                error3 = ex3.ToString();
                errorCode3 = ex3.HResult.ToString();
            }
            if (ex2 != null)
            {
                 error2 = ex2.ToString();
                 errorCode2 = ex2.ResultCode.ToString();
            }

            if (state == 0)
            {
                    using (var deviceLostBox = new DeviceLostBox(error, error2, error3, errorCode, errorCode2, error3))
                    {
                        deviceLostBox.ShowDialog();
                        deviceExceptionResult = deviceLostBox.DialogResult;
                    }
                    if (deviceExceptionResult == DialogResult.Yes) //Yes and Exit
                    {
                        EditorInstance.BackupSceneBeforeCrash();
                        Environment.Exit(1);

                    }
                    else if (deviceExceptionResult == DialogResult.No) //No and try to Restart
                    {
                        DisposeDeviceResources();
                        Init(EditorInstance);

                    }
                    else if (deviceExceptionResult == DialogResult.Retry) //Yes and try to Restart
                    {
                        EditorInstance.BackupSceneBeforeCrash();
                        DisposeDeviceResources();
                        Init(EditorInstance);
                    }
                    else if (deviceExceptionResult == DialogResult.Ignore) //No and Exit
                    {
                        Environment.Exit(1);
                    }
            }
            else if (state == 1)
            {

                using (var deviceLostBox = new DeviceLostBox(error, error2, error3, errorCode, errorCode2, errorCode3, 1))
                {
                    deviceLostBox.ShowDialog();
                    deviceExceptionResult = deviceLostBox.DialogResult;

                }
                if (deviceExceptionResult == DialogResult.Yes) //Yes and Exit
                {
                    EditorInstance.BackupSceneBeforeCrash();
                    Environment.Exit(1);

                }
                else if (deviceExceptionResult == DialogResult.No) //No and try to Restart
                {
                    DisposeDeviceResources();
                    Init(EditorInstance);

                }
                else if (deviceExceptionResult == DialogResult.Retry) //Yes and try to Restart
                {
                    EditorInstance.BackupSceneBeforeCrash();
                    DisposeDeviceResources();
                    Init(EditorInstance);
                }
                else if (deviceExceptionResult == DialogResult.Ignore) //No and Exit
                {
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// Rendering-method
        /// </summary>
        public void Render()
        {
            if (deviceLost) AttemptRecovery(null);
            
            if (_device == null)
            {
                AttemptRecovery(null);
                return;
            }


            try
            {
                Rectangle screen = _parent.GetScreen();
                double zoom = _parent.GetZoom();

                //Clear the backbuffer
                _device.Clear(ClearFlags.Target, new SharpDX.Color(_deviceBackColor.R, _deviceBackColor.B, _deviceBackColor.G, _deviceBackColor.A), 1.0f, 0);

                //Begin the scene
                _device.BeginScene();

                sprite.Transform = Matrix.Scaling((float)zoom, (float)zoom, 1f);

                if (EditorInstance.UseLargeDebugStats) HUD.Transform = Matrix.Scaling(2f, 2f, 2f);
                else HUD.Transform = Matrix.Scaling(1f, 1f, 1f);


                sprite2.Begin(SpriteFlags.AlphaBlend);


                    var state1 = _device.GetSamplerState(0, SamplerState.MinFilter);
                    var state2 = _device.GetSamplerState(0, SamplerState.MagFilter);
                    var state3 = _device.GetSamplerState(0, SamplerState.MipFilter);

                    // If zoomin, just do near-neighbor scaling
                    _device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
                    _device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.None);
                    _device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.None);
                
                    HUD.Begin(SpriteFlags.AlphaBlend);

                    _device.SetSamplerState(0, SamplerState.MinFilter, state1);
                    _device.SetSamplerState(0, SamplerState.MagFilter, state2);
                    _device.SetSamplerState(0, SamplerState.MipFilter, state3);

                if (zoom > 1)
                {
                    // If zoomin, just do near-neighbor scaling
                    _device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
                    _device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.None);
                    _device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.None);
                }
                sprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthFrontToBack | SpriteFlags.DoNotModifyRenderState);


                // Render of scene here
                if (OnRender != null && !deviceLost)
                {
                    OnRender(this, new DeviceEventArgs(_device));
                }



                sprite.Transform = Matrix.Scaling(1f, 1f, 1f);
                HUD.Transform = Matrix.Scaling(1f, 1f, 1f);

                Rectangle rect1 = new Rectangle(DrawWidth - screen.X, 0, Width - DrawWidth, Height);
                rect1.Intersect(new Rectangle(0, 0, screen.Width, screen.Height));
                Rectangle rect2 = new Rectangle(0, DrawHeight - screen.Y, DrawWidth, Height - DrawHeight);
                rect2.Intersect(new Rectangle(0, 0, screen.Width, screen.Height));
                DrawTexture(tx, new Rectangle(0, 0, rect1.Width, rect1.Height), new Vector3(0, 0, 0), new Vector3(rect1.X, rect1.Y, 0), SystemColors.Control);
                DrawTexture(tx, new Rectangle(0, 0, rect2.Width, rect2.Height), new Vector3(0, 0, 0), new Vector3(rect2.X, rect2.Y, 0), SystemColors.Control);

                sprite.End();
                sprite2.End();
                HUD.End();
                //End the scene
                _device.EndScene();
                _device.Present();


        }
        catch (SharpDXException ex)
        {
                deviceLost = true;
                AttemptRecovery(ex);
        }



}

        #endregion

        #region Overrides

        protected override void OnPaint(PaintEventArgs e)
        {
            // Render on each Paint
            this.Render();
        }

        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            /*// Close on escape
            if ((int)(byte)e.KeyChar == (int)System.Windows.Forms.Keys.Escape)
                _parent.Close();*/
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up: return true;
                case Keys.Down: return true;
                case Keys.Right: return true;
                case Keys.Left: return true;
                case Keys.PageUp: return true;
                case Keys.PageDown: return true;
            }
            return base.IsInputKey(keyData);
        }

        /*protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            MapEditor.Instance.GraphicPanel_OnKeyDown(null, e);
        }

        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            MapEditor.Instance.GraphicPanel_OnKeyUp(null, e);
        }*/

        protected override Point ScrollToControl(Control activeControl)
        {
            return this.AutoScrollPosition;
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            lastEvent = e;
            base.OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, e.X + _parent.GetScreen().X, e.Y + _parent.GetScreen().Y, e.Delta));
        }

        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseWheel(new MouseEventArgs(e.Button, e.Clicks, e.X + _parent.GetScreen().X, e.Y + _parent.GetScreen().Y, e.Delta));
        }

        protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseClick(new MouseEventArgs(e.Button, e.Clicks, e.X + _parent.GetScreen().X, e.Y + _parent.GetScreen().Y, e.Delta));
        }

        protected override void OnMouseDoubleClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDoubleClick(new MouseEventArgs(e.Button, e.Clicks, e.X + _parent.GetScreen().X, e.Y + _parent.GetScreen().Y, e.Delta));
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, e.X + _parent.GetScreen().X, e.Y + _parent.GetScreen().Y, e.Delta));
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(new MouseEventArgs(e.Button, e.Clicks, e.X + _parent.GetScreen().X, e.Y + _parent.GetScreen().Y, e.Delta));
        }

        #endregion

        public Rectangle GetScreen()
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            if (zoom == 1.0)
                return screen;
            else
                return new Rectangle((int)Math.Floor(screen.X / zoom),
                    (int)Math.Floor(screen.Y / zoom),
                    (int)Math.Ceiling(screen.Width / zoom),
                    (int)Math.Ceiling(screen.Height / zoom));
        }

        public bool IsObjectOnScreen(int x, int y, int width, int height)
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            if (zoom == 1.0)
                return !(x > screen.X + screen.Width
                || x + width < screen.X
                || y > screen.Y + screen.Height
                || y + height < screen.Y);
            else
                return !(x * zoom > screen.X + screen.Width
                || (x + width) * zoom < screen.X
                || y * zoom > screen.Y + screen.Height
                || (y + height) * zoom < screen.Y);
        }

        private void DrawTexture(Texture image, Rectangle srcRect, Vector3 center, Vector3 position, Color color)
        {
            if (!deviceLost)
            {
                sprite.Draw(image, new SharpDX.Color(color.R, color.G, color.B, color.A), new SharpDX.Rectangle(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height), center, position);
            }
        }

        private void DrawTexture2(Texture image, Rectangle srcRect, Vector3 center, Vector3 position, Color color)
        {
            if (!deviceLost)
            {
                sprite2.Draw(image, new SharpDX.Color(color.R, color.G, color.B, color.A), new SharpDX.Rectangle(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height), center, position);
            }
            
        }
        private void DrawHUD(Texture image, Rectangle srcRect, Vector3 center, Vector3 position, Color color)
        {
            HUD.Draw(image, new SharpDX.Color(color.R, color.G, color.B, color.A), new SharpDX.Rectangle(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height), center, position);
        }

        public void DrawBitmap(Texture image, int x, int y, int width, int height, bool selected, int transparency)
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            DrawTexture(image, new Rectangle(0, 0, width, height), new Vector3(), new Vector3(x - (int)(screen.X / zoom), y - (int)(screen.Y / zoom), 0), (selected) ? Color.BlueViolet : Color.FromArgb(transparency, Color.White));
        }

        public void DrawHUDBitmap(Texture image, int x, int y, int width, int height, bool selected, int transparency)
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            DrawHUD(image, new Rectangle(0, 0, width, height), new Vector3(), new Vector3(x, y, 0), (selected) ? Color.BlueViolet : Color.FromArgb(transparency, Color.White));
        }

        public void DrawCircle(int x, int y, Color color)
        {
            if (!IsObjectOnScreen(x - 4, y - 4, 9, 9)) return;
            //tcircle = new Texture(_device, tcircleb, Usage.Dynamic, Pool.Default);

            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            DrawTexture(tcircle, new Rectangle(0, 0, 9, 9), new Vector3(0, 0, 0), new Vector3(x - 4 - (int)(screen.X / zoom), y - 4 - (int)(screen.Y / zoom), 0), color);
        }

        public void DrawEmptyCircle(int x, int y, Color color)
        {
            if (!IsObjectOnScreen(x - 6, y - 6, 14, 14)) return;
            //tecircle = new Texture(_device, tecircleb, Usage.Dynamic, Pool.Default);

            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            DrawTexture(tecircle, new Rectangle(0, 0, 14, 14), new Vector3(0, 0, 0), new Vector3(x - 6 - (int)(screen.X / zoom), y - 6 - (int)(screen.Y / zoom), 0), color);
        }

        public void DrawLine(int X1, int Y1, int X2, int Y2, Color color)
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            int width = Math.Abs(X2 - X1);
            int height = Math.Abs(Y2 - Y1);
            int x = Math.Min(X1, X2);
            int y = Math.Min(Y1, Y2);
            int pixel_width = Math.Max((int)zoom, 1);

            if (!IsObjectOnScreen(x, y, width, height)) return;

            //tx = new Texture(_device, txb, Usage.Dynamic, Pool.Default);


            sprite.Transform = Matrix.Scaling(1f, 1f, 1f); 
            if (width == 0 || height == 0)
            {
                if (width == 0) width = pixel_width;
                else width = (int)(width * zoom);
                if (height == 0) height = pixel_width;
                else height = (int)(height * zoom);
                DrawTexture(tx, new Rectangle(0, 0, width, height), new Vector3(0, 0, 0), new Vector3((int)((x - (int)(screen.X / zoom)) * zoom), (int)((y - (int)(screen.Y / zoom)) * zoom), 0), color);
            }
            else
            {
                DrawLinePBP(X1, Y1, X2, Y2, color);
            }
            sprite.Transform = Matrix.Scaling((float)zoom, (float)zoom, 1f);
        }

        public void DrawLinePaperRoller(int X1, int Y1, int X2, int Y2, Color color, Color color2, Color color3, Color color4)
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            int width = Math.Abs(X2 - X1);
            int height = Math.Abs(Y2 - Y1);
            int x = Math.Min(X1, X2);
            int y = Math.Min(Y1, Y2);
            int pixel_width = Math.Max((int)zoom, 1);

            if (!IsObjectOnScreen(x, y, width, height)) return;

            //tx = new Texture(_device, txb, Usage.Dynamic, Pool.Default);


            sprite.Transform = Matrix.Scaling(1f, 1f, 1f);
            /*
            if (width == 0 || height == 0)
            {
                if (width == 0) width = pixel_width;
                else width = (int)(width * zoom);
                if (height == 0) height = pixel_width;
                else height = (int)(height * zoom);
                DrawTexture(tx, new Rectangle(0, 0, width, height), new Vector3(0, 0, 0), new Vector3((int)((x - (int)(screen.X / zoom)) * zoom), (int)((y - (int)(screen.Y / zoom)) * zoom), 0), color);
            }
            else
            {*/
                DrawLinePBPDoted(X1, Y1, X2, Y2, color, color2, color3, color4);
            //}
            sprite.Transform = Matrix.Scaling((float)zoom, (float)zoom, 1f);
        }

        public void DrawSnowLines(int X1, int Y1, int X2, int Y2, Color color)
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            int width = Math.Abs(X2 - X1);
            int height = Math.Abs(Y2 - Y1);
            int x = Math.Min(X1, X2);
            int y = Math.Min(Y1, Y2);
            int pixel_width = Math.Max((int)zoom, 1);

            if (width == 0 || height == 0)
            {
                if (width == 0) width = 1;
                else width = (int)(width * zoom);
                if (height == 0) height = 1;
                else height = (int)(height * zoom);
                DrawTexture(tx, new Rectangle(0, 0, width, height), new Vector3(0, 0, 0), new Vector3((int)((x)), (int)((y)), 0), color);
            }
            else
            {
                DrawLinePBP(X1, Y1, X2, Y2, color);
            }
        }

        public void DrawArrow(int x0, int y0, int x1, int y1, Color color)
        {
            int x2, y2, x3, y3;

            double angle = Math.Atan2(y1 - y0, x1 - x0) + Math.PI;

            x2 = (int)(x1 + 10 * Math.Cos(angle - Math.PI / 8));
            y2 = (int)(y1 + 10 * Math.Sin(angle - Math.PI / 8));
            x3 = (int)(x1 + 10 * Math.Cos(angle + Math.PI / 8));
            y3 = (int)(y1 + 10 * Math.Sin(angle + Math.PI / 8));

            DrawLine(x1, y1, x0, y0, color);
            DrawLine(x1, y1, x2, y2, color);
            DrawLine(x1, y1, x3, y3, color);
        }
        void DrawLinePBP(int x0, int y0, int x1, int y1, Color color)
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            int dx, dy, inx, iny, e;
            int pixel_width = (int)Math.Ceiling(zoom);

            dx = x1 - x0;
            dy = y1 - y0;
            inx = dx > 0 ? 1 : -1;
            iny = dy > 0 ? 1 : -1;

            dx = Math.Abs(dx);
            dy = Math.Abs(dy);

            if (dx >= dy)
            {
                dy <<= 1;
                e = dy - dx;
                dx <<= 1;
                while (x0 != x1)
                {
                    DrawTexture(tx, new Rectangle(0, 0, pixel_width, pixel_width), new Vector3(0, 0, 0), new Vector3((int)((x0 - (int)(screen.X / zoom)) * zoom), (int)((y0 - (int)(screen.Y / zoom)) * zoom), 0), color);
                    if (e >= 0)
                    {
                        y0 += iny;
                        e -= dx;
                    }
                    e += dy; x0 += inx;
                }
            }
            else
            {
                dx <<= 1;
                e = dx - dy;
                dy <<= 1;
                while (y0 != y1)
                {
                    DrawTexture(tx, new Rectangle(0, 0, pixel_width, pixel_width), new Vector3(0, 0, 0), new Vector3((int)((x0 - (int)(screen.X / zoom)) * zoom), (int)((y0 - (int)(screen.Y / zoom)) * zoom), 0), color);
                    if (e >= 0)
                    {
                        x0 += inx;
                        e -= dy;
                    }
                    e += dx; y0 += iny;
                }
            }
            DrawTexture(tx, new Rectangle(0, 0, pixel_width, pixel_width), new Vector3(0, 0, 0), new Vector3((int)((x0 - (int)(screen.X / zoom)) * zoom), (int)((y0 - (int)(screen.Y / zoom)) * zoom), 0), color);
        }

        void DrawLinePBPDoted(int x0, int y0, int x1, int y1, Color color, Color color2, Color color3, Color color4)
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            int dx, dy, inx, iny, e;
            int pixel_width = (int)(Math.Ceiling(zoom + 0.3));

            dx = x1 - x0;
            dy = y1 - y0;
            inx = dx > 0 ? 1 : -1;
            iny = dy > 0 ? 1 : -1;

            dx = Math.Abs(dx);
            dy = Math.Abs(dy);

            Color currentColor = color;
            int iterations = 0;

            if (dx >= dy)
            {
                dy <<= 1;
                e = dy - dx;
                dx <<= 1;
                while (x0 != x1)
                {
                    if (iterations >= 5)
                    {
                        if (currentColor == color4) currentColor = color;
                        else if (currentColor == color3) currentColor = color4;
                        else if (currentColor == color2) currentColor = color3;
                        else if (currentColor == color) currentColor = color2;

                        iterations = 0;
                    }


                    DrawTexture(tx, new Rectangle(0, 0, pixel_width, pixel_width), new Vector3(0, 0, 0), new Vector3((int)((x0 - (int)(screen.X / zoom)) * zoom), (int)((y0 - (int)(screen.Y / zoom)) * zoom), 0), currentColor);
                    if (e >= 0)
                    {
                        y0 += iny;
                        e -= dx;
                    }
                    e += dy; x0 += inx; iterations++;

                }
            }
            else
            {
                dx <<= 1;
                e = dx - dy;
                dy <<= 1;
                while (y0 != y1)
                {
                    if (iterations >= 5)
                    {
                        if (currentColor == color4) currentColor = color;
                        else if (currentColor == color3) currentColor = color4;
                        else if (currentColor == color2) currentColor = color3;
                        else if (currentColor == color) currentColor = color2;

                        iterations = 0;
                    }

                    DrawTexture(tx, new Rectangle(0, 0, pixel_width, pixel_width), new Vector3(0, 0, 0), new Vector3((int)((x0 - (int)(screen.X / zoom)) * zoom), (int)((y0 - (int)(screen.Y / zoom)) * zoom), 0), currentColor);
                    if (e >= 0)
                    {
                        x0 += inx;
                        e -= dy;
                    }
                    e += dx; y0 += iny; iterations++;
                }
            }
            DrawTexture(tx, new Rectangle(0, 0, pixel_width, pixel_width), new Vector3(0, 0, 0), new Vector3((int)((x0 - (int)(screen.X / zoom)) * zoom), (int)((y0 - (int)(screen.Y / zoom)) * zoom), 0), color);
        }

        public void DrawRectangle(int x1, int y1, int x2, int y2, Color color)
        {
            if (!IsObjectOnScreen(x1, y1, x2 - x1, y2 - y1)) return;
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            //tx = new Texture(_device, txb, Usage.Dynamic, Pool.Default);

            DrawTexture(tx, new Rectangle(0, 0, x2 - x1, y2 - y1), new Vector3(0, 0, 0), new Vector3(x1 - (int)(screen.X / zoom), y1 - (int)(screen.Y / zoom), 0), color);
        }
        public void DrawText(string text, int x, int y, int width, Color color, bool bold)
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            if (width >= 30)
            {
                ((bold) ? fontBold : font).DrawText(sprite, text, new SharpDX.Rectangle(x - (int)(screen.X / zoom), y - (int)(screen.Y / zoom), width, 1000), FontDrawFlags.WordBreak, new SharpDX.Color(color.R, color.G, color.B, color.A));
            }
            else
            {
                ((bold) ? fontBold : font).DrawText(sprite, text, x - (int)(screen.X / zoom), y - (int)(screen.Y / zoom), new SharpDX.Color(color.R, color.G, color.B, color.A));
            }
        }
        public void DrawTextSmall(string text, int x, int y, int width, Color color, bool bold)
        {
            Rectangle screen = _parent.GetScreen();
            double zoom = _parent.GetZoom();
            sprite.Transform = Matrix.Scaling((float)zoom / 4, (float)zoom / 4, 1f);
            if (width >= 10)
            {
                ((bold) ? fontBold : font).DrawText(sprite, text, new SharpDX.Rectangle((x - (int)(screen.X / zoom)) * 4, (y - (int)(screen.Y / zoom)) * 4, width * 4, 1000), FontDrawFlags.WordBreak, new SharpDX.Color(color.R, color.G, color.B, color.A));
            }
            else
            {
                ((bold) ? fontBold : font).DrawText(sprite, text, (x - (int)(screen.X / zoom)) * 4, (y - (int)(screen.Y / zoom)) * 4, new SharpDX.Color(color.R, color.G, color.B, color.A));
            }
            sprite.Transform = Matrix.Scaling((float)zoom, (float)zoom, 1f);
        }
        public void Draw2DCursor(int x, int y)
        {
            //hvcursor = new Texture(_device, hvcursorb, Usage.Dynamic, Pool.Default);
            if (!deviceLost)
            {
                DrawTexture2(hvcursor, new Rectangle(Point.Empty, Cursors.NoMove2D.Size), new Vector3(0, 0, 0), new Vector3(x - 16, y - 15, 0), Color.White);
            }

        }
        public void DrawHorizCursor(int x, int y)
        {
            //hcursor = new Texture(_device, hcursorb, Usage.Dynamic, Pool.Default);
            if (!deviceLost)
            {
                DrawTexture2(hcursor, new Rectangle(Point.Empty, Cursors.NoMove2D.Size), new Vector3(0, 0, 0), new Vector3(x - 16, y - 15, 0), Color.White);
            }
        }

        public void DrawVertCursor(int x, int y)
        {
            //vcursor = new Texture(_device, vcursorb, Usage.Dynamic, Pool.Default);
            if (!deviceLost)
            {
                DrawTexture2(vcursor, new Rectangle(Point.Empty, Cursors.NoMove2D.Size), new Vector3(0, 0, 0), new Vector3(x - 16, y - 15, 0), Color.White);
                DrawTexture2(hcursor, new Rectangle(Point.Empty, Cursors.NoMove2D.Size), new Vector3(0, 0, 0), new Vector3(x - 16, y - 15, 0), Color.White);
            }
        }

        public void OnMouseMoveEventCreate()
        {
            Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y);
        }

        public void DisposeDeviceResources(int type = 0)
        {
            if (tx != null)
            {
                tx.Dispose();
                tx = null;
            }
            if (tcircle != null)
            {
                tcircle.Dispose();
                tcircle = null;
            }
            if (tecircle != null)
            {
                tecircle.Dispose();
                tecircle = null;
            }
            if (hvcursor != null)
            {
                hvcursor.Dispose();
                hvcursor = null;
            }
            if (vcursor != null)
            {
                vcursor.Dispose();
                vcursor = null;
            }
            if (hcursor != null)
            {
                hcursor.Dispose();
                hcursor = null;
            }

            if (font != null)
            {
                font.Dispose();
                font = null;
            }
            if (fontBold != null)
            {
                fontBold.Dispose();
                fontBold = null;
            }

            if (sprite != null || type == 1)
            {
                sprite.Dispose();
                sprite = null;
            }
            if (sprite2 != null || type == 1)
            {
                sprite2.Dispose();
                sprite2 = null;
            }
            if (HUD != null || type == 1)
            {
                HUD.Dispose();
                HUD = null;
            }

            if (_parent != null && type == 1)
            {
                _parent.DisposeTextures();
                _parent = null;
            }
            if (_device != null && type == 1)
            {
                _device.Dispose();
                _device = null;
            }
            if (direct3d != null && type == 1)
            {
                direct3d.Dispose();
                direct3d = null;
            }
            
        }
        public void ForceDisposeDeviceSpriteResources()
        {

        }

        public new void Dispose()
        {
            DisposeDeviceResources();
            _parent.DisposeTextures();
            _device.Dispose();
            direct3d.Dispose();
            base.Dispose();
        }

        public Image GetImage()
        {
            Surface backbuffer = _device.GetBackBuffer(0, 0);         
            Image deviceImage = Image.FromStream(Surface.ToStream(backbuffer, ImageFileFormat.Png));
            backbuffer.Dispose();
            return deviceImage;
        } 


    }
}
