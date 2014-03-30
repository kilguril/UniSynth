using UnityEngine;
using System.Collections;

namespace UniSynth
{
	public interface ISoundPass 
	{
		void Pass( float[] data, int index );
	}
}