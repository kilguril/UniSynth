﻿using UnityEngine;
using System.Collections;

namespace UniSynth2
{
	public interface ISynthGraphNode
	{
		int GetSourceCount ();
		ISynthGraphNode[] GetSourceNodes ();
		
		void SetSourceNode (ISynthGraphNode node, int index);
		
		float Process (SoundClipState state);
	}
}