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

namespace Liersch.Utilities.Test
{
  sealed class UnitTest
  {
    public static void Test()
    {
      UnitTest config;


      Console.WriteLine("Testing variable 7 bit encoding");
      Console.WriteLine("===============================");
      Console.WriteLine();
      config=new UnitTest(
        false, // keine vollständige Kompatibilität zwischen 32-Bit- und 64-Bit-Funktionen
        SL7BitEncodingVariable.Write7BitEncodedInt32,
        SL7BitEncodingVariable.Write7BitEncodedInt64,
        SL7BitEncodingVariable.Read7BitEncodedInt32,
        SL7BitEncodingVariable.Read7BitEncodedInt64,
        null,
        null);
      config.PerformTest();


      Console.WriteLine("Testing symetric 7 bit encoding");
      Console.WriteLine("===============================");
      Console.WriteLine();
      config=new UnitTest(
        true, // vollständige Kompatibilität zwischen 32-Bit- und 64-Bit-Funktionen
        SL7BitEncodingSymmetric.WritePacked,
        SL7BitEncodingSymmetric.WritePacked,
        SL7BitEncodingSymmetric.ReadPackedInt32,
        SL7BitEncodingSymmetric.ReadPackedInt64,
        SL7BitEncodingSymmetric.ReadPackedUInt32,
        SL7BitEncodingSymmetric.ReadPackedUInt64);
      config.PerformTest();
    }

    delegate void SLWriteInt32(Stream stream, Int32 value);
    delegate void SLWriteInt64(Stream stream, Int64 value);
    delegate Int32 SLReadInt32(Stream stream);
    delegate Int64 SLReadInt64(Stream stream);
    delegate UInt32 SLReadUInt32(Stream stream);
    delegate UInt64 SLReadUInt64(Stream stream);

    UnitTest(
      bool fullCompatibility,
      SLWriteInt32 writeInt32,
      SLWriteInt64 writeInt64,
      SLReadInt32 readInt32,
      SLReadInt64 readInt64,
      SLReadUInt32 readUInt32,
      SLReadUInt64 readUInt64)
    {
      m_FullCompatibility=fullCompatibility;
      m_WriteInt32=writeInt32;
      m_WriteInt64=writeInt64;
      m_ReadInt32=readInt32;
      m_ReadInt64=readInt64;
      m_ReadUInt32=readUInt32;
      m_ReadUInt64=readUInt64;
    }

    void PerformTest()
    {
      MemoryStream stream=new MemoryStream(256);


      // 32-Bit-Tests
      Console.WriteLine("Writing and reading 32 bit values...");
      int[] distribution=new int[6];
      int count=0;
      for(int i=-1000; i<=1000; i++)
      {
        double d=Math.Pow(i/1000f, 5); // Wertebereich für f(x): -1..+1
        d=d*(Int32.MaxValue*0.95);
        Int32 i32=(Int32)d;
        WriteRead32(stream, distribution, i32);
        count++;
      }
      WriteRead32(stream, distribution, Int32.MinValue);
      WriteRead32(stream, distribution, Int32.MaxValue);
      count+=2;
      Console.WriteLine(count+" different values successfully tested");
      PrintDistribution(distribution);
      Console.WriteLine();


      // 64-Bit-Tests
      Console.WriteLine("Writing and reading 64 bit values...");
      distribution=new int[11];
      count=0;
      for(int i=-1000; i<=1000; i++)
      {
        double d=Math.Pow(i/1000f, 9); // Wertebereich für f(x): -1..+1
        d=d*(Int64.MaxValue*0.95);
        Int64 i64=(Int64)d;
        WriteRead64(stream, distribution, i64);
        count++;
      }
      WriteRead64(stream, distribution, Int64.MinValue);
      WriteRead64(stream, distribution, Int64.MaxValue);
      count+=2;
      Console.WriteLine(count+" different values successfully tested");
      PrintDistribution(distribution);
      Console.WriteLine();
    }

    void WriteRead32(MemoryStream stream, int[] distribution, Int32 value)
    {
      stream.SetLength(0);
      m_WriteInt32(stream, value);
      distribution[stream.Length]++;
      Read32(stream, value);

      if(m_FullCompatibility || value>=0)
      {
        // Positive 32-Bit-Werte, die mit Write7BitEncodedInt32 geschrieben
        // wurden, können auch mit Read7BitEncodedInt64 gelesen werden.
        Read64(stream, value);

        // Positive 32-Bit-Werte, die mit Write7BitEncodedInt64 geschrieben
        // wurden, können auch mit Read7BitEncodedInt32 gelesen werden.
        stream.SetLength(0);
        m_WriteInt64(stream, value);
        Read32(stream, value);
        Read64(stream, value);
      }
    }

    void WriteRead64(MemoryStream stream, int[] distribution, Int64 value)
    {
      stream.SetLength(0);
      m_WriteInt64(stream, value);
      distribution[stream.Length]++;
      Read64(stream, value);

      // Positive 64-Bit-Werte, die mit Write7BitEncodedInt64 geschrieben
      // wurden, können auch mit Read7BitEncodedInt32 gelesen werden,
      // sofern sie im 32-Bit-Wertebereich liegen.
      if(m_FullCompatibility || value>=0)
      {
        if(value>=Int32.MinValue && value<=Int32.MaxValue)
          Read32(stream, (Int32)value);
      }
    }

    void Read32(MemoryStream stream, Int32 value)
    {
      stream.Position=0;
      Int32 i32=m_ReadInt32(stream);
      if(i32!=value)
        throw new InvalidOperationException();

      if(stream.Position!=stream.Length)
        throw new InvalidOperationException();


      if(value>=0 && m_ReadUInt32!=null)
      {
        stream.Position=0;
        UInt32 ui32=m_ReadUInt32(stream);
        if(ui32!=value)
          throw new InvalidOperationException();

        if(stream.Position!=stream.Length)
          throw new InvalidOperationException();
      }
    }

    void Read64(MemoryStream stream, Int64 value)
    {
      stream.Position=0;
      Int64 i64=m_ReadInt64(stream);
      if(i64!=value)
        throw new InvalidOperationException();

      if(stream.Position!=stream.Length)
        throw new InvalidOperationException();


      if(value>=0 && m_ReadUInt32!=null)
      {
        stream.Position=0;
        UInt64 ui64=m_ReadUInt64(stream);
        if(ui64!=(UInt64)value)
          throw new InvalidOperationException();

        if(stream.Position!=stream.Length)
          throw new InvalidOperationException();
      }
    }

    static void PrintDistribution(int[] distribution)
    {
      for(int i=1; i<distribution.Length; i++)
        Console.WriteLine("* "+distribution[i]+" values with "+i+" bytes");
    }

    bool m_FullCompatibility;
    SLWriteInt32 m_WriteInt32;
    SLWriteInt64 m_WriteInt64;
    SLReadInt32 m_ReadInt32;
    SLReadInt64 m_ReadInt64;
    SLReadUInt32 m_ReadUInt32;
    SLReadUInt64 m_ReadUInt64;
  }
}