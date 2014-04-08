using UnityEngine;
using System.Collections;

namespace UniSynth2
{
	public class SynthNodeGeneratorWave : SynthGraphNodeBase 
	{
#region Static wave generation methods
		private const float PI		= Mathf.PI;
		private const float TWO_PI 	= Mathf.PI * 2.0f;
		
		// Uniform signature for all wave generating methods. ( t = normalized time 0.0 -> 1.0 within a single phase )
		private delegate float WaveGenerator( float t );
	
		// Sine wave computed using Mathf.Sin
		private static float WaveSine( float t )
		{
			return Mathf.Sin( t * TWO_PI );
		}
		
		// Sine wave computed using high percision quadratic bezier curver approximation
		private static float WaveSineHighP( float t )
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
		private static float WaveSineLowP( float t )
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
		
		private static float WaveSquare( float t )
		{
			return ( t < 0.5f ? -1.0f : 1.0f );
		}
		
		private static float WaveTriangle( float t )
		{
			if ( t < 0.25f ) return t * 4.0f;
			if ( t < 0.75f ) return 1.0f - ( t - 0.25f ) * 4.0f;
			return -1.0f + ( t - 0.75f ) * 4.0f;	
		}
		
		private static float WaveSaw( float t )
		{
			return ( t < 0.5f ? t * 2.0f : t * 2.0f - 2.0f );
		}	
		
		private static WaveGenerator GetGenerationMethod( WaveShape shape )
		{
			switch( shape )
			{
				default:
				case WaveShape.SINE_APPROX_LP:
					return WaveSineLowP;
					
				case WaveShape.SINE_APPROX_HP:
					return WaveSineHighP;				
					
				case WaveShape.SINE:
					return WaveSine;
										
				case WaveShape.SQUARE:
					return WaveSquare;
					
				case WaveShape.TRIANGLE:
					return WaveTriangle;
					
				case WaveShape.SAW:
					return WaveSaw;
			}			
		}
#endregion
	
		public enum WaveShape
		{
			SINE,
			SINE_APPROX_LP,
			SINE_APPROX_HP,
			SQUARE,
			TRIANGLE,
			SAW
		}		
		
		public WaveShape	m_waveShape;
		public float		m_frequency;
		public float		m_gain;
	
		public SynthNodeGeneratorWave()
		{
			m_sourceNodes = new ISynthGraphNode[0];
		}
		
		public override void Process (float[] data, SoundClipState state)
		{
			if ( m_frequency <= 0.0f )
			{
				return;		// If frequency is 0 or negative do nothing.
			}
		
			// Get wave generation method
			WaveGenerator generator = GetGenerationMethod( m_waveShape );
		
			// Get phase length
			float phaseLength = state.m_sampleRate / m_frequency;
			
			float phase;		
			for ( int i = 0; i < data.Length; i++ )
			{
				phase = ( state.m_index + i ) / phaseLength;
				phase -= (int)phase;
				data[ i ] = generator( phase ) * m_gain;
			}
		}
	}
}