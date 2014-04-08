using UnityEngine;
using System.Collections;

namespace UniSynth
{
	public static class WaveShapes
	{
		// Define 2 * PI as constant
		public const float PI		= Mathf.PI;
		public const float TWO_PI 	= Mathf.PI * 2.0f;
		
		// Enumares all available wave shapes
		public enum Shape
		{
			SINE,
			SINE_APPROX_HP,
			SINE_APPROX_LP,
			SQUARE,
			TRIANGLE,
			SAW
		}
		
		// Uniform signature for all wave generating methods. ( t = normalized time 0.0 -> 1.0 within a single phase )
		public delegate float WaveGenerator( float t );
		
		// Sine wave computed using Mathf.Sin
		public static float WaveSine( float t )
		{
			return Mathf.Sin( t * TWO_PI );
		}
		
		// Sine wave computed using high percision quadratic bezier curver approximation
		public static float WaveSineHighP( float t )
		{
			float sin = 0;
			
			t *= TWO_PI;
		
			if (t < -PI)
				t += TWO_PI;
			else
				if (t >  PI)
					t -= TWO_PI;	
			
			if (t < 0)
			{
				sin = 1.27323954f * t + .405284735f * t * t;
				
				if (sin < 0)
					sin = .225f * (sin *-sin - sin) + sin;
				else
					sin = .225f * (sin * sin - sin) + sin;
			}
			else
			{
				sin = 1.27323954f * t - 0.405284735f * t * t;
				
				if (sin < 0)
					sin = .225f * (sin *-sin - sin) + sin;
				else
					sin = .225f * (sin * sin - sin) + sin;
			}
			
			return sin;
		}
		
		// Sine wave computed using  low percision quadratic bezier curve approximation
		public static float WaveSineLowP( float t )
		{
			t *= TWO_PI;
		
			if (t < -PI)
				t += TWO_PI;
			else
				if (t >  PI)
					t -= TWO_PI;
					
			if (t < 0)
				return 1.27323954f * t + .405284735f * t * t;
			else
				return 1.27323954f * t - 0.405284735f * t * t;		
		}
		
		public static float WaveSquare( float t )
		{
			return ( t < 0.5f ? -1.0f : 1.0f );
		}
		
		public static float WaveTriangle( float t )
		{
			if ( t < 0.25f ) return t * 4.0f;
			if ( t < 0.75f ) return 1.0f - ( t - 0.25f ) * 4.0f;
			return -1.0f + ( t - 0.75f ) * 4.0f;	
		}
		
		public static float WaveSaw( float t )
		{
			return ( t < 0.5f ? t * 2.0f : t * 2.0f - 2.0f );
		}
}
}
