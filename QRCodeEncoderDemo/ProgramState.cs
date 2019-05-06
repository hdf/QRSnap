/////////////////////////////////////////////////////////////////////
//
//	QR Code Library
//
//	QR Code encoder save and restore program state.
//
//	Author: Uzi Granot
//	Version: 1.0
//	Date: June 4, 2018
//	Copyright (C) 2013-2018 Uzi Granot. All Rights Reserved
//
//	QR Code Library C# class library and the attached test/demo
//  applications are free software.
//	Software developed by this author is licensed under CPOL 1.02.
//	Some portions of the QRCodeVideoDecoder are licensed under GNU Lesser
//	General Public License v3.0.
//
//	The solution is made of 5 projects:
//	1. QRCodeEncoderDecoderLibrary: QR code encoding and decoding.
//	2. QRCodeEncoderDemo: Create QR Code images.
//	3. QRCodeDecoderDemo: Decode QR code image files.
//	4. QRCodeVideoDecoder: Decode QR code using web camera.
//		This demo program is using some of the source modules of
//		Camera_Net project published at CodeProject.com:
//		https://www.codeproject.com/Articles/671407/Camera_Net-Library
//		and at GitHub: https://github.com/free5lot/Camera_Net.
//		This project is based on DirectShowLib.
//		http://sourceforge.net/projects/directshownet/
//		This project includes a modified subset of the source modules.
//
//	The main points of CPOL 1.02 subject to the terms of the License are:
//
//	Source Code and Executable Files can be used in commercial applications;
//	Source Code and Executable Files can be redistributed; and
//	Source Code can be modified to create derivative works.
//	No claim of suitability, guarantee, or any warranty whatsoever is
//	provided. The software is provided "as-is".
//	The Article accompanying the Work may not be distributed or republished
//	without the Author's consent
//
//	For version history please refer to QRCode.cs
/////////////////////////////////////////////////////////////////////

using QRCodeEncoderDecoderLibrary;
using System;
using System.Drawing.Imaging;
using System.IO;

namespace QRCodeEncoderDemo
{
public class ProgramState
	{
	public ErrorCorrection EncodeErrorCorrection = ErrorCorrection.M;
	public int EncodeModuleSize = 4;
	public int EncodeQuietZone = 16;
	public ImageFormat EncodeImageFormat = ImageFormat.Png;
	public string EncodeData = "QR Code encoder and decoder C# open source library including test/demo programs.";

	public int SaveSpecialImageWidth = 1920;
	public int SaveSpecialImageHeight = 1920;
	public int SaveSpecialImageMargin = 96;
	public double SaveSpecialCameraDistance = 3840.0;
	public double SaveSpecialCameraRotation = 0.0;
	public double SaveSpecialCameraViewAngle = 0.0;
	public int SaveSpecialFileFormat = 0;
	public double SaveSpecialPercentError = 0;
	public int SaveSpecialErrorArea = 0;

	public static ProgramState	State;
	private static string FileName = "QRCodeEncodeState.txt";

	////////////////////////////////////////////////////////////////////
	// Save Program State
	////////////////////////////////////////////////////////////////////

	public static void SaveState()
		{
		// save state
		using(StreamWriter Output = new StreamWriter(new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None)))
			{
			Output.WriteLine(string.Format("{0},{1},{2},{3}",
				State.EncodeErrorCorrection.ToString(), State.EncodeModuleSize, State.EncodeQuietZone, State.EncodeImageFormat.ToString()));
			Output.WriteLine(State.EncodeData);
			}

		// exit
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Load Program State
	////////////////////////////////////////////////////////////////////

	public static void LoadState()
		{
		State = new ProgramState();

		// load program state
		try
			{
			using(StreamReader Input = new StreamReader(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
				{
				string Line = Input.ReadLine();
				if(Line == null) throw new ApplicationException("Load state");
				string[] Flds = Line.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
				if(Flds.Length != 4) throw new ApplicationException("Load state");
				switch(Flds[0][0])
					{
					case 'L':
						State.EncodeErrorCorrection = ErrorCorrection.L;
						break;

					case 'M':
						State.EncodeErrorCorrection = ErrorCorrection.M;
						break;

					case 'Q':
						State.EncodeErrorCorrection = ErrorCorrection.Q;
						break;

					case 'H':
						State.EncodeErrorCorrection = ErrorCorrection.H;
						break;

					default:
						throw new ApplicationException("Load state");
					}
				State.EncodeModuleSize = int.Parse(Flds[1]);
				State.EncodeQuietZone = int.Parse(Flds[2]);
				switch(Flds[3])
					{
					case "Png":
						State.EncodeImageFormat = ImageFormat.Png;
						break;

					case "Jpeg":
						State.EncodeImageFormat = ImageFormat.Jpeg;
						break;

					case "Bmp":
						State.EncodeImageFormat = ImageFormat.Bmp;
						break;

					case "Gif":
						State.EncodeImageFormat = ImageFormat.Gif;
						break;

					default:
						throw new ApplicationException("Load state");
					}

				Line = Input.ReadToEnd();
				if(Line == null) throw new ApplicationException("Load state");
				State.EncodeData = Line;				
				}
			}
		catch
			{
			State = null;
			}

		// we have no program state file
		if(State == null)
			{
			// create new default program state
			State = new ProgramState();

			// save default
			SaveState();
			}

		// exit
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Load Program State
	////////////////////////////////////////////////////////////////////

	public static void SetDefaultState()
		{
		// create new default program state
		State = new ProgramState();

		// save default
		SaveState();

		// exit
		return;
		}
	}
}
