using UnityEditor;
using UnityEngine;
using System.Collections;

using UniSynth2.Editor;
using UniSynth2.Editor.Utils;

namespace UniSynth2.Editor.Windows
{
	public class SynthGraphEditorWaveformWindow : SynthGraphEditorWindow
	{	
		private const float		MIN_GRAPH_WIDTH 			= 100.0f;
		private const float		MIN_GRAPH_HEIGHT 			= 60.0f; 
	
		private const float		GRAPH_MARGIN_HORIZONTAL		= 20.0f;
		private const float		GRAPH_MARGIN_VERTICAL		= 10.0f;
	
		private ISynthGraphNode m_rootNode;
	
		public SynthGraphEditorWaveformWindow( int windowId, string windowTitle, Rect windowRect ) : base( windowId, windowTitle, windowRect )
		{
			NodeGUIBase.OnNodeSelected += HandleNodeSelected;
		}
	
		protected override void Window (int windowId)
		{
			if ( m_rootNode != null )
			{
				if ( ( m_windowRect.width >= MIN_GRAPH_WIDTH) && ( m_windowRect.height >= MIN_GRAPH_HEIGHT ) )
				{
					DrawGraph();
				}
			}
		}	
		
		private float[] ProcessNode()
		{
			SoundClip clip = SynthGraphEditorState.ActiveClip;
			
			// Calculate clip output
			float[] buffer = new float[ clip.SampleLength ];
			SoundClipState state = new SoundClipState();
			
			state.m_sampleRate   = clip.SampleRate;
			state.m_sampleLength = clip.SampleLength;
			state.m_index		 = 0;
			
			for ( int i = 0; i < buffer.Length; i++, state.m_index++ )
			{
				buffer[ i ] = m_rootNode.Process( state );
			}
			
			return buffer;
		}
		
		private void DrawGraph()
		{
			DrawGrid();
		
			SynthGraphCoordMapper mapper = new SynthGraphCoordMapper( -1.0f, 1.0f, m_windowRect.height - GRAPH_MARGIN_VERTICAL, SynthGraphEditorFactory.WINDOW_HEADER_HEIGHT + GRAPH_MARGIN_VERTICAL, true );
					
			float[] data = ProcessNode();
			int sampleSkip = Mathf.FloorToInt( data.Length / ( m_windowRect.width - GRAPH_MARGIN_HORIZONTAL * 2.0f ) );
			
			SynthGraphEditorUtils.BeginLines( Color.green );
			
			Vector2 from, to;
			float val0, val1;
			
			float x = GRAPH_MARGIN_HORIZONTAL;
			
			for ( int i = sampleSkip; i < data.Length && x <= m_windowRect.width - GRAPH_MARGIN_HORIZONTAL ; i += sampleSkip )
			{
				val0 = data[ i - sampleSkip ];
				val1 = data[ i ];
				
				from.x = x;
				from.y = mapper.MapFromTo( val0 );
				
				to.x   = x += 1.0f;	
				to.y   = mapper.MapFromTo( val1 );
				
				SynthGraphEditorUtils.DrawLine( from, to );
			}
			
			SynthGraphEditorUtils.EndLines();
		}
		
		private void DrawGrid()
		{
			SynthGraphEditorUtils.BeginLines( Color.white );
						
			float x,y;
			Vector2 from, to;						
					
			x = GRAPH_MARGIN_HORIZONTAL;
								
			from.x = x;
			from.y = GRAPH_MARGIN_VERTICAL + SynthGraphEditorFactory.WINDOW_HEADER_HEIGHT;			
			to.x   = x;
			to.y   = m_windowRect.height - GRAPH_MARGIN_VERTICAL;
			SynthGraphEditorUtils.DrawLine( from, to );			
			
			y = SynthGraphEditorFactory.WINDOW_HEADER_HEIGHT + ( m_windowRect.height - SynthGraphEditorFactory.WINDOW_HEADER_HEIGHT ) / 2.0f;
			from.x 	= GRAPH_MARGIN_HORIZONTAL;
			from.y 	= y;
			to.x 	= m_windowRect.width - GRAPH_MARGIN_HORIZONTAL;
			to.y 	= y;
			SynthGraphEditorUtils.DrawLine( from, to );			
						
			SynthGraphEditorUtils.EndLines();
			
			// Draw labels
			GUI.Label( new Rect( 0, SynthGraphEditorFactory.WINDOW_HEADER_HEIGHT, GRAPH_MARGIN_HORIZONTAL, 20.0f ), "+1");
			GUI.Label( new Rect( 0, y - 10.0f, GRAPH_MARGIN_HORIZONTAL, 20.0f ), " 0");
			GUI.Label( new Rect( 0, m_windowRect.height - SynthGraphEditorFactory.WINDOW_HEADER_HEIGHT, GRAPH_MARGIN_HORIZONTAL, 20.0f ), "-1");
		}
		
		private void HandleNodeSelected( NodeGUIBase node )
		{
			m_rootNode = node.DataNode;
		
			Log ( string.Format("{0} selected", node.ToString()));
		}
	}
}