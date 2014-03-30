using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using UniSynth;

public class Test : MonoBehaviour 
{
	private const string SAMPLE_PATH = "/Samples/";
	
	private SoundClip sound;
	private float	  chain = 0.0f;

	// Use this for initialization
	void Start () 
	{
		gameObject.AddComponent< AudioSource >();
		
		sound = LoadFromXML("SerializeTest.xml");
		audio.clip = sound.Clip;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ( Input.GetKeyDown( KeyCode.X ) ) 
		{
			audio.Play();
		}
		
		if ( Input.GetKeyDown( KeyCode.Z ) )
		{
			chain++;
			
			if ( chain > 10.0f )
			{
				chain = 0.0f;
			}
			
			sound.SetData( "Chain", chain );
		}
	}
	
	private SoundClip LoadFromXML( string fileName )
	{
		string path = string.Format("{0}{1}{2}", Application.dataPath, SAMPLE_PATH, fileName );
		
		FileStream fs = new FileStream( path, FileMode.Open );
		
		XmlDocument xml = new XmlDocument();
		xml.Load( fs );
		
		fs.Close();
		
		return SoundXmlSerializer.FromXML( xml );
	}
}
