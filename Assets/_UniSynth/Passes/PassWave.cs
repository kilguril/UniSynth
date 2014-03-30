using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UniSynth
{
	public class WaveKeyframe
	{
		public float m_time;
		public float m_frequency;
		public float m_gain;
		
		private List< WaveKeyframeDataBinding > m_dataBindings;
		
		public WaveKeyframe( float time, float frequency, float gain )
		{
			m_time 		= time;
			m_frequency = frequency;
			m_gain 		= gain;
			
			m_dataBindings = new List< WaveKeyframeDataBinding >();
		}
		
		public void AddDataBinding( WaveKeyframeDataBinding binding )
		{	
			m_dataBindings.Add( binding );
		}
	}

	public class PassWave : ISoundPass
	{
		private WaveKeyframe[]				m_keyframes;
	
		private int							m_sampleRate;
		private int							m_sampleLength;
	
		private WaveShapes.WaveGenerator	m_waveGenerator;
	
		public PassWave( int sampleLength, int sampleRate, WaveShapes.WaveGenerator wave, WaveKeyframe[] keyframes )
		{
			m_sampleLength  = sampleLength;
			m_sampleRate    = sampleRate;
			m_waveGenerator = wave;
			m_keyframes		= keyframes;
		}
	
		public void Pass (float[] data, int index)
		{
			for ( int i = 0; i < data.Length; i++ )
			{
				float t = (float)( index + i ) / m_sampleLength;

				// Get current keyframe
				int key = 0;
				for ( ; key < m_keyframes.Length - 1 ; key++ )
				{
					if ( t <= m_keyframes[ key + 1 ].m_time )
					{
						break;
					}
				}
				
				if ( t > m_keyframes[ m_keyframes.Length - 1 ].m_time )
				{
					key = m_keyframes.Length - 1;
				}
				
				// Get current frequency and gain
				float freq, gain;
				
				if ( ( key == 0 && m_keyframes.Length <= 1 ) || key == m_keyframes.Length - 1 )
				{
					freq = m_keyframes[ key ].m_frequency;
					gain = m_keyframes[ key ].m_gain;
				}
				else
				{
					// Interpolate values
					float keytime = ( t - m_keyframes[ key ].m_time ) / ( m_keyframes[ key + 1 ].m_time - m_keyframes[ key ].m_time );
					freq = Mathf.Lerp( m_keyframes[ key ].m_frequency, m_keyframes[ key + 1 ].m_frequency, keytime );
					gain = Mathf.Lerp( m_keyframes[ key ].m_gain, m_keyframes[ key + 1 ].m_gain, keytime );
				}
				
				// Get phase length
				float phaseLength = m_sampleRate / freq;
				
				float phase = ( index + i ) / phaseLength;
				phase -= (int)phase;
				
				// Get value
				data[ i ] += m_waveGenerator( phase ) * gain;
			}
		}
	}
}