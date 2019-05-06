using System;
using System.Drawing;
using System.Windows.Forms;
using QRCodeEncoderDecoderLibrary;

namespace QRSnap {
  public partial class Form1 : Form {
    private QRDecoder QRCodeDecoder;
    private Bitmap QRCodeInputImage;

    public Form1() {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e) {
      QRCodeDecoder = new QRDecoder();
      this.BackColor = System.Drawing.Color.Gray;
      this.TransparencyKey = System.Drawing.Color.Gray;
      Scan();
    }

    private void Scan() {
      if (ClientSize.Width == 0 || QRCodeDecoder == null) return;
      if (QRCodeInputImage != null) QRCodeInputImage.Dispose();
      int w = this.ClientRectangle.Width;
      int h = this.ClientRectangle.Height;
      int l = this.Left + (this.Width - w) / 2;
      int t = this.Top + (this.Height - h - l + this.Left);
      //this.Opacity = 0;
      textBox1.Text = "";
      textBox1.BackColor = System.Drawing.Color.Gray;
      QRCodeInputImage = new Bitmap(w, h);
      using (Graphics g = Graphics.FromImage(QRCodeInputImage))
        g.CopyFromScreen(l, t, 0, 0, QRCodeInputImage.Size, CopyPixelOperation.SourceCopy);
      //pictureBox1.Image = QRCodeInputImage;
      byte[][] DataByteArray = QRCodeDecoder.ImageDecoder(QRCodeInputImage);
      textBox1.Text = QRDecoder.QRCodeResult(DataByteArray);
      if (textBox1.Text != "") {
        textBox1.BackColor = System.Drawing.Color.White;
        System.Windows.Forms.Clipboard.SetText(textBox1.Text);
      }
      //this.Opacity = 100;
    }

    private void Form1_Resize(object sender, EventArgs e) {
      //button1.Left = (this.Width - button1.Width) / 2;
      //button1.Top = (this.Height - button1.Height) / 2 - button1.Height / 2;
      Scan();
    }

    private void Form1_Move(object sender, EventArgs e) {
      Scan();
    }
  }
}
