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
	}
}