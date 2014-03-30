using UnityEditor;
using UnityEngine;
using System.Collections;

using UniSynth2.Editor;

namespace UniSynth2.Editor.Windows
{
	public class SynthGraphEditorToolboxWindow : SynthGraphEditorWindow
	{
		public delegate void ToolSelected( ToolReturnValue tool );
		public event ToolSelected OnToolSelected;
	
		public enum ToolReturnValue : int
		{			
			ADD_WAVE_NODE
		}
	
		private GUIButtonValue[] m_controls = new GUIButtonValue[]{
			new GUIButtonValue( "Wave Node", (int)ToolReturnValue.ADD_WAVE_NODE ),
		};
		
		public SynthGraphEditorToolboxWindow( int windowId, string windowTitle, Rect windowRect ) : base( windowId, windowTitle, windowRect )
		{
		}
	
		protected override void Window (int windowId)
		{
			int result = SynthGraphEditorFactory.ButtonList( m_controls );
			
			if ( result >= 0 )
			{
				if ( OnToolSelected != null )
				{
					OnToolSelected( (ToolReturnValue)result );
				}
				
			}
		}	
	
	}
}