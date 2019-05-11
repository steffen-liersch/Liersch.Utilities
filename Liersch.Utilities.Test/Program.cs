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

namespace Liersch.Utilities.Test
{
  static class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Please visit www.steffen-liersch.de");
      Console.WriteLine();

      UnitTest.Test();
      Console.ReadKey(true);
    }
  }
}