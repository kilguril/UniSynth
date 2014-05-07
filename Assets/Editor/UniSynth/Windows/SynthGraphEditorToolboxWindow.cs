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
			ADD_WAVE_NODE,
			ADD_CONSTANT_NODE,
			ADD_OPERATION_NODE,
			ADD_TIME_NODE,
			ADD_NOISE_NODE,
			ADD_MAPPER_NODE
		}
	
		private GUIButtonValue[] m_controls = new GUIButtonValue[]{
			new GUIButtonValue( "Wave Node", (int)ToolReturnValue.ADD_WAVE_NODE ),
			new GUIButtonValue( "Constant Node", (int)ToolReturnValue.ADD_CONSTANT_NODE ),
			new GUIButtonValue( "Operation Node", (int)ToolReturnValue.ADD_OPERATION_NODE ),
			new GUIButtonValue( "Time Node", (int)ToolReturnValue.ADD_TIME_NODE ),
			new GUIButtonValue( "Noise Node TEMP", (int)ToolReturnValue.ADD_NOISE_NODE ),
			new GUIButtonValue( "Mapper Node", (int)ToolReturnValue.ADD_MAPPER_NODE )
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