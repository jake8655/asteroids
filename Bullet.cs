using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
	class Bullet
	{
		public Texture2D texture;
		public Vector2 position;
		public Vector2 velocity;
		public Vector2 origin;
		public bool isVisible;

		public Bullet(Texture2D texture)
		{
			this.texture = texture;
			isVisible = false;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, position, null, Color.White, 0f, origin, .05f, SpriteEffects.None, 0);
		}
	}
}
