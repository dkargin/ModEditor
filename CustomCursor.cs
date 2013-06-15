using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ship_Game;
using System;
public class CustomCursor : DrawableGameComponent
{
	private ContentManager content;
	private SpriteBatch spriteBatch;
	private int frameRate;
	private int frameCounter;
	private TimeSpan elapsedTime = TimeSpan.Zero;
	public CustomCursor(Game game) : base(game)
	{
		this.content = new ContentManager(game.Services);
	}
	protected override void LoadContent()
	{
		this.spriteBatch = new SpriteBatch(base.GraphicsDevice);
	}
	protected override void UnloadContent()
	{
		this.content.Unload();
	}
	public override void Update(GameTime gameTime)
	{
		this.elapsedTime += gameTime.ElapsedGameTime;
		if (this.elapsedTime > TimeSpan.FromSeconds(1.0))
		{
			this.elapsedTime -= TimeSpan.FromSeconds(1.0);
			this.frameRate = this.frameCounter;
			this.frameCounter = 0;
		}
	}
	public override void Draw(GameTime gameTime)
	{
		this.frameCounter++;
		string.Format("fps: {0}", this.frameRate);
		Vector2 MousePos = new Vector2((float)Mouse.GetState().X, (float)Mouse.GetState().Y);
		this.spriteBatch.Begin();
		this.spriteBatch.Draw(ResourceManager.TextureDict["Cursors/Cursor"], MousePos, Color.White);
		this.spriteBatch.End();
	}
}
