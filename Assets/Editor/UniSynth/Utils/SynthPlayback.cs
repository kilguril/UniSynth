using UnityEngine;
using System.Collections;

namespace UniSynth2.Editor
{
	public class SynthPlayback
	{
		public AudioSource Source
		{
			get {
				if ( m_playbackObject == null )
				{
					return null;
				}
				
				return m_playbackObject.audio;
			}
		}
	
		public delegate void SynthPlaybackStarted( SynthPlayback playback );
		public static event SynthPlaybackStarted OnSynthPlaybackStarted;
		
		public delegate void SynthPlaybackEnded();
		public static event SynthPlaybackEnded OnSynthPlaybackEnded;
	
		private const string PLAYBACK_OBJECT_NAME = "__UniSynthPlaybackObject";
	
		private GameObject m_playbackObject;
		
		public void PlayClip( SoundClip clip )
		{
			if ( m_playbackObject == null )
			{
				CreatePlaybackObject();
			}
			
			m_playbackObject.audio.clip = clip.Clip;
			m_playbackObject.audio.Play();
		}
		
		public void Update()
		{
			if ( m_playbackObject != null && m_playbackObject.audio != null )
			{
				if ( !m_playbackObject.audio.isPlaying )
				{
					DestroyPlaybackObject();
				}
			}
		}
		
		private void CreatePlaybackObject()
		{
			m_playbackObject = new GameObject( PLAYBACK_OBJECT_NAME );
			m_playbackObject.AddComponent< AudioSource >();
			
			if ( OnSynthPlaybackStarted != null )
			{
				OnSynthPlaybackStarted( this );
			}
			
		}
		
		private void DestroyPlaybackObject()
		{
			GameObject.DestroyImmediate( m_playbackObject );	
			
			if ( OnSynthPlaybackEnded != null )
			{
				OnSynthPlaybackEnded();
			}
		}	
	}
}