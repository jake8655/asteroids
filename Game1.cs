using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Asteroids
{
	public class Sprite
	{
		public Sprite(string file, int x, int y, int width, int height)
		{
			this.file = file;
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		public Texture2D sprite;
		public readonly string file;
		public int x;
		public int y;
		public int width;
		public int height;
	}

	public class Game1 : Game
	{
		private readonly GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private MouseState mouse;
		Scrolling background1;
		Scrolling background2;
		private bool shot = false;
		private int timer = 0;
		private int asteroidInterval = 120;
		private float asteroidSpeed = 5f;
		private int gameTimer = 0;

		private Sprite playerSprite = new Sprite("player", 50, 50, 50, 50);
		private SpriteFont scoreSprite;
		private int score = 0;

		Song song;
		SoundEffect shotEffect;
		SoundEffect explosion;

		List<Bullet> bullets = new List<Bullet>();
		List<Asteroid> asteroids = new List<Asteroid>();

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			playerSprite.sprite = Content.Load<Texture2D>(playerSprite.file);
			scoreSprite = Content.Load<SpriteFont>("font");

			background1 = new Scrolling(Content.Load<Texture2D>("background"), new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
			background2 = new Scrolling(Content.Load<Texture2D>("background"), new Rectangle(_graphics.PreferredBackBufferWidth, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));

			song = Content.Load<Song>("song");
			MediaPlayer.Volume = .1f;
			MediaPlayer.Play(song);
			MediaPlayer.IsRepeating = true;

			shotEffect = Content.Load<SoundEffect>("shotEffect");
			explosion = Content.Load<SoundEffect>("explosion");
		}

		protected override void Update(GameTime gameTime)
		{
			if (score <= -1)
				Exit();

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			mouse = Mouse.GetState();

			if (background1.rect.X <= -_graphics.PreferredBackBufferWidth)
				background1.rect.X = _graphics.PreferredBackBufferWidth;
			if (background2.rect.X <= -_graphics.PreferredBackBufferWidth)
				background2.rect.X = _graphics.PreferredBackBufferWidth;
			background1.Update();
			background2.Update();

			if (Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up))
				playerSprite.y -= 5;
			if (Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down))
				playerSprite.y += 5;
			if (playerSprite.y <= playerSprite.height) playerSprite.y = playerSprite.height;
			if (playerSprite.y >= _graphics.PreferredBackBufferHeight-playerSprite.height) playerSprite.y = _graphics.PreferredBackBufferHeight - playerSprite.height;

			if (Keyboard.GetState().IsKeyDown(Keys.Space) && !shot)
			{
				Shoot();
				shot = true;
			}
			if (Keyboard.GetState().IsKeyUp(Keys.Space))
				shot = false;

			UpdateBullets();
			UpdateAsteroids();

			base.Update(gameTime);
		}

		private void UpdateBullets()
		{
			foreach(Bullet bullet in bullets)
			{
				bullet.position += bullet.velocity;
				if (Vector2.Distance(bullet.position, new Vector2(playerSprite.x, playerSprite.y)) > 800)
					bullet.isVisible = false;
			}

			for (int i = 0; i < bullets.Count; i++)
			{
				if (!bullets[i].isVisible)
				{
					bullets.RemoveAt(i);
					i--;
				}
			}
		}

		private void UpdateAsteroids()
		{
			timer++;
			gameTimer++;
			if(gameTimer >= 600)
			{
				gameTimer = 0;
				if(asteroidInterval > 60)
					asteroidInterval -= 20;
				if (asteroidInterval < 10f)
					asteroidSpeed += 1f;
			}

			if (timer >= asteroidInterval)
			{
				Spawn();
				timer = 0;
			}

			foreach (Asteroid asteroid in asteroids)
			{
				asteroid.position += asteroid.velocity;
				if (Vector2.Distance(asteroid.position, new Vector2(playerSprite.x, playerSprite.y)) <= 60)
				{
					asteroid.isVisible = false;
					score -= 2;
					explosion.Play(volume: .3f, pitch: 0f, pan: 0f);
				}
				if(asteroid.position.X <= -30)
				{
					score--;
					asteroid.isVisible = false;
				}

				foreach(Bullet bullet in bullets)
				{
					if (Vector2.Distance(asteroid.position, bullet.position) <= 60)
					{
						bullet.isVisible = false;
						asteroid.isVisible = false;
						score++;
						explosion.Play(volume: .3f, pitch: 0f, pan: 0f);
					}
				}
			}

			for (int i = 0; i < asteroids.Count; i++)
			{
				if (!asteroids[i].isVisible)
				{
					asteroids.RemoveAt(i);
					i--;
				}
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			_spriteBatch.Begin();

			background1.Draw(_spriteBatch);
			background2.Draw(_spriteBatch);

			_spriteBatch.Draw(playerSprite.sprite, new Rectangle(playerSprite.x+playerSprite.width/2, playerSprite.y-playerSprite.height/2, playerSprite.width, playerSprite.height), null, Color.White, (float)Math.PI/2f, new Vector2(playerSprite.width / 2, playerSprite.height / 2), SpriteEffects.None, 0f);

			foreach (Bullet bullet in bullets)
				bullet.Draw(_spriteBatch);
			foreach (Asteroid asteroid in asteroids)
				asteroid.Draw(_spriteBatch);

			if(score <= -1)
				_spriteBatch.DrawString(scoreSprite, "Score:0", new Vector2(10, 10), Color.White);
			else
				_spriteBatch.DrawString(scoreSprite, "Score:"+score, new Vector2(10, 10), Color.White);

			_spriteBatch.End();

			base.Draw(gameTime);
		}

		public void Shoot()
		{
			Bullet newBullet = new Bullet(Content.Load<Texture2D>("bullet"))
			{
				velocity = new Vector2(asteroidSpeed, 0)
			};
			newBullet.position = new Vector2(playerSprite.x, playerSprite.y) + newBullet.velocity * 5f;
			newBullet.isVisible = true;

			if (bullets.Count < 2)
			{
				bullets.Add(newBullet);
				shotEffect.Play(volume: .1f, pitch: 0f, pan: 0f);
			}
		}

		public void Spawn()
		{
			Asteroid newAsteroid = new Asteroid(Content.Load<Texture2D>("asteroid"))
			{
				velocity = new Vector2(-5, 0)
			};

			Random rand = new Random();

			newAsteroid.position = new Vector2(rand.Next(_graphics.PreferredBackBufferWidth-60, _graphics.PreferredBackBufferWidth-30), rand.Next(30, _graphics.PreferredBackBufferHeight-30)) + newAsteroid.velocity * 5f;
			newAsteroid.isVisible = true;

			asteroids.Add(newAsteroid);
		}
	}

}
