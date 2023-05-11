using System;

namespace GXPEngine.Core
{
	public struct Vec2
	{
		public float x;
		public float y;
		
		public Vec2 (float x, float y)
		{
			this.x = x;
			this.y = y;
		}
		
		override public string ToString() {
			return "[Vec2 " + x + ", " + y + "]";
		}
	}
}

