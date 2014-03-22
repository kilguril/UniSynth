using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace UniSynth
{
	public class SoundXmlSerializer
	{
		private const string					DEFAULT_CLIP_NAME			= "Undefined";
		private const float						DEFAULT_CLIP_LENGTH			= 1.0f;
		private const int						DEFAULT_CLIP_SAMPLERATE		= 1;
		private const bool						DEFAULT_CLIP_STEREO			= false;
		private const bool						DEFAULT_CLIP_3D				= false;
	
		private static WaveShapes.WaveGenerator	DEFAULT_WAVE_SHAPE			= WaveShapes.WaveSquare;
	
		private const float						DEFAULT_KEYFRAME_TIME 		= 0.0f;
		private const float						DEFAULT_KEYFRAME_FREQUENCY  = 1.0f;
		private const float						DEFAULT_KEYFRAME_GAIN		= 0.0f;
	
		// Deserialize from XML
		public static SoundClip FromXML( XmlDocument xml )
		{
			XmlNode clipNode = xml.SelectSingleNode("Clip");
		
			string			   clipName				= ParseString( clipNode, "name", DEFAULT_CLIP_NAME );
			float			   clipLength			= ParseFloat( clipNode, "length", DEFAULT_CLIP_LENGTH );
			int				   clipSampleRate		= ParseInt( clipNode, "sampleRate", DEFAULT_CLIP_SAMPLERATE );
			bool			   clipStereo			= ParseBool( clipNode, "stereo", DEFAULT_CLIP_STEREO );
			bool			   clip3D				= ParseBool( clipNode, "is3D", DEFAULT_CLIP_3D );
			List< ISoundPass > clipSoundPasses		= new List< ISoundPass >();
			
			foreach( XmlNode waveNode in clipNode.SelectNodes("Wave") )
			{
				clipSoundPasses.Add( ParseWave( waveNode, clipSampleRate, Mathf.FloorToInt( clipSampleRate * clipLength ) ) );
			}
		
			return new SoundClip( clipName, clipLength, clipSampleRate, clipStereo, clip3D, clipSoundPasses.ToArray() );
		}
		
		// Serialize to XML
		public static XmlDocument ToXML( SoundClip clip )
		{
			throw new NotImplementedException();
		}
		
		private static PassWave ParseWave( XmlNode node, int sampleRate, int sampleLength )
		{
			XmlNodeList	keyframeNodes = node.SelectNodes( "Key" );
			WaveKeyframe[] keyframes = new WaveKeyframe[ keyframeNodes.Count + 1 ];
			
			keyframes[ 0 ] = ParseWaveKeyframe( node );
			
			for ( int i = 0; i < keyframeNodes.Count; i++ )
			{
				keyframes[ i + 1 ] = ParseWaveKeyframe( keyframeNodes[ i ] );
			}
			
			return new PassWave( sampleLength, sampleRate, ParseWaveShape( node ), keyframes );
		}
		
		private static WaveKeyframe ParseWaveKeyframe( XmlNode node )
		{
			float time, frequency, gain;
			
			time 	  = ParseFloat( node, "time", DEFAULT_KEYFRAME_TIME );
			frequency = ParseFloat( node, "frequency", DEFAULT_KEYFRAME_FREQUENCY );
			gain 	  = ParseFloat( node, "gain", DEFAULT_KEYFRAME_GAIN );
						
			return new WaveKeyframe( time, frequency, gain );
		}
		
		private static WaveShapes.WaveGenerator ParseWaveShape( XmlNode node )
		{
			try
			{
				string shapeIdentifier = node.Attributes[ "shape" ].InnerText;
				
				switch( shapeIdentifier )
				{
					case "Square":
						return WaveShapes.WaveSquare;
					
					case "Triangle":
						return WaveShapes.WaveTriangle;
					
					case "Saw":
						return WaveShapes.WaveSaw;
					
					case "Sine":
						return WaveShapes.WaveSine;
					
					case "SineLP":
						return WaveShapes.WaveSineLowP;
					
					case "SineHP":
						return WaveShapes.WaveSineHighP;
					
					default:
						return DEFAULT_WAVE_SHAPE;
				}
			}
			catch
			{
				return DEFAULT_WAVE_SHAPE;
			}
		}
		
		private static string ParseString( XmlNode node, string attributeName, string defaultValue )
		{
			try
			{
				return node.Attributes[ attributeName ].InnerText;
			}
			catch
			{
				return defaultValue;
			}
		}
		
		private static float ParseFloat( XmlNode node, string attributeName, float defaultValue )
		{
			try
			{
				return float.Parse( node.Attributes[ attributeName ].InnerText );
			}
			catch
			{
				return defaultValue;
			}
		}
		
		private static int ParseInt( XmlNode node, string attributeName, int defaultValue )
		{
			try
			{
				return int.Parse( node.Attributes[ attributeName ].InnerText );
			}
			catch
			{
				return defaultValue;
			}
		}
		
		private static bool ParseBool( XmlNode node, string attributeName, bool defaultValue )
		{
			try
			{
				return bool.Parse( node.Attributes[ attributeName ].InnerText );
			}
			catch
			{
				return defaultValue;
			}
		}
	}
}
