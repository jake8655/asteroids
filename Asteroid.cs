using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Asteroids
{
	class Asteroid
	{
		public Texture2D texture;
		public Vector2 position;
		public Vector2 velocity;
		public Vector2 origin;
		public bool isVisible;

		public Asteroid(Texture2D texture)
		{
			this.texture = texture;
			isVisible = false;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, new Vector2(position.X-60, position.Y-60), null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0);
		}
	}
}
