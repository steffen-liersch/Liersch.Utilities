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
  // http://www.steffen-liersch.de/content/de/2010/05/variable-7-bit-encoding/
  public static class SL7BitEncodingVariable
  {
    public static void Write7BitEncodedInt32(Stream stream, Int32 value)
    {
      Int32 part=value;
      value=value>>7 & (1<<32-7)-1;
      while(true)
      {
        if(value==0)
        {
          stream.WriteByte(unchecked((byte)part));
          break;
        }
        stream.WriteByte(unchecked((byte)(part | 0x80)));
        part=value;
        value>>=7;
      }
    }

    public static void Write7BitEncodedInt64(Stream stream, Int64 value)
    {
      Int64 part=value;
      value=value>>7 & (1L<<64-7)-1;
      while(true)
      {
        if(value==0)
        {
          stream.WriteByte(unchecked((byte)part));
          break;
        }
        stream.WriteByte(unchecked((byte)(part | 0x80)));
        part=value;
        value>>=7;
      }
    }

    public static Int32 Read7BitEncodedInt32(Stream stream)
    {
      Int32 value, res=0;
      int shift=0;
      while(true)
      {
        value=stream.ReadByte();
        if(value<0)
          throw new EndOfStreamException();

        res|=(value & 0x7f)<<shift;

        if(shift>=28)
        {
          if(value>0x0f)
            throw new OverflowException();
          return res;
        }

        if((value & 0x80)==0)
          return res;

        shift+=7;
      }
    }

    public static Int64 Read7BitEncodedInt64(Stream stream)
    {
      Int64 value, res=0;
      int shift=0;
      while(true)
      {
        value=stream.ReadByte();
        if(value<0)
          throw new EndOfStreamException();

        res|=(value & 0x7f)<<shift;

        if(shift>=63)
        {
          if(value>0x01)
            throw new OverflowException();
          return res;
        }

        if((value & 0x80)==0)
          return res;

        shift+=7;
      }
    }

    /*
     *  Versatz | Bedeutung
     * ---------+---------------------------------------------------------
     *        7 |
     *       14 |
     *       21 |
     *       28 | noch 4 Bit verbleiben beim Lesen von 32-Bit-Werten
     *       35 |
     *       42 |
     *       49 |
     *       56 |
     *       63 | noch 1 Bit verbleibt beim Lesen von 64-Bit-Werten
     */
  }
}