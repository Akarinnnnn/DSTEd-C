﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DSTEd.Core.IO.EnumerableFileSystem;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.IO;

namespace DSTEd.Test.IO.EnumerableFileSystem
{
	[TestClass]
	public class FSUtilTest
	{
		[TestMethod]
		public void GetFileList()
		{
			RecursiveDirectoryIterator iter = new RecursiveDirectoryIterator(@".\FileSystemTest");
			Debug.WriteLine(iter.OriginalDirectoryInfo.FullName);
			FileInfo[] except = 
			{
				new FileInfo(@".\FileSystemTest\File1.txt"),
				new FileInfo(@".\FileSystemTest\File2.txt"),
				new FileInfo(@".\FileSystemTest\Directory1\File1.txt"),
				new FileInfo(@".\FileSystemTest\Directory1\File2.txt"),
				new FileInfo(@".\FileSystemTest\Directory1\.DotFolderTest\File1.txt"),
				new FileInfo(@".\FileSystemTest\Directory1\.DotFolderTest\File2.txt")
			};
			//CollectionAssert.AreEquivalent(except, iter, "iter not equivent to excepted collection");
			for(int i = 0; i<6; i++)
			{
				string except_fullpath = except[i].FullName;
				string actual_fullpath = iter[i].FullName;
				Assert.AreEqual(except_fullpath, actual_fullpath, "\nexcxept:{0}\nActual{1}", except_fullpath, actual_fullpath);
			}
		}
	}
}
