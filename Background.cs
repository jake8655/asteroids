using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
	class Background
	{
		public Texture2D texture;
		public Rectangle rect;

		public void Draw(SpriteBatch spritebatch)
		{
			spritebatch.Draw(texture, rect, Color.White);
		}
	}

	class Scrolling : Background
	{
		public Scrolling(Texture2D newTexture, Rectangle newRect)
		{
			texture = newTexture;
			rect = newRect;
		}

		public void Update()
		{
			rect.X -= 3;
		}
	}
}
