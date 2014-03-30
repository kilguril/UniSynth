using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UniSynth
{
	public class SoundClip
	{
		// Returns the underlying AudioClip object to be played
		public AudioClip  Clip
		{
			get { return m_clip; }
		}
		
		// Maps exposed parameter 
		private Dictionary< string, ObservableProperty< float > > m_paramList;
	
		private AudioClip 	 m_clip;				// Underlying unity AudioClip object
		private int		  	 m_index;				// Index to the last set position
		
		private string	  	 m_clipName;			// Clip name
		private int	      	 m_lengthSamples;		// Clip length in samples ( sample rate * duration )
		private int		  	 m_sampleRate;			// Clip sample rate ( samples / second )
		private int		  	 m_channels;			// Clip number of channels ( 1 - mono, 2 - stereo )
		private bool	  	 m_3D;					// Is sound 3D
		
		private ISoundPass[] m_passes;				// All passes used to create the effect, executed in order
	
		public SoundClip( string clipName, float length, int sampleRate, bool stereo, bool is3D, ISoundPass[] soundPasses, Dictionary< string, ObservableProperty< float > > paramList )
		{
			m_clipName 		= clipName;
			m_lengthSamples = Mathf.FloorToInt( length * sampleRate );
			m_channels		= ( stereo ? 2 : 1 );
			m_sampleRate	= sampleRate;
			m_3D			= is3D;
		
			m_passes = soundPasses;
			
			m_paramList = paramList;

			m_clip = AudioClip.Create(
				m_clipName,
				m_lengthSamples,
				m_channels,
				m_sampleRate,
				m_3D,
				true,
				OnDataRead,
				OnPositionSet
			);		
		}
		
		public void SetData( string key, float val )
		{
			m_paramList[ key ].Value = val;
		}
		
		public float GetData( string key )
		{
			return m_paramList[ key ].Value;
		}
		
		private void OnDataRead( float[] data )
		{
			for ( int i = 0; i < data.Length; i++ )
			{
				data[ i ] = 0.0f;
			}
		
			for ( int i = 0; i < m_passes.Length; i++ )
			{
				m_passes[ i ].Pass( data, m_index );
			}
			
			m_index += data.Length;
		}
		
		private void OnPositionSet( int position )
		{
			m_index = position;
		}	
	}
}