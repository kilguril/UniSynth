using UnityEngine;
using System.Collections;

namespace UniSynth2
{
	public class SynthNodeOutputStereo : SynthGraphNodeBase 
	{
		public SynthNodeOutputStereo()
		{
			m_sourceNodes = new ISynthGraphNode[2];
		}
		
		public override float Process (SoundClipState state)
		{
			if ( state.m_index % 2 == 0 )
			{
				return m_sourceNodes[ 0 ].Process( state );
			}
			
			return m_sourceNodes[ 1 ].Process( state );
		}
	}
}