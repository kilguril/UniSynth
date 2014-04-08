using UnityEditor;
using UnityEngine;
using System.Collections;

using UniSynth2;

namespace UniSynth2.Editor
{
	public class NodeGUIOperation : NodeGUIBase
	{	
		private SynthNodeOperation m_operationNode;
	
		public NodeGUIOperation( ISynthGraphNode dataNode ) : base ( dataNode )
		{
			m_operationNode = dataNode as SynthNodeOperation;
		}
		
		public override Rect GetDefaultRect()
		{
			return new Rect( 0.0f, 0.0f, SynthGraphEditorFactory.NODE_WIDTH, SynthGraphEditorFactory.NODE_HEADER_HEIGHT + SynthGraphEditorFactory.NODE_LINE_HEIGHT * 2.0f);
		}
		
		public override string GetTitle()
		{
			if ( m_operationNode != null )
			{
				switch( m_operationNode.m_operation )
				{
					case SynthNodeOperation.WaveOperation.ADD:
						return "Add";
					case SynthNodeOperation.WaveOperation.SUBTRACT:
						return "Subtract";
					case SynthNodeOperation.WaveOperation.MULTIPLY:
						return "Multiply";
					case SynthNodeOperation.WaveOperation.DIVIDE:
						return "Divide";
				}
			}
		
			return "Operation";
		}
		
		protected override void DrawNode()
		{
			m_operationNode.m_operation = (SynthNodeOperation.WaveOperation) SynthGraphEditorFactory.EnumFieldWithLabel( m_operationNode.m_operation, "Operation" );			
		}
		
		protected override bool ShouldDrawInputHandles ()
		{
			return true;
		}
		
	}
}