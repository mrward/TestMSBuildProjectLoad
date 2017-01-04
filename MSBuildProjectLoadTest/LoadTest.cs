//
// LoadTest.cs
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
using System.Xml;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;

namespace MSBuildProjectLoadTest
{
	static class LoadTest
	{
		static string file = @"TestDotNetCore.csproj";

		public static void Run ()
		{
			var paths = new DotNetCoreSdkPaths ();
			paths.FindSdkPaths ("Microsoft.NET.Sdk");

			//LoadProjectFromFile ();
			LoadProjectFromXml2 ();
		}

		static void LoadProjectFromFile ()
		{
			var engine = new ProjectCollection { DefaultToolsVersion = "15.0" };
			Project p = engine.LoadProject (file);
			Console.WriteLine (p.FullPath);
		}

		/// <summary>
		/// Throws an Microsoft.Build.Exceptions.InvalidProjectFileException:
		/// The value \"\" of the \"Project\" attribute in element <Import> is invalid.
		/// Parameter \"path\" cannot have zero length.  /usr/local/share/dotnet/sdk/1.0.0-preview5-004275/Sdks/Microsoft.NET.Sdk/Sdk/Sdk.targets\n 
		/// 
		/// The problem is that the .NET Core Sdk imports need the MSBuildProjectExtension property
		/// defined. This is only defined if the FullPath is set on the Project. This cannot be
		/// set when ProjectCollection.LoadProject is passed an XmlTextReader.
		/// </summary>
		static void LoadProjectFromXml ()
		{
			var engine = new ProjectCollection { DefaultToolsVersion = "15.0" };
			string content = File.ReadAllText (file);
			Project p = engine.LoadProject (new XmlTextReader (new StringReader (content)));
			p.FullPath = file;
		}

		static void LoadProjectFromXml2 ()
		{
			var engine = new ProjectCollection { DefaultToolsVersion = "15.0" };
			string content = File.ReadAllText (file);

			var projectRoot = ProjectRootElement.Create (new XmlTextReader (new StringReader (content)));
			projectRoot.FullPath = file;
			string toolsVersion = projectRoot.ToolsVersion ?? engine.DefaultToolsVersion;
			var p = new Project (projectRoot, null, toolsVersion, engine);

			Console.WriteLine (p.FullPath);
		}
	}
}
