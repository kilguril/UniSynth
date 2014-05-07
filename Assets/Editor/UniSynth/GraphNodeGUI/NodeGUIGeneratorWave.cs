using UnityEditor;
using UnityEngine;
using System.Collections;

using UniSynth2;

namespace UniSynth2.Editor
{
	public class NodeGUIGeneratorWave : NodeGUIBase
	{	
		private SynthNodeGeneratorWave m_generatorNode;
	
		public NodeGUIGeneratorWave( ISynthGraphNode dataNode ) : base ( dataNode )
		{
			m_generatorNode = dataNode as SynthNodeGeneratorWave;
		}
		
		public override Rect GetDefaultRect()
		{
			return new Rect( 0.0f, 0.0f, SynthGraphEditorFactory.NODE_WIDTH, SynthGraphEditorFactory.NODE_HEADER_HEIGHT + SynthGraphEditorFactory.NODE_LINE_HEIGHT * 6.0f);
		}
		
		public override string GetTitle()
		{
			return "Wave Generator";
		}
		
		protected override void DrawNode()
		{			
			ISynthGraphNode[] sources = m_generatorNode.GetSourceNodes();
			
			if ( sources.Length > 0 && sources[ 0 ] != null )
			{
				SynthGraphEditorFactory.FloatFieldOccupiedWithLabel( "Frequency" );
			}
			else
			{
				m_generatorNode.m_frequency = SynthGraphEditorFactory.FloatFieldWithLabel( m_generatorNode.m_frequency , "Frequency" );
			}
			
			if ( sources.Length > 1 && sources[ 1 ] != null )
			{
				SynthGraphEditorFactory.FloatFieldOccupiedWithLabel( "Gain" );
			}
			else
			{
				m_generatorNode.m_gain 		= SynthGraphEditorFactory.FloatFieldWithLabel( m_generatorNode.m_gain, "Gain" );			
			}
			
			m_generatorNode.m_waveShape = (SynthNodeGeneratorWave.WaveShape)SynthGraphEditorFactory.EnumFieldWithLabel ( m_generatorNode.m_waveShape, "Shape" );
		}
		
		protected override bool ShouldDrawInputHandles ()
		{
			return false;
		}
		
		protected override int GetSecondaryInputCount ()
		{
			return 2;
		}
	}
}