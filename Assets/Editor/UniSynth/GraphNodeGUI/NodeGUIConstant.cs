using UnityEditor;
using UnityEngine;
using System.Collections;

using UniSynth2;

namespace UniSynth2.Editor
{
	public class NodeGUIConstant : NodeGUIBase
	{	
		private SynthNodeConstant m_constantNode;
	
		public NodeGUIConstant( ISynthGraphNode dataNode ) : base ( dataNode )
		{
			m_constantNode = dataNode as SynthNodeConstant;
		}
		
		public override Rect GetDefaultRect()
		{
			return new Rect( 0.0f, 0.0f, SynthGraphEditorFactory.NODE_WIDTH, SynthGraphEditorFactory.NODE_HEADER_HEIGHT + SynthGraphEditorFactory.NODE_LINE_HEIGHT * 2.0f);
		}
		
		public override string GetTitle()
		{
			return "Constant";
		}
		
		protected override void DrawNode()
		{
			m_constantNode.m_constantValue = SynthGraphEditorFactory.FloatFieldWithLabel( m_constantNode.m_constantValue, "Value" );			
		}
		
		protected override bool ShouldDrawInputHandles ()
		{
			return false;
		}
		
	}
}