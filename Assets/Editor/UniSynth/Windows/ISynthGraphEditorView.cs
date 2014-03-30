using UnityEngine;
using System.Collections;

namespace UniSynth2.Editor.Windows
{
	public interface ISynthGraphEditorView
	{
		void Draw();
		
		Rect GetRect();
		void SetRect( Rect rect );
	}
}