using UnityEditor;
using UnityEngine;
using System.Collections;

namespace UniSynth2.Editor
{
	public class SynthGraphEditorState
	{
		public static SoundClip ActiveClip
		{
			get { return m_activeClip; }
			set { m_activeClip = value;}
		}
	
		private static SoundClip m_activeClip;
	
		[MenuItem ("UniSynth/Open Editor")]
		public static void ShowGraphEditor()
		{
			if ( ActiveClip == null )
			{
				ActiveClip = new SoundClip(
					SynthGraphEditor.DEFAULT_CLIP_NAME,
					SynthGraphEditor.DEFAULT_CLIP_LENGTH,
					SynthGraphEditor.DEFAULT_CLIP_SAMPLE,
					SynthGraphEditor.DEFAULT_CLIP_STEREO,
					SynthGraphEditor.DEFAULT_CLIP_3D
				);
			}
		
			EditorWindow.GetWindow( typeof( SynthGraphEditor ) );
		}
	}
}