//----------------------------------------------------------------------------
//
// Copyright © 2010-2019 Dipl.-Ing. (BA) Steffen Liersch
// All rights reserved.
//
// Steffen Liersch
// Robert-Schumann-Straße 1
// 08289 Schneeberg
// Germany
//
// Phone: +49-3772-38 28 08
// E-Mail: S.Liersch@gmx.de
//
//----------------------------------------------------------------------------

using System;
using System.IO;

namespace Liersch.Utilities
{
  // http://www.steffen-liersch.de/content/de/2010/05/symmetric-7-bit-encoding/
  public static class SL7BitEncodingSymmetric
  {
    public static void WritePacked(Stream stream, Int32 value)
    {
      if(value<0)
      {
        if(value==Int32.MinValue)
          WritePacked(stream, 1UL<<31, 0x40);
        else WritePacked(stream, unchecked(-value), 0x40);
      }
      else WritePacked(stream, value, 0);
    }

    public static void WritePacked(Stream stream, Int64 value)
    {
      if(value<0)
      {
        if(value==Int64.MinValue)
          WritePacked(stream, 1UL<<63, 0x40);
        else WritePacked(stream, unchecked(-value), 0x40);
      }
      else WritePacked(stream, value, 0);
    }


    [CLSCompliant(false)]
    public static void WritePacked(Stream stream, UInt32 value)
    {
      WritePacked(stream, value, 0);
    }

    [CLSCompliant(false)]
    public static void WritePacked(Stream stream, UInt64 value)
    {
      WritePacked(stream, value, 0);
    }


    static void WritePacked(Stream stream, Int32 value, Int32 sign)
    {
      if(value>=0x40)
      {
        stream.WriteByte(unchecked((byte)(value & 0x3f | sign | 0x80)));
        value>>=6;
        while(value>=0x80)
        {
          stream.WriteByte(unchecked((byte)(value | 0x80)));
          value>>=7;
        }
        stream.WriteByte(unchecked((byte)value));
      }
      else stream.WriteByte(unchecked((byte)(value | sign)));
    }

    static void WritePacked(Stream stream, Int64 value, Int64 sign)
    {
      if(value>=0x40)
      {
        stream.WriteByte(unchecked((byte)(value & 0x3f | sign | 0x80)));
        value>>=6;
        while(value>=0x80)
        {
          stream.WriteByte(unchecked((byte)(value | 0x80)));
          value>>=7;
        }
        stream.WriteByte(unchecked((byte)value));
      }
      else stream.WriteByte(unchecked((byte)(value | sign)));
    }

    static void WritePacked(Stream stream, UInt64 value, UInt64 sign)
    {
      if(value>=0x40)
      {
        stream.WriteByte(unchecked((byte)(value & 0x3f | sign | 0x80)));
        value>>=6;
        while(value>=0x80)
        {
          stream.WriteByte(unchecked((byte)(value | 0x80)));
          value>>=7;
        }
        stream.WriteByte(unchecked((byte)value));
      }
      else stream.WriteByte(unchecked((byte)(value | sign)));
    }


    public static Int32 ReadPackedInt32(Stream stream)
    {
      Int32 value, res, sign;

      value=stream.ReadByte();
      if(value<0)
        throw new EndOfStreamException();

      res=value & 0x3f;
      sign=value & 0x40;
      if(value<0x80)
        return sign!=0 ? checked(-res) : res;

      int shift=6;
      while(true)
      {
        value=stream.ReadByte();
        if(value<0)
          throw new EndOfStreamException();

        res|=(value & 0x7f)<<shift;

        if(shift>=27)
        {
          if(value>0x0f)
          {
            if(value==0x10 && res==Int32.MinValue)
              return res;
            throw new OverflowException();
          }
          return sign!=0 ? checked(-res) : res;
        }

        if((value & 0x80)==0)
          return sign!=0 ? checked(-res) : res;

        shift+=7;
      }
    }

    public static Int64 ReadPackedInt64(Stream stream)
    {
      Int64 value, res, sign;

      value=stream.ReadByte();
      if(value<0)
        throw new EndOfStreamException();

      res=value & 0x3f;
      sign=value & 0x40;
      if(value<0x80)
        return sign!=0 ? checked(-res) : res;

      int shift=6;
      while(true)
      {
        value=stream.ReadByte();
        if(value<0)
          throw new EndOfStreamException();

        res|=(value & 0x7f)<<shift;

        if(shift>=62)
        {
          if(value>0x01)
          {
            if(value==0x02 && res==Int64.MinValue)
              return res;
            throw new OverflowException();
          }
          return sign!=0 ? checked(-res) : res;
        }

        if((value & 0x80)==0)
          return sign!=0 ? checked(-res) : res;

        shift+=7;
      }
    }


    [CLSCompliant(false)]
    public static UInt32 ReadPackedUInt32(Stream stream)
    {
      UInt32 value, res;

      int temp=stream.ReadByte();
      if(temp<0)
        throw new EndOfStreamException();

      if((temp & 0x40)!=0)
        throw new OverflowException();

      if(temp<0x80)
        return (UInt32)temp;

      value=(UInt32)temp;
      res=value & 0x3f;

      int shift=6;
      while(true)
      {
        temp=stream.ReadByte();
        if(temp<0)
          throw new EndOfStreamException();

        value=(UInt32)temp;
        res|=(value & 0x7f)<<shift;

        if(shift>=27)
        {
          if(temp>0x1f)
            throw new OverflowException();
          return res;
        }

        if((temp & 0x80)==0)
          return res;

        shift+=7;
      }
    }

    [CLSCompliant(false)]
    public static UInt64 ReadPackedUInt64(Stream stream)
    {
      UInt64 value, res;

      int temp=stream.ReadByte();
      if(temp<0)
        throw new EndOfStreamException();

      if((temp & 0x40)!=0)
        throw new OverflowException();

      if(temp<0x80)
        return (UInt64)temp;

      value=(UInt64)temp;
      res=value & 0x3f;

      int shift=6;
      while(true)
      {
        temp=stream.ReadByte();
        if(temp<0)
          throw new EndOfStreamException();

        value=(UInt64)temp;
        res|=(value & 0x7f)<<shift;

        if(shift>=62)
        {
          if(temp>0x03)
            throw new OverflowException();
          return res;
        }

        if((temp & 0x80)==0)
          return res;

        shift+=7;
      }
    }


    /*
     *  Versatz | Bedeutung
     * ---------+---------------------------------------------------------
     *        6 |
     *       13 |
     *       20 |
     *       27 | noch zu lesende Bits: 4 Bit für Int32, 5 Bit für UInt32
     *       34 |
     *       41 |
     *       48 |
     *       55 |
     *       62 | noch zu lesende Bits: 1 Bit für Int64, 2 Bit für UInt64
     */
  }
}