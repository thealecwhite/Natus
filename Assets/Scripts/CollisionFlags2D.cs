using System;

[Flags]
public enum CollisionFlags2D
{
	None = 0,
	Above = 1,
	Below = 2,
	Left = 4,
	Right = 8,
	Stuck = Above | Below | Left | Right
}
