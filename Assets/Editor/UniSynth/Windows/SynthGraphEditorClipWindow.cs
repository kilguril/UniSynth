using UnityEditor;
using UnityEngine;
using System.Collections;

using UniSynth2.Editor;

namespace UniSynth2.Editor.Windows
{
	public class SynthGraphEditorClipWindow : SynthGraphEditorWindow
	{	
		public delegate void PlaybackRequested();
		public event PlaybackRequested OnPlaybackRequested;
	
		public enum OptionReturnValue : int
		{			
			NEW_CLIP,
			LOAD_GRAPH,
			SAVE_GRAPH,
			EXPORT_WAV,
			APPLY_CLIP_CHANGES,
			REVERT_CLIP_CHANGES,
			PLAY_PREVIEW
		}
		
		private GUIButtonValue[] m_controls = new GUIButtonValue[]{
			new GUIButtonValue( "Play Clip", (int)OptionReturnValue.PLAY_PREVIEW ),
			new GUIButtonValue( "New SoundClip", (int)OptionReturnValue.NEW_CLIP ),
			new GUIButtonValue( "Load Graph", (int)OptionReturnValue.LOAD_GRAPH ),
			new GUIButtonValue( "Save Graph", (int)OptionReturnValue.SAVE_GRAPH ),
			new GUIButtonValue( "Export WAV", (int)OptionReturnValue.EXPORT_WAV ),
		};
		
		private GUIButtonValue[] m_clipControls = new GUIButtonValue[]{
			new GUIButtonValue( "Apply Changes", (int)OptionReturnValue.APPLY_CLIP_CHANGES ),
			new GUIButtonValue( "Revert Changes", (int)OptionReturnValue.REVERT_CLIP_CHANGES )
		};
		
		private string	m_clipName;
		private float	m_clipLength;
		private int 	m_sampleRate;
		private int		m_sampleCount;
		private bool	m_isStereo;
		private bool	m_is3D;
	
		public SynthGraphEditorClipWindow( int windowId, string windowTitle, Rect windowRect ) : base( windowId, windowTitle, windowRect )
		{
			RevertClipSettings();
		}
	
		protected override void Window (int windowId)
		{
			SoundClip clip = SynthGraphEditorState.ActiveClip;
			int clipval = SynthGraphEditorFactory.BUTTON_GROUP_NO_INPUT;
			
			if ( clip != null )
			{
				EditorGUILayout.BeginVertical();
				
				EditorGUILayout.LabelField( SynthGraphEditor.GetFullProductName() );
				EditorGUILayout.Space();
				
				EditorGUILayout.LabelField( "Clip Name:" );
				m_clipName = EditorGUILayout.TextField( m_clipName );
				
				EditorGUILayout.LabelField( "Clip Length (sec):" );
				m_clipLength = EditorGUILayout.FloatField( m_clipLength );
				
				EditorGUILayout.LabelField( "Sample Rate:" );			
				m_sampleRate = EditorGUILayout.IntField( m_sampleRate );
				
				m_sampleCount = Mathf.FloorToInt(m_sampleRate * m_clipLength);
				EditorGUILayout.LabelField( string.Format( "Sample Count: {0}", m_sampleCount ) );				
				m_isStereo = EditorGUILayout.ToggleLeft( "Stereo", m_isStereo );
				m_is3D = EditorGUILayout.ToggleLeft( "3D Sound", m_is3D );
				
				EditorGUILayout.Separator();							
				EditorGUILayout.EndVertical();
				
				clipval = SynthGraphEditorFactory.ButtonList( m_clipControls );
				
				EditorGUILayout.BeginVertical();
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.EndVertical();
			}
			
			int rval = SynthGraphEditorFactory.ButtonList( m_controls );
			
			if ( clipval == SynthGraphEditorFactory.BUTTON_GROUP_NO_INPUT )
			{
				clipval = rval;
			}
			
			if ( clipval > SynthGraphEditorFactory.BUTTON_GROUP_NO_INPUT )
			{
				switch( (OptionReturnValue)clipval )
				{
					case OptionReturnValue.REVERT_CLIP_CHANGES:
						RevertClipSettings();
					break;
					
					case OptionReturnValue.PLAY_PREVIEW:
						if ( OnPlaybackRequested != null )
						{
							OnPlaybackRequested();
						}
					break;
				}
			}
		}	
		
		private void RevertClipSettings()
		{
			SoundClip clip = SynthGraphEditorState.ActiveClip;
			
			if ( clip != null )
			{
				m_clipName 	  = clip.Name;
				m_sampleCount = clip.SampleLength;
				m_sampleRate  = clip.SampleRate;
				m_clipLength  = m_sampleCount / m_sampleRate;
				m_isStereo    = clip.IsStereo;
				m_is3D		  = clip.Is3D;
			}
		}
	
	}
}