using UnityEngine;
using System.Collections;

namespace UniSynth2
{
	public class SoundClip
	{
		// Gets the underlying AudioClip object to be played
		public AudioClip  Clip
		{
			get { return m_clip; }
		}
		
		public string Name
		{
			get { return m_clipName;  }
		}
		
		public int SampleLength
		{
			get { return m_lengthSamples; }			
		}
		
		public int SampleRate
		{
			get { return m_sampleRate; }
		}
		
		public bool IsStereo
		{
			get { return ( m_channels > 1 ); }
		}
		
		public bool Is3D
		{
			get { return m_3D; }
		}
		
		// Gets the output / root node of the process
		public ISynthGraphNode Root
		{
			get { return m_outputNode; }
		}
		
		private AudioClip 	 m_clip;				// Underlying unity AudioClip object
		private int		  	 m_index;				// Index to the last set position
		
		private string	  	 m_clipName;			// Clip name
		private int	      	 m_lengthSamples;		// Clip length in samples ( sample rate * duration )
		private int		  	 m_sampleRate;			// Clip sample rate ( samples / second )
		private int		  	 m_channels;			// Clip number of channels ( 1 - mono, 2 - stereo )
		private bool	  	 m_3D;					// Is sound 3D
		
		ISynthGraphNode		 m_outputNode;			// Data output node ( process will propagate upwards )
		
		private SoundClipState m_clipState;
		
		public SoundClip( string clipName, float length, int sampleRate, bool stereo, bool is3D )
		{
			// Set parameters
			m_clipName 		= clipName;
			m_lengthSamples = Mathf.FloorToInt( length * sampleRate );
			m_channels		= ( stereo ? 2 : 1 );
			m_sampleRate	= sampleRate;
			m_3D			= is3D;
			
			// Create audioclip
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
			
			m_clipState = new SoundClipState();
			m_clipState.m_index = 0;
			m_clipState.m_sampleLength = m_lengthSamples;
			m_clipState.m_sampleRate   = m_sampleRate;
			
			// Initialize root node based on output type ( mono / stereo )
			if ( m_channels > 1 )
			{
				m_outputNode = new SynthNodeOutputStereo();
			}
			else
			{
				m_outputNode = new SynthNodeOutputMono();
			}
		}
		
		private void OnDataRead( float[] data )
		{
			if ( ( Object.ReferenceEquals( m_clip, null ) ) || ( Object.ReferenceEquals( m_outputNode, null ) ) )
			{
				return;
			}
		
			for ( int i = 0; i < data.Length; i++, m_index++ )
			{
				m_clipState.m_index = m_index; 
				data[ i ] = m_outputNode.Process( m_clipState );
			}
		}
		
		private void OnPositionSet( int position )
		{
			m_index = position;
		}	
	}
}