﻿using UnityEngine;
using System.Collections;

namespace UniSynth2
{
	public class SynthNodeOutputMono : SynthGraphNodeBase 
	{
		public SynthNodeOutputMono()
		{
			m_sourceNodes = new ISynthGraphNode[1];
		}
		
		public override void Process (float[] data, SoundClipState state)
		{
			for ( int i = 0; i < data.Length; i++ )
			{
				data[ i ] = 0;
			}
			
			if ( m_sourceNodes[ 0 ] != null )
			{
				m_sourceNodes[ 0 ].Process( data, state );
			}
		}
	}
}