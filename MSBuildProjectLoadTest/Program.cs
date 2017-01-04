//
// Program.cs
//
// Author:
//       Matt Ward <matt.ward@xamarin.com>
//
// Copyright (c) 2017 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MSBuildProjectLoadTest
{
	class MainClass
	{
		static string msbuildBinDir = @"/Library/Frameworks/Mono.framework/Versions/4.8.0/lib/mono/msbuild/15.0/bin/";
		//static string msbuildBinDir = @"~/projects/utils/msbuild/bin/Bootstrap";
		[STAThread]
		public static void Main (string[] args)
		{
			try {
				AppDomain.CurrentDomain.AssemblyResolve += MSBuildAssemblyResolver;

				LoadTest.Run ();
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}
		}

		static Assembly MSBuildAssemblyResolver (object sender, ResolveEventArgs args)
		{
			var msbuildAssemblies = new string[] {
				"Microsoft.Build",
				"Microsoft.Build.Engine",
				"Microsoft.Build.Framework",
				"Microsoft.Build.Tasks.Core",
				"Microsoft.Build.Utilities.Core" };

			var asmName = new AssemblyName (args.Name);
			if (!msbuildAssemblies.Any (n => string.Compare (n, asmName.Name, StringComparison.OrdinalIgnoreCase) == 0))
				return null;

			string fullPath = Path.Combine (msbuildBinDir, asmName.Name + ".dll");
			if (File.Exists (fullPath)) {
				// If the file exists under the msbuild bin dir, then we need
				// to load it only from there. If that fails, then let that exception
				// escape
				return Assembly.LoadFrom (fullPath);
			} else
				return null;
		}
	}
}
