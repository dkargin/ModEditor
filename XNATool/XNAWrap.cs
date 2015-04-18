using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ModEditor.XNATool
{
    public class XNAWrap : Game
    {
        public enum WindowMode
        {
            Fullscreen,
            Windowed,
            Borderless
        }
        public GraphicsDeviceManager graphics;
        //public static Game1 Instance;
        //  public ScreenManager screenManager;
        public bool IsLoaded;
        //public Game1.WindowMode CurrentMode = Game1.WindowMode.Borderless;
        public XNAWrap()
        {
            bool OK = SteamManager.SteamInitialize();
            if (OK)
            {
                OK = SteamManager.RequestCurrentStats();
                bool ok = SteamManager.SetAchievement("Thanks");
                if (ok)
                {
                    SteamManager.SaveAllStatAndAchievementChanges();
                }
            }
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
            this.graphics.MinimumVertexShaderProfile = ShaderProfile.VS_2_0;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Directory.CreateDirectory(path + "/StarDrive");
            Directory.CreateDirectory(path + "/StarDrive/Saved Games");
            Directory.CreateDirectory(path + "/StarDrive/Fleet Designs");
            Directory.CreateDirectory(path + "/StarDrive/Saved Designs");
            Directory.CreateDirectory(path + "/StarDrive/WIP");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/StarDrive/Saved Games/Headers");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/StarDrive/Saved Games/Fog Maps");
            /*
            GlobalStats.Config = new Config();
            string vol = ConfigurationSettings.AppSettings["MusicVolume"];
            int musicVol = 100;
            if (int.TryParse(vol, out musicVol))
            {
                GlobalStats.Config.MusicVolume = (float)musicVol / 100f;
            }
            vol = ConfigurationSettings.AppSettings["EffectsVolume"];
            int fxVol = 100;
            if (int.TryParse(vol, out fxVol))
            {
                GlobalStats.Config.EffectsVolume = (float)fxVol / 100f;
            }
            GlobalStats.Config.Language = ConfigurationSettings.AppSettings["Language"];
            if (GlobalStats.Config.Language != "English" && GlobalStats.Config.Language != "German")
            {
                GlobalStats.Config.Language = "English";
            }
            GlobalStats.Config.RanOnce = !(ConfigurationSettings.AppSettings["RanOnce"] == "false");
            int winmode = 0;
            string w = ConfigurationSettings.AppSettings["WindowMode"];
            if (int.TryParse(w, out winmode))
            {
                GlobalStats.Config.WindowMode = winmode;
            }
            GlobalStats.Config.RanOnce = false;
            if (!GlobalStats.Config.RanOnce)
            {
                this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                this.graphics.IsFullScreen = true;
            }
            else
            {
                int xres = 1280;
                int yres = 720;
                string x = ConfigurationSettings.AppSettings["XRES"];
                if (int.TryParse(x, out xres))
                {
                    this.graphics.PreferredBackBufferWidth = xres;
                    GlobalStats.Config.XRES = xres;
                }
                if (int.TryParse(ConfigurationSettings.AppSettings["YRES"], out yres))
                {
                    this.graphics.PreferredBackBufferHeight = yres;
                    GlobalStats.Config.YRES = yres;
                }
            }*/
            this.graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            this.graphics.PreferMultiSampling = true;
            this.graphics.SynchronizeWithVerticalRetrace = true;
            this.graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(this.PrepareDeviceSettings);
            /*
            if (!GlobalStats.Config.RanOnce)
            {
                GlobalStats.Config.WindowMode = 0;
            }
            switch (GlobalStats.Config.WindowMode)
            {
                case 0:
                    this.SetWindowMode(Game1.WindowMode.Fullscreen, (!GlobalStats.Config.RanOnce) ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width : GlobalStats.Config.XRES, (!GlobalStats.Config.RanOnce) ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height : GlobalStats.Config.YRES);
                    break;
                case 1:
                    this.SetWindowMode(Game1.WindowMode.Windowed, GlobalStats.Config.XRES, GlobalStats.Config.YRES);
                    break;
                case 2:
                    this.SetWindowMode(Game1.WindowMode.Borderless, GlobalStats.Config.XRES, GlobalStats.Config.YRES);
                    break;
            }*/
            Bitmap cur = new Bitmap("Content/Cursors/Cursor.png", true);
            Graphics.FromImage(cur);
            IntPtr ptr = cur.GetHicon();
            Cursor c = new Cursor(ptr);
            Control.FromHandle(base.Window.Handle).Cursor = c;
            base.IsMouseVisible = true;
        }
        private void PrepareDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            int quality = 0;
            GraphicsAdapter adapter = e.GraphicsDeviceInformation.Adapter;
            SurfaceFormat format = adapter.CurrentDisplayMode.Format;
            if (adapter.CheckDeviceMultiSampleType(DeviceType.Hardware, format, false, MultiSampleType.TwoSamples, out quality))
            {
                e.GraphicsDeviceInformation.PresentationParameters.MultiSampleQuality = ((quality == 1) ? 0 : 1);
                e.GraphicsDeviceInformation.PresentationParameters.MultiSampleType = MultiSampleType.TwoSamples;
            }
            else
            {
                e.GraphicsDeviceInformation.PresentationParameters.MultiSampleType = MultiSampleType.None;
                e.GraphicsDeviceInformation.PresentationParameters.MultiSampleQuality = 0;
            }
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PlatformContents;
        }
        protected override void Initialize()
        {
            base.Window.Title = "StarDrive";
            base.Content.RootDirectory = "Content";
            // this.screenManager = new ScreenManager(this, this.graphics);
            //this.screenManager.splashScreenGameComponent = new SplashScreenGameComponent(this, this.graphics);
            //base.Components.Add(this.screenManager.splashScreenGameComponent);
            // AudioManager.Initialize(this, "Content/Audio/ShipGameProject.xgs", "Content/Audio/Wave Bank.xwb", "Content/Audio/Sound Bank.xsb");
            // Game1.Instance = this;
            base.Initialize();
        }
        protected override void LoadContent()
        {
            if (this.IsLoaded)
            {
                return;
            }
            // this.screenManager.LoadContent();
            //Fonts.LoadContent(base.Content);
            //this.screenManager.AddScreen(new GameLoadingScreen());
            this.IsLoaded = true;
        }
        public void ApplySettings()
        {
            this.graphics.ApplyChanges();
        }
        protected override void UnloadContent()
        {
        }
        /*
        public void SetWindowMode(Game1.WindowMode mode, int width, int height)
        {
            Form form = (Form)Control.FromHandle(base.Window.Handle);
            switch (mode)
            {
                case Game1.WindowMode.Fullscreen:
                    if (!this.graphics.IsFullScreen)
                    {
                        this.graphics.ToggleFullScreen();
                    }
                    this.graphics.PreferredBackBufferWidth = width;
                    this.graphics.PreferredBackBufferHeight = height;
                    this.graphics.ApplyChanges();
                    this.CurrentMode = Game1.WindowMode.Fullscreen;
                    GlobalStats.Config.WindowMode = 0;
                    return;
                case Game1.WindowMode.Windowed:
                    if (this.graphics.IsFullScreen)
                    {
                        this.graphics.ToggleFullScreen();
                    }
                    this.graphics.PreferredBackBufferWidth = width;
                    this.graphics.PreferredBackBufferHeight = height;
                    this.graphics.ApplyChanges();
                    form.WindowState = FormWindowState.Normal;
                    form.FormBorderStyle = FormBorderStyle.Fixed3D;
                    form.ClientSize = new Size(width, height);
                    form.Location = new System.Drawing.Point(Screen.PrimaryScreen.WorkingArea.Size.Width / 2 - width / 2, Screen.PrimaryScreen.WorkingArea.Size.Height / 2 - height / 2);
                    this.CurrentMode = Game1.WindowMode.Windowed;
                    GlobalStats.Config.WindowMode = 1;
                    return;
                case Game1.WindowMode.Borderless:
                    if (this.graphics.IsFullScreen)
                    {
                        this.graphics.ToggleFullScreen();
                    }
                    this.graphics.PreferredBackBufferWidth = width;
                    this.graphics.PreferredBackBufferHeight = height;
                    this.graphics.ApplyChanges();
                    form.FormBorderStyle = FormBorderStyle.None;
                    form.WindowState = FormWindowState.Normal;
                    form.ClientSize = new Size(width, height);
                    form.Location = new System.Drawing.Point(Screen.PrimaryScreen.WorkingArea.Size.Width / 2 - width / 2, Screen.PrimaryScreen.WorkingArea.Size.Height / 2 - height / 2);
                    this.CurrentMode = Game1.WindowMode.Borderless;
                    GlobalStats.Config.WindowMode = 2;
                    return;
                default:
                    return;
            }
        }*/
        protected override void Update(GameTime gameTime)
        {
            //this.screenManager.Update(gameTime);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            if (base.GraphicsDevice.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal)
            {
                base.GraphicsDevice.Clear(Microsoft.Xna.Framework.Graphics.Color.Black);
                /* if (!SplashScreen.DisplayComplete)
                 {
                     this.screenManager.splashScreenGameComponent.Draw(gameTime);
                 }
                 this.screenManager.Draw(gameTime);*/
                base.Draw(gameTime);
            }
        }

        public void Start()
        {
            Initialize();
        }
    }
}
