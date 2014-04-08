using UnityEditor;
using UnityEngine;
using System.Collections;

using UniSynth2;

namespace UniSynth2.Editor
{
	public class NodeGUIOutput : NodeGUIBase
	{	
		public NodeGUIOutput( ISynthGraphNode dataNode ) : base ( dataNode )
		{
		}
		
		public override Rect GetDefaultRect()
		{
			return new Rect( 0.0f, 0.0f, SynthGraphEditorFactory.NODE_WIDTH, SynthGraphEditorFactory.NODE_HEADER_HEIGHT );
		}
		
		public override string GetTitle()
		{
			if ( m_dataNode != null )
			{
				if ( m_dataNode.GetSourceCount() > 1 )
				{
					return "Stereo Output";
				}
				else
				{
					return "Mono Output";
				}
			}
		
			return "Output";
		}
	
		protected override bool ShouldDrawOutputHandles ()
		{
			return false;
		}		
	}
}