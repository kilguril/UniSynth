using UnityEngine;
using System.Collections;

namespace UniSynth2
{
	public class SynthNodeOutputMono : SynthGraphNodeBase 
	{
		public SynthNodeOutputMono()
		{
			m_sourceNodes = new ISynthGraphNode[1];
		}
		
		public override float Process (SoundClipState state)
		{
			if ( m_sourceNodes[ 0 ] == null )
			{
				return 0.0f;
			}
		
			return m_sourceNodes[ 0 ].Process( state );
		}
	}
}