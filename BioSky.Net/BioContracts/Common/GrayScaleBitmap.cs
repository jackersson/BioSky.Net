using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Common
{
  public class GrayScaleBitmap
  {
    private BITMAPFILEHEADER fileHeaderBitmap;
    private BITMAPINFO infoBitmap;
    private byte[] bmpData;

    public GrayScaleBitmap()
    {
      fileHeaderBitmap = new BITMAPFILEHEADER();
      infoBitmap = new BITMAPINFO();
    }
    public GrayScaleBitmap(int nWidth, int nHeight, byte[] pImage)
    {
      int offset = Init(nWidth, nHeight);
      BuildImageFromBytes(nWidth, nHeight, offset, pImage);
    }

    public GrayScaleBitmap(int nWidth, int nHeight, IntPtr pImage)
    {
      int offset = Init(nWidth, nHeight);
      BuildImageFromIntPtr(nWidth, nHeight, offset, pImage);
    }

    ~GrayScaleBitmap()
    {
      bmpData = null;
      fileHeaderBitmap = null;
      infoBitmap = null;
      GC.Collect();
    }
    private void BuildImageFromBytes(int nWidth, int nHeight, int offset, byte[] pImage)
    {
      int nImgOffset = 0;
      byte[] pRotateImage = new byte[nWidth * nHeight];
      for (int iCyc = 0; iCyc < nHeight; iCyc++)
      {
        Array.Copy(pImage, (nHeight - iCyc - 1) * nWidth, pRotateImage, nImgOffset, nWidth);
        nImgOffset += nWidth;
      }
      Array.Copy(pRotateImage, 0, bmpData, offset, pRotateImage.Length);
      pRotateImage = null;
    }

    private void BuildImageFromIntPtr(int nWidth, int nHeight, int offset, IntPtr pImage)
    {
      IntPtr pPtr;
      int nImgOffset = 0;
      byte[] pRotateImage = new byte[nWidth * nHeight];
      for (int iCyc = 0; iCyc < nHeight; iCyc++)
      {
        pPtr = (IntPtr)(pImage.ToInt32() + (nHeight - iCyc - 1) * nWidth);
        Marshal.Copy(pPtr, pRotateImage, nImgOffset, nWidth);
        nImgOffset += nWidth;
      }
      Array.Copy(pRotateImage, 0, bmpData, offset, pRotateImage.Length);
      pRotateImage = null;
    }

    private int Init(int nWidth, int nHeight)
    {
      fileHeaderBitmap = new BITMAPFILEHEADER();
      infoBitmap = new BITMAPINFO();

      int length = fileHeaderBitmap.SizeOfBFH + infoBitmap.SizeOfBI + nWidth * nHeight;

      fileHeaderBitmap.BfSize = (uint)length;
      fileHeaderBitmap.BfOffBits = (uint)(fileHeaderBitmap.SizeOfBFH + infoBitmap.SizeOfBI);

      infoBitmap.bmiHeader.BiWidth = nWidth;
      infoBitmap.bmiHeader.BiHeight = nHeight;

      bmpData = new byte[length];
      byte[] TempData = fileHeaderBitmap.GetByteData();
      Array.Copy(TempData, 0, bmpData, 0, TempData.Length);

      int offset = TempData.Length;
      TempData = infoBitmap.bmiHeader.GetByteData();
      Array.Copy(TempData, 0, bmpData, offset, TempData.Length);

      offset += TempData.Length;
      TempData = infoBitmap.bmiColors.GetGRBTableByteData();
      Array.Copy(TempData, 0, bmpData, offset, TempData.Length);
      offset += TempData.Length;

      TempData = null;

      return offset;
    }

    public byte[] BitmatFileData { get { return bmpData; } }
  }
}

public class BITMAPFILEHEADER
{
  private ushort bfType;
  private uint   bfSize;
  private ushort bfReserved1;
  private ushort bfReserved2;
  private uint   bfOffBits;

  public BITMAPFILEHEADER()
  {
    bfType = 'B' + 'M' * 0x100;
    bfReserved1 = bfReserved2 = 0;
  }
  public int SizeOfBFH {
    get {
      return (Marshal.SizeOf(bfType) + Marshal.SizeOf(bfSize) + Marshal.SizeOf(bfReserved1)
                  + Marshal.SizeOf(bfReserved2) + Marshal.SizeOf(bfOffBits));
    }
  }
  public uint BfSize {
    set { bfSize = value; }
  }
  public uint BfOffBits {
    set { bfOffBits = value; }
  }
  public byte[] GetByteData()
  {
    byte[] m_Data = new byte[SizeOfBFH];
    byte[] temp = BitConverter.GetBytes(bfType);

    int offset = 0;
    Array.Copy(temp, 0, m_Data, 0, temp.Length);
    offset = temp.Length;
    temp = System.BitConverter.GetBytes(bfSize);
    Array.Copy(temp, 0, m_Data, offset, temp.Length);
    offset += temp.Length;
    temp = System.BitConverter.GetBytes(bfReserved1);
    Array.Copy(temp, 0, m_Data, offset, temp.Length);
    offset += temp.Length;
    temp = System.BitConverter.GetBytes(bfReserved2);
    Array.Copy(temp, 0, m_Data, offset, temp.Length);
    offset += temp.Length;
    temp = System.BitConverter.GetBytes(bfOffBits);
    Array.Copy(temp, 0, m_Data, offset, temp.Length);
    return m_Data;
  }
};

public class RGBQUAD
{
  private byte rgbBlue;
  private byte rgbGreen;
  private byte rgbRed;
  private byte rgbReserved;

  public RGBQUAD() { rgbReserved = 0; }
  public int SizeOfRgbquad
  {
    get
    {
      return (Marshal.SizeOf(rgbBlue) + Marshal.SizeOf(rgbGreen) + Marshal.SizeOf(rgbRed) + Marshal.SizeOf(rgbReserved));
    }
  }
  public byte RGBBlue
  {
    set { rgbBlue = value; }
  }
  public byte RGBGreen
  {
    set { rgbGreen = value; }
  }
  public byte RGBRed
  {
    set { rgbRed = value; }
  }
  public byte[] GetGRBTableByteData()
  {
    byte[] m_Data = new byte[256 * SizeOfRgbquad];
    int nOffset = 0;
    for (int i = 0; i < 256; i++)
    {
      m_Data[nOffset] = (byte)i;
      m_Data[nOffset + 1] = (byte)i;
      m_Data[nOffset + 2] = (byte)i;
      m_Data[nOffset + 3] = 0;
      nOffset += 4;
    }
    return m_Data;
  }
};

public class BITMAPINFOHEADER
{
  private uint biSize;
  private int biWidth;
  private int biHeight;
  private ushort biPlanes;
  private ushort biBitCount;
  private uint biCompression;
  private uint biSizeImage;
  private int biXPelsPerMeter;
  private int biYPelsPerMeter;
  private uint biClrUsed;
  private uint biClrImportant;

  public BITMAPINFOHEADER()
  {
    biPlanes = 1;
    biBitCount = 8;
    biCompression = 0;  //BI_RGB; #define BI_RGB        0L
    biSizeImage = 0;
    biClrUsed = biClrImportant = 0;
    biXPelsPerMeter = 0x4CE6; //500DPI
    biYPelsPerMeter = 0x4CE6; //500DPI
    biSize = (uint)SizeOfBIH;
  }
  public int SizeOfBIH
  {
    get
    {
      return (Marshal.SizeOf(biSize) + Marshal.SizeOf(biWidth) + Marshal.SizeOf(biHeight) + Marshal.SizeOf(biPlanes)
            + Marshal.SizeOf(biBitCount) + Marshal.SizeOf(biCompression) + Marshal.SizeOf(biSizeImage) + Marshal.SizeOf(biXPelsPerMeter)
            + Marshal.SizeOf(biYPelsPerMeter) + Marshal.SizeOf(biClrUsed) + Marshal.SizeOf(biClrImportant));
    }
  }
  public uint BiSize
  {
    get { return biSize; }
    set { biSize = value; }
  }
  public int BiWidth
  {
    set { biWidth = value; }
  }
  public int BiHeight
  {
    set { biHeight = value; }
  }
  public int BiXPelsPerMeter
  {
    set { biXPelsPerMeter = value; }
  }
  public int BiYPelsPerMeter
  {
    set { biYPelsPerMeter = value; }
  }
  public byte[] GetByteData()
  {
    byte[] m_Data = new byte[SizeOfBIH];
    byte[] temp = System.BitConverter.GetBytes(biSize);
    int offset = 0;
    Array.Copy(temp, 0, m_Data, 0, temp.Length);
    offset = temp.Length;
    temp = System.BitConverter.GetBytes(biWidth);
    Array.Copy(temp, 0, m_Data, offset, temp.Length);
    offset += temp.Length;
    temp = System.BitConverter.GetBytes(biHeight);
    Array.Copy(temp, 0, m_Data, offset, temp.Length);
    offset += temp.Length;
    temp = System.BitConverter.GetBytes(biPlanes);
    Array.Copy(temp, 0, m_Data, offset, temp.Length);
    offset += temp.Length;
    temp = System.BitConverter.GetBytes(biBitCount);
    Array.Copy(temp, 0, m_Data, offset, temp.Length);
    offset += temp.Length;
    temp = System.BitConverter.GetBytes(biCompression);
    Array.Copy(temp, 0, m_Data, offset, temp.Length);
    offset += temp.Length;
    temp = System.BitConverter.GetBytes(biSizeImage);
    Array.Copy(temp, 0, m_Data, offset, temp.Length);
    offset += temp.Length;
    temp = System.BitConverter.GetBytes(biXPelsPerMeter);
    Array.Copy(temp, 0, m_Data, offset, temp.Length);
    offset += temp.Length;
    temp = System.BitConverter.GetBytes(biYPelsPerMeter);
    Array.Copy(temp, 0, m_Data, offset, temp.Length);
    offset += temp.Length;
    temp = System.BitConverter.GetBytes(biClrUsed);
    Array.Copy(temp, 0, m_Data, offset, temp.Length);
    offset += temp.Length;
    temp = System.BitConverter.GetBytes(biClrImportant);
    Array.Copy(temp, 0, m_Data, offset, temp.Length);
    return m_Data;
  }
}

public class BITMAPINFO
{
  public BITMAPINFOHEADER bmiHeader;
  public RGBQUAD bmiColors;

  public BITMAPINFO()
  {
    bmiHeader = new BITMAPINFOHEADER();
    bmiColors = new RGBQUAD();
  }
  ~BITMAPINFO()
  {
    bmiHeader = null;
    bmiColors = null;
  }
  public int SizeOfBI
  {
    get { return (bmiHeader.SizeOfBIH + bmiColors.SizeOfRgbquad * 256); }
  }
}
