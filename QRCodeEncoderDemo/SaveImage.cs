/////////////////////////////////////////////////////////////////////
//
//	QR Code Library
//
//	QR Code encoder save image screen.
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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace QRCodeEncoderDemo {
  /// <summary>
  /// Save QR code as an image file
  /// </summary>
  public partial class SaveImage : Form {
    private QRCode QRCode;
    private int QRModuleSize;
    private int QRQuietZone;
    private Bitmap ImageBitmap;

    public SaveImage
        (
        QRCode QRCode
        ) {
      this.QRCode = QRCode;
      InitializeComponent();
      return;
    }

    private void OnLoad(object sender, EventArgs e) {
      // display initial values
      QRCodeDimensionLabel.Text = QRCode.QRCodeDimension.ToString();
      QRModuleSize = 4;
      QRModuleSizeTextBox.Text = QRModuleSize.ToString();
      QRQuietZone = 16;
      QRQuietZoneTextBox.Text = QRQuietZone.ToString();

      // load image file type combo box
      foreach (ImageFormat ImageFormat in QRCode.FileFormat) ImageFormatComboBox.Items.Add(ImageFormat);
      ImageFormatComboBox.SelectedIndex = 0;

      // load hatch style combo box
      for (int Index = -1; Index < 53; Index++) HatchStyleComboBox.Items.Add(Index);
      HatchStyleComboBox.SelectedIndex = 0;

      BrushColorButton.BackColor = Color.LightSkyBlue;
      ImageRotationTextBox.Text = "0";
      CameraViewRotationTextBox.Text = "0";
      CameraViewRotationTextBox.Text = "0";

      // set none radio button
      NoneRadioButton.Checked = true;

      // set none error radio button
      ErrorNoneRadioButton.Checked = true;
      ErrorDiameterTextBox.Text = "8";
      ErrorNumberTextBox.Text = "2";
      return;
    }

    private void OnImageFileFormat
        (
        object sender,
        ListControlConvertEventArgs e
        ) {
      e.Value = QRCode.FileSelect((ImageFormat)e.ListItem);
      return;
    }

    private void OnHatchStyleFormat(object sender, ListControlConvertEventArgs e) {
      int Item = (int)e.ListItem;
      e.Value = (int)e.ListItem < 0 ? "Solid" : ((HatchStyle)e.ListItem).ToString();
      return;
    }

    private void OnModuleWidthChanged(object sender, EventArgs e) {
      // size of white or black square
      if (!int.TryParse(QRModuleSizeTextBox.Text.Trim(), out QRModuleSize) || QRModuleSize <= 0 || QRModuleSize > 1000) QRModuleSize = 0;

      // set some fields
      SetQRImageDimension();
      return;
    }

    private void OnQuietZoneChanged(object sender, EventArgs e) {
      // quiet zone width
      if (!int.TryParse(QRQuietZoneTextBox.Text, out QRQuietZone) || QRQuietZone < 0 || QRQuietZone > 1000) QRQuietZone = -1;

      // set some fields
      SetQRImageDimension();
      return;
    }

    private void OnBrushSizeChanged(object sender, EventArgs e) {
      int BrushWidth, BrushHeight;
      if (int.TryParse(BrushWidthTextBox.Text, out BrushWidth) && BrushWidth > 0 && BrushWidth <= 10000 &&
        int.TryParse(BrushHeightTextBox.Text, out BrushHeight) && BrushHeight > 0 && BrushHeight <= 10000) {
        QRCodePosXTextBox.Text = (BrushWidth / 2).ToString();
        QRCodePosYTextBox.Text = (BrushHeight / 2).ToString();
      }
      return;
    }

    private void OnBackgroundRadioButton(object sender, EventArgs e) {
      // radio button changed from off to on
      if (((RadioButton)sender).Checked) {
        // enable/disable controls
        EnableControls();

        // set some fields
        SetQRImageDimension();
      }
      return;
    }

    private void EnableControls() {
      // background image
      ImageBrowseButton.Enabled = ImageRadioButton.Checked;
      ImageFileNameTextBox.Enabled = ImageRadioButton.Checked;

      // background brush
      BrushColorButton.Enabled = BrushRadioButton.Checked;
      HatchStyleComboBox.Enabled = BrushRadioButton.Checked;
      BrushWidthTextBox.Enabled = BrushRadioButton.Checked;
      BrushHeightTextBox.Enabled = BrushRadioButton.Checked;

      // background image or brush
      QRCodePosXTextBox.Enabled = !NoneRadioButton.Checked;
      QRCodePosYTextBox.Enabled = !NoneRadioButton.Checked;
      ImageRotationTextBox.Enabled = !NoneRadioButton.Checked;
      CameraDistanceTextBox.Enabled = !NoneRadioButton.Checked;
      CameraViewRotationTextBox.Enabled = !NoneRadioButton.Checked;
      return;
    }

    private void SetQRImageDimension() {
      // image of qrcode has valid dimensions
      if (QRModuleSize != 0 && QRQuietZone >= 0) {
        int QRImageDimension = QRCode.QRCodeImageDimension(QRModuleSize, QRQuietZone);
        QRImageDimensionLabel.Text = QRImageDimension.ToString();
        BrushWidthTextBox.Text = (2 * QRImageDimension).ToString();
        BrushHeightTextBox.Text = (2 * QRImageDimension).ToString();
        CameraDistanceTextBox.Text = (4 * QRImageDimension).ToString();
        ErrorDiameterTextBox.Text = (2 * QRModuleSize).ToString();
      } else {
        QRImageDimensionLabel.Text = string.Empty;
        BrushWidthTextBox.Text = string.Empty;
        BrushHeightTextBox.Text = string.Empty;
        CameraDistanceTextBox.Text = string.Empty;
        ErrorDiameterTextBox.Text = string.Empty;
      }
      return;
    }

    private void OnImageBrowse(object sender, EventArgs e) {
      // open file dialog box
      OpenFileDialog Dialog = new OpenFileDialog();
      Dialog.Filter = "Image Files(*.PNG;*.JPG;*.JPEG;*.BMP;*.GIF)|*.PNG;*.JPG;*.JPEG;*.BMP;*.GIF|All files (*.*)|*.*";
      Dialog.Title = "Load Background Image";
      Dialog.InitialDirectory = Directory.GetCurrentDirectory();
      Dialog.RestoreDirectory = true;
      if (Dialog.ShowDialog() == DialogResult.OK) ImageFileNameTextBox.Text = Dialog.FileName;
      return;
    }

    private void ImageFileNameChanged(object sender, EventArgs e) {
      string FileName = ImageFileNameTextBox.Text.Trim();
      if (File.Exists(FileName)) {
        ImageBitmap = new Bitmap(FileName);
        ImageWidthLabel.Text = ImageBitmap.Width.ToString();
        ImageHeightLabel.Text = ImageBitmap.Height.ToString();
        QRCodePosXTextBox.Text = (ImageBitmap.Width / 2).ToString();
        QRCodePosYTextBox.Text = (ImageBitmap.Height / 2).ToString(); ;
      } else {
        ImageBitmap = null;
        ImageWidthLabel.Text = string.Empty;
        ImageHeightLabel.Text = string.Empty;
        QRCodePosXTextBox.Text = string.Empty;
        QRCodePosYTextBox.Text = string.Empty;
      }
      return;
    }

    private void OnSelectColor(object sender, EventArgs e) {
      ColorDialog Dialog = new ColorDialog();
      Dialog.FullOpen = true;
      if (Dialog.ShowDialog(this) == DialogResult.OK) BrushColorButton.BackColor = Dialog.Color;
      return;
    }

    private void OnErrorRadioButton(object sender, EventArgs e) {
      // radio button changed from off to on
      if (((RadioButton)sender).Checked) {
        // enable/disable controls
        ErrorDiameterTextBox.Enabled = !ErrorNoneRadioButton.Checked;
        ErrorNumberTextBox.Enabled = !ErrorNoneRadioButton.Checked;
      }
      return;
    }

    private void OnSaveClick(object sender, EventArgs e) {
      // reset some variables
      int CameraDistance = 0;
      int CameraRotation = 0;
      int ImageWidth = 0;
      int ImageHeight = 0;
      int QRImageDimension = 0;
      int QRCodePosX = 0;
      int QRCodePosY = 0;
      int ViewXRotation = 0;
      ErrorSpotControl ErrorControl = ErrorSpotControl.None;
      int ErrorDiameter = 0;
      int ErrorSpots = 0;

      // make sure module size is not zero
      if (QRModuleSize == 0) {
        MessageBox.Show("Module size must be defined");
        return;
      }

      // make sure quiet zone is not zero
      if (QRQuietZone < 0) {
        MessageBox.Show("Quiet zone must be defined");
        return;
      }


      // image dimension
      QRImageDimension = QRCode.QRCodeImageDimension(QRModuleSize, QRQuietZone);

      // display qr code over image made with a brush
      if (BrushRadioButton.Checked) {
        // area width
        if (!int.TryParse(BrushWidthTextBox.Text.Trim(), out ImageWidth) ||
          ImageWidth <= 0 || ImageWidth >= 100000) {
          MessageBox.Show("Brush background width is invalid");
          return;
        }

        // area width
        if (!int.TryParse(BrushHeightTextBox.Text.Trim(), out ImageHeight) ||
          ImageHeight <= 0 || ImageHeight >= 100000) {
          MessageBox.Show("Brush background height is invalid");
          return;
        }
      }

      // display qr code over an image
      if (ImageRadioButton.Checked) {
        // image must be defined
        if (ImageBitmap == null) {
          MessageBox.Show("Background image must be defined");
          return;
        }

        ImageWidth = ImageBitmap.Width;
        ImageHeight = ImageBitmap.Height;
      }

      if (!NoneRadioButton.Checked) {
        // QR code position X
        if (!int.TryParse(QRCodePosXTextBox.Text.Trim(), out QRCodePosX) || QRCodePosX <= 0 || QRCodePosX >= ImageWidth) {
          MessageBox.Show("QR code position X must be within image width");
          return;
        }

        // QR code position Y
        if (!int.TryParse(QRCodePosYTextBox.Text.Trim(), out QRCodePosY) || QRCodePosY <= 0 || QRCodePosY >= ImageHeight) {
          MessageBox.Show("QR code position Y must be within image height");
          return;
        }

        // rotation
        if (!int.TryParse(ImageRotationTextBox.Text.Trim(), out CameraRotation) || CameraRotation < -360 || CameraRotation > 360) {
          MessageBox.Show("Rotation must be -360 to 360");
          return;
        }

        // camera distance
        if (!int.TryParse(CameraDistanceTextBox.Text.Trim(), out CameraDistance) || CameraDistance < 10 * QRModuleSize) {
          MessageBox.Show("Camera distance is invalid");
          return;
        }

        // Axis X Rotation
        if (!int.TryParse(CameraViewRotationTextBox.Text.Trim(), out ViewXRotation) || ViewXRotation > 160 || ViewXRotation < -160) {
          MessageBox.Show("View X rotation invalid");
          return;
        }
      }

      // error
      if (!ErrorNoneRadioButton.Checked) {
        if (ErrorWhiteRadioButton.Checked) ErrorControl = ErrorSpotControl.White;
        else if (ErrorBlackRadioButton.Checked) ErrorControl = ErrorSpotControl.Black;
        else ErrorControl = ErrorSpotControl.Alternate;

        int MaxSpotDiameter = QRCode.QRCodeImageDimension(QRModuleSize, QRQuietZone) / 8;
        if (!int.TryParse(ErrorDiameterTextBox.Text.Trim(), out ErrorDiameter) ||
          ErrorDiameter <= 0 || ErrorDiameter > MaxSpotDiameter) {
          MessageBox.Show("Error diameter is invalid");
          return;
        }

        if (!int.TryParse(ErrorNumberTextBox.Text.Trim(), out ErrorSpots) ||
          ErrorSpots <= 0 || ErrorSpots > 100) {
          MessageBox.Show("Number of error spots is invalid");
          return;
        }
      }

      // get file name
      string FileName = SaveFileName();
      if (FileName == null) return;

      // output bitmap
      Bitmap OutputBitmap;

      // display QR Code image by itself
      if (NoneRadioButton.Checked) {
        OutputBitmap = QRCodeToBitmap.CreateBitmap(QRCode, QRModuleSize, QRQuietZone);
      } else {
        if (ImageRadioButton.Checked) {
          OutputBitmap = new Bitmap(ImageBitmap);
        } else {
          // create area brush
          Brush AreaBrush = (int)HatchStyleComboBox.SelectedItem < 0 ? (Brush)new SolidBrush(BrushColorButton.BackColor) :
            (Brush)new HatchBrush((HatchStyle)((int)HatchStyleComboBox.SelectedItem), Color.Black, BrushColorButton.BackColor);

          // create picture object and and paint it with the brush
          OutputBitmap = new Bitmap(ImageWidth, ImageHeight);
          Graphics Graphics = Graphics.FromImage(OutputBitmap);
          Graphics.FillRectangle(AreaBrush, 0, 0, ImageWidth, ImageHeight);
        }

        if (ViewXRotation == 0) {
          OutputBitmap = QRCodeToBitmap.CreateBitmap(QRCode, QRModuleSize, QRQuietZone, OutputBitmap,
            QRCodePosX, QRCodePosY, CameraRotation);
        } else {
          OutputBitmap = QRCodeToBitmap.CreateBitmap(QRCode, QRModuleSize, QRQuietZone, OutputBitmap,
            QRCodePosX, QRCodePosY, CameraRotation, CameraDistance, ViewXRotation);
        }
      }

      // Error spots
      if (ErrorControl != ErrorSpotControl.None) {
        QRCodeToBitmap.AddErrorSpots(OutputBitmap, ErrorControl, ErrorDiameter, ErrorSpots);
      }

      // save image
      FileStream FS = new FileStream(FileName, FileMode.Create);
      OutputBitmap.Save(FS, (ImageFormat)ImageFormatComboBox.SelectedItem);
      FS.Close();

      // start image editor
      Process.Start(FileName);
      return;
    }

    private string SaveFileName() {
      // save file dialog box
      SaveFileDialog Dialog = new SaveFileDialog();
      Dialog.AddExtension = true;
      Dialog.Filter = QRCode.FileFilter((ImageFormat)ImageFormatComboBox.SelectedItem);
      Dialog.Title = "Save QR Code Image";
      Dialog.InitialDirectory = Directory.GetCurrentDirectory();
      Dialog.RestoreDirectory = true;
      Dialog.FileName = string.Format("QRCode{0}{1}.{2}",
        QRCode.QRCodeVersion, QRCode.ErrorCorrection.ToString(), ((ImageFormat)ImageFormatComboBox.SelectedItem).ToString().ToLower());
      if (Dialog.ShowDialog() == DialogResult.OK) return Dialog.FileName;
      return null;
    }

    private void OnCancelClick(object sender, EventArgs e) {
      DialogResult = DialogResult.Cancel;
      return;
    }
  }
}
