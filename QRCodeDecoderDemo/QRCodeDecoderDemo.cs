﻿/////////////////////////////////////////////////////////////////////
//
//	QR Code Library
//
//	QR Code Decoder test/demo application.
//
//	Author: Uzi Granot
//	Version: 1.0
//	Date: June 30, 2018
//	Copyright (C) 2013-2018 Uzi Granot. All Rights Reserved
//
//	QR Code Library C# class library and the attached test/demo
//  applications are free software.
//	Software developed by this author is licensed under CPOL 1.02.
//	Some portions of the QRCodeVideoDecoder are licensed under GNU Lesser
//	General Public License v3.0.
//
//	The solution is made of 4 projects:
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
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace QRCodeDecoderDemo {
  /// <summary>
  /// Test QR Code Decoder
  /// </summary>
  public partial class QRCodeDecoderDemo : Form {
    private QRDecoder QRCodeDecoder;
    private Bitmap QRCodeInputImage;
    private Rectangle ImageArea = new Rectangle();

    /// <summary>
    /// Constructor
    /// </summary>
    public QRCodeDecoderDemo() {
      InitializeComponent();
      return;
    }

    /// <summary>
    /// Test decode program initialization
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    private void OnLoad(object sender, EventArgs e) {
      // program title
      Text = "QRCodeDecoderDemo - " + QRCode.VersionNumber + " \u00a9 2013-2018 Uzi Granot. All rights reserved.";

#if DEBUG
    // current directory
    string CurDir = Environment.CurrentDirectory;
    string WorkDir = CurDir.Replace("bin\\Debug", "Work");
    if(WorkDir != CurDir && Directory.Exists(WorkDir)) Environment.CurrentDirectory = WorkDir;

    // open trace file
    QRCodeTrace.Open("QRCodeDecoderTrace.txt");
    QRCodeTrace.Write("QRCodeDecoderDemo");
#endif

      // create decoder
      QRCodeDecoder = new QRDecoder();

      // resize window
      OnResize(sender, e);
      return;
    }

    private void OnLoadImage(object sender, EventArgs e) {
      // get file name to decode
      OpenFileDialog Dialog = new OpenFileDialog {
        Filter = "Image Files(*.png;*.jpg;*.gif;*.tif)|*.png;*.jpg;*.gif;*.tif;*.bmp)|All files (*.*)|*.*",
        Title = "Load QR Code Image",
        InitialDirectory = Directory.GetCurrentDirectory(),
        RestoreDirectory = true,
        FileName = string.Empty
      };

      // display dialog box
      if (Dialog.ShowDialog() != DialogResult.OK) return;

      // clear parameters
      ImageFileLabel.Text = Dialog.FileName;

      // disable buttons
      LoadImageButton.Enabled = false;

      // dispose previous image
      if (QRCodeInputImage != null) QRCodeInputImage.Dispose();

      // load image to bitmap
      QRCodeInputImage = new Bitmap(Dialog.FileName);

      // trace
#if DEBUG
    QRCodeTrace.Format("****");
    QRCodeTrace.Format("Decode image: {0} ", Dialog.FileName);
    QRCodeTrace.Format("Image width: {0}, Height: {1}", QRCodeInputImage.Width, QRCodeInputImage.Height);
#endif

      // decode image
      byte[][] DataByteArray = QRCodeDecoder.ImageDecoder(QRCodeInputImage);

      // convert results to text
      DataTextBox.Text = QRDecoder.QRCodeResult(DataByteArray);

      // enable buttons
      LoadImageButton.Enabled = true;

      // force repaint
      Invalidate();
      return;
    }

    ////////////////////////////////////////////////////////////////////
    // paint QR Code image
    ////////////////////////////////////////////////////////////////////

    private void OnPaint(object sender, PaintEventArgs e) {
      // no image
      if (QRCodeInputImage == null) return;

      // calculate image area width and height to preserve aspect ratio
      Rectangle ImageRect = new Rectangle {
        Height = (ImageArea.Width * QRCodeInputImage.Height) / QRCodeInputImage.Width
      };

      if (ImageRect.Height <= ImageArea.Height) {
        ImageRect.Width = ImageArea.Width;
      } else {
        ImageRect.Width = (ImageArea.Height * QRCodeInputImage.Width) / QRCodeInputImage.Height;
        ImageRect.Height = ImageArea.Height;
      }

      // calculate position
      ImageRect.X = ImageArea.X + (ImageArea.Width - ImageRect.Width) / 2;
      ImageRect.Y = ImageArea.Y + (ImageArea.Height - ImageRect.Height) / 2;
      e.Graphics.DrawImage(QRCodeInputImage, ImageRect);
      return;
    }

    ////////////////////////////////////////////////////////////////////
    // Resize Encoder Control
    ////////////////////////////////////////////////////////////////////

    private void OnResize(object sender, EventArgs e) {
      // minimize
      if (ClientSize.Width == 0) return;

      // center header label
      HeaderLabel.Left = (ClientSize.Width - HeaderLabel.Width) / 2;

      // put button at bottom left
      LoadImageButton.Top = ClientSize.Height - LoadImageButton.Height - 8;

      // image file label
      ImageFileLabel.Top = LoadImageButton.Top + (LoadImageButton.Height - ImageFileLabel.Height) / 2;
      ImageFileLabel.Width = ClientSize.Width - ImageFileLabel.Left - 16;

      // data text box
      DataTextBox.Top = LoadImageButton.Top - DataTextBox.Height - 8;
      DataTextBox.Width = ClientSize.Width - 8 - SystemInformation.VerticalScrollBarWidth;

      // decoded data label
      DecodedDataLabel.Top = DataTextBox.Top - DecodedDataLabel.Height - 4;

      // image area
      ImageArea.X = 4;
      ImageArea.Y = HeaderLabel.Bottom + 4;
      ImageArea.Width = ClientSize.Width - ImageArea.X - 4;
      ImageArea.Height = DecodedDataLabel.Top - ImageArea.Y - 4;

      if (QRCodeInputImage != null) Invalidate();
      return;
    }
  }
}
