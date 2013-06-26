using ModEditor.XNATool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ModEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {           
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm mainForm = new MainForm();
            XNAWrap baseGame = new XNAWrap();
            Ship_Game.GlobalStats.Config = new Ship_Game.Config();
            baseGame.Content.RootDirectory = "Content";
            Ship_Game.ResourceManager.localContentManager = baseGame.Content;
            mainForm.InitXNA(baseGame, baseGame.graphics);
            Application.Run(mainForm);
            //baseGame.graphics.GraphicsDevice.Present
        }
    }
}
