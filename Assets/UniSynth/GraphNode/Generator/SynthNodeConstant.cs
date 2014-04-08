using UnityEngine;
using System.Collections;


namespace UniSynth2
{
	public class SynthNodeConstant : SynthGraphNodeBase 
	{
		public float	m_constantValue;
	
		public SynthNodeConstant()
		{
			m_sourceNodes = new ISynthGraphNode[0];
		}
	
		public override void Process (float[] data, SoundClipState state)
		{
			for( int i = 0; i < data.Length; i++ )
			{
				data[ i ] = m_constantValue;
			}
		}
	}
}