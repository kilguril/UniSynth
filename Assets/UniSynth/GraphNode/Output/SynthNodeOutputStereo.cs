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
		
		public override void Process (float[] data, SoundClipState state)
		{
			// IMPLEMENT ME
			for ( int i = 0; i < data.Length; i++ )
			{
				data[ i ] = 0;
			}
			
			m_sourceNodes[ 0 ].Process( data, state );
		}
	}
}