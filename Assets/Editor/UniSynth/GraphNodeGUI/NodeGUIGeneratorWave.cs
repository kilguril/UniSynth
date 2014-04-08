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
			m_generatorNode.m_waveShape = (SynthNodeGeneratorWave.WaveShape)SynthGraphEditorFactory.EnumFieldWithLabel ( m_generatorNode.m_waveShape, "Shape" );
			m_generatorNode.m_frequency = SynthGraphEditorFactory.FloatFieldWithLabel( m_generatorNode.m_frequency , "Frequency" );
			m_generatorNode.m_gain 		= SynthGraphEditorFactory.FloatFieldWithLabel( m_generatorNode.m_gain, "Gain" );			
		}
		
		protected override bool ShouldDrawInputHandles ()
		{
			return false;
		}
		
	}
}